using LogDebugging;
using System.ComponentModel;
using System;

namespace Compta
{

    public class Heure : ObjetGestion
    {
        public enum Operation_e
        {
            [Description("Fab")]
            cFab = 1,
            [Description("Pose")]
            cPose = 2,
            [Description("Etude")]
            cEtude = 3,
        }

        public Heure() { }

        public Heure(Poste poste)
        {
            Bdd2.Ajouter(this);

            Poste = poste;
        }

        [Propriete]
        public override int No
        {
            get { return base.No; }
            set { base.No = value; }
        }

        private int? _Id_Poste = null;
        private Poste _Poste = null;
        [CleEtrangere]
        public Poste Poste
        {
            get
            {
                if (_Poste == null)
                    _Poste = Bdd2.Parent<Poste, Heure>(this);

                return _Poste;
            }
            set
            {
                if (SetObjetGestion(ref _Poste, value, this))
                    _Poste.ListeHeure.Add(this);
            }
        }

        private DateTime _DateValeur = DateTime.Now;
        [Propriete, Tri]
        public DateTime DateValeur
        {
            get { return _DateValeur; }
            set { SetPropGestion(ref _DateValeur, value, this); }
        }

        private Operation_e _Operation = Operation_e.cFab;
        [Propriete]
        public Operation_e Operation
        {
            get { return _Operation; }
            set { SetPropGestion(ref _Operation, value, this); }
        }

        private String _Description = "";
        [Propriete]
        public String Description
        {
            get { return _Description; }
            set { SetPropGestion(ref _Description, value, this); }
        }

        private Double _Qte = 0;
        [Propriete]
        public Double Qte
        {
            get { return _Qte; }
            set
            {
                if (SetPropGestion(ref _Qte, value, this))
                    CalculerHeure();
            }
        }

        private Double _Montant = 0;
        [Propriete]
        public Double Montant
        {
            get { return _Montant; }
            set { SetPropGestion(ref _Montant, value, this); }
        }

        public void CalculerHeure(Boolean Propager = true)
        {
            Montant = Qte * Poste.Chantier.TauxHoraire;

            if(Propager)
                Poste.CalculerHeure();
        }

        public override bool Supprimer()
        {
            if (!EstCharge) return false;

            Poste.ListeHeure.Remove(this);

            Bdd2.Supprimer(this);

            Poste.CalculerHeure();

            return true;
        }
    }
}
