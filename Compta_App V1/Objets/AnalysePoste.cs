using LogDebugging;
using System;

namespace Compta
{
    public class AnalysePoste : ObjetWpf
    {
        public AnalysePoste(Poste poste)
        {
            _Poste = poste;
        }

        private AnalyseChantier _AnalyseChantier = null;
        public AnalyseChantier AnalyseChantier
        {
            get
            {
                return _AnalyseChantier;
            }
            set
            {
                SetWpf(ref _AnalyseChantier, value);
            }
        }

        private Poste _Poste = null;
        public Poste Poste
        {
            get
            {
                return _Poste;
            }
            set
            {
                SetWpf(ref _Poste, value);
            }
        }

        private Double _Achat = 0;
        [Propriete]
        public Double Achat
        {
            get { return _Achat; }
            set
            {
                if(SetWpf(ref _Achat, value))
                { }
            }
        }

        private Double _HeureQte = 0;
        [Propriete]
        public Double HeureQte
        {
            get { return _HeureQte; }
            set { SetWpf(ref _HeureQte, value); }
        }

        private Double _HeureMt = 0;
        [Propriete]
        public Double HeureMt
        {
            get { return _HeureMt; }
            set
            {
                if(SetWpf(ref _HeureMt, value))
                { }
            }
        }

        private Double _Depense = 0;
        public Double Depense
        {
            get { return _Depense; }
            set
            {
                if(SetWpf(ref _Depense, value))
                {   }
            }
        }

        public void AjouterAchat(Achat achat)
        {
            _Achat += achat.Montant;
            _Depense += achat.Montant;
            ListeAchat.Add(achat);
        }

        public void AjouterHeure(Heure heure)
        {
            _HeureQte += heure.Qte;
            _HeureMt += heure.Montant;
            _Depense += heure.Montant;
            ListeHeure.Add(heure);
        }

        private ListeObservable<Achat> _ListeAchat = null;
        public ListeObservable<Achat> ListeAchat
        {
            get
            {
                if (_ListeAchat == null)
                    _ListeAchat = new ListeObservable<Achat>();

                return _ListeAchat;
            }

            set
            {
                SetListeWpf(ref _ListeAchat, value);
            }
        }

        private ListeObservable<Heure> _ListeHeure = null;
        public ListeObservable<Heure> ListeHeure
        {
            get
            {
                if (_ListeHeure == null)
                    _ListeHeure = new ListeObservable<Heure>();

                return _ListeHeure;
            }

            set
            {
                SetListeWpf(ref _ListeHeure, value);
            }
        }
    }
}
