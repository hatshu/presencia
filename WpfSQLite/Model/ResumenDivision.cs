using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presencia.Model
{
   class ResumenDivision : INotifyPropertyChanged
   {
      public string Nombre { get; set; }
      public string RangoFechas { get; set; }
      public string HorasEnCentro { get; set; }
      public string PRUEBA { get; set; }

      //Para que se detecte cuando se cambia cada elemento
      public event PropertyChangedEventHandler PropertyChanged;

      private void NotifyPropertyChanged(string info)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
      }
   }
}