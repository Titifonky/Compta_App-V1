using System;
using System.Windows;
using System.Windows.Controls;

namespace Compta
{
    public partial class GridExpander : Expander
    {
        private Double SvgWidth = 0;

        private ColumnDefinition Colonne = null;

        private void Init()
        {
            int Index = Grid.GetColumn(this);
            FrameworkElement El = this as FrameworkElement;
            Grid G = El.Parent as Grid;
            Colonne = G.ColumnDefinitions[Index];
        }

        protected override void OnExpanded()
        {
            if (IsInitialized)
                Colonne.Width = new GridLength(SvgWidth);

            base.OnExpanded();
        }

        protected override void OnCollapsed()
        {
            if (IsInitialized)
            {
                SvgWidth = Colonne.ActualWidth;
                Colonne.Width = GridLength.Auto;
            }

            base.OnCollapsed();
        }

        public override void EndInit()
        {
            base.EndInit();
            Init();
        }
    }
}