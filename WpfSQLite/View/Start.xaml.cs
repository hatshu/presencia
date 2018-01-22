using System.Windows;
using System.Windows.Controls;

namespace Presencia.View
{
   /// <summary>
   /// Lógica de interacción para Start.xaml
   /// </summary>
   public partial class Start : Page
   {
      public Start()
      {
         InitializeComponent();
      }

      private void Back_OnClick(object sender, RoutedEventArgs e)
      {
         NavigationService?.GoBack();
      }
   }
}
