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
              typeof(ListeObjet), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Boolean Intitule
        {
            get { return (Boolean)GetValue(IntituleDP); }
            set { SetValue(IntituleDP, value); }
        }

        public static readonly DependencyProperty IntituleDP =
            DependencyProperty.Register("Intitule", typeof(Boolean),
              typeof(ListeObjet), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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
        
        private void MajEditable()
        {
            if (xBase == null || xValeur == null) return;

            try
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
                    if (xValeur.SelectedValue != null && String.IsNullOrWhiteSpace(xValeur.SelectedValue.ToString()))
                        xBase.Visibility = Visibility.Collapsed;

                    xValeur.Visibility = Visibility.Collapsed;
                    xValeur.IsHitTestVisible = false;
                    xValeur.ToolTip = null;
                    xValeur.Background = Brushes.Transparent;
                    xValeur.BorderThickness = new Thickness(0);
                    xValeurTexte.Visibility = Visibility.Visible;
                }
            }
            catch { }
        }

        private void MajSelectedValue()
        {
            if (Intitule == true)
                xIntitule.Visibility = Visibility.Visible;
            else
                xIntitule.Visibility = Visibility.Collapsed;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((e.Property == SelectedValueDP) && String.IsNullOrWhiteSpace(Objet))
                InfosBinding(SelectedValueDP, ref Objet, ref ProprieteValeur, ref TypeProprieteValeur);

            if (IsLoaded)
            {
                if (e.Property == EditableDP)
                    MajEditable();

                if (e.Property == SelectedValueDP)
                    MajSelectedValue();
            }

            base.OnPropertyChanged(e);
        }

        public ListeObjet()
        {
            Loaded += ListeObjet_Loaded;
            InitializeComponent();
        }

        private void ListeObjet_Loaded(object sender, RoutedEventArgs e)
        {
            MajEditable();
            MajSelectedValue();

            xIntitule.Text = DicIntitules.Intitule(Objet, ProprieteValeur) + " :";

            String ToolTip = DicIntitules.Info(Objet, ProprieteValeur);
            if (!String.IsNullOrWhiteSpace(ToolTip))
                xBase.ToolTip = ToolTip;
        }
    }
}
