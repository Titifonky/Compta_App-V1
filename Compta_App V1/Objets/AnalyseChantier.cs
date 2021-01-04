using LogDebugging;
using System;
using System.Windows;

namespace Compta
{
    public class AnalyseChantier : ObjetWpf
    {
        public AnalyseChantier(Chantier chantier)
        {
            _Chantier = chantier;
        }

        private AnalysePeriode _AnalysePeriode = null;
        public AnalysePeriode AnalysePeriode
        {
            get
            {
                return _AnalysePeriode;
            }
            set
            {
                SetWpf(ref _AnalysePeriode, value);
            }
        }

        private Chantier _Chantier = null;
        public Chantier Chantier
        {
            get
            {
                return _Chantier;
            }
            set
            {
                SetWpf(ref _Chantier, value);
            }
        }

        private Double _Achat = 0;
        public Double Achat
        {
            get { return _Achat; }
            set { SetWpf(ref _Achat, value); }
        }

        private Double _HeureQte = 0;
        public Double HeureQte
        {
            get { return _HeureQte; }
            set
            {
                if (SetWpf(ref _HeureQte, value))
                {
                }
            }
        }

        private Double _HeureMt = 0;
        public Double HeureMt
        {
            get { return _HeureMt; }
            set { SetWpf(ref _HeureMt, value); }
        }

        private Double _Depense = 0;
        public Double Depense
        {
            get { return _Depense; }
            set
            {
                if(SetWpf(ref _Depense, value))
                {
                }
            }
        }

        public void AjouterAnalysePoste(AnalysePoste analysePoste)
        {
            _Achat += analysePoste.Achat;
            _HeureQte += analysePoste.HeureQte;
            _HeureMt += analysePoste.HeureMt;
            _Depense += analysePoste.Depense;
            ListeAnalysePoste.Add(analysePoste);
        }

        private ListeObservable<AnalysePoste> _ListeAnalysePoste = null;
        public ListeObservable<AnalysePoste> ListeAnalysePoste
        {
            get
            {
                if (_ListeAnalysePoste == null)
                    _ListeAnalysePoste = new ListeObservable<AnalysePoste>();

                return _ListeAnalysePoste;
            }

            set
            {
                SetListeWpf(ref _ListeAnalysePoste, value);
            }
        }
    }
}
