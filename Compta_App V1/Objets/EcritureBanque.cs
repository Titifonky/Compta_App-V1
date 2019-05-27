using System;

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

            LigneBanque lb = new LigneBanque(this);

            IdBanque = idBanque;
            DateValeur = dateValeur;
            Intitule = intitule;
            Valeur = valeur;

            lb.DateValeur = dateValeur;
            lb.Description = intitule;
            lb.Valeur = valeur;
        }

        [Propriete]
        public override int No
        {
            get { return base.No; }
            set { base.No = value; }
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

        protected String _Description = "";
        [Propriete]
        public String Description
        {
            get { return _Description; }
            set { Set(ref _Description, value, this); }
        }

        protected Groupe _GroupeCompte = null;
        // Le champ peut être NULL, donc aucune contrainte de base
        [CleEtrangere(Contrainte = ""), ForcerCopie]
        public Groupe GroupeCompte
        {
            get
            {
                if (_GroupeCompte == null)
                    _GroupeCompte = Bdd.Parent<Groupe, EcritureBanque>(this);

                return _GroupeCompte;
            }
            set
            {
                Set(ref _GroupeCompte, value, this);
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
            }
        }

        protected Double _Valeur = 0;
        [Propriete]
        public Double Valeur
        {
            get { return _Valeur; }
            set { Set(ref _Valeur, value, this); }
        }

        protected Boolean _Ventiler = false;
        [Propriete]
        public Boolean Ventiler
        {
            get { return _Ventiler; }
            set { Set(ref _Ventiler, value, this); }
        }

        protected Boolean _Unique = false;
        [Propriete]
        public Boolean Unique
        {
            get { return _Unique; }
            set { Set(ref _Unique, value, this); }
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
