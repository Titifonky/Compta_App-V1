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

        [Propriete, Max]
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
                    _Societe = Bdd.Parent<Societe, Compte>(this);

                return _Societe;
            }
            set
            {
                if (SetObjetGestion(ref _Societe, value, this))
                    _Societe.ListeCompte.Ajouter(this);
            }
        }

        private int? _Id_Groupe = null;
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
                if (_Groupe != null && !_Groupe.ModeSupprimer && (_Groupe != value) && _Groupe.EstCharge && EstCharge && (_Groupe.ListeCompte.Count == 1))
                {
                    Log.Message("Impossible de supprimer le compte");
                    SetObjetGestion(ref _Groupe, _Groupe, this, true);
                    return;
                }

                if (_Groupe != null && (_Groupe != value) && _Groupe.EstCharge && EstCharge)
                    _Groupe.ListeCompte.Supprimer(this);

                if (SetObjetGestion(ref _Groupe, value, this))
                    _Groupe.ListeCompte.Ajouter(this);
            }
        }

        private String _Nom = "-";
        [Propriete, Tri]
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
            set
            {
                if (Set(ref _SoldeInitial, value, this))
                    Calculer();
            }
        }

        private Double _Solde = 0;
        [Propriete]
        public Double Solde
        {
            get { return _Solde; }
            set { Set(ref _Solde, value, this); }
        }

        private void CalculerNb()
        {
            Nb = (ListeLigneBanque.Count + ListeLigneCompta.Count).ToString();
        }

        private String _Nb = "";
        public String Nb
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_Nb))
                {
                    ListeLigneBanque.OnAjouter += (a, b) => CalculerNb();
                    ListeLigneBanque.OnSupprimer += (a, b) => CalculerNb();
                    ListeLigneCompta.OnAjouter += (a, b) => CalculerNb();
                    ListeLigneCompta.OnSupprimer += (a, b) => CalculerNb();
                    _Nb = (ListeLigneBanque.Count + ListeLigneCompta.Count).ToString();
                }

                return _Nb;
            }
            set { Set(ref _Nb, value, this); }
        }

        private Boolean _Supprimable = true;
        [Propriete]
        public Boolean Supprimable
        {
            get { return _Supprimable; }
            set
            {
                Set(ref _Supprimable, value, this);
            }
        }

        private ListeObservable<LigneBanque> _ListeLigneBanque = null;
        [ListeObjetGestion]
        public ListeObservable<LigneBanque> ListeLigneBanque
        {
            get
            {
                if (_ListeLigneBanque == null)
                    _ListeLigneBanque = Bdd.Enfants<LigneBanque, Compte>(this);

                if (!_ListeLigneBanque.OptionsCharges)
                {
                    _ListeLigneBanque.OnAjouter += delegate (LigneBanque obj, int? id) { Calculer(); };
                    _ListeLigneBanque.OnSupprimer += delegate (LigneBanque obj, int? id) { Calculer(); };
                    _ListeLigneBanque.OptionsCharges = true;
                }

                return _ListeLigneBanque;
            }

            set
            {
                SetListe(ref _ListeLigneBanque, value);
            }
        }

        private ListeObservable<LigneCompta> _ListeLigneCompta = null;
        [ListeObjetGestion]
        public ListeObservable<LigneCompta> ListeLigneCompta
        {
            get
            {
                if (_ListeLigneCompta == null)
                    _ListeLigneCompta = Bdd.Enfants<LigneCompta, Compte>(this);

                if (!_ListeLigneCompta.OptionsCharges)
                {
                    _ListeLigneCompta.OnAjouter += delegate (LigneCompta obj, int? id) { Calculer(); };
                    _ListeLigneCompta.OnSupprimer += delegate (LigneCompta obj, int? id) { Calculer(); };
                    _ListeLigneCompta.OptionsCharges = true;
                }

                return _ListeLigneCompta;
            }

            set
            {
                SetListe(ref _ListeLigneCompta, value);
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
            if (!EstCharge || !Supprimable) return false;

            if (Groupe.ListeCompte.Count > 1)
            {
                foreach (var lb in ListeLigneBanque)
                    lb.Groupe = Societe.ListeGroupe[0];

                foreach (var lc in ListeLigneCompta)
                    lc.Groupe = Societe.ListeGroupe[0];

                Groupe.ListeCompte.Supprimer(this);
                Societe.ListeCompte.Supprimer(this);

                Bdd.Supprimer(this);
                return true;
            }

            return false;
        }
    }
}
