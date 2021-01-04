using LogDebugging;
using System;
using System.ComponentModel;
using System.Windows;

namespace Compta
{
    public class Chantier : ObjetGestion
    {
        public Chantier() { }

        public Chantier(Societe societe)
        {
            Bdd2.Ajouter(this);

            Societe = societe;
        }

        [Propriete]
        [Tri(No = 2, DirectionTri = ListSortDirection.Descending)]
        public override int No
        {
            get { return base.No; }
            set { base.No = value; }
        }

        private int? _Id_Societe = null;
        private Societe _Societe = null;
        [CleEtrangere]
        public Societe Societe
        {
            get
            {
                if (_Societe == null)
                    _Societe = Bdd2.Parent<Societe, Chantier>(this);

                return _Societe;
            }
            set
            {
                if (SetObjetGestion(ref _Societe, value, this))
                    _Societe.ListeChantier.Add(this);
            }
        }

        private Boolean _Favori = false;
        [Propriete]
        [Tri(No = 1, DirectionTri = ListSortDirection.Descending)]
        public Boolean Favori
        {
            get { return _Favori; }
            set { SetPropGestion(ref _Favori, value, this); }
        }

        private String _Reference = "";
        [Propriete]
        public String Reference
        {
            get { return _Reference; }
            set { SetPropGestion(ref _Reference, value, this); }
        }

        private String _Description = "";
        [Propriete]
        public String Description
        {
            get { return _Description; }
            set { SetPropGestion(ref _Description, value, this); }
        }

        private Double _TauxHoraire = 0;
        [Propriete]
        public Double TauxHoraire
        {
            get { return _TauxHoraire; }
            set
            {
                if(SetPropGestion(ref _TauxHoraire, value, this))
                {
                    foreach (var poste in ListePoste)
                    {
                        foreach (var heure in poste.ListeHeure)
                            heure.CalculerHeure(false);

                        poste.CalculerHeure(false);
                    }

                    CalculerHeure();
                }
            }
        }

        private Double _Montant = 0;
        [Propriete]
        public Double Montant
        {
            get { return _Montant; }
            set
            {
                if(SetPropGestion(ref _Montant, value, this))
                {
                    CalculerMarge();
                }
            }
        }

        private Double _AvanceMt = 0;
        [Propriete]
        public Double AvanceMt
        {
            get { return _AvanceMt; }
            set
            {
                if (SetPropGestion(ref _AvanceMt, value, this))
                {
                    CalculerMarge();
                }
            }
        }

        private Double _AvancePct = 0;
        [Propriete]
        public Double AvancePct
        {
            get { return _AvancePct; }
            set { SetPropGestion(ref _AvancePct, value, this); }
        }

        private Double _Achat = 0;
        [Propriete]
        public Double Achat
        {
            get { return _Achat; }
            set { SetPropGestion(ref _Achat, value, this); }
        }

        private Double _HeureQte = 0;
        [Propriete]
        public Double HeureQte
        {
            get { return _HeureQte; }
            set
            {
                if (SetPropGestion(ref _HeureQte, value, this))
                {
                    CalculerDepense();
                }
            }
        }

        private Double _HeureMt = 0;
        [Propriete]
        public Double HeureMt
        {
            get { return _HeureMt; }
            set { SetPropGestion(ref _HeureMt, value, this); }
        }

        private Double _Depense = 0;
        [Propriete]
        public Double Depense
        {
            get { return _Depense; }
            set
            {
                if(SetPropGestion(ref _Depense, value, this))
                {
                    CalculerMarge();
                }
            }
        }

        private Double _MargeMt = 0;
        [Propriete]
        public Double MargeMt
        {
            get { return _MargeMt; }
            set { SetPropGestion(ref _MargeMt, value, this); }
        }

        private Double _MargePct = 0;
        [Propriete]
        public Double MargePct
        {
            get { return _MargePct; }
            set { SetPropGestion(ref _MargePct, value, this); }
        }

        private ListeObservable<Poste> _ListePoste = null;
        [ListeObjetGestion]
        public ListeObservable<Poste> ListePoste
        {
            get
            {
                if (_ListePoste == null)
                    _ListePoste = Bdd2.Enfants<Poste, Chantier>(this);

                return _ListePoste;
            }

            set
            {
                SetListeWpf(ref _ListePoste, value);
            }
        }

        public void CalculerMontant()
        {
            Double r = 0;
            foreach (var poste in ListePoste)
                r += poste.Montant;

            foreach (var poste in ListePoste)
                poste.MontantPct = Montant > 0 ? ArrondiPct((poste.Montant / Montant) * 100) : 100;

            Montant = r;
        }

        public void CalculerAvanceMt()
        {
            Double r = 0;

            foreach (var poste in ListePoste)
                r += poste.AvanceMt;

            AvanceMt = r;
            AvancePct = Montant > 0 ? ArrondiPct(r * 100 / Montant) : 100;
        }

        public void CalculerAchat()
        {
            Double r = 0;

            foreach (var poste in ListePoste)
                r += poste.Achat;

            Achat = r;
        }

        public void CalculerHeure()
        {
            Double r = 0;

            foreach (var poste in ListePoste)
                r += poste.HeureQte;

            HeureMt = r * TauxHoraire;
            HeureQte = r;
        }

        public void CalculerDepense()
        {
            Depense = Achat + HeureMt;
        }

        public void CalculerMarge()
        {
            MargeMt = (Montant * AvancePct / 100) - Depense;
            MargePct = Depense > 0 ? ArrondiPct(((Montant * AvancePct) / Depense) - 100) : 0;
        }

        public override Boolean Supprimer()
        {
            if (!EstCharge) return false;

            while (ListePoste.Count > 0)
                ListePoste[0].Supprimer();

            Societe.ListeChantier.Remove(this);

            Bdd2.Supprimer(this);

            return true;
        }
    }
}
