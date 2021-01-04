using System.Windows;

namespace Compta
{
    public partial class Intitule : ControlBase
    {
        public object Valeur
        {
            get { return (object)GetValue(ValeurDP); }
            set { SetValue(ValeurDP, value); }
        }

        public static readonly DependencyProperty ValeurDP =
            DependencyProperty.Register("Valeur", typeof(object),
              typeof(Intitule), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        public Intitule()
        {
            InitializeComponent();
        }
    }
}