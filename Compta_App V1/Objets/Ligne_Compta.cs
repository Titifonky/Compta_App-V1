using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace Compta
{

    public class Ligne_Compta : ObjetGestion
    {
        private Boolean Init = false;

        public Ligne_Compta() { }

        public Ligne_Compta(Societe s)
        {
            Societe = s;

            Bdd.Ajouter(this);

            No = s.ListeLigne_Compta.Count + 1;

            Init = true;
        }

        private Societe _Societe = null;
        [CleEtrangere]
        public Societe Societe
        {
            get
            {
                if (_Societe == null)
                    _Societe = Bdd.Parent<Societe, Ligne_Compta>(this);

                return _Societe;
            }
            set
            {
                Set(ref _Societe, value, this);
                if (_Societe.ListeLigne_Compta != null)
                    _Societe.ListeLigne_Compta.Add(this);
            }
        }

        [Propriete, Tri(Modifiable=true)]
        public override int No
        {
            get { return base.No; }
            set { base.No = value; }
        }

        private Boolean _Pointer = false;
        [Propriete]
        public Boolean Pointer
        {
            get { return _Pointer; }
            set { Set(ref _Pointer, value, this); }
        }

        private DateTime _Date_Valeur = DateTime.Now;
        [Propriete]
        public DateTime Date_Valeur
        {
            get { return _Date_Valeur; }
            set { Set(ref _Date_Valeur, value, this); }
        }

        private DateTime _Date_Compta = DateTime.Now;
        [Propriete]
        public DateTime Date_Compta
        {
            get { return _Date_Compta; }
            set { Set(ref _Date_Compta, value, this); }
        }

        private Ligne_Banque _Poste = null;
        [CleEtrangere]
        public Ligne_Banque Poste
        {
            get
            {
                if (_Poste == null)
                    _Poste = Bdd.Parent<Ligne_Banque, Ligne_Compta>(this);

                return _Poste;
            }
            set
            {
                Set(ref _Poste, value, this);
                if (_Poste.ListeLigne_Compta != null)
                    _Poste.ListeLigne_Compta.Add(this);
            }
        }

        private Categorie _Categorie = null;
        // Le champ peut être NULL, donc aucune contrainte de base
        [CleEtrangere(Contrainte=""), ForcerCopie]
        public Categorie Categorie
        {
            get
            {
                if (_Categorie == null)
                    _Categorie = Bdd.Parent<Categorie, Ligne_Compta>(this);

                return _Categorie;
            }
            set
            {
                Set(ref _Categorie, value, this);
            }
        }

        private Compte _Compte = null;
        // Le champ peut être NULL, donc aucune contrainte de base
        [CleEtrangere(Contrainte = ""), ForcerCopie]
        public Compte Compte
        {
            get
            {
                if (_Compte == null)
                    _Compte = Bdd.Parent<Compte, Ligne_Compta>(this);

                return _Compte;
            }
            set
            {
                Set(ref _Compte, value, this);
            }
        }

        private String _Libelle = "";
        [Propriete]
        public String Libelle
        {
            get { return _Libelle; }
            set { Set(ref _Libelle, value, this); }
        }

        private Double _Valeur = 0;
        [Propriete]
        public Double Valeur
        {
            get { return _Valeur; }
            set { Set(ref _Valeur, value, this); }
        }

        private Double _Tva_Pct = 0;
        [Propriete]
        public Double Tva_Pct
        {
            get { return _Tva_Pct; }
            set { Set(ref _Tva_Pct, value, this); }
        }

        private Double _Tva = 0;
        [Propriete]
        public Double Tva
        {
            get { return _Tva; }
            set { Set(ref _Tva, value, this); }
        }

        public override Boolean Supprimer()
        {
            if (!EstCharge) return false;

            if(Poste != null)
                Poste.ListeLigne_Compta.Remove(this);

            Bdd.Supprimer<Ligne_Compta>(this);

            if (Poste != null)
                Poste.Calculer();

            return true;
        }
    }
}
