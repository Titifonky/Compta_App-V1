using System;
using System.Collections.ObjectModel;
using System.Data;

namespace Compta
{
    [ForcerAjout]
    public class Societe : ObjetGestion
    {
        public Societe() { Bdd.Ajouter(this); }

        private String _Nom = "";
        [Propriete]
        public String Nom
        {
            get { return _Nom; }
            set { Set(ref _Nom, value, this); }
        }

        private ListeObservable<Categorie> _ListeCategorie = null;
        public ListeObservable<Categorie> ListeCategorie
        {
            get
            {
                if (_ListeCategorie == null)
                    _ListeCategorie = Bdd.Enfants<Categorie, Societe>(this);

                return _ListeCategorie;
            }
        }

        private ListeObservable<Ligne_Banque> _ListeLigne_Banque = null;
        public ListeObservable<Ligne_Banque> ListeLigne_Banque
        {
            get
            {
                if (_ListeLigne_Banque == null)
                    _ListeLigne_Banque = Bdd.Enfants<Ligne_Banque, Societe>(this);

                return _ListeLigne_Banque;
            }
        }

        private ListeObservable<Ligne_Compta> _ListeLigne_Compta = null;
        public ListeObservable<Ligne_Compta> ListeLigne_Compta
        {
            get
            {
                if (_ListeLigne_Compta == null)
                    _ListeLigne_Compta = Bdd.Enfants<Ligne_Compta, Societe>(this);

                return _ListeLigne_Compta;
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
    }
}
