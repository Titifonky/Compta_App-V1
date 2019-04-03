using System;
using System.Collections.ObjectModel;
using System.Data;

namespace Compta
{
    [ForcerAjout]
    public class Banque : ObjetGestion
    {
        public Banque() { }

        public Banque(Societe s)
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
                    _Societe = Bdd.Parent<Societe, Banque>(this);

                return _Societe;
            }
            set
            {
                Set(ref _Societe, value, this);
                if (_Societe.ListeBanque != null)
                    _Societe.ListeBanque.Add(this);
            }
        }

        private String _Nom = "";
        [Propriete]
        public String Nom
        {
            get { return _Nom; }
            set { Set(ref _Nom, value, this); }
        }

        private Double _Solde_initial = 0;
        [Propriete]
        public Double Solde_initial
        {
            get { return _Solde_initial; }
            set { Set(ref _Solde_initial, value, this); }
        }
    }
}
