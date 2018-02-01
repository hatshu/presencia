using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using Presencia.Model;

namespace Presencia.ViewModel
{
   class StartViewModel
   {

      #region Attributes

      //cadena de conexion a la base de datos
      string conexionString = ConfigurationManager.ConnectionStrings["ConexionBaseDeDatos"].ConnectionString;

      //FOR COMBOBOX ACCESS WITH MVVM
      //public ObservableCollection<string> ActiveUsers { get; set; }

      private string _sActiveUser;

      public string SActiveUser
      {
         get { return _sActiveUser; }
         set { _sActiveUser = value; }
      }

      private string _sAreaCentro;

      public string SAreaCentro
      {
         get { return _sAreaCentro; }
         set { _sAreaCentro = value; }
      }

      public DateTime StartDate { get; set; } = DateTime.Today.Date;

      public DateTime EndDate { get; set; } = DateTime.Today.Date;

      public DelegateCommand SearchCommand { get; set; }

      private ObservableCollection<IdUser> _usersIdAndNameList { get; set; }


      public ObservableCollection<IdUser> UsersIdAndNameList
      {
         get { return _usersIdAndNameList; }
         set
         {
            _usersIdAndNameList = value;
            NotifyPropertyChanged("UsersIdAndNameList");
         }
      }

      private ObservableCollection<string> _areasCentro = new ObservableCollection<string>();

      public ObservableCollection<string> AreasCentro
      {
         get { return _areasCentro; }
         set
         {
            _areasCentro = value;
            NotifyPropertyChanged("AreasCentro");
         }

      }

      public List<IdUser> CardCodeAndUserIdList = new List<IdUser>();

      public List<LockAuditTrail> LockAuditTrailList = new List<LockAuditTrail>();

      public ObservableCollection<UserData> UserDataBrutoList;

      public ObservableCollection<UserData> UserDataxDia;

      public List<List<UserData>> UserDataxDiaDefinitivo;

      private ObservableCollection<UserData> _listaFinal = new ObservableCollection<UserData>();

      public ObservableCollection<UserData> ListaFinal
      {
         get { return _listaFinal; }
         set
         {
            _listaFinal = value;
            NotifyPropertyChanged("ListaFinal");
         }
      }



      private ObservableCollection<string> _totalConjuntoHoras;

      public ObservableCollection<string> TotalConjuntoHoras
      {
         get { return _totalConjuntoHoras; }
         set
         {
            _totalConjuntoHoras = value;
            NotifyPropertyChanged("TotalConjuntoHoras");
         }

      }



      #endregion

      #region Constructor

      public StartViewModel()
      {
         //ActiveUsers = new ObservableCollection<string>();
         UsersIdAndNameList = new ObservableCollection<IdUser>();
         UserDataBrutoList = new ObservableCollection<UserData>();
         UserDataxDia = new ObservableCollection<UserData>();
         UserDataxDiaDefinitivo = new List<List<UserData>>();
         //TODO introducir area y filtrado por area
         ObtenerIdUserAndCardCode();
         //cargaCombobox();

         SearchCommand = new DelegateCommand(SearchCommand_Execute, SearchCommand_CanExecute);
         TotalConjuntoHoras = new ObservableCollection<string>();
      }

      #endregion

      // Para que se detecte cuando se cambia cada elemento
      public event PropertyChangedEventHandler PropertyChanged;

