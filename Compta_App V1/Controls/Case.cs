using LogDebugging;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Compta
{
    public partial class Case : ControlBase
    {
        public Boolean Editable
        {
            get { return (Boolean)GetValue(EditableDP); }
            set { SetValue(EditableDP, value); }
        }

        public static readonly DependencyProperty EditableDP =
            DependencyProperty.Register("Editable", typeof(Boolean),
              typeof(Case), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Boolean Intitule
        {
            get { return (Boolean)GetValue(IntituleDP); }
            set { SetValue(IntituleDP, value); }
        }

        public static readonly DependencyProperty IntituleDP =
            DependencyProperty.Register("Intitule", typeof(Boolean),
              typeof(Case), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Boolean IntituleDerriere
        {
            get { return (Boolean)GetValue(IntituleDerriereDP); }
            set { SetValue(IntituleDerriereDP, value); }
        }

        public static readonly DependencyProperty IntituleDerriereDP =
            DependencyProperty.Register("IntituleDerriere", typeof(Boolean),
              typeof(Case), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object Valeur
        {
            get { return (object)GetValue(ValeurDP); }
            set { SetValue(ValeurDP, value); }
        }

        public static readonly DependencyProperty ValeurDP =
            DependencyProperty.Register("Valeur", typeof(object),
              typeof(Case), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void ApplyEditable()
        {
            if (xBase == null || xValeur == null) return;

            try
            {
                if (Editable == true)
                {
                    xBase.Visibility = Visibility.Visible;
                    xValeur.Visibility = Visibility.Visible;
                    xValeur.IsHitTestVisible = true;
                }
                else
                {
                    xValeur.Visibility = Visibility.Visible;
                    xValeur.IsHitTestVisible = false;

                    if (xValeur.IsChecked == false)
                        xBase.Visibility = Visibility.Collapsed;
                }
            }
            catch { }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            ApplyEditable();

            if (e.Property == IntituleDerriereDP)
            {
                String Objet = "";
                String Propriete = "";
                String TypePropriete = "";

                if (InfosBinding(e.Property, ref Objet, ref Propriete, ref TypePropriete))
                {
                    String pIntitule = DicIntitules.Intitule(Objet, Propriete);
                    if (IntituleDerriere == false)
                        pIntitule = pIntitule + " :";

                    xIntitule.Text = pIntitule;
                }
            }

            if (e.Property == ValeurDP)
            {
                if (Intitule == true)
                    xIntitule.Visibility = Visibility.Visible;
                else
                    xIntitule.Visibility = Visibility.Collapsed;

                if (IntituleDerriere == true)
                {
                    Grid.SetColumn(xIntitule, 1);
                    Grid.SetColumn(xValeur, 0);
                    xIntitule.Margin = new Thickness(5, 0, 0, 0);
                }

                String Objet = "";
                String Propriete = "";
                String TypePropriete = "";

                if (InfosBinding(e.Property, ref Objet, ref Propriete, ref TypePropriete))
                {
                    String pIntitule = DicIntitules.Intitule(Objet, Propriete);
                    if (IntituleDerriere == false)
                        pIntitule = pIntitule + " :";

                    xIntitule.Text = pIntitule;

                    String ToolTip = DicIntitules.Info(Objet, Propriete);
                    if (!String.IsNullOrWhiteSpace(ToolTip))
                        xBase.ToolTip = ToolTip;
                }
            }

            base.OnPropertyChanged(e);
        }

        public Case()
        {
            InitializeComponent();
        }
    }
}
