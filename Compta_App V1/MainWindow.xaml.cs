using LogDebugging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Compta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    public partial class MainWindow : Window
    {
        public static TabItem DernierOngletActif = null;

        private RechercheTexte<EcritureBanque> _RechercherEcritureBanque;
        private RechercheTexte<LigneCompta> _RechercherLigneCompta;
        private RechercheTexte<Compte> _RechercherCompte;

        public Societe pSociete;

        public MainWindow()
        {
            this.Closing += new CancelEventHandler(MainWindow_Closing);
            this.Loaded += MainWindow_Loaded;

            InitializeComponent();

            this.AddHandler(OngletSupprimable.TabItemClosed_Event, new RoutedEventHandler(this.SupprimerOnglet));

            xOnglets.SelectionChanged += this.SelectionOnglet;

            if (!Start()) this.Close();

            WindowParam.AjouterParametre(this);
            WindowParam.RestaurerParametre(this);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Message("Requette SQL : " + Bdd2.TempsRequete);
        }

        private Boolean Start()
        {
            Log.Entete();

            String BaseSelectionnee = "";
            List<String> ListeBase = Bdd2.ListeBase();
            if (ListeBase.Count == 1)
                BaseSelectionnee = ListeBase[0];
            else
            {
                SelectionnerBase Fenetre = new SelectionnerBase(ListeBase);
                Fenetre.ShowDialog();
                BaseSelectionnee = Fenetre.BaseSelectionnee;
            }

            if (!Bdd2.Initialiser(BaseSelectionnee))
            {
                Log.Message("Impossible de se connecter à la base");
                MessageBox.Show("Impossible de se connecter à la base");
                return false;
            }

            xConnexionCourante.Text = BaseSelectionnee + ", connecté à l'adresse : " + Bdd2.ConnexionCourante;

            pSociete = Bdd2.Liste<Societe>()[0];

            Log.Message("Precharger Societe");
            var result1 = Bdd2.PreCharger(typeof(Societe), new List<ObjetGestion>() { pSociete });

            Log.Message("Precharger Banque");
            var result2 = Bdd2.PreCharger(typeof(Banque), result1[typeof(Banque)]);

            Log.Message("Precharger EcritureBanque");
            var result3 = Bdd2.PreCharger(typeof(EcritureBanque), result2[typeof(EcritureBanque)]);

            Log.Message("Precharger Groupe");
            var result4 = Bdd2.PreCharger(typeof(Groupe), result1[typeof(Groupe)]);

            Log.Message("Precharger Compte");
            var result5 = Bdd2.PreCharger(typeof(Compte), result4[typeof(Compte)]);

            Log.Message("Precharger Fin");

            //foreach (Compte cp in result4[typeof(Compte)])
            //{
            //    Log.Message(cp.Nom);
            //    Log.Message(cp.ListeLigneBanque.Count);
            //    Log.Message(cp.ListeLigneCompta.Count);
            //}

            Log.Message("test chargement");

            this.DataContext = pSociete;

            pSociete.OnModifyBanque += new Societe.OnModifyBanqueEventHandler(id => { Properties.Settings.Default.IdBanque = id; Properties.Settings.Default.Save(); });

            pSociete.BanqueCourante = pSociete.ListeBanque.FirstOrDefault(b => { return b.Id == Properties.Settings.Default.IdBanque; }) ?? pSociete.ListeBanque[0];

            //TrierListe<EcritureBanque>(xListeEcritureBanque);
            //TrierListe<LigneCompta>(xListeLigneCompta);

            _RechercherEcritureBanque = new RechercheTexte<EcritureBanque>(xListeEcritureBanque, true);
            xRechercherEcritureBanque.DataContext = _RechercherEcritureBanque;

            _RechercherLigneCompta = new RechercheTexte<LigneCompta>(xListeLigneCompta);
            xRechercherLigneCompta.DataContext = _RechercherLigneCompta;

            _RechercherCompte = new RechercheTexte<Compte>(xListeCompte);
            xRechercherCompte.DataContext = _RechercherCompte;

            //foreach (var cp in pSociete.ListeBanque[0].ListeEcritureBanque)
            //    cp.Pointer = false;

            return true;
        }

        private void TrierListe<T>(ListBox Box)
            where T : ObjetGestion
        {
            foreach (var info in Bdd2.DicProp.Dic[typeof(T)].ListeTri)
                Box.Items.SortDescriptions.Add(new SortDescription(info.NomProp, info.Tri.DirectionTri));

            Box.Items.IsLiveSorting = true;
        }

        public void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            MessageBoxResult R = MessageBoxResult.No;
            if (Bdd2.DoitEtreEnregistre)
                R = MessageBox.Show("Voulez vous enregistrer la base ?", "Info", MessageBoxButton.YesNo);

            if (R == MessageBoxResult.Yes)
                Bdd2.Enregistrer();

            Bdd2.Deconnecter();

            WindowParam.SauverParametre(this);
        }
    }

    public static class WindowParam
    {
        private static String DimEcran
        {
            get
            {
                return System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height + "x" + System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            }
        }

        public static void AjouterParametre(Window w)
        {
            ConfigModule Cfg = new ConfigModule(DimEcran);

            Cfg.AjouterP<Double>(w.Name + "_Left", w.Left, "");
            Cfg.AjouterP<Double>(w.Name + "_Top", w.Top, "");
            Cfg.AjouterP<Double>(w.Name + "_Width", w.Width, "");
            Cfg.AjouterP<Double>(w.Name + "_Height", w.Height, "");

            Cfg.Sauver();
        }

        public static void RestaurerParametre(Window w)
        {
            ConfigModule Cfg = new ConfigModule(DimEcran);

            w.Left = Cfg.GetP<Double>(w.Name + "_Left").GetValeur<Double>();
            w.Top = Cfg.GetP<Double>(w.Name + "_Top").GetValeur<Double>();
            w.Width = Cfg.GetP<Double>(w.Name + "_Width").GetValeur<Double>();
            w.Height = Cfg.GetP<Double>(w.Name + "_Height").GetValeur<Double>();
        }

        public static void SauverParametre(Window w)
        {
            ConfigModule Cfg = new ConfigModule(DimEcran);

            Cfg.GetP<Double>(w.Name + "_Left").SetValeur<Double>(w.Left);
            Cfg.GetP<Double>(w.Name + "_Top").SetValeur<Double>(w.Top);
            Cfg.GetP<Double>(w.Name + "_Width").SetValeur<Double>(w.Width);
            Cfg.GetP<Double>(w.Name + "_Height").SetValeur<Double>(w.Height);

            Cfg.Sauver();
        }
    }
}