      private void NotifyPropertyChanged(string info)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
      }

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
                  Id = ObtenerIdUser(),
                  CardCode = ObtenerCardCodeDeId(ObtenerIdUser()),
                  FechaInicio = StartDate,
                  FechaFin = EndDate.AddHours(23).AddMinutes(59)

               };
               MessageBox.Show("Se ha seleccionado el usuario: " + item.Nombre + "Area: " + SAreaCentro+ " Fecha inicio " + item.FechaInicio + " Fecha fin " + item.FechaFin);
               consultaEventosSQLdeFechas(item);
               TotalConjuntoHoras.Clear();
               TotalConjuntoHoras.Add(calculoTotalHorasDeLista());
               UpdateUI();
            }
            else
            {
               MessageBox.Show("Los parámetros de búsqueda no son correctos o algún campo está vacio.");
            }

         }
      }

      private int  ObtenerIdUser()
      {
         var id = 0;
         foreach (var itemUser in UsersIdAndNameList)
         {
            if (itemUser.Nombre.Equals(SActiveUser))
            {
               id = itemUser.Id;
               return id;
            }
         }
         return 0;
      }


      //SACA LISTA DE ENTRADAS Y SALIDAS

      private void consultaEventosSQLdeFechas(SearchItem item)
      {

         //Hacer consulta sql de entradas y salidas de todos los cardode en las fechas seleccionadas
         SqlConnection conn = new SqlConnection(conexionString);
         LockAuditTrailList.Clear();
         UserDataBrutoList.Clear();
         UserDataxDia.Clear();
         ListaFinal.Clear();
         try
         {
            conn.Open();
            string Query =
               "SELECT id_event, dt_Audit, id_lock, id_user, id_function, NCopy  FROM tb_LockAuditTrail WHERE  (id_lock='1026') AND (dt_Audit >= '" +
               item.FechaInicio + "' AND dt_Audit <='" + item.FechaFin + "') AND (id_function='17' OR id_function='145' OR id_function='84' OR id_function='85' OR id_function='212' OR id_function='213')  ORDER BY dt_Audit";
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

            //buscar la relaccion entre CardCode y el id_event
            foreach (var itemAuditTrail in LockAuditTrailList)
            {
               //Pasamos desde "string con formato hexadecimal" a int
               var substring = itemAuditTrail.Id_Event.Substring(15, 5);
               int miNcopyReal = Convert.ToInt32(substring,16);
               //Calculamos el "Cardcode" ( Cardcode = NCopy "real" / 4 )
               var miCardCode = miNcopyReal / 4;
               itemAuditTrail.CardCode = miCardCode;
            }

            //creacion de lista con los datos del usuario seleccionado de los dias pertinentes
            foreach (var itemEvent in LockAuditTrailList)
            {
               if (itemEvent.CardCode.Equals(item.CardCode))
               {
                  var userDataFind = new UserData();
                  userDataFind.Nombre = item.Nombre;
                  userDataFind.Id = item.Id;
                  userDataFind.CardCode = item.CardCode;
                  userDataFind.Entrada = StartDate;
                  userDataFind.Salida = EndDate;
                  userDataFind.FechaEvento = itemEvent.Dt_Audit;
                  userDataFind.TipoEvento = conocertipodeevento(itemEvent);
                  UserDataBrutoList.Add(userDataFind);
               }
            }


            //Conocer cuantos días se han seleccionado
            var fechaDias = item.FechaFin - item.FechaInicio;
            var totalDias = fechaDias.Days;


            var list = UserDataBrutoList.GroupBy(d => DateTime.Parse(d.FechaEvento).Date).Select(g => g.ToList()).ToList();

            foreach (var itemData in list)
            {
               //TODO: contemplar que hacer si el primero no sea un elemento salida y el último no sea salida

               if (itemData.First().TipoEvento.Equals("ENTRADA"))
               {
                  UserDataxDia.Add(itemData.First());
               }
               else
               {
                  //UserDataxDia.Move(0,1);
               }
               if (itemData.Last().TipoEvento.Equals("SALIDA"))
               {
                  UserDataxDia.Add(itemData.Last());
               }
            }

            UserDataxDiaDefinitivo = UserDataxDia.GroupBy(d => DateTime.Parse(d.FechaEvento).Date).Select(g => g.ToList()).ToList();

            foreach (var itemData in UserDataxDiaDefinitivo)
            {
               UserData data = new UserData();
               var entradaHoraAux = new DateTime();
               foreach (var subitemData in itemData)
               {
                  if (subitemData.TipoEvento.Equals("ENTRADA"))
                  {
                     entradaHoraAux= DateTime.Parse(subitemData.FechaEvento);
                  }
                  if (subitemData.TipoEvento.Equals("SALIDA"))
                  {
                     data.TotalHoras = (DateTime.Parse(subitemData.FechaEvento) - entradaHoraAux.TimeOfDay);
                     data.Id = subitemData.Id;
                     data.Entrada = entradaHoraAux;
                     data.Nombre = subitemData.Nombre;
                     data.CardCode = subitemData.CardCode;
                     data.FechaEvento = subitemData.FechaEvento.Substring(0, 10);
                     data.Salida = DateTime.Parse(subitemData.FechaEvento);
                     data.Ausencia = "No lanzada";
                     ListaFinal.Add(data);
                  }

               }
            }


         }
         catch (Exception e)
         {
            MessageBox.Show(e.Message);
         }
      }

      private string calculoTotalHorasDeLista()
      {
         float horas =0, min=0,enminutos =0, enhoras=0;
         string hora = " ";

         foreach (var itemData in ListaFinal)
         {
            horas = itemData.TotalHoras.Hour + horas;
            min = itemData.TotalHoras.Minute + min;
         }
         enminutos = (horas * 60) + min;
         enhoras = enminutos / 60;
         hora = enhoras.ToString();

         return hora;
      }

      private string conocertipodeevento(LockAuditTrail itemEvent)
      {
         if (itemEvent.Id_Function == 17)
         {
            return "ENTRADA";
         }
         if (itemEvent.Id_Function == 84)
         {
            return "ENTRADA";
         }
         if (itemEvent.Id_Function == 85)
         {
            return "ENTRADA";
         }
         if (itemEvent.Id_Function == 145)
         {
            return "SALIDA";
         }
         if (itemEvent.Id_Function == 212)
         {
            return "SALIDA";
         }
         if (itemEvent.Id_Function == 213)
         {
            return "SALIDA";
         }

         return "";
      }


      private int ObtenerCardCodeDeId(int id)
      {
         var value = 0;
         foreach (var itemUser in UsersIdAndNameList)
         {
            if (itemUser.Id.Equals(id))
            {
               return itemUser.CardCode;
            }
         }

         return 0;
      }

      bool SearchCommand_CanExecute(object parameters)
      {
         return true;
      }

      #endregion

      #region Obtener lista con nombres, id y cardCode


      void ObtenerIdUserAndCardCode()
      {
         //OBTENCION DE NOMBRE DE USUARIO E ID USER
         SqlConnection conn = new SqlConnection(conexionString);
         try
         {
            conn.Open();
            string Query = "SELECT id_user, Title, FirstName, LastName status FROM tb_Users WHERE (Title='CTC') AND (status='1') ORDER BY FirstName";
            SqlCommand createCommand = new SqlCommand(Query, conn);
            SqlDataReader dr = createCommand.ExecuteReader();
            while (dr.Read())
            {
               var userItem = new IdUser();
               userItem.Nombre = dr[2].ToString();
               userItem.Id = dr[0].GetHashCode();
               userItem.Area = dr[3].ToString();
               UsersIdAndNameList.Add(userItem);
            }
            conn.Close();
         }
         catch (Exception e)
         {
            MessageBox.Show(e.Message);
         }

         //OBTENCION DEL CARDCODE
         try
         {
            conn.Open();
            string Query = "SELECT  id_user, Cardcode, NewCount FROM tb_Cards ORDER BY NewCount ASC";
            SqlCommand createCommand = new SqlCommand(Query, conn);
            SqlDataReader dr = createCommand.ExecuteReader();
            while (dr.Read())
            {
               var userItem = new IdUser();
               userItem.Id = dr[0].GetHashCode();
               userItem.CardCode = dr[1].GetHashCode();
               if (userItem.Id!=1)
               {
                  CardCodeAndUserIdList.Add(userItem);
               }
             }
            conn.Close();
         }
         catch (Exception e)
         {
            MessageBox.Show(e.Message);
         }

         //UNIR CARDCODE Y  USER ID

         foreach (var item in UsersIdAndNameList)
         {
            foreach (var itemToCompare in CardCodeAndUserIdList)
            {
               if ( itemToCompare.Id.Equals(item.Id))
               {
                  item.CardCode = itemToCompare.CardCode;
               }
            }
         }

         //OBTENCION DE AREAS

         //TODO Obtener areas y filtrado de areas

       ObservableCollection<string> areasAux = new ObservableCollection<string>();
            foreach (var item in UsersIdAndNameList)
            {
               {
                  areasAux.Add(item.Area);
               }
            }


         AreasCentro = new ObservableCollection<string>(areasAux.Distinct());

      }




      #endregion

      //#region Carga de Combobox con nombres
      ////TODO: intentar cambiar el contenido del combobox a el nombre de la lista  UsersIdAndNameList
      //private void cargaCombobox()
      //{
      //   SqlConnection conn = new SqlConnection(conexionString);

      //   try
      //   {
      //      conn.Open();
      //      string Query = "select * from tb_Users where status='1' and name like '%CTC%' ORDER BY FirstName";
      //      SqlCommand createCommand = new SqlCommand(Query, conn);
      //      SqlDataReader dr = createCommand.ExecuteReader();
      //      while (dr.Read())
      //      {
      //         string userName = dr.GetString(3);
      //         ActiveUsers.Add(userName);
      //      }
      //      conn.Close();
      //   }
      //   catch (Exception e)
      //   {
      //      MessageBox.Show(e.Message);
      //      //TODO: volver atras en la navegación si da error

      //   }
      //}




      //#endregion

      public void UpdateUI()
      {
         //Here update your label, button or any string related object.
         //Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
         Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
      }


   }
}
