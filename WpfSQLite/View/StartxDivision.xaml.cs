using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Presencia.ViewModel;

namespace Presencia.View
{
   /// <summary>
   /// Lógica de interacción para StartxDivision.xaml
   /// </summary>
   public partial class StartxDivision : Page
   {
      private StartxDivisionViewModel StartxDivisionViewModel;

      public StartxDivision()
      {
         InitializeComponent();
         StartxDivisionViewModel = new StartxDivisionViewModel();
         this.DataContext = StartxDivisionViewModel;
      }

      private void Back_OnClick(object sender, RoutedEventArgs e)
      {
         NavigationService?.GoBack();
      }

      private void DataGrid_Loaded(object sender, RoutedEventArgs e)
      {
         DataGrid dataGrid = sender as DataGrid;
         dataGrid?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
      }
   }
}