using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presencia.Model
{
   class ElementoListaResumenFinal
   {
      public string Nombre { get; set; }
      public string Dia { get; set; }
      public string Entrada { get; set; }
      public string Salida { get; set; }
      public string Ausencia { get; set; }
      public string Aus_Entrada { get; set; }
      public string Aus_Salida { get; set; }
      public string HorasEnCentro { get; set; }
      public string Comentarios { get; set; }

      //Para que se detecte cuando se cambia cada elemento
      public event PropertyChangedEventHandler PropertyChanged;

      private void NotifyPropertyChanged(string info)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
      }
   }
}
