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


      #endregion

      #region Constructor

      public StartViewModel()
      {
         ActiveUsers = new ObservableCollection<string>();
         SearchCommand = new DelegateCommand(SearchCommand_Execute, SearchCommand_CanExecute);
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
               //consultaSQL(item);
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
      //todo llamada a la base de datos con estos datos para ver entradas y salidas de esos días
      private void consultaSQL(SearchItem item)
      {
         SqlConnection conn = new SqlConnection(conexionString);
         string codigopuerta = "1026";
         try
         {
            conn.Open();
            string Query = "SELECT id_event, dt_Audit, id_function, id_lock  FROM tb_LockAuditTrail WHERE (id_lock=" + codigopuerta + ") AND (dt_Audit >= '" + StartDate + "' AND dt_Audit <= '" + EndDate + " 23:00:00') AND (id_function='17' OR id_function='145' OR id_function='84' OR id_function='85' OR id_function='212' OR id_function='213') ORDER BY dt_Audit";
            SqlCommand createCommand = new SqlCommand(Query, conn);
            SqlDataReader dr = createCommand.ExecuteReader();
            while (dr.Read())
            {

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
