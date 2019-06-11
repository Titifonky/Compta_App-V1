using LogDebugging;
using System;

namespace Compta
{

    public class LigneBanque : Ligne
    {
        public LigneBanque() { }

        public LigneBanque(EcritureBanque ecritureBanque)
        {
            Bdd.Ajouter(this);

            No = ecritureBanque.ListeLigneBanque.Count + 1;
            EcritureBanque = ecritureBanque;
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
                if (SetObjetGestion(ref _EcritureBanque, value, this))
                    _EcritureBanque.ListeLigneBanque.Add(this);
            }
        }

        //public new Boolean Editer
        //{
        //    get
        //    {
        //        if (EstCharge && !EcritureBanque.Ventiler)
        //            _Editer = false;

        //        return _Editer;
        //    }
        //    set
        //    {
        //        if (EstCharge && !EcritureBanque.Ventiler)
        //            value = false;

        //        Set(ref _Editer, value, this);
        //    }
        //}

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
                    _Compte = Bdd.Parent<Compte, LigneBanque>(this);

                return _Compte;
            }
            set
            {
                if (_Compte != null && (_Compte != value) && _Compte.EstCharge && EstCharge )
                {
                    _Compte.ListeLigneBanque.Supprimer(this);
                    _Compte.Calculer();
                }

                if (SetObjetGestion(ref _Compte, value, this))
                {
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
                    value = EcritureBanque.Compta;

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

            EcritureBanque.ListeLigneBanque.Remove(this);
            Compte.ListeLigneBanque.Remove(this);

            Bdd.Supprimer(this);

            return true;
        }
    }
}
