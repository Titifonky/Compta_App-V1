using LogDebugging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Compta
{
    public partial class MainWindow : Window
    {
        private void SupprimerOnglet(object source, RoutedEventArgs args)
        {
            TabItem tabItem = args.Source as TabItem;
            if (tabItem != null)
            {
                FermerOnglet(tabItem.DataContext);

                xOnglets.Items.Remove(tabItem);
                if (DernierOngletActif != null)
                    xOnglets.SelectedItem = DernierOngletActif;

                return;
            }
        }

        private void SelectionOnglet(object sender, SelectionChangedEventArgs e)
        {
            //DernierOngletActif = xOnglets.SelectedItem as TabItem;
        }

        /// <summary>
        /// Valider les champs texte avec la touche entrée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void TextBox_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tBox = (TextBox)sender;
                DependencyProperty prop = TextBox.TextProperty;

                BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
                if (binding != null) { binding.UpdateSource(); }
            }
        }

        private void Editer_Menu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem I = sender as MenuItem;
            if (I != null)
            {
                Societe S = I.DataContext as Societe;
                if (S != null)
                {
                    FrameworkElement F = sender as FrameworkElement;
                    String Nom = F.Tag as String;

                    if (Nom == "Societe")
                    { EditerOnglet<Societe, Societe>(S); return; }

                    if (Nom == "Famille")
                    { EditerOnglet<Groupe, Societe>(S); return; }
                }

            }
        }

        private void Editer_Click(object sender, RoutedEventArgs e)
        {
            MenuItem I = sender as MenuItem;
            if (I != null)
            {
                ListBox B = ((ContextMenu)I.Parent).PlacementTarget as ListBox;
                if (B != null)
                {
                    //Client C = B.SelectedItem as Client;
                    //if (C != null)
                    //{ EditerOnglet<Client>(C); return; }

                    //Devis D = B.SelectedItem as Devis;
                    //if (D != null)
                    //{ EditerOnglet<Devis>(D); return; }

                    //Facture F = B.SelectedItem as Facture;
                    //if (F != null)
                    //{EditerOnglet<Facture>(F); return; }

                    //Fournisseur Fr = B.SelectedItem as Fournisseur;
                    //if (Fr != null)
                    //{EditerOnglet<Fournisseur>(Fr); return; }

                    //Utilisateur U = B.SelectedItem as Utilisateur;
                    //if (U != null)
                    //{ EditerOnglet<Utilisateur>(U); return; }
                }
            }
        }

        private void Editer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                EcritureBanque EC = ((FrameworkElement)sender).DataContext as EcritureBanque;
                if (EC != null)
                {
                    EC.Pointer = !EC.Pointer;
                    return;
                }

                ObjetGestion OB = ((FrameworkElement)sender).DataContext as ObjetGestion;
                if (OB != null)
                {
                    OB.Editer = !OB.Editer;

                    Compte C = OB as Compte;
                    if (C != null)
                        C.Groupe.Editer = !C.Groupe.Editer;

                    return;
                }
            }
        }

        private void Editer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EcritureBanque EC = ((FrameworkElement)sender).DataContext as EcritureBanque;
            if (EC != null)
            {
                EC.Pointer = !EC.Pointer;
                return;
            }

            Banque B = ((FrameworkElement)sender).DataContext as Banque;
            if (B != null)
            {
                B.Editer = !B.Editer;
                return;
            }
        }

        private Boolean EditerOnglet<T>(T DataContext)
            where T : ObjetGestion
        {
            return EditerOnglet<T, T>(DataContext);
        }

        private Boolean EditerOnglet<T, U>(U DataContext)
            where T : ObjetGestion
            where U : ObjetGestion
        {
            //String Titre = ""; String ModeleTitre = ""; String ModeleCorps = "";

            //if (typeof(U) == typeof(Societe))
            //{
            //    if (typeof(T) == typeof(Societe))
            //    { Titre = "Societe"; ModeleCorps = "xOngletSocieteControlTemplate"; }

            //    else if (typeof(T) == typeof(GroupeCompte))
            //    { Titre = "Famille"; ModeleCorps = "xOngletFamilleControlTemplate"; }

            //    else if (typeof(T) == typeof(Fournisseur))
            //    { Titre = "Fournisseur"; ModeleCorps = "xOngletFournisseurControlTemplate"; }

            //    else if (typeof(T) == typeof(Utilisateur))
            //    { Titre = "Utilisateur"; ModeleCorps = "xOngletUtilisateurControlTemplate"; }
            //}
            //else if (typeof(T) == typeof(Client))
            //{ ModeleTitre = "xTitreClient"; ModeleCorps = "xEditerClientControlTemplate"; }

            //else if (typeof(T) == typeof(Devis))
            //{ ModeleTitre = "xTitreDevis"; ModeleCorps = "xEditerDevisControlTemplate"; }

            //else if (typeof(T) == typeof(Facture))
            //{ ModeleTitre = "xTitreFacture"; ModeleCorps = "xEditerFactureControlTemplate"; }

            //else if (typeof(T) == typeof(Fournisseur))
            //{ ModeleTitre = "xTitreFournisseur"; ModeleCorps = "xEditerFournisseurControlTemplate"; }

            //else if (typeof(T) == typeof(Utilisateur))
            //{ ModeleTitre = "xTitreUtilisateur"; ModeleCorps = "xEditerUtilisateurControlTemplate"; }

            //if (DataContext != null)
            //{

            //    OngletSupprimable Onglet = null;

            //    foreach (TabItem pTab in xOnglets.Items)
            //    {
            //        if (pTab.DataContext == (object)DataContext)
            //        {
            //            Onglet = pTab as OngletSupprimable;
            //            if (Onglet == null)
            //                continue;
            //        }
            //    }

            //    if (Onglet == null)
            //    {

            //        Onglet = new OngletSupprimable();
            //        if (String.IsNullOrWhiteSpace(Titre))
            //        {
            //            Onglet.Header = DataContext;
            //            Onglet.HeaderTemplate = (DataTemplate)this.Resources[ModeleTitre];
            //        }
            //        else
            //        {
            //            Onglet.Header = Titre;
            //        }

            //        ContentControl Control = new ContentControl();
            //        Control.Template = (ControlTemplate)this.Resources[ModeleCorps];
            //        Onglet.Content = Control;
            //        xOnglets.Items.Add(Onglet);
            //        Onglet.DataContext = DataContext;
            //    }
            //    DernierOngletActif = xOnglets.SelectedItem as TabItem;
            //    xOnglets.SelectedItem = Onglet;
            //    return true;
            //}

            return false;
        }

        private void FermerOnglet(Object DataContext)
        {
            //Client C = DataContext as Client;
            //if (C != null)
            //{ C.CreerDossier(true); return; }

            //Devis D = DataContext as Devis;
            //if (D != null)
            //{ D.CreerDossier(false); return; }
        }

        private void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (Bdd.DoitEtreEnregistre)
            {
                Bdd.Enregistrer();
                xDerniereSvg.Text = "Dernière sauvegarde à " + DateTime.Now.Hour + "h" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
            }
            else
                xDerniereSvg.Text = "Base de donnée à jour";
        }

        private void EffacerTextBox_Click(object sender, RoutedEventArgs e)
        {
            Button B = sender as Button;
            if (B == null) return;
            TextBox T = B.DataContext as TextBox;
            if (B == null) return;

            T.Text = "";

            DependencyProperty prop = TextBox.TextProperty;
            BindingExpression binding = BindingOperations.GetBindingExpression(T, prop);
            if (binding != null) { binding.UpdateSource(); }
        }

        //void ListViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    //xListeEcritureBanque.Ajuster_Colonnes();

        //    EcritureBanque item = ((FrameworkElement)e.OriginalSource).DataContext as EcritureBanque;
        //    if (item != null)
        //    {
        //        Log.Message(item.Description);
        //        item.Pointer = !item.Pointer;
        //    }
        //}

        #region EVENEMENT MENU LIST

        Object _Copie_Liste;

        private Boolean Info<T>(MenuItem I, out ListBox V, out ListeObservable<T> Liste, out List<T> Ls, out T L)
            where T : INotifyPropertyChanged
        {
            V = null; Liste = null; Ls = null; L = default(T);
            if (I != null)
            {
                V = (I.Parent as ContextMenu).PlacementTarget as ListBox;
                Liste = V.ItemsSource as ListeObservable<T>;
                Ls = V.SelectedItems.Cast<T>().ToList();
                L = (T)V.SelectedItem;

                if ((V != null) && (Liste != null) && (Ls != null) && ((L != null) || (Liste.Count == 0)))
                    return true;
            }
            return false;
        }

        private T Ajouter<T>()
            where T : ObjetGestion, new()
        {
            return new T();
        }

        private T Ajouter<T, U>(U Parent)
            where T : ObjetGestion, new()
            where U : ObjetGestion
        {
            Type classType = typeof(T);
            ConstructorInfo classConstructor = classType.GetConstructor(new Type[] { typeof(U) });
            return (T)classConstructor.Invoke(new object[] { Parent });
        }

        private ListeObservable<T> Ajouter_List<T>(object sender, RoutedEventArgs e, Boolean UnSeul = false)
            where T : ObjetGestion, new()
        {
            ListeObservable<T> pListe = new ListeObservable<T>();

            ListBox V; ListeObservable<T> Liste; List<T> Ls; T L;
            if (Info(sender as MenuItem, out V, out Liste, out Ls, out L))
            {
                int Nb = Math.Max(Ls.Count, 1);
                if (UnSeul)
                    Nb = 1;

                // On force à faire au minimum un tour de boucle dans le cas ou la liste est vide.
                for (int i = 0; i < Nb; i++)
                    pListe.Add(Ajouter<T>());
            }

            return pListe;
        }

        private ListeObservable<T> Ajouter_List<T, U>(object sender, RoutedEventArgs e, Boolean UnSeul = false)
            where T : ObjetGestion, new()
            where U : ObjetGestion
        {
            ListeObservable<T> pListe = new ListeObservable<T>();

            ListBox V; ListeObservable<T> Liste; List<T> Ls; T L;
            if (Info(sender as MenuItem, out V, out Liste, out Ls, out L))
            {
                U Parent = (U)V.DataContext;
                // On force à faire au minimum un tour de boucle dans le cas ou la liste est vide.

                int Nb = Math.Max(Ls.Count, 1);
                if (UnSeul)
                    Nb = 1;

                for (int i = 0; i < Nb; i++)
                    pListe.Add(Ajouter<T, U>(Parent));
            }

            return pListe;
        }

        private ListeObservable<T> Inserer_List<T>(object sender, RoutedEventArgs e)
            where T : ObjetGestion, new()
        {
            ListeObservable<T> pListe = new ListeObservable<T>();

            ListBox V; ListeObservable<T> Liste; List<T> Ls; T L;
            if (Info(sender as MenuItem, out V, out Liste, out Ls, out L))
            {
                foreach (T iL in Ls)
                {
                    T Obj = Ajouter<T>();
                    pListe.Add(Obj);
                    Liste.Move(Liste.IndexOf(Obj), Liste.IndexOf(L));
                }
                Liste.Numeroter();
            }
            return pListe;
        }

        private ListeObservable<T> Inserer_List<T, U>(object sender, RoutedEventArgs e)
            where T : ObjetGestion, new()
            where U : ObjetGestion
        {
            ListeObservable<T> pListe = new ListeObservable<T>();

            ListBox V; ListeObservable<T> Liste; List<T> Ls; T L;
            if (Info(sender as MenuItem, out V, out Liste, out Ls, out L))
            {
                U Parent = (U)V.DataContext;
                foreach (T iL in Ls)
                {
                    T Obj = Ajouter<T, U>(Parent);
                    pListe.Add(Obj);
                    Liste.Move(Liste.IndexOf(Obj), Liste.IndexOf(L));
                }
                Liste.Numeroter();
            }
            return pListe;
        }

        private void Monter_List<T>(object sender, RoutedEventArgs e)
            where T : ObjetGestion
        {
            ListBox V; ListeObservable<T> Liste; List<T> Ls; T L;
            if (Info(sender as MenuItem, out V, out Liste, out Ls, out L))
            {
                // On test si une ligne n'est pas à la position 0 pour eviter les erreurs
                foreach (T iL in Ls)
                {
                    if (Liste.IndexOf(iL) == 0)
                        return;
                }

                // Si c'est bon, on les déplace toutes
                foreach (T iL in Ls)
                {
                    int De = Liste.IndexOf(iL);

                    Liste.Move(De, De - 1);
                }

                Liste.Numeroter();
            }
        }

        private void Descendre_List<T>(object sender, RoutedEventArgs e)
            where T : ObjetGestion
        {
            ListBox V; ListeObservable<T> Liste; List<T> Ls; T L;
            if (Info(sender as MenuItem, out V, out Liste, out Ls, out L))
            {
                // On test si une ligne n'est pas à la dernière position pour eviter les erreurs
                foreach (T iL in Ls)
                {
                    if (Liste.IndexOf(iL) == (Liste.Count - 1))
                        return;
                }

                // Si c'est bon, on les déplace toutes
                Ls.Reverse();
                foreach (T iL in Ls)
                {
                    int De = Liste.IndexOf(iL);

                    Liste.Move(De, De + 1);
                }

                Liste.Numeroter();
            }
        }

        private void Supprimer_List<T>(object sender, RoutedEventArgs e, Boolean Message = false, Boolean UnItemMini = false)
            where T : ObjetGestion
        {
            ListBox V; ListeObservable<T> Liste; List<T> Ls; T L;
            if (Info(sender as MenuItem, out V, out Liste, out Ls, out L))
            {
                int mini = UnItemMini ? 1 : 0;

                foreach (T iL in Ls)
                {
                    Boolean Supprimer = !Message;

                    if (Liste.Count > mini)
                    {
                        if (Message && MessageBox.Show(String.Format("Voulez vous vraiement supprimer : {0} {1} ?", DicIntitules.IntituleType(typeof(T).Name), iL.No), "Suppression", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            Supprimer = true;

                        if (Supprimer && iL.Supprimer())
                            Liste.Remove(iL);
                    }
                }
                Liste.Numeroter();
            }
        }

        private void Copier_List<T>(object sender, RoutedEventArgs e)
            where T : ObjetGestion
        {
            ListBox V; ListeObservable<T> Liste; List<T> Ls; T L;
            if (Info(sender as MenuItem, out V, out Liste, out Ls, out L))
            {
                _Copie_Liste = Ls;
            }
        }

        private void Coller_List<T>(object sender, RoutedEventArgs e)
            where T : ObjetGestion
        {
            ListBox V; ListeObservable<T> Liste; List<T> Ls; T L;
            if (Info(sender as MenuItem, out V, out Liste, out Ls, out L) && (_Copie_Liste != null))
            {
                List<T> pListe = new List<T>();

                if (_Copie_Liste != null)
                    pListe = _Copie_Liste as List<T>;

                for (int i = 0; i < Ls.Count; i++)
                {
                    if (i < pListe.Count)
                    {
                        Ls[i].Copier(pListe[i]);
                    }
                }
                Liste.Numeroter();
            }
        }

        private ListeObservable<T> Inserer_Coller_List<T>(object sender, RoutedEventArgs e)
            where T : ObjetGestion, new()
        {
            ListeObservable<T> pListe = new ListeObservable<T>();

            ListBox V; ListeObservable<T> Liste; List<T> Ls; T L;
            if (Info(sender as MenuItem, out V, out Liste, out Ls, out L) && (_Copie_Liste != null))
            {
                List<T> pListeCopie = new List<T>();

                pListeCopie = _Copie_Liste as List<T>;

                foreach (T iL in pListeCopie)
                {
                    T Obj = Ajouter<T>();
                    pListe.Add(Obj);
                    Obj.Copier(iL);

                    // Si la liste contient des lignes, on la déplace
                    if (L != null)
                        Liste.Move(Liste.IndexOf(Obj), Liste.IndexOf(L));
                }
                Liste.Numeroter();
            }
            return pListe;
        }

        private ListeObservable<T> Inserer_Coller_List<T, U>(object sender, RoutedEventArgs e)
            where T : ObjetGestion, new()
            where U : ObjetGestion
        {
            // Liste temporaire renvoyé par la fonction
            ListeObservable<T> pListe = new ListeObservable<T>();

            ListBox V; ListeObservable<T> Liste; List<T> Ls; T L;
            if (Info(sender as MenuItem, out V, out Liste, out Ls, out L) && (_Copie_Liste != null))
            {
                List<T> pListeCopie = new List<T>();

                U Parent = (U)V.DataContext;
                pListeCopie = _Copie_Liste as List<T>;

                foreach (T iL in pListeCopie)
                {
                    // On ajoute une ligne
                    T Obj = Ajouter<T, U>(Parent);
                    pListe.Add(Obj);

                    // On copie la ligne
                    Obj.Copier(iL);

                    // Si la liste contient des lignes, on la déplace
                    if (L != null)
                        Liste.Move(Liste.IndexOf(Obj), Liste.IndexOf(L));
                }

                Liste.Numeroter();
            }
            return pListe;
        }

        #endregion

        #region EVENEMENT LigneBanque

        private void Ajouter_LigneBanque_Click(object sender, RoutedEventArgs e)
        {
            Ajouter_List<LigneBanque, EcritureBanque>(sender, e);
        }

        private void Supprimer_LigneBanque_Click(object sender, RoutedEventArgs e)
        {
            Supprimer_List<LigneBanque>(sender, e, false, true);
        }

        #endregion

        #region EVENEMENT LigneCompta

        private void Ajouter_LigneCompta_Click(object sender, RoutedEventArgs e)
        {
            Ajouter_List<LigneCompta, Societe>(sender, e);
        }

        private void Supprimer_LigneCompta_Click(object sender, RoutedEventArgs e)
        {
            Supprimer_List<LigneCompta>(sender, e, false, false);
        }

        #endregion

        #region EVENEMENT Banque

        private void Ajouter_Banque_Click(object sender, RoutedEventArgs e)
        {
            Ajouter_List<Banque, Societe>(sender, e);
        }

        private void Supprimer_Banque_Click(object sender, RoutedEventArgs e)
        {
            Supprimer_List<Banque>(sender, e, false, false);
        }

        #endregion

        #region EVENEMENT Compte

        private void Ajouter_Compte_Click(object sender, RoutedEventArgs e)
        {
            ListBox V = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as ListBox;
            Compte L = (Compte)V.SelectedItem;

            if (L != null)
                new Compte(L.Groupe);
        }

        private void Supprimer_Compte_Click(object sender, RoutedEventArgs e)
        {
            ListBox V = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as ListBox;
            Compte L = (Compte)V.SelectedItem;

            if (L != null)
                L.Supprimer();
        }

        private void Ajouter_Groupe_Click(object sender, RoutedEventArgs e)
        {
            ListBox V = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as ListBox;
            Compte L = (Compte)V.SelectedItem;

            if (L != null)
                new Groupe(L.Societe);
        }

        private void Supprimer_Groupe_Click(object sender, RoutedEventArgs e)
        {
            ListBox V = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as ListBox;
            Compte L = (Compte)V.SelectedItem;

            if (L != null)
            {
                Groupe G = L.Groupe;
                G.Supprimer();
            }
        }

        #endregion

        private void Atteindre_Click(object sender, RoutedEventArgs e)
        {
            ListBox V = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as ListBox;
            LigneBanque L = (LigneBanque)V.SelectedItem;
            if (L != null)
            {
                xComptabilite.Focus();

                xListeEcritureBanque.ScrollIntoView(L.EcritureBanque);
                xListeEcritureBanque.SelectedItem = L.EcritureBanque;
                
            }
        }

        private void xListeCompte_Loaded(object sender, RoutedEventArgs e)
        {
            //ICollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(xListeCompte.ItemsSource);
            //view.SortDescriptions.Add(new SortDescription("Groupe", ListSortDirection.Ascending));

            //xListeCompte.ItemsSource = view;
        }

        #region LISTVIEW

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as UIElement).FindVisualParent<ListView>().Ajuster_Colonnes();
        }

        private void ComboBox_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            (sender as UIElement).FindVisualParent<ListView>().Ajuster_Colonnes();
        }

        private void TextBlock_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            (sender as UIElement).FindVisualParent<ListView>().Ajuster_Colonnes();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            (sender as UIElement).FindVisualParent<ListView>().Ajuster_Colonnes();
        }

        private void Control_KeyUp(object sender, KeyEventArgs e)
        {
            (sender as UIElement).FindVisualParent<ListView>().Ajuster_Colonnes();
        }

        private void Control_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as UIElement).FindVisualParent<ListView>().Ajuster_Colonnes();
        }

        // Fontion pour eviter le freezz du scroll quand la souris est sur une listview contenue dans une listbox
        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }

        #endregion

    }
}
