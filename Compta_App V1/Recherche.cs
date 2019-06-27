using LogDebugging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Compta
{
    public class RechercheTexte<T> : INotifyPropertyChanged
        where T : ObjetGestion
    {
        private Selector _Box = null;
        private List<Selector> _ListeBox = null;
        private String _Valeur;
        private CollectionView _Vue = null;
        private List<CollectionView> _ListeVue = null;
        private Predicate<Object> _Methode = null;
        private Boolean _SousListe = false;

        public String Valeur
        {
            get
            { return _Valeur; }

            set
            {
                if (_Valeur != value)
                {
                    _Valeur = value;

                    if (_Box != null)
                    {
                        _Vue = (CollectionView)CollectionViewSource.GetDefaultView(_Box.ItemsSource);

                        if (_Vue == null) return;

                        if (_Vue.Filter != _Methode)
                            _Vue.Filter = _Methode;

                        _Vue.Refresh();
                        _Box.SelectedIndex = 0;
                    }

                    if (_ListeBox != null)
                    {
                        _ListeVue = new List<CollectionView>();
                        foreach (Selector Box in _ListeBox)
                        {

                            CollectionView Vue = (CollectionView)CollectionViewSource.GetDefaultView(Box.ItemsSource);
                            if (Vue == null) continue;

                            if (Vue.Filter != _Methode)
                                Vue.Filter = _Methode;

                            Vue.Refresh();
                            Box.SelectedIndex = 0;

                            _ListeVue.Add(Vue);
                        }
                    }
                    OnPropertyChanged();
                }
            }
        }

        public RechercheTexte(Selector Box, Boolean sousListe = false)
        {
            _Box = Box;
            _Methode = new Predicate<object>(Filtre);
            _SousListe = sousListe;
        }

        public RechercheTexte(List<Selector> ListeBox, Boolean sousListe = false)
        {
            _ListeBox = ListeBox;
            _Methode = new Predicate<object>(Filtre);
            _SousListe = sousListe;
        }

        private Boolean Filtre(object Source)
        {
            if (String.IsNullOrWhiteSpace(_Valeur))
                return true;

            T Obj = Source as T;

            try
            {
                if (Obj != null)
                {
                    String Chaine_Recherche = Valeur;

                    foreach (var info in Bdd.DicProp.Dic[typeof(T)].ListeInfo)
                    {
                        if (Regex.IsMatch(info.Propriete.GetValue(Obj).ToString().RemoveDiacritics(), Chaine_Recherche, RegexOptions.IgnoreCase))
                            return true;
                    }

                    if (_SousListe)
                    {
                        foreach (var info in Bdd.DicProp.Dic[typeof(T)].ListeListeObjet)
                        {
                            Object ObjListe = info.Propriete.GetValue(Obj);

                            int n = (int)info.Propriete.PropertyType.GetProperty("Count").GetValue(ObjListe);

                            for (int i = 0; i < n; i++)
                            {
                                // Get the list element as type object  
                                object[] index = { i };
                                object sObj = info.Propriete.PropertyType.GetProperty("Item").GetValue(ObjListe, index);

                                foreach (var val in Bdd.DicProp.Dic[info.TypeObjet].ListeInfo)
                                {
                                    if (Regex.IsMatch(val.Propriete.GetValue(sObj).ToString().RemoveDiacritics(), Chaine_Recherche, RegexOptions.IgnoreCase))
                                        return true;
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Message(e.ToString());
                return true;
            }

            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] String NomProp = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(NomProp));
        }
    }
}
