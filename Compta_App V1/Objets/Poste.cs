using LogDebugging;
using System;
using System.ComponentModel;

namespace Compta
{

    public class Poste : ObjetGestion
    {
        public Poste() { }

        public Poste(Chantier chantier)
        {
            Bdd2.Ajouter(this);

            Chantier = chantier;
        }

        [Propriete]
        public override int No
        {
            get { return base.No; }
            set { base.No = value; }
        }

        private int? _Id_Chantier = null;
        private Chantier _Chantier = null;
        [CleEtrangere]
        public Chantier Chantier
        {
            get
            {
                if (_Chantier == null)
                    _Chantier = Bdd2.Parent<Chantier, Poste>(this);

                return _Chantier;
            }
            set
            {
                if (SetObjetGestion(ref _Chantier, value, this))
                    _Chantier.ListePoste.Add(this);
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

        private Double _Montant = 0;
        [Propriete]
        public Double Montant
        {
            get { return _Montant; }
            set
            {
                if (SetPropGestion(ref _Montant, value, this))
                {
                    CalculerMontant();
                    CalculerAvanceMt();
                    CalculerMarge();
                    Chantier.CalculerMontant();
                }
            }
        }

        private Double _MontantPct = 0;
        [Propriete]
        public Double MontantPct
        {
            get { return _MontantPct; }
            set { SetPropGestion(ref _MontantPct, value, this); }
        }

        private Double _AvancePct = 100;
        [Propriete]
        public Double AvancePct
        {
            get { return _AvancePct; }
            set
            {
                if (SetPropGestion(ref _AvancePct, value, this))
                {
                    CalculerAvanceMt();
                    CalculerMarge();
                    Chantier.CalculerAvanceMt();
                }
            }
        }

        private Double _AvanceMt = 0;
        [Propriete]
        public Double AvanceMt
        {
            get { return _AvanceMt; }
            set { SetPropGestion(ref _AvanceMt, value, this); }
        }

        private Double _Achat = 0;
        [Propriete]
        public Double Achat
        {
            get { return _Achat; }
            set
            {
                if(SetPropGestion(ref _Achat, value, this))
                {
                    CalculerDepense();
                    CalculerMarge();

                    if (Propager)
                        Chantier.CalculerAchat();
                }
            }
        }

        private Double _HeureQte = 0;
        [Propriete]
        public Double HeureQte
        {
            get { return _HeureQte; }
            set { SetPropGestion(ref _HeureQte, value, this); }
        }

        private Double _HeureMt = 0;
        [Propriete]
        public Double HeureMt
        {
            get { return _HeureMt; }
            set
            {
                if(SetPropGestion(ref _HeureMt, value, this))
                {
                    CalculerDepense();
                    CalculerMarge();

                    if (Propager)
                        Chantier.CalculerHeure();
                }
            }
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

        private ListeObservable<Achat> _ListeAchat = null;
        [ListeObjetGestion]
        public ListeObservable<Achat> ListeAchat
        {
            get
            {
                if (_ListeAchat == null)
                    _ListeAchat = Bdd2.Enfants<Achat, Poste>(this);

                return _ListeAchat;
            }

            set
            {
                SetListeWpf(ref _ListeAchat, value);
            }
        }

        private ListeObservable<Heure> _ListeHeure = null;
        [ListeObjetGestion]
        public ListeObservable<Heure> ListeHeure
        {
            get
            {
                if (_ListeHeure == null)
                    _ListeHeure = Bdd2.Enfants<Heure, Poste>(this);

                return _ListeHeure;
            }

            set
            {
                SetListeWpf(ref _ListeHeure, value);
            }
        }

        public void CalculerAchat()
        {
            Double r = 0;
            foreach (var achat in ListeAchat)
                r += achat.Montant;

            Achat = r;
        }

        public Boolean Propager = true;

        public void CalculerHeure(Boolean propager = true)
        {
            Double r = 0;
            foreach (var heure in ListeHeure)
                r += heure.Qte;
            
            Propager = propager;

            HeureQte = r;
            HeureMt = r * Chantier.TauxHoraire;

            Propager = true;
        }

        public void CalculerDepense()
        {
            Depense = Achat + HeureMt;
        }

        public void CalculerMontant()
        {
            Chantier.CalculerMontant();
        }

        public void CalculerMarge()
        {
            MargeMt = (Montant * AvancePct / 100) - Depense;
            MargePct = Depense > 0 ? ArrondiPct(((Montant * AvancePct) / Depense) - 100) : 0;
        }

        public void CalculerAvanceMt()
        {
            AvanceMt = Montant * AvancePct / 100;
        }

        public override bool Supprimer()
        {
            if (!EstCharge) return false;

            while (ListeAchat.Count > 0)
                ListeAchat[0].Supprimer();

            while (ListeHeure.Count > 0)
                ListeHeure[0].Supprimer();

            Chantier.ListePoste.Remove(this);

            Bdd2.Supprimer(this);

            return true;
        }
    }
}
