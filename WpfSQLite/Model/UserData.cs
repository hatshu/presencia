using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presencia.Model
{
   class UserData :INotifyPropertyChanged
   {
      
      public string Nombre { get; set; }
      public int Id { get; set; }
      public DateTime FechaEntrada { get; set; }
      public DateTime FechaSalida { get; set; }
      public DateTime FechaInicio { get; set; }
      public DateTime FechaFin { get; set; }
      public string Ausencia { get; set; }
      public DateTime AusenciaEntrada { get; set; }
      public DateTime AusenciaSalida { get; set; }
      public int TotalHoras { get; set; }


      //Para que se detecte cuando se cambia cada elemento
      public event PropertyChangedEventHandler PropertyChanged;

      private void NotifyPropertyChanged(string info)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
      }
   }
}

