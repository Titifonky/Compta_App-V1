using LogDebugging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;


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

        public Societe pSociete;

        public MainWindow()
        {
            this.Closing += new CancelEventHandler(MainWindow_Closing);

            InitializeComponent();

            this.AddHandler(OngletSupprimable.TabItemClosed_Event, new RoutedEventHandler(this.SupprimerOnglet));

            xOnglets.SelectionChanged += this.SelectionOnglet;

            if (!Start()) this.Close();

            WindowParam.AjouterParametre(this);
            WindowParam.RestaurerParametre(this);
        }

        private Boolean Start()
        {
            Log.Entete();

            String BaseSelectionnee = "";
            List<String> ListeBase = Bdd.ListeBase();
            if (ListeBase.Count == 1)
                BaseSelectionnee = ListeBase[0];
            else
            {
                SelectionnerBase Fenetre = new SelectionnerBase(ListeBase);
                Fenetre.ShowDialog();
                BaseSelectionnee = Fenetre.BaseSelectionnee;
            }

            if (!Bdd.Initialiser(BaseSelectionnee))
            {
                Log.Message("Impossible de se connecter � la base");
                return false;
            }

            xConnexionCourante.Text = BaseSelectionnee + ", connect� � l'adresse : " + Bdd.ConnexionCourante;

            pSociete = Bdd.Liste<Societe>()[0];

            this.DataContext = pSociete;

            pSociete.OnModifyBanque += new Societe.OnModifyBanqueEventHandler(id => { Properties.Settings.Default.IdBanque = id; Properties.Settings.Default.Save(); });

            pSociete.BanqueCourante = pSociete.ListeBanque.FirstOrDefault(b => { return b.Id == Properties.Settings.Default.IdBanque; }) ?? pSociete.ListeBanque[0];

            TrierListe<EcritureBanque>(xListeEcritureBanque);
            TrierListe<LigneCompta>(xListeLigneCompta);

            _RechercherEcritureBanque = new RechercheTexte<EcritureBanque>(xListeEcritureBanque);
            xRechercherEcritureBanque.DataContext = _RechercherEcritureBanque;

            _RechercherLigneCompta = new RechercheTexte<LigneCompta>(xListeLigneCompta);
            xRechercherLigneCompta.DataContext = _RechercherLigneCompta;

            return true;
        }

        private void TrierListe<T>(ListBox Box)
            where T : ObjetGestion
        {
            List<PropertyInfo> pListeTri = Bdd.DicProprietes.ListeTri(typeof(T));

            foreach (PropertyInfo P in pListeTri)
            {
                ListSortDirection Dir = (P.GetCustomAttributes(typeof(Tri)).First() as Tri).DirectionTri;
                Box.Items.SortDescriptions.Add(new SortDescription(P.Name, Dir));
            }
            Box.Items.IsLiveSorting = true;
        }

        public void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            MessageBoxResult R = MessageBoxResult.No;
            if (Bdd.DoitEtreEnregistre)
                R = MessageBox.Show("Voulez vous enregistrer la base ?", "Info", MessageBoxButton.YesNo);

            if (R == MessageBoxResult.Yes)
                Bdd.Enregistrer();

            Bdd.Deconnecter();

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