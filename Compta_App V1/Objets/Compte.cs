using System;

namespace Compta
{
    public class Compte : ObjetGestion
    {
        public Compte() { }

        public Compte(Groupe groupe)
        {
            Groupe = groupe;
            Bdd.Ajouter(this);
        }

        [Propriete, Max, Tri(Modifiable=true)]
        public override int No
        {
            get { return base.No; }
            set { base.No = value; }
        }

        private Groupe _Groupe = null;
        [CleEtrangere]
        public Groupe Groupe
        {
            get
            {
                if (_Groupe == null)
                    _Groupe = Bdd.Parent<Groupe, Compte>(this);

                return _Groupe;
            }
            set
            {
                Set(ref _Groupe, value, this);
                if (_Groupe.ListeCompte != null)
                    _Groupe.ListeCompte.Add(this);
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

        private Double _Solde = 0;
        [Propriete]
        public Double Solde
        {
            get { return _Solde; }
            set { Set(ref _Solde, value, this); }
        }

        private ListeObservable<LigneCompta> _ListeLigneCompta = null;
        public ListeObservable<LigneCompta> ListeLigneCompta
        {
            get
            {
                if (_ListeLigneCompta == null)
                    _ListeLigneCompta = Bdd.Enfants<LigneCompta, Compte>(this);

                return _ListeLigneCompta;
            }
        }

        public override Boolean Supprimer()
        {
            if (!EstCharge) return false;

            Bdd.Supprimer<Compte>(this);
            return true;
        }
    }
}
