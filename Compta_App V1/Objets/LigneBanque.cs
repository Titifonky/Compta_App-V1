using LogDebugging;
using System;

namespace Compta
{

    public class LigneBanque : Ligne
    {
        public LigneBanque() { }

        public LigneBanque(EcritureBanque ecritureBanque)
        {
            Bdd2.Ajouter(this);

            No = ecritureBanque.ListeLigneBanque.Count + 1;
            EcritureBanque = ecritureBanque;
            Compte = ecritureBanque.Banque.Societe.CompteBase;
            Ventiler = this.Ventiler;
        }

        private int? _Id_EcritureBanque = null;
        private EcritureBanque _EcritureBanque = null;
        [CleEtrangere]
        public EcritureBanque EcritureBanque
        {
            get
            {
                if (_EcritureBanque == null)
                    _EcritureBanque = Bdd2.Parent<EcritureBanque, LigneBanque>(this);

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

        private Boolean EditionGroupe = false;

        protected Groupe _Groupe = null;
        public Groupe Groupe
        {
            get
            {
                if (_Groupe == null && Compte != null)
                    _Groupe = Compte.Groupe;

                return _Groupe;
            }
            set
            {
                if (EditionGroupe) return;

                EditionGroupe = true;
                
                if (SetObjetGestion(ref _Groupe, value, this))
                {
                    ListeCompte = _Groupe.ListeCompte;
                    Compte = ListeCompte[0];
                }

                EditionGroupe = false;
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

        private Boolean EditionCompte = false;

        // Le champ peut être NULL, donc aucune contrainte de base
        [CleEtrangere(Contrainte = ""), ForcerCopie]
        public override Compte Compte
        {
            get
            {
                if (_Compte == null)
                    _Compte = Bdd2.Parent<Compte, LigneBanque>(this);

                return _Compte;
            }
            set
            { 
                if (EditionCompte) return;

                EditionCompte = true;

                // Ancien compte
                if (_Compte != null && (_Compte != value) && _Compte.EstCharge && EstCharge )
                {
                    _Compte.ListeLigneBanque.Supprimer(this);
                    _Compte.Calculer();
                }

                // Nouveau compte
                if (SetObjetGestion(ref _Compte, value, this))
                {
                    _Compte.ListeLigneBanque.Ajouter(this);
                    _Compte.Calculer();

                    if (Groupe != _Compte.Groupe)
                        Groupe = _Compte.Groupe;

                    if (!Ventiler)
                        EcritureBanque.Compte = _Compte;
                }

                EditionCompte = false;
            }
        }

        protected Boolean _Compta = false;
        [Propriete]
        public Boolean Compta
        {
            get
            {
                if (EstCharge && !EcritureBanque.Ventiler)
                {
                    _Compta = EcritureBanque.Compta;
                    Set(ref _Compta, _Compta, this);
                }

                return _Compta;
            }
            set
            {
                if (EstCharge && !EcritureBanque.Ventiler)
                    value = EcritureBanque.Compta;

                Boolean calculer = false;

                if (_Compta != value) calculer = true;

                Set(ref _Compta, value, this);

                if (calculer) Compte.Calculer();
            }
        }

        protected Boolean _Ventiler = false;
        [Propriete]
        public Boolean Ventiler
        {
            get { return _Ventiler; }
            set { Set(ref _Ventiler, value, this); }
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

            Bdd2.Supprimer(this);

            return true;
        }
    }
}
