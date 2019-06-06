using LogDebugging;
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

            Compte = Societe.CompteBase;
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

        private ListeObservable<Groupe> _ListeGroupe = null;
        public ListeObservable<Groupe> ListeGroupe
        {
            get
            {
                if (_ListeGroupe == null)
                    _ListeGroupe = Societe.ListeGroupe;

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

        public override Boolean Supprimer()
        {
            if (!EstCharge) return false;

            if (Societe != null)
                Societe.ListeLigneCompta.Remove(this);

            Bdd.Supprimer(this);

            return true;
        }
    }
}
