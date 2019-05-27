using System;

namespace Compta
{

    public class LigneCompta : ObjetGestion
    {
        public LigneCompta() { }

        public LigneCompta(Societe societe)
        {
            Societe = societe;

            Bdd.Ajouter(this);

            No = Societe.ListeLigneCompta.Count + 1;
        }

        [Propriete]
        public override int No
        {
            get { return base.No; }
            set { base.No = value; }
        }

        private Societe _Societe = null;
        [CleEtrangere]
        public Societe Societe
        {
            get
            {
                if (_Societe == null)
                    _Societe = Bdd.Parent<Societe, LigneCompta>(this);

                return _Societe;
            }
            set
            {
                Set(ref _Societe, value, this);
                if (_Societe.ListeLigneCompta != null)
                    _Societe.ListeLigneCompta.Add(this);
            }
        }

        protected Boolean _Pointer = false;
        [Propriete]
        public Boolean Pointer
        {
            get { return _Pointer; }
            set { Set(ref _Pointer, value, this); }
        }

        protected Boolean _Unique = false;
        [Propriete]
        public Boolean Unique
        {
            get { return _Unique; }
            set { Set(ref _Unique, value, this); }
        }

        protected DateTime _DateValeur = DateTime.Now;
        [Propriete, Tri]
        public DateTime DateValeur
        {
            get { return _DateValeur; }
            set { Set(ref _DateValeur, value, this); }
        }

        protected DateTime _DateCompta = DateTime.Now;
        [Propriete]
        public DateTime DateCompta
        {
            get { return _DateCompta; }
            set { Set(ref _DateCompta, value, this); }
        }

        protected Groupe _Groupe = null;
        // Le champ peut être NULL, donc aucune contrainte de base
        [CleEtrangere(Contrainte = ""), ForcerCopie]
        public Groupe Groupe
        {
            get
            {
                if (_Groupe == null)
                    _Groupe = Bdd.Parent<Groupe, LigneCompta>(this);

                return _Groupe;
            }
            set
            {
                Set(ref _Groupe, value, this);
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
                    _Compte = Bdd.Parent<Compte, LigneCompta>(this);

                return _Compte;
            }
            set
            {
                Set(ref _Compte, value, this);
            }
        }

        protected String _Description = "";
        [Propriete]
        public String Description
        {
            get { return _Description; }
            set { Set(ref _Description, value, this); }
        }

        protected Double _Valeur = 0;
        [Propriete]
        public Double Valeur
        {
            get { return _Valeur; }
            set { Set(ref _Valeur, value, this); }
        }

        protected Double _TvaPct = 20;
        [Propriete]
        public Double TvaPct
        {
            get { return _TvaPct; }
            set { Set(ref _TvaPct, value, this); }
        }

        protected Double _Tva = 0;
        [Propriete]
        public Double Tva
        {
            get { return _Tva; }
            set { Set(ref _Tva, value, this); }
        }
    }
}
