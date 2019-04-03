using System;
using System.ComponentModel;

namespace Compta
{
    public class Compte : ObjetGestion
    {
        public Compte() { }

        public Compte(Categorie c)
        {
            Categorie = c;
            Bdd.Ajouter(this);
        }

        private Categorie _Categorie = null;
        [CleEtrangere]
        public Categorie Categorie
        {
            get
            {
                if (_Categorie == null)
                    _Categorie = Bdd.Parent<Categorie, Compte>(this);

                return _Categorie;
            }
            set
            {
                Set(ref _Categorie, value, this);
                if (_Categorie.ListeCompte != null)
                    _Categorie.ListeCompte.Add(this);
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

        public override Boolean Supprimer()
        {
            if (!EstCharge) return false;

            Bdd.Supprimer<Compte>(this);
            return true;
        }
    }
}
