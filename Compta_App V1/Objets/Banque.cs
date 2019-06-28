using LogDebugging;
using System;
using System.Collections.Generic;
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
            Bdd.Ajouter(this);

            Societe = s;
        }

        private int? _Id_Societe = null;
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
                if (SetObjetGestion(ref _Societe, value, this))
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

        private Double _SoldeInitial = 0;
        [Propriete]
        public Double SoldeInitial
        {
            get { return _SoldeInitial; }
            set
            {
                Set(ref _SoldeInitial, value, this);
                if (EstCharge)
                    CalculerSolde();
            }
        }

        public void CalculerSolde()
        {
            foreach (var ec in ListeEcritureBanque)
                ec.CalculerSolde(true);
        }

        private Double _Solde = 0;
        [Propriete]
        public Double Solde
        {
            get
            {
                return _Solde;
            }
            set
            {
                Set(ref _Solde, value, this);
            }
        }

        // Pour tester si la liste est triée
        private ListeObservable<EcritureBanque> _ListeEcritureBanque = null;
        [ListeObjetGestion]
        public ListeObservable<EcritureBanque> ListeEcritureBanque
        {
            get
            {
                if (_ListeEcritureBanque == null)
                    _ListeEcritureBanque = Bdd.Enfants<EcritureBanque, Banque>(this);

                if (!_ListeEcritureBanque.OptionsCharges)
                {
                    _ListeEcritureBanque.TrierAsc += (a, b) => { return a.DateValeur.CompareTo(b.DateValeur); };
                    _ListeEcritureBanque.OptionsCharges = true;
                }

                return _ListeEcritureBanque;
            }

            set
            {
                SetListe(ref _ListeEcritureBanque, value);
            }
        }

        public override Boolean Supprimer()
        {
            if (!EstCharge) return false;

            while (ListeEcritureBanque.Count > 0)
                ListeEcritureBanque[0].Supprimer();

            if (Societe != null)
                Societe.ListeBanque.Remove(this);

            Bdd.Supprimer(this);

            return true;
        }
    }
}
