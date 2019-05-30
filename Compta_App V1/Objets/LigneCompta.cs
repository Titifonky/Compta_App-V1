using System;

namespace Compta
{

    public class LigneCompta : Ligne
    {
        public LigneCompta() { }

        public LigneCompta(Societe societe)
        {
            Societe = societe;

            Bdd.Ajouter(this);

            No = Societe.ListeLigneCompta.Count + 1;
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

        protected DateTime _DateCompta = DateTime.Now;
        [Propriete]
        public DateTime DateCompta
        {
            get { return _DateCompta; }
            set { Set(ref _DateCompta, value, this); }
        }

        // Le champ peut être NULL, donc aucune contrainte de base
        [CleEtrangere(Contrainte = ""), ForcerCopie]
        public override Compte Compte
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
    }
}
