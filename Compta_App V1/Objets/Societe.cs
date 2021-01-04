using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compta
{
    public class Societe : ObjetGestion
    {
        private String _Nom = "";
        [Propriete]
        public String Nom
        {
            get { return _Nom; }
            set { SetPropGestion(ref _Nom, value, this); }
        }

        private int _NbMois = 2;
        public int NbMois
        {
            get { return _NbMois; }
            set
            {
                if (SetPropGestion(ref _NbMois, value, this))
                {
                    ListeAnalysePeriode.Clear();

                    if (_NbMois > 0)
                        calculerListeAnalysePeriode();
                }
            }
        }

        public void calculerListeAnalysePeriode()
        {
            ListeAnalysePeriode.Clear();

            if (NbMois > 0)
            {
                for (int i = -1; i < NbMois; i++)
                {
                    var s = DateTime.Now.AddMonths(-i);
                    var e = s.AddMonths(1);
                    s = new DateTime(s.Year, s.Month, 1, 0, 0, 0);
                    e = new DateTime(e.Year, e.Month, 1, 0, 0, 0);

                    var periode = new AnalysePeriode();
                    if (periode.Analyser(s, e))
                        ListeAnalysePeriode.Add(periode);
                }
            }
        }

        private ListeObservable<AnalysePeriode> _ListeAnalysePeriode = new ListeObservable<AnalysePeriode>();
        public ListeObservable<AnalysePeriode> ListeAnalysePeriode
        {
            get { return _ListeAnalysePeriode; }
            set { SetListeWpf(ref _ListeAnalysePeriode, value); }
        }

        private ListeObservable<Chantier> _ListeChantier = null;
        public ListeObservable<Chantier> ListeChantier
        {
            get
            {
                if (_ListeChantier == null)
                    _ListeChantier = Bdd2.Enfants<Chantier, Societe>(this);

                return _ListeChantier;
            }

            set
            {
                SetListeWpf(ref _ListeChantier, value);
            }
        }
    }
}
