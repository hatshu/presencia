using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Presencia.ViewModel
{
   class StartViewModel
   {
      #region Attributes

      string conexionString = ConfigurationManager.ConnectionStrings["ConexionBaseDeDatos"].ConnectionString;

      //FOR COMBOBOX ACCESS WITH MVVM

      public ObservableCollection<string> ActiveUsers { get; set; }


      #endregion

      #region Constructor

      public StartViewModel()
      {
         ActiveUsers = new ObservableCollection<string>();
         cargaCombobox();
      }

      #endregion

      #region MVVM
      // Para que se detecte cuando se cambia cada elemento
      public event PropertyChangedEventHandler PropertyChanged;

      private void NotifyPropertyChanged(string info)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
      }
      #endregion

      #region Button

      #endregion

      #region Combobox
      private void cargaCombobox()
      {
         SqlConnection conn = new SqlConnection(conexionString);

         try
         {
            conn.Open();
            string Query = "select * from tb_Users where status='1' and name like '%CTC%' ";
            SqlCommand createCommand = new SqlCommand(Query, conn);
            SqlDataReader dr = createCommand.ExecuteReader();
            while (dr.Read())
            {
               string userName = dr.GetString(3);
               ActiveUsers.Add(userName);
            }
            conn.Close();
         }
         catch (Exception e)
         {
            MessageBox.Show(e.Message);
            //TODO: volver atras en la navegación si da error
            
         }
      }


      #endregion
   }
}
