using System;
using System.Data.SqlClient;
using System.Windows;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using System.Windows.Controls;
using Presencia.ViewModel;

namespace Presencia.View
{
   /// <summary>
   /// Lógica de interacción para Start.xaml
   /// </summary>
   public partial class Start : Page
   {
      private StartViewModel StartViewModel;
      //string conexionString = ConfigurationManager.ConnectionStrings["ConexionBaseDeDatos"].ConnectionString;


      public Start()
      {
         InitializeComponent();
         StartViewModel = new StartViewModel();
         this.DataContext = StartViewModel;
         
         //cargaCombobox();

      }


      

      //private void cargaCombobox()
      //{
      //   SqlConnection conn = new SqlConnection(conexionString);
      //   try
      //   {
      //      conn.Open();
      //      string Query = "select * from tb_Users where status='1' and name like '%CTC%' ";
      //      SqlCommand createCommand = new SqlCommand(Query, conn);
      //      SqlDataReader dr = createCommand.ExecuteReader();
      //      while (dr.Read())
      //      {
      //         string userName = dr.GetString(3);
      //         ComboBox.Items.Add(userName);
      //      }
      //      conn.Close();
      //   }
      //   catch (Exception e)
      //   {
      //      MessageBox.Show(e.Message);
      //      //TODO: volver atras en la navegación si da error
      //   }
      //}


      private void Back_OnClick(object sender, RoutedEventArgs e)
      {
         NavigationService?.GoBack();
      }

   }
}
