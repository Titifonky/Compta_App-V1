using System;
using System.Collections.ObjectModel;
using System.Data;

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

        private ListeObservable<LigneCompta> _ListeLigneCompta = null;
        public ListeObservable<LigneCompta> ListeLigneCompta
        {
            get
            {
                if (_ListeLigneCompta == null)
                    _ListeLigneCompta = Bdd.Enfants<LigneCompta, Societe>(this);

                return _ListeLigneCompta;
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
        }

        private ListeObservable<Groupe> _ListeCompte = null;
        public ListeObservable<Groupe> ListeCompte
        {
            get
            {
                if (_ListeCompte == null)
                    _ListeCompte = Bdd.Enfants<Groupe, Societe>(this);

                return _ListeCompte;
            }
        }
    }
}
