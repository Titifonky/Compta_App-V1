using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Compta
{
    public class Groupe : ObjetGestion
    {
        public Groupe() { }

        public Groupe(Societe societe)
        {
            Bdd.Ajouter(this);
            Nom = "Nouveau groupe";

            Societe = societe;
            Compte C = new Compte(this);
            C.Nom = "-";
        }

        [Propriete, Max, Tri(Modifiable = true)]
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
                    _Societe = Bdd.Parent<Societe, Groupe>(this);

                return _Societe;
            }
            set
            {
                if (SetObjetGestion(ref _Societe, value, this))
                    _Societe.ListeGroupe.Ajouter(this);
            }
        }

        private String _Nom = "";
        [Propriete]
        public String Nom
        {
            get { return _Nom; }
            set
            {
                Set(ref _Nom, value, this);
            }
        }

        private String _Description = "";
        [Propriete]
        public String Description
        {
            get { return _Description; }
            set { Set(ref _Description, value, this); }
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

        private ListeObservable<Compte> _ListeCompte = null;
        public ListeObservable<Compte> ListeCompte
        {
            get
            {
                if (_ListeCompte == null)
                    _ListeCompte = Bdd.Enfants<Compte, Groupe>(this);

                return _ListeCompte;
            }
            set
            {
                SetListe(ref _ListeCompte, value);
            }
        }

        public Boolean ModeSupprimer = false;

        public override Boolean Supprimer()
        {
            if (!EstCharge || !Supprimable || (Societe.ListeGroupe.IndexOf(this) == 0)) return false;

            ModeSupprimer = true;

            List<Compte> LC = new List<Compte>();

            foreach (var c in ListeCompte)
                LC.Add(c);

            foreach (var c in LC)
                c.Groupe = null;

            foreach (var c in LC)
                c.Groupe = Societe.ListeGroupe[0];

            Societe.ListeGroupe.Supprimer(this);

            Bdd.Supprimer(this);
            return true;
        }
    }
}
