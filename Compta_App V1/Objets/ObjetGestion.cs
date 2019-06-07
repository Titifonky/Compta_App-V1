using LogDebugging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Compta
{
    public class ObjetGestion : INotifyPropertyChanged
    {

        public static readonly int DEFAULT_ID = -1;
        protected static readonly int DEFAULT_ARRONDI_EURO = 2;
        protected static readonly int DEFAULT_ARRONDI_PCT = 1;

        protected static Double ArrondiEuro(Double Val)
        {
            return Math.Round(Val, DEFAULT_ARRONDI_EURO);
        }

        protected static Double ArrondiPct(Double Val)
        {
            return Math.Round(Val, DEFAULT_ARRONDI_PCT);
        }

        protected Type T = null;

        protected int _Id = DEFAULT_ID;
        private Boolean _EstCharge = false;

        public Boolean EstSvgDansLaBase
        {
            get { return _Id != DEFAULT_ID; }
        }

        [ClePrimaire]
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public Boolean EstCharge
        {
            get { return _EstCharge; }
            set { _EstCharge = value; }
        }

        protected int _No = 0;
        [Propriete, Max, Tri]
        public virtual int No
        {
            get { return _No; }
            set { Set(ref _No, value, this); }
        }

        public ObjetGestion()
        {
            T = this.GetType();
        }

        public virtual Boolean Supprimer()
        { return false; }

        protected Boolean CopierBase<T>(T ObjetBase)
            where T : ObjetGestion
        {
            if (!ObjetBase.EstCharge || !this.EstCharge) return false;

            try
            {
                List<PropertyInfo> pListeProp = typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(Propriete), true)).ToList<PropertyInfo>();

                foreach (PropertyInfo Prop in pListeProp)
                {
                    if (Attribute.IsDefined(Prop, typeof(ForcerCopie)) || !(Attribute.IsDefined(Prop, typeof(ClePrimaire)) || Attribute.IsDefined(Prop, typeof(CleEtrangere)) || Attribute.IsDefined(Prop, typeof(NePasCopier))))
                        Prop.SetValue(this, Prop.GetValue(ObjetBase));
                }
            }
            catch (Exception e)
            {
                this.LogMethode(e);
                return false;
            }

            return true;
        }

        public virtual void Copier<T>(T ObjetBase)
            where T : ObjetGestion
        {
            CopierBase(ObjetBase);
        }

        protected void SupprimerListe<T>(ListeObservable<T> Liste)
            where T : ObjetGestion
        {
            if (Liste != null)
                while (Liste.Count > 0)
                    Liste[0].Supprimer();
        }

        public override string ToString()
        {
            return _Id.ToString();
        }

        #region Notification WPF

        protected bool Set<U, V>(ref U field, U value, V Objet, [CallerMemberName]string propertyName = "")
            where V : ObjetGestion
        {
            if (EqualityComparer<U>.Default.Equals(field, value)) { return false; }
            field = value;
            OnPropertyChanged(propertyName);
            if (EstSvgDansLaBase)
                Bdd.Maj(Objet, T, propertyName);
            return true;
        }

        protected bool Set<U>(ref U field, U value, [CallerMemberName]string propertyName = "")
        {
            if (EqualityComparer<U>.Default.Equals(field, value)) { return false; }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] String NomProp = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(NomProp));
        }

        #endregion
    }

    public class ListeObservable<T> : ObservableCollection<T>
    {
        private String _Intitule = null;
        private Dictionary<String, String> _ListeEntete = null;

        public String Intitule
        {
            get
            {
                if (_Intitule == null)
                    _Intitule = DicIntitules.IntituleType(typeof(T).Name);

                return _Intitule;
            }
        }
        public Dictionary<String, String> ListeEntete
        {
            get
            {
                if (_ListeEntete == null)
                    _ListeEntete = DicIntitules.DicEntete(typeof(T).Name);

                return _ListeEntete;
            }
        }

        public void Numeroter()
        {
            // On liste les proprietes triables
            List<PropertyInfo> pListeProp = typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(Tri), true)).ToList<PropertyInfo>();

            // S'il n'y en a pas on sort
            if (pListeProp.Count == 0)
                return;

            // On recherche la première propriété triable ET modifiable
            PropertyInfo pPropTriModifiable = null;
            foreach (PropertyInfo Prop in pListeProp)
            {
                Tri pAttTri = (Tri)Prop.GetCustomAttributes(typeof(Tri)).First();
                if (pAttTri.Modifiable == true)
                {
                    pPropTriModifiable = Prop;
                    break;
                }
            }

            // S'il n'y en a pas, on sort aussi
            if (pPropTriModifiable == null)
                return;

            // Si tout est ok, on reindexe chaque Element avec sa position dans la liste + 1 pour ne pas avoir de 0
            foreach (T Element in this)
            {
                pPropTriModifiable.SetValue(Element, Convert.ChangeType(this.IndexOf(Element) + 1, pPropTriModifiable.PropertyType), null);
            }
        }

        public ListeObservable()
            : base()
        {
        }

        public ListeObservable(ListeObservable<T> Liste)
        {
            foreach (T item in Liste)
                base.Add(item);
        }

        public delegate void OnAjouterHandler(T obj, int? id);

        public event OnAjouterHandler OnAjouter;

        public delegate void OnSupprimerHandler(T obj, int? id);

        public event OnSupprimerHandler OnSupprimer;

        public void Ajouter(T Item, Boolean Debut = false)
        {
            if (Contains(Item)) return;

            if (Debut)
                base.Insert(0, Item);
            else
                base.Add(Item);

            OnAjouter?.Invoke(Item, null);
        }

        public void Supprimer(T Item)
        {
            base.Remove(Item);

            OnSupprimer?.Invoke(Item, null);
        }

        public new void Add(T Item)
        {
            if (Contains(Item)) return;
            
            base.Add(Item);

            OnAjouter?.Invoke(Item, null);
        }

        public new void Insert(int Index, T Item)
        {
            if (Contains(Item)) return;
            
            base.Insert(Index, Item);

            OnAjouter?.Invoke(Item, Index);
        }

        public new void InsertItem(int Index, T Item)
        {
            if (Contains(Item)) return;

            base.InsertItem(Index, Item);

            OnAjouter?.Invoke(Item, Index);
        }

        public new void Remove(T Item)
        {
            base.Remove(Item);

            OnSupprimer?.Invoke(Item, null);
        }

        public new void RemoveAt(int Index)
        {
            var Item = base[Index];

            base.RemoveAt(Index);

            OnSupprimer?.Invoke(Item, Index);
        }

        public new void RemoveItem(int Index)
        {
            var Item = base[Index];

            base.RemoveItem(Index);

            OnSupprimer?.Invoke(Item, Index);
        }

        public ListeObservable(IEnumerable<T> Liste)
            : this()
        {
            foreach (var item in Liste)
                this.Add(item);
        }
    }

    public class ListeAvecTitre<T> : ObservableCollection<T>
    {
        private String _Intitule = "";

        public String Intitule
        {
            get { return _Intitule; }
            set { _Intitule = value; }
        }

        public ListeAvecTitre() { }

        public ListeAvecTitre(String intitule) { _Intitule = intitule; }

        public void Importer(ICollection<T> collection)
        {
            foreach (T obj in collection)
                Add(obj);
        }
    }
}
