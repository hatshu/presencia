using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presencia.Model
{
   class ElementoListaResumen: INotifyPropertyChanged
   {

      public string Nombre { get; set; }
      public string FechaEvento { get; set; }
      public string Entrada { get; set; }
      public string Salida { get; set; }
      public string Ausencia { get; set; }
      public string AusenciaEntrada { get; set; }
      public string AusenciaSalida { get; set; }
      public string TotalHoras { get; set; }


      //Para que se detecte cuando se cambia cada elemento
      public event PropertyChangedEventHandler PropertyChanged;

      private void NotifyPropertyChanged(string info)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
      }
   }
}

