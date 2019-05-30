using System;

namespace Compta
{

    public abstract class Ligne : ObjetGestion
    {
        public Ligne() { }

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

        protected DateTime _DateValeur = DateTime.Now;
        [Propriete, Tri]
        public DateTime DateValeur
        {
            get { return _DateValeur; }
            set { Set(ref _DateValeur, value, this); }
        }

        protected Compte _Compte = null;
        // Le champ peut être NULL, donc aucune contrainte de base
        [CleEtrangere(Contrainte = ""), ForcerCopie]
        public abstract Compte Compte { get; set; }

        protected String _Description = "";
        [Propriete]
        public String Description
        {
            get { return _Description; }
            set { Set(ref _Description, value, this); }
        }

        public virtual void Calculer() { }

        protected Double _Valeur = 0;
        [Propriete]
        public Double Valeur
        {
            get { return _Valeur; }
            set
            {
                Set(ref _Valeur, value, this);
                Calculer();
            }
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
