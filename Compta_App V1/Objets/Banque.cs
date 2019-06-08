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

        protected Boolean _Editer = false;
        public Boolean Editer
        {
            get { return _Editer; }
            set { Set(ref _Editer, value, this); }
        }

        private ListeObservable<EcritureBanque> _ListeEcritureBanque = null;
        public ListeObservable<EcritureBanque> ListeEcritureBanque
        {
            get
            {
                if (_ListeEcritureBanque == null)
                {
                    _ListeEcritureBanque = new ListeObservable<EcritureBanque>();
                    var ListTmp = Bdd.Enfants<EcritureBanque, Banque>(this);
                    var SsTmp = new SortedSet<EcritureBanque>(Comparer<EcritureBanque>.Create(
                        delegate (EcritureBanque a, EcritureBanque b)
                        {
                            if (a.DateValeur < b.DateValeur)
                                return -1;
                            else if (a.DateValeur == b.DateValeur)
                                return 0;

                            return 1;

                        }
                        ));
                    foreach (var ec in ListTmp)
                        SsTmp.Add(ec);

                    foreach (var ec in SsTmp)
                        _ListeEcritureBanque.Add(ec);
                }

                return _ListeEcritureBanque;
            }

            set
            {
                Set(ref _ListeEcritureBanque, value);
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
