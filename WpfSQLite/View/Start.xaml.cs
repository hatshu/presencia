using System;
using System.Data.SqlClient;
using System.Windows;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
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
         cargaCombobox();
      }

      private static void cargaCombobox()
      {
         string conexionString = ConfigurationManager.ConnectionStrings["ConexionPruebas"].ConnectionString;
         using (SqlConnection conn = new SqlConnection(conexionString))
         {
            conn.Open();
            MessageBox.Show(conn.DataSource);
            DataTable dt = new DataTable();
            SqlCommand consultaUsuarios = new SqlCommand("SELECT * FROM tb_Users", conn );
            conn.Close();
         }
         ;
      }


      private void Back_OnClick(object sender, RoutedEventArgs e)
      {
         NavigationService?.GoBack();
      }
   }
}
