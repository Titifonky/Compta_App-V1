using LogDebugging;
using System;

namespace Compta
{

    public class LigneBanque : Ligne
    {
        public LigneBanque() { }

        public LigneBanque(EcritureBanque ecritureBanque)
        {
            EcritureBanque = ecritureBanque;

            Bdd.Ajouter(this);

            No = EcritureBanque.ListeLigneBanque.Count + 1;

            Compte = ecritureBanque.Banque.Societe.CompteBase;
        }

        private EcritureBanque _EcritureBanque = null;
        [CleEtrangere]
        public EcritureBanque EcritureBanque
        {
            get
            {
                if (_EcritureBanque == null)
                    _EcritureBanque = Bdd.Parent<EcritureBanque, LigneBanque>(this);

                return _EcritureBanque;
            }
            set
            {
                Set(ref _EcritureBanque, value, this);
                if (_EcritureBanque.ListeLigneBanque != null)
                    _EcritureBanque.ListeLigneBanque.Add(this);
            }
        }

        private ListeObservable<Groupe> _ListeGroupe = null;
        public ListeObservable<Groupe> ListeGroupe
        {
            get
            {
                if (_ListeGroupe == null)
                    _ListeGroupe = EcritureBanque.Banque.Societe.ListeGroupe;

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
                    _Compte = Bdd.Parent<Compte, LigneBanque>(this);

                return _Compte;
            }
            set
            {
                Set(ref _Compte, value, this);
            }
        }

        public override void Calculer()
        {
            EcritureBanque.ControlerVentilation();
        }

        public override bool Supprimer()
        {
            if (!EstCharge) return false;

            if (EcritureBanque != null)
                EcritureBanque.ListeLigneBanque.Remove(this);

            Bdd.Supprimer(this);

            return true;
        }
    }
}
