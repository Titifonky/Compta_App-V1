using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Compta
{
    public static class OutilsXaml
    {
        public static void Ajuster_Colonnes(this ListView L)
        {
            if (L == null) return;

            GridView G = L.View as GridView;
            if (G != null)
            {
                foreach (GridViewColumn C in G.Columns)
                {
                    C.Width = C.ActualWidth;

                    C.Width = double.NaN;
                }
            }
        }

        public static T FindVisualParent<T>(this UIElement element) where T : UIElement
        {
            UIElement parent = element; while (parent != null)
            {
                T correctlyTyped = parent as T; if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }
                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }
    }
}
