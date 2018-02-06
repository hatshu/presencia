using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presencia.Model
{
   class Ausencia: INotifyPropertyChanged
   {
      public string usuario { get; set; }
      public string Tipo { get; set; }
      public string Dia { get; set; }
      public string FechaInicio { get; set; }
      public string FechaFin { get; set; }
      public bool cancelado { get; set; }
      public string Comentarios { get; set; }
      //Para que se detecte cuando se cambia cada elemento
      public event PropertyChangedEventHandler PropertyChanged;

      private void NotifyPropertyChanged(string info)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
      }
   }

}
