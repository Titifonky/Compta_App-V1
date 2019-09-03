using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Data;
using System.Linq;

namespace Compta
{
    [ForcerAjout]
    public class Societe : ObjetGestion
    {
        public Societe()
        {
            Bdd2.Ajouter(this);
        }

        private String _Nom = "";
        [Propriete]
        public String Nom
        {
            get { return _Nom; }
            set { Set(ref _Nom, value, this); }
        }

        public delegate void OnModifyBanqueEventHandler(int id);

        public event OnModifyBanqueEventHandler OnModifyBanque;

        private Banque _BanqueCourante = null;
        public Banque BanqueCourante
        {
            get { return _BanqueCourante; }
            set
            {
                SetListe(ref _BanqueCourante, value);
                OnModifyBanque(_BanqueCourante.Id);
            }
        }

        private Compte _CompteBase = null;
        public Compte CompteBase
        {
            get
            {
                if (_CompteBase == null)
                {
                    var grp = ListeGroupe.Where(c => c.No == 4).First();
                    _CompteBase = grp.ListeCompte.Where(c => c.No == 162).First();
                }

                return _CompteBase;
            }
        }

        private ListeObservable<Groupe> _ListeGroupe = null;
        [ListeObjetGestion]
        public ListeObservable<Groupe> ListeGroupe
        {
            get
            {
                if (_ListeGroupe == null)
                    _ListeGroupe = Bdd2.Enfants<Groupe, Societe>(this);

                if (!_ListeGroupe.OptionsCharges)
                {
                    _ListeGroupe.ItemsNotifyPropertyChanged = true;
                    _ListeGroupe.OptionsCharges = true;
                }

                return _ListeGroupe;
            }
            set
            {
                SetListe(ref _ListeGroupe, value);
            }
        }

        private ListeObservable<Compte> _ListeCompte = null;
        [ListeObjetGestion]
        public ListeObservable<Compte> ListeCompte
        {
            get
            {
                if (_ListeCompte == null)
                    _ListeCompte = Bdd2.Liste<Compte>();

                if (!_ListeCompte.OptionsCharges)
                {
                    _ListeCompte.ItemsNotifyPropertyChanged = true;
                    _ListeCompte.OptionsCharges = true;
                }

                return _ListeCompte;
            }
            set
            {
                SetListe(ref _ListeCompte, value);
            }
        }

        private ListeObservable<LigneCompta> _ListeLigneCompta = null;
        [ListeObjetGestion]
        public ListeObservable<LigneCompta> ListeLigneCompta
        {
            get
            {
                if (_ListeLigneCompta == null)
                    _ListeLigneCompta = Bdd2.Enfants<LigneCompta, Societe>(this);

                return _ListeLigneCompta;
            }
            set
            {
                SetListe(ref _ListeLigneCompta, value);
            }
        }

        private ListeObservable<Banque> _ListeBanque = null;
        [ListeObjetGestion]
        public ListeObservable<Banque> ListeBanque
        {
            get
            {
                if (_ListeBanque == null)
                    _ListeBanque = Bdd2.Enfants<Banque, Societe>(this);

                return _ListeBanque;
            }
            set
            {
                SetListe(ref _ListeBanque, value);
            }
        }
        
    }
}
