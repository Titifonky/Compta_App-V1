using LogDebugging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Compta
{


    public partial class MainWindow : Window
    {
        private enum TypeEcriture
        {
            Debit = 0,
            Credit = 1
        }

        private class Enregistrement
        {
            public TypeEcriture TypeEcriture { get; set; }
            public DateTime DateValeur { get; set; }
            public Double Valeur { get; set; }
            public String Ref { get; set; }
            public String Lib1 { get; set; }
            public String Lib2 { get; set; }
            public Boolean Lcr { get; set; }

            private Double eToDouble(String s)
            {
                Double Val = 0;

                try
                {
                    Val = (Double)Convert.ChangeType(s, typeof(Double));
                }
                catch
                {
                    try
                    {
                        Val = Double.Parse(s, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo);
                    }
                    catch (Exception e)
                    { Log.Message(e); }

                    Val = 0;
                }

                return Val;
            }

            private int eToInteger(String s)
            {
                int Val = 0;

                try
                {
                    Val = (int)Convert.ChangeType(s, typeof(int));
                }
                catch
                {
                    try
                    {
                        Val = int.Parse(s, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo);
                    }
                    catch
                    {
                        Val = 0;
                    }
                }

                return Val;
            }

            public void FillData(String ligne)
            {
                if (ligne.StartsWith("<TRNTYPE>"))
                {
                    TypeEcriture = TypeEcriture.Credit;
                    if (ligne.Replace("<TRNTYPE>", "").Trim() == "DEBIT")
                        TypeEcriture = TypeEcriture.Debit;
                }
                else if (ligne.StartsWith("<DTPOSTED>"))
                {
                    String d = ligne.Replace("<DTPOSTED>", "").Trim().Substring(0, 8);
                    DateValeur = new DateTime(eToInteger(d.Substring(0, 4)), eToInteger(d.Substring(4, 2)), eToInteger(d.Substring(6, 2)));
                }
                else if (ligne.StartsWith("<TRNAMT>"))
                    Valeur = eToDouble(ligne.Replace("<TRNAMT>", "").Trim());
                else if (ligne.StartsWith("<FITID>"))
                    Ref = ligne.Replace("<FITID>", "").Trim();
                else if (ligne.StartsWith("<NAME>"))
                {
                    Lib1 = ligne.Replace("<NAME>", "").Trim();
                    if (ligne.Contains("RELEVE LCR") || ligne.Contains("REMISE CHEQUES"))
                        Lcr = true;
                }
                else if (ligne.StartsWith("<MEMO>"))
                    Lib2 = ligne.Replace("<MEMO>", "").Trim();
            }
        }

        private void ImporterReleve_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog pDialogue = new Microsoft.Win32.OpenFileDialog();
            pDialogue.CheckFileExists = true;
            pDialogue.Title = "Ouvrir le fichier de banque";
            pDialogue.DefaultExt = ".ofx";
            pDialogue.Filter = "Fichier banque (*.ofx)|*.ofx";
            pDialogue.InitialDirectory = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders",
                                                                                "{374DE290-123F-4565-9164-39C4925E467B}",
                                                                                String.Empty).ToString();
            bool? result = pDialogue.ShowDialog();

            if (result == true)
            {
                int NbImport = 0;
                // On récupère la liste des enregistrements existants
                var Banque = pSociete.BanqueCourante;
                // On crée un Hash des ref des enregistrements pour pouvoir vérifier s'il ne sont pas encore intégré.
                HashSet<String> EnregistrementsExistants = new HashSet<String>();
                foreach (var E in Banque.ListeEcritureBanque)
                    EnregistrementsExistants.Add(E.IdBanque);

                using (StreamReader sr = new StreamReader(pDialogue.FileName))
                {
                    while (sr.Peek() > -1)
                    {
                        String ligne = sr.ReadLine();

                        if (ligne == "<STMTTRN>")
                        {
                            Enregistrement E = new Enregistrement();

                            while (ligne != "</STMTTRN>")
                            {
                                ligne = sr.ReadLine().Trim();
                                E.FillData(ligne);
                            }

                            if (!EnregistrementsExistants.Contains(E.Ref))
                            {
                                new EcritureBanque(Banque, E.Ref, E.DateValeur, E.Lib1 + (String.IsNullOrWhiteSpace(E.Lib2) ? "" : " / " + E.Lib2), E.Valeur);
                                NbImport++;
                            }
                        }
                    }
                };

                MessageBox.Show(String.Format("{0} ligne(s) importée(s)", NbImport));

                if (NbImport > 0)
                    Banque.CalculerSolde();
            }
        }
    }
}
