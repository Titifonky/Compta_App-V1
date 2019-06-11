using LogDebugging;
using System;

namespace Compta
{

    public class LigneCompta : Ligne
    {
        public LigneCompta() { }

        public LigneCompta(Societe societe)
        {
            Bdd.Ajouter(this);

            No = societe.ListeLigneCompta.Count + 1;
            Societe = societe;
            Compte = societe.CompteBase;
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
                if (SetObjetGestion(ref _Societe, value, this))
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
                SetListe(ref _ListeGroupe, value);
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
                    if (SetObjetGestion(ref _Groupe, value, this))
                    {
                        Log.Message("");
                        ListeCompte = _Groupe.ListeCompte;
                        Compte = ListeCompte[0];
                    }
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
                SetListe(ref _ListeCompte, value);
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

                if (_Compte == null)
                    _Compte = Societe.ListeGroupe[0].ListeCompte[0];

                return _Compte;
            }
            set
            {
                if (_Compte != null && (_Compte != value) && _Compte.EstCharge && EstCharge)
                {
                    _Compte.ListeLigneCompta.Supprimer(this);
                    _Compte.Calculer();
                }

                if (SetObjetGestion(ref _Compte, value, this))
                {
                    _Compte.ListeLigneCompta.Ajouter(this);
                    _Compte.Calculer();
                }
            }
        }

        public override void Calculer()
        {
            if (!EstCharge) return;

            Compte.Calculer();
        }

        public override Boolean Supprimer()
        {
            if (!EstCharge) return false;

            Societe.ListeLigneCompta.Remove(this);
            Compte.ListeLigneCompta.Remove(this);


            Bdd.Supprimer(this);

            return true;
        }
    }
}
