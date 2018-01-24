using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Presencia.Model;

namespace Presencia.ViewModel
{
   class StartViewModel
   {
      #region Attributes

      //cadena de conexion a la base de datos
      string conexionString = ConfigurationManager.ConnectionStrings["ConexionBaseDeDatos"].ConnectionString;

      //FOR COMBOBOX ACCESS WITH MVVM
      public ObservableCollection<string> ActiveUsers { get; set; }

      private string _sActiveUser;

      public string SActiveUser
      {
         get { return _sActiveUser; }
         set { _sActiveUser = value; }
      }

      private DateTime _startDate = DateTime.Today.Date;

      public DateTime StartDate
      {
         get { return _startDate; }
         set { _startDate = value; }
      }

      private DateTime _endDate = DateTime.Today.Date;

      public DateTime EndDate
      {
         get { return _endDate; }
         set { _endDate = value; }
      }

      public DelegateCommand SearchCommand { get; set; }

      List<IdUser> UsersAndId = new List<IdUser>();

      #endregion

      #region Constructor

      public StartViewModel()
      {
         ActiveUsers = new ObservableCollection<string>();
         SearchCommand = new DelegateCommand(SearchCommand_Execute, SearchCommand_CanExecute);
         cargaCombobox();
         ObtenerIdUserAndCardCode();
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

      #region SearchCommand Button

      void SearchCommand_Execute(object parameters)
      {
         if (SearchCommand_CanExecute(parameters))
         {
            if (SActiveUser!=null && StartDate<=EndDate)
            {
               SearchItem item = new SearchItem
               {
                  Nombre = SActiveUser,
                  FechaInicio = StartDate,
                  FechaFin = EndDate

               };
               MessageBox.Show("Se ha seleccionado el usuario: " + item.Nombre + " Fecha inicio " + item.FechaInicio.Date + " Fecha fin " + item.FechaFin.Date);

            }
            else
            {
               MessageBox.Show("Los parámetros de búsqueda no son correctos o algún campo está vacio.");
            }

         }
      }



      bool SearchCommand_CanExecute(object parameters)
      {
         return true;
      }


      #endregion

      #region Rellenar datos de fechas y usuarios solicitados
      //todo obtener lista con nombres, id y cardCode

      void ObtenerIdUserAndCardCode()
      {
         SqlConnection conn = new SqlConnection(conexionString);
         try
         {
            conn.Open();
            string Query = "SELECT id_user, Title, FirstName, Cardcode status FROM tb_Users WHERE (Title='CTC') AND (status='1') ORDER BY FirstName";
            SqlCommand createCommand = new SqlCommand(Query, conn);
            SqlDataReader dr = createCommand.ExecuteReader();
            while (dr.Read())
            {
               var userItem = new IdUser();
               userItem.Nombre = dr[2].ToString();
               userItem.Id = dr[0].GetHashCode();
               userItem.CardCode = dr[3].GetHashCode();
               UsersAndId.Add(userItem);
            }
            conn.Close();
         }
         catch (Exception e)
         {
            MessageBox.Show(e.Message);
         }
      }




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
