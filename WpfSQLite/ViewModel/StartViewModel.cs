using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
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

      public DateTime StartDate { get; set; } = DateTime.Today.Date;

      public DateTime EndDate { get; set; } = DateTime.Today.Date;

      public DelegateCommand SearchCommand { get; set; }

      public List<IdUser> UsersIdAndCodeCardsList = new List<IdUser>();

      public List<LockAuditTrail> LockAuditTrailList = new List<LockAuditTrail>();

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


      #region SearchCommand Button

      void SearchCommand_Execute(object parameters)
      {
         if (SearchCommand_CanExecute(parameters))
         {
            if (SActiveUser!=null && StartDate<=EndDate )
            {
               SearchItem item = new SearchItem
               {
                  Nombre = SActiveUser,
                  FechaInicio = StartDate,
                  FechaFin = EndDate.AddHours(23).AddMinutes(59)

               };
               MessageBox.Show("Se ha seleccionado el usuario: " + item.Nombre + " Fecha inicio " + item.FechaInicio.Date + " Fecha fin " + item.FechaFin.Date);
               consultaEventosSQLdeFechas(item);

            }
            else
            {
               MessageBox.Show("Los parámetros de búsqueda no son correctos o algún campo está vacio.");
            }

         }
      }

      private void consultaEventosSQLdeFechas(SearchItem item)
      {
       ////TODO:  buscar  item.Nombre en la lista y obeter el id
       //  foreach (var userData in UsersIdAndCodeCardsList)
       //  {
       //     if (userData.Nombre.Equals(item.Nombre))
       //     {
       //        //TODO: comprobar si con user_id se puede si no con CardCode NO CALE EL USER_ID

       //        item.Id = userData.CardCode;
       //        break;
       //     }
       //
       //  }


         //TODO: Hacer consulta sql de entradas y salidas de este cardode en las fechas seleccionadas
         SqlConnection conn = new SqlConnection(conexionString);
         LockAuditTrailList.Clear();
         try
         {
            conn.Open();
            string Query =
               "SELECT id_event, dt_Audit, id_lock, id_user, id_function, NCopy  FROM tb_LockAuditTrail WHERE  (id_lock='1026') AND (dt_Audit >= '" +
               item.FechaInicio + "' AND dt_Audit <='" + item.FechaFin + "')  ORDER BY dt_Audit";
            SqlCommand createCommand = new SqlCommand(Query, conn);
            SqlDataReader dr = createCommand.ExecuteReader();
            while (dr.Read())
            {
               var lockAuditTrail = new LockAuditTrail();
               lockAuditTrail.Id_User = dr[3].GetHashCode();
               lockAuditTrail.Id_Event = dr[0].ToString();
               lockAuditTrail.Dt_Audit = dr[1].ToString();
               lockAuditTrail.Id_Function = dr[4].GetHashCode();
               lockAuditTrail.NCopy = dr[5].GetHashCode();
               LockAuditTrailList.Add(lockAuditTrail);
            }
            conn.Close();

            //TODO: buscar la relaccion entre CardCode y el id_event
            foreach (var itemAuditTrail in LockAuditTrailList)
            {
               //Pasamos desde "string con formato hexadecimal" a int
               var substring = itemAuditTrail.Id_Event.Substring(16, 4);
               var miNcopyReal = int.Parse(substring);
               //Calculamos el "Cardcode" ( Cardcode = NCopy "real" / 4 )
               var miCardCode = miNcopyReal / 4;
               itemAuditTrail.CardCode = miCardCode;
            }

         }
         catch (Exception e)
         {
            MessageBox.Show(e.Message);
         }
      }

      bool SearchCommand_CanExecute(object parameters)
      {
         return true;
      }

      #endregion

      #region Obtener lista con nombres, id y cardCode


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
               UsersIdAndCodeCardsList.Add(userItem);
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
      //TODO: intentar cambiar el contenido del combobox a el nombre de la lista  UsersIdAndCodeCardsList
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
