using LogDebugging;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Compta
{
    public partial class ListeObjet : ControlBase
    {
        protected void TextBox_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tBox = (TextBox)sender;
                DependencyProperty prop = TextBox.TextProperty;

                BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
                if (binding != null) { binding.UpdateSource(); }
            }
        }

        public Boolean Editable
        {
            get { return (Boolean)GetValue(EditableDP); }
            set { SetValue(EditableDP, value); }
        }

        public static readonly DependencyProperty EditableDP =
            DependencyProperty.Register("Editable", typeof(Boolean),
              typeof(ListeObjet), new PropertyMetadata(null));

        public Boolean Intitule
        {
            get { return (Boolean)GetValue(IntituleDP); }
            set { SetValue(IntituleDP, value); }
        }

        public static readonly DependencyProperty IntituleDP =
            DependencyProperty.Register("Intitule", typeof(Boolean),
              typeof(ListeObjet), new PropertyMetadata(null));

        public String DisplayMemberPath
        {
            get { return (String)GetValue(DisplayMemberPathDP); }
            set { SetValue(DisplayMemberPathDP, value); }
        }

        public static readonly DependencyProperty DisplayMemberPathDP =
            DependencyProperty.Register("DisplayMemberPath", typeof(String),
              typeof(ListeObjet), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object SelectedValue
        {
            get { return (object)GetValue(SelectedValueDP); }
            set { SetValue(SelectedValueDP, value); }
        }

        public static readonly DependencyProperty SelectedValueDP =
            DependencyProperty.Register("SelectedValue", typeof(object),
              typeof(ListeObjet), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceDP); }
            set { SetValue(ItemsSourceDP, value); }
        }

        public static readonly DependencyProperty ItemsSourceDP =
            DependencyProperty.Register("ItemsSource", typeof(object),
              typeof(ListeObjet), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        
        private void ApplyEditable()
        {
            if (Editable == true)
            {
                xBase.Visibility = Visibility.Visible;
                xValeur.Visibility = Visibility.Visible;
                xValeur.IsHitTestVisible = true;
                xValeur.Background = Brushes.White;
                xValeur.BorderThickness = new Thickness(1);
                xValeurTexte.Visibility = Visibility.Collapsed;
            }
            else
            {
                xValeur.Visibility = Visibility.Collapsed;
                xValeur.IsHitTestVisible = false;
                xValeur.ToolTip = null;
                xValeur.Background = Brushes.Transparent;
                xValeur.BorderThickness = new Thickness(0);
                xValeurTexte.Visibility = Visibility.Visible;
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == EditableDP)
            {
                ApplyEditable();
                //(this as UIElement).FindVisualParent<ListView>().Ajuster_Colonnes();
            }

            if (e.Property == SelectedValueDP)
            {
                ApplyEditable();

                if (Intitule == true)
                    xIntitule.Visibility = Visibility.Visible;
                else
                    xIntitule.Visibility = Visibility.Collapsed;

                String Objet = "";
                String Propriete = "";
                String TypePropriete = "";

                if (InfosBinding(e.Property, ref Objet, ref Propriete, ref TypePropriete))
                {
                    String pIntitule = DicIntitules.Intitule(Objet, Propriete);
                    xIntitule.Text = pIntitule + " :";

                    if (String.IsNullOrWhiteSpace(SelectedValue.ToString()) && (Editable == false))
                        xBase.Visibility = Visibility.Collapsed;

                    String ToolTip = DicIntitules.Info(Objet, Propriete);
                    if (!String.IsNullOrWhiteSpace(ToolTip))
                        xBase.ToolTip = ToolTip;
                }
            }

            base.OnPropertyChanged(e);
        }

        public ListeObjet()
        {
            InitializeComponent();

            if (Intitule == true)
                xIntitule.Visibility = Visibility.Visible;
            else
                xIntitule.Visibility = Visibility.Collapsed;

            if (Editable == true)
                xValeur.IsHitTestVisible = true;
            else
            {
                xValeur.IsHitTestVisible = false;
                xValeur.ToolTip = null;
            }
        }
    }
}
