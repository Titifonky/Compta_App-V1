using LogDebugging;
using System;

namespace Compta
{

    public class Achat : ObjetGestion
    {
        public Achat() { }

        public Achat(Poste poste)
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
                    _Poste = Bdd2.Parent<Poste, Achat>(this);

                return _Poste;
            }
            set
            {
                if (SetObjetGestion(ref _Poste, value, this))
                    _Poste.ListeAchat.Add(this);
            }
        }

        private DateTime _DateValeur = DateTime.Now;
        [Propriete, Tri]
        public DateTime DateValeur
        {
            get { return _DateValeur; }
            set { SetPropGestion(ref _DateValeur, value, this); }
        }

        private String _Fournisseur = " ";
        [Propriete]
        public String Fournisseur
        {
            get { return _Fournisseur; }
            set { SetPropGestion(ref _Fournisseur, value, this); }
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
                    CalculerAchat();
            }
        }

        public void CalculerAchat()
        {
            Poste.CalculerAchat();
        }

        public override bool Supprimer()
        {
            if (!EstCharge) return false;

            Poste.ListeAchat.Remove(this);

            Bdd2.Supprimer(this);

            return true;
        }
    }
}
