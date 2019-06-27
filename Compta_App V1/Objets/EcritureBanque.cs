using LogDebugging;
using System;
using System.Windows;

namespace Compta
{
    public class EcritureBanque : ObjetGestion
    {
        public EcritureBanque() { }

        public EcritureBanque(Banque banque, String idBanque, DateTime dateValeur, String intitule, Double valeur)
        {
            Bdd.Ajouter(this);

            IdBanque = idBanque;
            DateValeur = dateValeur;
            Intitule = intitule;
            Valeur = valeur;
            No = banque.ListeEcritureBanque.Count + 1;
            Compte = banque.Societe.CompteBase;
            Banque = banque;
            InitLigneBanque(new LigneBanque(this));
        }

        private void InitLigneBanque(LigneBanque lb)
        {
            lb.DateValeur = DateValeur;
            lb.Description = Intitule;
            lb.Valeur = Valeur;
            lb.Compte = Compte;
        }

        [Propriete]
        public override int No
        {
            get { return base.No; }
            set { base.No = value; }
        }

        private Boolean _Pointer = false;
        [Propriete]
        public Boolean Pointer
        {
            get { return _Pointer; }
            set { Set(ref _Pointer, value, this); }
        }

        private int? _Id_Banque = null;
        private Banque _Banque = null;
        [CleEtrangere]
        public Banque Banque
        {
            get
            {
                if (_Banque == null)
                    _Banque = Bdd.Parent<Banque, EcritureBanque>(this);

                return _Banque;
            }
            set
            {
                if (SetObjetGestion(ref _Banque, value, this))
                    _Banque.ListeEcritureBanque.Ajouter(this);
            }
        }

        private String _IdBanque = "";
        [Propriete]
        public String IdBanque
        {
            get { return _IdBanque; }
            set { _IdBanque = value; }
        }

        private DateTime _DateValeur = DateTime.Now;
        // Ne pas modifier cette attribut, cela conditionne le calcul
        // du solde intermédiaire de chaque ecriture
        [Propriete, Tri(DirectionTri = System.ComponentModel.ListSortDirection.Ascending)]
        public DateTime DateValeur
        {
            get { return _DateValeur; }
            set { Set(ref _DateValeur, value, this); }
        }

        private String _Intitule = "";
        [Propriete]
        public String Intitule
        {
            get { return _Intitule; }
            set { Set(ref _Intitule, value, this); }
        }

        private Boolean _AfficherDescription;
        public Boolean AfficherDescription
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(Description))
                    _AfficherDescription = true;

