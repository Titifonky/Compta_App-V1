using System;
using System.ComponentModel;

namespace Compta
{
    public class Categorie : ObjetGestion
    {
        public Categorie() { }

        public Categorie(Societe s)
        {
            Societe = s;
            Bdd.Ajouter(this);
        }

        private Societe _Societe = null;
        [CleEtrangere]
        public Societe Societe
        {
            get
            {
                if (_Societe == null)
                    _Societe = Bdd.Parent<Societe, Categorie>(this);

                return _Societe;
            }
            set
            {
                Set(ref _Societe, value, this);
                if (_Societe.ListeCategorie != null)
                    _Societe.ListeCategorie.Add(this);
            }
        }

        [Propriete, Max, Tri(Modifiable=true)]
        public override int No
        {
            get { return base.No; }
            set { base.No = value; }
        }

        private String _Nom = "";
        [Propriete]
        public String Nom
        {
            get { return _Nom; }
            set { Set(ref _Nom, value, this); }
        }

        private String _Description = "";
        [Propriete]
        public String Description
        {
            get { return _Description; }
            set { Set(ref _Description, value, this); }
        }

        private ListeObservable<Compte> _ListeCompte = null;
        public ListeObservable<Compte> ListeCompte
        {
            get
            {
                if (_ListeCompte == null)
                    _ListeCompte = Bdd.Enfants<Compte, Categorie>(this);

                return _ListeCompte;
            }
        }

        public override Boolean Supprimer()
        {
            if (!EstCharge) return false;

            SupprimerListe(_ListeCompte);

            Societe.ListeCategorie.Remove(this);

            Bdd.Supprimer<Categorie>(this);
            return true;
        }
    }
}
