using System;
using System.Windows;

namespace Compta
{
    public partial class Date : ControlBase
    {
        public Boolean Editable
        {
            get { return (Boolean)GetValue(EditableDP); }
            set { SetValue(EditableDP, value); }
        }

        public static readonly DependencyProperty EditableDP =
            DependencyProperty.Register("Editable", typeof(Boolean),
              typeof(Date), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Boolean Intitule
        {
            get { return (Boolean)GetValue(IntituleDP); }
            set { SetValue(IntituleDP, value); }
        }

        public static readonly DependencyProperty IntituleDP =
            DependencyProperty.Register("Intitule", typeof(Boolean),
              typeof(Date), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Boolean Info
        {
            get { return (Boolean)GetValue(InfosDP); }
            set { SetValue(InfosDP, value); }
        }

        public static readonly DependencyProperty InfosDP =
            DependencyProperty.Register("Info", typeof(Boolean),
              typeof(Date), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public DateTime Valeur
        {
            get { return (DateTime)GetValue(ValeurDP); }
            set { SetValue(ValeurDP, value); }
        }

        public static readonly DependencyProperty ValeurDP =
            DependencyProperty.Register("Valeur", typeof(object),
              typeof(Date), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void MajEditable()
        {
            if (xBase == null || xValeur == null) return;

            try
            {
                if (Editable)
                {
                    xBase.Visibility = Visibility.Visible;
                    xValeur.Visibility = Visibility.Visible;
                    xAfficher.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (Valeur != null && String.IsNullOrWhiteSpace(Valeur.ToString()))
                    {
                        xBase.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        xValeur.Visibility = Visibility.Collapsed;
                        xAfficher.Visibility = Visibility.Visible;
                    }
                }
            }
            catch { }
        }

        private void MajValeur()
        {
            if (Intitule == true)
                xIntitule.Visibility = Visibility.Visible;
            else
                xIntitule.Visibility = Visibility.Collapsed;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((e.Property == ValeurDP) && String.IsNullOrWhiteSpace(Objet))
                InfosBinding(ValeurDP, ref Objet, ref ProprieteValeur, ref TypeProprieteValeur);

            if (IsLoaded)
            {
                if (e.Property == EditableDP)
                    MajEditable();

                if (e.Property == ValeurDP)
                    MajValeur();
            }

            base.OnPropertyChanged(e);
        }

        public Date()
        {
            Loaded += Date_Loaded;
            InitializeComponent();
        }

        private void Date_Loaded(object sender, RoutedEventArgs e)
        {
            MajEditable();
            MajValeur();

            xIntitule.Text = DicIntitules.Intitule(Objet, ProprieteValeur) + " :";
            String ToolTip = DicIntitules.Info(Objet, ProprieteValeur);
            if (!String.IsNullOrWhiteSpace(ToolTip) && (Info == true))
                xBase.ToolTip = ToolTip;
        }
    }
}