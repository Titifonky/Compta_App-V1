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
            Banque = banque;

            Bdd.Ajouter(this);

            No = Banque.ListeEcritureBanque.Count + 1;

            IdBanque = idBanque;
            DateValeur = dateValeur;
            Intitule = intitule;
            Valeur = valeur;
            Compte = Banque.Societe.CompteBase;

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

        protected Boolean _Pointer = false;
        [Propriete]
        public Boolean Pointer
        {
            get { return _Pointer; }
            set { Set(ref _Pointer, value, this); }
        }

        protected Banque _Banque = null;
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
                Set(ref _Banque, value, this);
                if (_Banque.ListeEcritureBanque != null)
                    _Banque.ListeEcritureBanque.Add(this);
            }
        }

        protected String _IdBanque = "";
        [Propriete]
        public String IdBanque
        {
            get { return _IdBanque; }
            set { _IdBanque = value; }
        }

        protected DateTime _DateValeur = DateTime.Now;
        [Propriete, Tri]
        public DateTime DateValeur
        {
            get { return _DateValeur; }
            set { Set(ref _DateValeur, value, this); }
        }

        protected String _Intitule = "";
        [Propriete]
        public String Intitule
        {
            get { return _Intitule; }
            set { Set(ref _Intitule, value, this); }
        }

        protected Boolean _AfficherDescription;
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

        protected String _Description = "";
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
                Set(ref _ListeGroupe, value);
            }
        }

        protected Groupe _Groupe = null;
        public Groupe Groupe
        {
            get
            {
                _Groupe = Compte.Groupe;
                return _Groupe;
            }
            set
            {
                try
                {
                    Set(ref _Groupe, value, this);
                    ListeCompte = _Groupe.ListeCompte;
                    if (ListeCompte.Count > 0)
                        Compte = ListeCompte[0];
                    else
                        Compte = null;
                }
                catch (Exception e)
                {
                    Log.Message(e.ToString());
                }
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
                Set(ref _ListeCompte, value);
            }
        }

        protected Compte _Compte = null;
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
                Set(ref _Compte, value, this);

                if (_ListeLigneBanque != null && _ListeLigneBanque.Count > 0 && Ventiler == false && Pointer == true)
                    _ListeLigneBanque[0].Compte = _Compte;
            }
        }

        protected Double _Valeur = 0;
        [Propriete]
        public Double Valeur
        {
            get { return _Valeur; }
            set { Set(ref _Valeur, value, this); }
        }

        protected Double _Solde = Double.NaN;
        public Double Solde
        {
            get
            {
                if (Double.IsNaN(_Solde))
                {
                    var index = Banque.ListeEcritureBanque.IndexOf(this);
                    if (index > 0)
                        _Solde = Banque.ListeEcritureBanque[index - 1].Solde + Valeur;
                    else
                        _Solde = Banque.Solde + Valeur;
                }

                return _Solde;
            }
            set
            {
                //Set(ref _Solde, value, this);
            }
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
                        new LigneBanque(this);
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
                if (value)
                    Ventiler = false;
            }
        }

        public void ControlerVentilation(Boolean Hit = true)
        {
            if ((Ventiler == false) || (_ListeLigneBanque == null)) return;

            Double Somme = 0;
            foreach (var lb in ListeLigneBanque)
                Somme += lb.Valeur;

            if (Hit)
                VerifVentilation = Somme;
            else
                _VerifVentilation = Somme;
        }

        protected Double _VerifVentilation = 0;
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
                Set(ref _ListeLigneBanque, value);
            }
        }

        public override Boolean Supprimer()
        {
            if (!EstCharge) return false;

            if (Banque != null)
                Banque.ListeEcritureBanque.Remove(this);

            Bdd.Supprimer<EcritureBanque>(this);

            return true;
        }
    }
}
