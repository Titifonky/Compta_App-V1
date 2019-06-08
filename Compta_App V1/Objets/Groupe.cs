using System;
using System.ComponentModel;

namespace Compta
{
    public class Groupe : ObjetGestion
    {
        public Groupe() { }

        public Groupe(Societe societe)
        {
            Bdd.Ajouter(this);

            Societe = societe;
            Compte C = new Compte(this);
            C.Nom = "-";
        }

        [Propriete, Max, Tri(Modifiable = true)]
        public override int No
        {
            get { return base.No; }
            set { base.No = value; }
        }

        private Societe _Societe = null;
        [CleEtrangere]
        public Societe Societe
        {
            get
            {
                if (_Societe == null)
                    _Societe = Bdd.Parent<Societe, Groupe>(this);

                return _Societe;
            }
            set
            {
                if (SetObjetGestion(ref _Societe, value, this))
                    _Societe.ListeGroupe.Add(this);
            }
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
                    _ListeCompte = Bdd.Enfants<Compte, Groupe>(this);

                return _ListeCompte;
            }
            set
            {
                Set(ref _ListeCompte, value);
            }
        }

        public override Boolean Supprimer()
        {
            if (!EstCharge) return false;

            SupprimerListe(_ListeCompte);

            Societe.ListeGroupe.Remove(this);

            Bdd.Supprimer<Groupe>(this);
            return true;
        }
    }
}