                return _AfficherDescription;
            }
            set
            {
                Set(ref _AfficherDescription, value, this);
            }
        }

        private String _Description = "";
        [Propriete]
        public String Description
        {
            get { return _Description; }
            set { Set(ref _Description, value, this); }
        }

        private ListeObservable<Groupe> _ListeGroupe = null;
        public ListeObservable<Groupe> ListeGroupe
        {
            get
            {
                if (_ListeGroupe == null)
                    _ListeGroupe = Banque.Societe.ListeGroupe;

                return _ListeGroupe;
            }

            set
            {
                SetListe(ref _ListeGroupe, value);
            }
        }

        private Boolean EditionGroupe = false;

        private Groupe _Groupe = null;
        public Groupe Groupe
        {
            get
            {
                if(_Groupe == null && Compte != null)
                    _Groupe = Compte.Groupe;

                return _Groupe;
            }
            set
            {
                if (EditionGroupe) return;

                EditionGroupe = true;

                if (SetObjetGestion(ref _Groupe, value, this))
                {
                    ListeCompte = _Groupe.ListeCompte;
                    Compte = ListeCompte[0];

                    Log.Message("Set compte : " + Compte.Nom);
                }

                EditionGroupe = false;
            }
        }

        private ListeObservable<Compte> _ListeCompte = null;
        public ListeObservable<Compte> ListeCompte
        {
            get
            {
                if (_ListeCompte == null)
                    _ListeCompte = Groupe.ListeCompte;

                return _ListeCompte;
            }

            set
            {
                SetListe(ref _ListeCompte, value);
            }
        }

        private Boolean EditionCompte = false;

        private int? _Id_Compte = null;
        private Compte _Compte = null;
        // Le champ peut être NULL, donc aucune contrainte de base
        [CleEtrangere(Contrainte = ""), ForcerCopie]
        public Compte Compte
        {
            get
            {
                if (_Compte == null)
                    _Compte = Bdd.Parent<Compte, EcritureBanque>(this);

                return _Compte;
            }
            set
            {
                if (EditionCompte) return;

                EditionCompte = true;

                if (SetObjetGestion(ref _Compte, value, this) && ListeLigneBanque.Count > 0)
                {
                    if (Groupe != _Compte.Groupe)
                        Groupe = _Compte.Groupe;

                    if (!Ventiler)
                        ListeLigneBanque[0].Compte = _Compte;
                }

                EditionCompte = false;
            }
        }

        private Double _Valeur = 0;
        [Propriete]
        public Double Valeur
        {
            get { return _Valeur; }
            set { Set(ref _Valeur, value, this); }
        }

        private Double _Solde = Double.NaN;
        [Propriete]
        public Double Solde
        {
            get
            {
                if (Double.IsNaN(_Solde))
                    CalculerSolde();

                return _Solde;
            }
            set
            {
                Set(ref _Solde, value, this);
            }
        }

        public void CalculerSolde(Boolean MajProp = false)
        {
            if (!EstCharge) return;

            Double Val = Double.NaN;
            var index = Banque.ListeEcritureBanque.IndexOf(this);
            if (index > 0)
                Val = Banque.ListeEcritureBanque[index - 1].Solde + Valeur;
            else
                Val = Banque.SoldeInitial + Valeur;

            Val = ArrondiEuro(Val);

            if (MajProp)
                Solde = Val;
            else
                _Solde = Val;

            if (index == (Banque.ListeEcritureBanque.Count - 1))
                Banque.Solde = Val;
        }

        protected Boolean _Ventiler = false;
        [Propriete]
        public Boolean Ventiler
        {
            get { return _Ventiler; }
            set
            {
                if (value)
                {
                    Set(ref _Ventiler, value, this);
                    Compta = false;
                    if (_ListeLigneBanque != null && _ListeLigneBanque.Count == 1)
                    {
                        _ListeLigneBanque[0].Description = "";
                        _ListeLigneBanque[0].Compte = Banque.Societe.CompteBase;
                        _ListeLigneBanque[0].Compta = false;
                        var lb = new LigneBanque(this);
                    }
                }
                else
                {
                    if (_ListeLigneBanque != null && value != _Ventiler)
                    {
                        if (MessageBox.Show("Voulez vous vraiement annuler la ventilation", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            Set(ref _Ventiler, value, this);

                            while (_ListeLigneBanque.Count > 1)
                                _ListeLigneBanque[1].Supprimer();

                            _ListeLigneBanque[0].Description = Intitule;
                            _ListeLigneBanque[0].Compte = Compte;
                            _ListeLigneBanque[0].Valeur = Valeur;
                            _ListeLigneBanque[0].Compta = false;
                            _ListeLigneBanque[0].Ventiler = false;
                        }
                    }
                    else
                        Set(ref _Ventiler, value, this);
                }
            }
        }

        protected Boolean _Compta = false;
        [Propriete]
        public Boolean Compta
        {
            get { return _Compta; }
            set
            {
                Set(ref _Compta, value, this);
                if (EstCharge)
                {
                    if (value)
                    {
                        Ventiler = false;
                        ListeLigneBanque[0].Compta = true;
                    }
                    else if (!value && !Ventiler)
                        ListeLigneBanque[0].Compta = false;
                }
            }
        }

        public void ControlerVentilation(Boolean Hit = true)
        {
            if (!EstCharge || !Ventiler || (_ListeLigneBanque == null)) return;

            Double Somme = 0;
            foreach (var lb in ListeLigneBanque)
                Somme += lb.Valeur;

            if (Hit)
                VerifVentilation = Somme;
            else
                _VerifVentilation = Somme;
        }

        private Double _VerifVentilation = 0;
        public Double VerifVentilation
        {
            get
            {
                ControlerVentilation(false);
                return _VerifVentilation;
            }
            set
            {
                Set(ref _VerifVentilation, value, this);
            }
        }

        private ListeObservable<LigneBanque> _ListeLigneBanque = null;
        [ListeObjetGestion]
        public ListeObservable<LigneBanque> ListeLigneBanque
        {
            get
            {
                if (_ListeLigneBanque == null)
                    _ListeLigneBanque = Bdd.Enfants<LigneBanque, EcritureBanque>(this);

                return _ListeLigneBanque;
            }

            set
            {
                SetListe(ref _ListeLigneBanque, value);
            }
        }

        public override Boolean Supprimer()
        {
            if (!EstCharge) return false;

            while (ListeLigneBanque.Count > 0)
                ListeLigneBanque[0].Supprimer();


            if (Banque != null)
                Banque.ListeEcritureBanque.Remove(this);

            Bdd.Supprimer(this);

            return true;
        }
    }
}
