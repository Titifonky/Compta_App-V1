using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Compta
{
    public class EnumToIntConverter : IValueConverter
    {
        private Type _T;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null) || (!value.GetType().IsEnum))
                return null;

            _T = value.GetType();
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (_T != null)
                return Enum.Parse(_T, value.ToString());

            return null;
        }
    }

    public partial class ListeEnum : ControlBase
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
              typeof(ListeEnum), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Boolean Intitule
        {
            get { return (Boolean)GetValue(IntituleDP); }
            set { SetValue(IntituleDP, value); }
        }

        public static readonly DependencyProperty IntituleDP =
            DependencyProperty.Register("Intitule", typeof(Boolean),
              typeof(ListeEnum), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object Valeur
        {
            get { return (object)GetValue(ValeurDP); }
            set { SetValue(ValeurDP, value); }
        }

        public static readonly DependencyProperty ValeurDP =
            DependencyProperty.Register("Valeur", typeof(object),
              typeof(ListeEnum), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void ApplyEditable()
        {
            try
            {
                if (Editable == true)
                {
                    xValeur.Visibility = Visibility.Visible;
                    xValeur.IsHitTestVisible = true;
                    xValeur.Background = Brushes.White;
                    xValeur.BorderThickness = new Thickness(1);
                    xValeurTexte.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(Valeur.ToString()))
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

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
                ApplyEditable();

            if (e.Property == ValeurDP)
            {
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
                    xValeur.ItemsSource = DicIntitules.Enum(TypePropriete);

                    String ToolTip = DicIntitules.Info(Objet, Propriete);
                    if (!String.IsNullOrWhiteSpace(ToolTip))
                        xBase.ToolTip = ToolTip;
                }
            }

            base.OnPropertyChanged(e);
        }

        public ListeEnum()
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
