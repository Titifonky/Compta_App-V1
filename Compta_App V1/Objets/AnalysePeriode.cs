using LogDebugging;
using System;
using System.Collections.Generic;

namespace Compta
{
    public class AnalysePeriode : ObjetWpf
    {
        public AnalysePeriode() { }

        public AnalysePeriode(DateTime dateDebut, DateTime dateFin)
        {
            Analyser(dateDebut, dateFin);
        }

        public Boolean Analyser(DateTime dateDebut, DateTime dateFin)
        {
            _ListeAnalyseChantier = new ListeObservable<AnalyseChantier>();

            if (dateFin <= dateDebut) return false;
            
            if((dateFin - dateDebut).TotalDays < 32)
                _DateValeur = String.Format("{0}", dateDebut.ToString("MMM yy")).UpperCaseFirstCharacter();
            else
                _DateValeur = String.Format("de {0} à {1}", dateDebut.ToString("MMM yy"), dateFin.ToString("MMM yy"));

            var format = "yyyy-MM-dd HH:mm:ss";
            var filtre = String.Format("datevaleur >= '{0}' AND datevaleur < '{1}' ",
                                       dateDebut.ToString(format),
                                       dateFin.ToString(format)
                                       );
            var lstAchat = Bdd2.ListeFiltre<Achat>(filtre);
            var lstHeure = Bdd2.ListeFiltre<Heure>(filtre);

            if ((lstAchat.Count + lstHeure.Count) == 0) return false;

            // Permet de précharger les parents et de limiter le nombre de requetes Sql
            var lstPostes = Bdd2.Parents<Poste, Achat>(lstAchat);

            foreach (var poste in Bdd2.Parents<Poste, Heure>(lstHeure))
                lstPostes.Add(poste);

            Bdd2.Parents<Chantier, Poste>(lstPostes);
            // fin du préchargement

            var dicAnalysePoste = new Dictionary<int, AnalysePoste>();

            foreach (var achat in lstAchat)
            {
                var pst = achat.Poste;
                AnalysePoste ap;

                if (dicAnalysePoste.ContainsKey(pst.Id))
                    ap = dicAnalysePoste[pst.Id];
                else
                    dicAnalysePoste.Add(pst.Id, ap = new AnalysePoste(pst));

                ap.AjouterAchat(achat);
            }

            foreach (var heure in lstHeure)
            {
                var pst = heure.Poste; ;

                AnalysePoste ap;

                if (dicAnalysePoste.ContainsKey(pst.Id))
                    ap = dicAnalysePoste[pst.Id];
                else
                    dicAnalysePoste.Add(pst.Id, ap = new AnalysePoste(pst));

                ap.AjouterHeure(heure);
            }

            var dicAnalyseChantier = new Dictionary<int, AnalyseChantier>();

            foreach (var analysePoste in dicAnalysePoste.Values)
            {
                var cht = analysePoste.Poste.Chantier;

                AnalyseChantier ac;

                if (dicAnalyseChantier.ContainsKey(cht.Id))
                    ac = dicAnalyseChantier[cht.Id];
                else
                    dicAnalyseChantier.Add(cht.Id, ac = new AnalyseChantier(cht));

                analysePoste.AnalyseChantier = ac;
                ac.AjouterAnalysePoste(analysePoste);
            }

            foreach (var analyseChantier in dicAnalyseChantier.Values)
            {
                analyseChantier.AnalysePeriode = this;
                this.AjouterAnalyseChantier(analyseChantier);
            }

            return true;
        }

        private String _DateValeur = "";
        public String DateValeur
        {
            get { return _DateValeur; }
            set { SetWpf(ref _DateValeur, value); }
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
                if (SetWpf(ref _Depense, value))
                {
                }
            }
        }

        public void AjouterAnalyseChantier(AnalyseChantier analyseChantier)
        {
            _Achat += analyseChantier.Achat;
            _HeureQte += analyseChantier.HeureQte;
            _HeureMt += analyseChantier.HeureMt;
            _Depense += analyseChantier.Depense;
            ListeAnalyseChantier.Add(analyseChantier);
        }

        private ListeObservable<AnalyseChantier> _ListeAnalyseChantier = null;
        public ListeObservable<AnalyseChantier> ListeAnalyseChantier
        {
            get
            {
                if (_ListeAnalyseChantier == null)
                    _ListeAnalyseChantier = new ListeObservable<AnalyseChantier>();

                return _ListeAnalyseChantier;
            }

            set
            {
                SetListeWpf(ref _ListeAnalyseChantier, value);
            }
        }
    }
}
