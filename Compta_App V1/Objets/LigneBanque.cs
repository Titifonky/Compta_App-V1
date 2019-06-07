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
                    if (Set(ref _Groupe, value, this) && EstCharge && _Groupe.EstCharge)
                    {
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
                var _OldCompte = _Compte;

                if (value == null) return;

                if (Set(ref _Compte, value, this) && EstCharge && _Compte.EstCharge)
                {
                    if (_OldCompte != null)
                    {
                        _OldCompte.ListeLigneBanque.Supprimer(this);
                        _OldCompte.Calculer();
                    }

                    _Compte.ListeLigneBanque.Ajouter(this);
                    _Compte.Calculer();
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
                if (EstCharge && !EcritureBanque.Ventiler)
                    value = false;

                Set(ref _Compta, value, this);
            }
        }

        public override void Calculer()
        {
            if (!EstCharge) return;

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
