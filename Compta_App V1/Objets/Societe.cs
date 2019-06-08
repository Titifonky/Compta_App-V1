using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Data;

namespace Compta
{
    [ForcerAjout]
    public class Societe : ObjetGestion
    {
        public Societe()
        {
            Bdd.Ajouter(this);
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
                Set(ref _BanqueCourante, value);
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
                    Groupe G = null;
                    foreach (var g in ListeGroupe)
                    {
                        if (g.No == 1)
                        {
                            G = g;
                            break;
                        }
                    }

                    if (G != null)
                    {
                        foreach (var c in G.ListeCompte)
                        {
                            if (c.No == 1)
                            {
                                _CompteBase = c;
                                break;
                            }
                        }
                    }
                }

                return _CompteBase;
            }
        }

        private ListeObservable<LigneCompta> _ListeLigneCompta = null;
        public ListeObservable<LigneCompta> ListeLigneCompta
        {
            get
            {
                if (_ListeLigneCompta == null)
                    _ListeLigneCompta = Bdd.Enfants<LigneCompta, Societe>(this);

                return _ListeLigneCompta;
            }
            set
            {
                Set(ref _ListeLigneCompta, value);
            }
        }

        private ListeObservable<Banque> _ListeBanque = null;
        public ListeObservable<Banque> ListeBanque
        {
            get
            {
                if (_ListeBanque == null)
                    _ListeBanque = Bdd.Enfants<Banque, Societe>(this);

                return _ListeBanque;
            }
            set
            {
                Set(ref _ListeBanque, value);
            }
        }

        private ListeObservable<Groupe> _ListeGroupe = null;
        public ListeObservable<Groupe> ListeGroupe
        {
            get
            {
                if (_ListeGroupe == null)
                    _ListeGroupe = Bdd.Enfants<Groupe, Societe>(this);

                return _ListeGroupe;
            }
            set
            {
                Set(ref _ListeGroupe, value);
            }
        }

        private ListeObservable<Compte> _ListeCompte = null;
        public ListeObservable<Compte> ListeCompte
        {
            get
            {
                if (_ListeCompte == null)
                {
                    _ListeCompte = Bdd.Liste<Compte>();
                    _ListeCompte.ItemsNotifyPropertyChanged = true;
                }

                return _ListeCompte;
            }
            set
            {
                Set(ref _ListeCompte, value);
            }
        }
    }
}
