using System.Windows;
using System.Windows.Controls;

namespace Presencia.View
{
   /// <summary>
   /// Lógica de interacción para MainPage.xaml
   /// </summary>
   public partial class MainPage : Page
   {
      public MainPage()
      {
         InitializeComponent();
      }

      private void Start_OnClick(object sender, RoutedEventArgs e)
      {
         NavigationService?.Navigate(new Start());
      }

      private void Exit_OnClick(object sender, RoutedEventArgs e)
      {
         System.Windows.Application.Current.Shutdown();
      }
   }
}
