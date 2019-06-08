using LogDebugging;
using System;
using System.Windows.Data;

namespace Compta
{
    public class Compte : ObjetGestion
    {
        public Compte() { }

        public Compte(Groupe groupe)
        {
            Bdd.Ajouter(this);

            Societe = groupe.Societe;
            Groupe = groupe;
        }

        [Propriete, Max, Tri(Modifiable=true)]
        public override int No
        {
            get { return base.No; }
            set { base.No = value; }
        }

        private Societe _Societe = null;
        [CleEtrangere]
        public Societe Societe
        {
            get
            {
                if (_Societe == null)
                    _Societe = Bdd.Parent<Societe, Compte>(this);

                return _Societe;
            }
            set
            {
                if (SetObjetGestion(ref _Societe, value, this))
                    _Societe.ListeCompte.Ajouter(this);
            }
        }

        private Groupe _Groupe = null;
        [CleEtrangere]
        public Groupe Groupe
        {
            get
            {
                if (_Groupe == null)
                    _Groupe = Bdd.Parent<Groupe, Compte>(this);

                return _Groupe;
            }
            set
            {
                // pour ne pas avoir la possibilité d'un groupe sans compte
                if (EstCharge && _Groupe != null && _Groupe.EstCharge && (_Groupe != value) && (_Groupe.ListeCompte.Count == 1))
                {
                    SetObjetGestion(ref _Groupe, _Groupe, this, true);
                    return;
                }

                if (SetObjetGestion(ref _Groupe, value, this))
                    _Groupe.ListeCompte.Ajouter(this);
            }
        }

        private String _Nom = "-";
        [Propriete]
        public String Nom
        {
            get { return _Nom; }
            set { Set(ref _Nom, value, this); }
        }

        private String _Description = "";
        [Propriete]
        public String Description
        {
            get { return _Description; }
            set { Set(ref _Description, value, this); }
        }

        private Double _SoldeInitial = 0;
        [Propriete]
        public Double SoldeInitial
        {
            get { return _SoldeInitial; }
            set { Set(ref _SoldeInitial, value, this); }
        }

        private Double _Solde = 0;
        [Propriete]
        public Double Solde
        {
            get { return _Solde; }
            set { Set(ref _Solde, value, this); }
        }

        private ListeObservable<LigneBanque> _ListeLigneBanque = null;
        public ListeObservable<LigneBanque> ListeLigneBanque
        {
            get
            {
                if (_ListeLigneBanque == null)
                    _ListeLigneBanque = Bdd.Enfants<LigneBanque, Compte>(this);

                return _ListeLigneBanque;
            }

            set
            {
                Set(ref _ListeLigneBanque, value);
            }
        }

        private ListeObservable<LigneCompta> _ListeLigneCompta = null;
        public ListeObservable<LigneCompta> ListeLigneCompta
        {
            get
            {
                if (_ListeLigneCompta == null)
                {
                    _ListeLigneCompta = Bdd.Enfants<LigneCompta, Compte>(this);
                    _ListeLigneCompta.OnAjouter += delegate (LigneCompta obj, int? id) { Calculer(); };
                    _ListeLigneCompta.OnSupprimer += delegate (LigneCompta obj, int? id) { Calculer(); };
                }

                return _ListeLigneCompta;
            }

            set
            {
                Set(ref _ListeLigneCompta, value);
            }
        }

        public void Calculer()
        {
            if (!EstCharge) return;

            Double soldeTmp = 0;

            try
            {
                foreach (var lb in ListeLigneBanque)
                {
                    if (!lb.Compta)
                        soldeTmp += lb.Valeur;
                }

                foreach (var lc in ListeLigneCompta)
                {
                    soldeTmp -= lc.Valeur;
                }
            }
            catch(Exception e)
            {
                Log.Message(e.ToString());
            }

            Solde = SoldeInitial + soldeTmp;
        }

        public override Boolean Supprimer()
        {
            if (!EstCharge) return false;

            if (Groupe.ListeCompte.Count > 1)
            {
                Groupe.ListeCompte.Supprimer(this);
                Societe.ListeCompte.Supprimer(this);

                Bdd.Supprimer(this);
                return true;
            }

            return false;
        }
    }
}
