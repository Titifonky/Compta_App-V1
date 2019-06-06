using LogDebugging;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Compta
{

    public partial class Texte : ControlBase
    {
        private void TextBox_ToucheEntreeUpdate(object sender, KeyEventArgs e)
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
              typeof(Texte), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Boolean Intitule
        {
            get { return (Boolean)GetValue(IntituleDP); }
            set { SetValue(IntituleDP, value); }
        }

        public static readonly DependencyProperty IntituleDP =
            DependencyProperty.Register("Intitule", typeof(Boolean),
              typeof(Texte), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Boolean Unite
        {
            get { return (Boolean)GetValue(UniteDP); }
            set { SetValue(UniteDP, value); }
        }

        public static readonly DependencyProperty UniteDP =
            DependencyProperty.Register("Unite", typeof(Boolean),
              typeof(Texte), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Boolean AcceptsReturn
        {
            get { return (Boolean)GetValue(InfosDP); }
            set { SetValue(InfosDP, value); }
        }

        public static readonly DependencyProperty AcceptsReturnDP =
            DependencyProperty.Register("AcceptsReturn", typeof(Boolean),
              typeof(Texte), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingDP); }
            set { SetValue(TextWrappingDP, value); }
        }

        public static readonly DependencyProperty TextWrappingDP =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping),
              typeof(Texte), new FrameworkPropertyMetadata(TextWrapping.WrapWithOverflow, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentDP); }
            set { SetValue(TextAlignmentDP, value); }
        }

        public static readonly DependencyProperty TextAlignmentDP =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment),
              typeof(Texte), new FrameworkPropertyMetadata(TextAlignment.Left, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationDP); }
            set { SetValue(OrientationDP, value); }
        }

        public static readonly DependencyProperty OrientationDP =
            DependencyProperty.Register("Orientation", typeof(Orientation),
              typeof(Texte), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Boolean Info
        {
            get { return (Boolean)GetValue(InfosDP); }
            set { SetValue(InfosDP, value); }
        }

        public static readonly DependencyProperty InfosDP =
            DependencyProperty.Register("Info", typeof(Boolean),
              typeof(Texte), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Boolean IntituleSeul
        {
            get { return (Boolean)GetValue(IntituleSeulDP); }
            set { SetValue(IntituleSeulDP, value); }
        }

        public static readonly DependencyProperty IntituleSeulDP =
            DependencyProperty.Register("IntituleSeul", typeof(Boolean),
              typeof(Texte), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Thickness MargeInterne
        {
            get { return (Thickness)GetValue(MargeInterneDP); }
            set { SetValue(MargeInterneDP, value); }
        }

        public static readonly DependencyProperty MargeInterneDP =
            DependencyProperty.Register("MargeInterne", typeof(Thickness),
              typeof(Texte), new FrameworkPropertyMetadata(new Thickness(0, 2, 0, 2)));

        public object Valeur
        {
            get { return (object)GetValue(ValeurDP); }
            set { SetValue(ValeurDP, value); }
        }

        public static readonly DependencyProperty ValeurDP =
            DependencyProperty.Register("Valeur", typeof(object),
              typeof(Texte), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object Suffix
        {
            get { return (object)GetValue(SuffixDP); }
            set { SetValue(SuffixDP, value); }
        }

        public static readonly DependencyProperty SuffixDP =
            DependencyProperty.Register("Suffix", typeof(object),
              typeof(Texte), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object SuffixConcat
        {
            get { return (object)GetValue(SuffixConcatDP); }
            set { SetValue(SuffixConcatDP, value); }
        }

        public static readonly DependencyProperty SuffixConcatDP =
            DependencyProperty.Register("SuffixConcat", typeof(object),
              typeof(Texte), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private String _Unite = "";

        private void MajSuffix()
        {
            String Texte = _Unite;
            if (!String.IsNullOrWhiteSpace((String)Suffix))
                Texte = _Unite + (String)SuffixConcat + (String)Suffix;

            if (!String.IsNullOrWhiteSpace(Texte))
                xUnite.Visibility = Visibility.Visible;
            else
                xUnite.Visibility = Visibility.Collapsed;

            xUnite.Text = Texte;
        }

        private void ApplyEditable()
        {
            try
            {
                if (Editable)
                {
                    xBase.Visibility = Visibility.Visible;
                    xValeur.Visibility = Visibility.Visible;
                    xValeur.Background = Brushes.White;
                    xValeur.BorderThickness = new Thickness(1);
                    xValeur.IsHitTestVisible = true;
                    if (Unite)
                        xGrille.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(Valeur.ToString()))
                        xBase.Visibility = Visibility.Collapsed;

                    xValeur.Background = Brushes.Transparent;
                    xValeur.TextWrapping = TextWrapping;
                    xValeur.BorderThickness = new Thickness(0);
                    xValeur.IsHitTestVisible = false;
                    if (Unite)
                        xGrille.ColumnDefinitions[0].Width = GridLength.Auto;
                }
            }
            catch { }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            try
            {
                ApplyEditable();

                if (e.Property == SuffixDP)
                {
                    MajSuffix();
                }

                if (e.Property == SuffixConcatDP)
                {
                    MajSuffix();
                }

                if (e.Property == ValeurDP)
                {
                    if (Orientation == Orientation.Horizontal)
                        DockPanel.SetDock(xIntitule, Dock.Left);
                    else
                    {
                        DockPanel.SetDock(xIntitule, Dock.Top);
                        xIntitule.HorizontalAlignment = HorizontalAlignment.Left;
                    }

                    if (Intitule)
                        xIntitule.Visibility = Visibility.Visible;
                    else
                        xIntitule.Visibility = Visibility.Collapsed;

                    if (Unite)
                        xUnite.Visibility = Visibility.Visible;
                    else
                        xUnite.Visibility = Visibility.Collapsed;

                    String Objet = "";
                    String Propriete = "";
                    String TypePropriete = "";

                    if (InfosBinding(e.Property, ref Objet, ref Propriete, ref TypePropriete))
                    {
                        String pIntitule = DicIntitules.Intitule(Objet, Propriete);
                        xIntitule.Text = pIntitule + " :";

                        if (Unite)
                        {
                            _Unite = DicIntitules.Unite(Objet, Propriete);
                            xUnite.Text = _Unite;
                        }

                        MajSuffix();

                        String ToolTip = DicIntitules.Info(Objet, Propriete);
                        if (!String.IsNullOrWhiteSpace(ToolTip))
                            xBase.ToolTip = ToolTip;

                        if (IntituleSeul)
                        {
                            xGrille.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
            catch (Exception ex)
            { Log.Message(ex.ToString()); }

            base.OnPropertyChanged(e);
        }

        public Texte()
        {
            InitializeComponent();
        }
    }
}
