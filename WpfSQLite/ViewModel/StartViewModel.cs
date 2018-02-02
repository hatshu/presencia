using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using Presencia.Model;
using System.Windows.Interactivity;
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
         set
         {
            _sAreaCentro = value;
            //MessageBox.Show("Area ARRIBA: " + _sAreaCentro);
            NotifyPropertyChanged("SAreaCentro");
            if (!_sAreaCentro.Equals(""))
            {
               filtrarPorArea(_sAreaCentro);

            }
         }
      }

      public DateTime StartDate { get; set; } = DateTime.Today.Date;

      public DateTime EndDate { get; set; } = DateTime.Today.Date;

      public DelegateCommand SearchCommand { get; set; }

      public DelegateCommand SelectionChangedArea { get; set; }

      public DelegateCommand LimpiarCommand { get; set; }


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

      private ObservableCollection<IdUser> _listaFiltradaporArea { get; set; }


      public ObservableCollection<IdUser> ListaFiltradaporArea
      {
         get { return _listaFiltradaporArea; }
         set
         {
            _listaFiltradaporArea = value;
            NotifyPropertyChanged("ListaFiltradaporArea");
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

      //Se peude quitar el NotifyPropertyChanged y poner normal ya que no se bindea esta lista

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

      private ObservableCollection<ElementoListaResumen> _elementoListaResumen = new ObservableCollection<ElementoListaResumen>();

      public ObservableCollection<ElementoListaResumen> ElementoListaResumen
      {
         get { return _elementoListaResumen; }
         set
         {
            _elementoListaResumen = value;
            NotifyPropertyChanged("ElementoListaResumen");
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
         ListaFiltradaporArea = new ObservableCollection<IdUser>();

         UserDataBrutoList = new ObservableCollection<UserData>();
         UserDataxDia = new ObservableCollection<UserData>();
         SAreaCentro = "";
         UserDataxDiaDefinitivo = new List<List<UserData>>();
         //TODO introducir area y filtrado por area
         ObtenerIdUserAndCardCode();
         //cargaCombobox();
         foreach (var item in UsersIdAndNameList)
         {
            //ListaFiltradaporArea = UsersIdAndNameList;
            if (ListaFiltradaporArea.IndexOf(item) < 0)
            {
               ListaFiltradaporArea.Add(item);
            }
         }
         SearchCommand = new DelegateCommand(SearchCommand_Execute, SearchCommand_CanExecute);
         LimpiarCommand = new DelegateCommand(LimpiarCommand_Execute, LimpiarCommand_CanExecute);
         SelectionChangedArea = new DelegateCommand(SelectionChangedArea_Execute, SelectionChangedArea_CanExecute);
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
            if (SActiveUser != null && StartDate <= EndDate && StartDate<=DateTime.Today.Date && EndDate <=DateTime.Today.Date)
            {
               SearchItem item = new SearchItem
               {
                  Nombre = SActiveUser,
                  Id = ObtenerIdUser(),
                  CardCode = ObtenerCardCodeDeId(ObtenerIdUser()),
                  FechaInicio = StartDate,
                  FechaFin = EndDate.AddHours(23).AddMinutes(59)

               };
               //MessageBox.Show("Se ha seleccionado el usuario: " + item.Nombre + " Fecha inicio " + item.FechaInicio + " Fecha fin " + item.FechaFin);
               consultaEventosSQLdeFechas(item);
               TotalConjuntoHoras.Clear();
               TotalConjuntoHoras.Add(calculoTotalHorasDeLista());
               //UpdateUI();
            }
            else
            {
               if (SActiveUser == null)
               {
                  MessageBox.Show("El campo usuario está vacio");

               }
               else
               {
                  MessageBox.Show("Las fechas seleccionadas no son correctas");
               }
            }

         }
      }

      bool SearchCommand_CanExecute(object parameters)
      {
         return true;
      }

      #endregion

      #region SelectionChangedArea

      void SelectionChangedArea_Execute(object parameters)
      {
         if (!SAreaCentro.Equals(""))
         {
            //MessageBox.Show("area: " + SAreaCentro);

         }

      }
      bool SelectionChangedArea_CanExecute(object parameters)
      {
         return true;
      }

      #endregion

      #region LimpiarCommand

      void LimpiarCommand_Execute(object parameters)
      {
         StartDate = DateTime.Today.Date;
         EndDate = DateTime.Today.Date;
         ElementoListaResumen.Clear();
         SActiveUser = String.Empty;
         SAreaCentro = String.Empty;

      }
      bool LimpiarCommand_CanExecute(object parameters)
      {
         return true;
      }

      #endregion


      private int ObtenerIdUser()
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


      //FILTRAR POR AREAS EL COMBOBOX

      private void filtrarPorArea(string Area)
      {
         ListaFiltradaporArea.Clear();
         var AuxListaFiltradaporArea = UsersIdAndNameList.Where(a => a.Area == Area);
         foreach (var AuxItem in AuxListaFiltradaporArea)
         {
            ListaFiltradaporArea.Add(AuxItem);
         }
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
         ElementoListaResumen.Clear();
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
               int miNcopyReal = Convert.ToInt32(substring, 16);
               //Calculamos el "Cardcode" ( Cardcode = NCopy "real" / 4 )
               var miCardCode = miNcopyReal / 4;
               itemAuditTrail.CardCode = miCardCode;
            }

            //creacion de lista con los datos del usuario seleccionado de los dias pertinentes
            foreach (var itemEvent in LockAuditTrailList)
            {
               if (itemEvent.CardCode.Equals(item.CardCode))
               {
                  var userDataFind = new UserData
                  {
                     Nombre = item.Nombre,
                     Id = item.Id,
                     CardCode = item.CardCode,
                     Entrada = StartDate,
                     Salida = EndDate,
                     FechaEvento = itemEvent.Dt_Audit,
                     TipoEvento = conocertipodeevento(itemEvent)
                  };
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
                     entradaHoraAux = DateTime.Parse(subitemData.FechaEvento);
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

            //Pasar de ListaFinal a ElementoListaResumen
            obteberElementoListaResumenFinal();


         }
         catch (Exception e)
         {
            MessageBox.Show(e.Message);
         }
      }

      private void obteberElementoListaResumenFinal()
      {
         foreach (var itemData in ListaFinal)
         {
            ElementoListaResumen elementoLista = new ElementoListaResumen();
            elementoLista.Nombre = itemData.Nombre;
            elementoLista.Dia = itemData.FechaEvento;
            elementoLista.Ausencia = itemData.Ausencia;
            elementoLista.Aus_Entrada = itemData.AusenciaEntrada.Hour.ToString() + ":" +
                                            itemData.AusenciaEntrada.Minute.ToString() + ":" +
                                            itemData.AusenciaEntrada.Second.ToString();
            elementoLista.Aus_Salida = itemData.AusenciaSalida.Hour.ToString() + ":" +
                                            itemData.AusenciaSalida.Minute.ToString() + ":" +
                                            itemData.AusenciaSalida.Second.ToString();
            elementoLista.Entrada = itemData.Entrada.Hour.ToString() + ":" +
                                    itemData.Entrada.Minute.ToString() + ":" +
                                    itemData.Entrada.Second.ToString();
            elementoLista.Salida = itemData.Salida.Hour.ToString() + ":" +
                                    itemData.Salida.Minute.ToString() + ":" +
                                    itemData.Salida.Second.ToString();
            elementoLista.HorasEnCentro = itemData.TotalHoras.Hour.ToString() + ":" +
                                       itemData.TotalHoras.Minute.ToString() + ":" +
                                       itemData.TotalHoras.Second.ToString();

            ElementoListaResumen.Add(elementoLista);
         }
      }

      private string calculoTotalHorasDeLista()
      {
         float horas = 0, min = 0, enminutos = 0, enhoras = 0;
         string hora = " ";

         foreach (var itemData in ListaFinal)
         {
            horas = itemData.TotalHoras.Hour + horas;
            min = itemData.TotalHoras.Minute + min;
         }
         enminutos = (horas * 60) + min;
         enhoras = enminutos / 60;
         hora = enhoras.ToString(CultureInfo.InvariantCulture);

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






      #region Obtener lista con nombres, id, cardCode y areas


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
               if (userItem.Id != 1)
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
               if (itemToCompare.Id.Equals(item.Id))
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



      //public void UpdateUI()
      //{
      //   //Here update your label, button or any string related object.
      //   //Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
      //   Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
      //}


   }
}
