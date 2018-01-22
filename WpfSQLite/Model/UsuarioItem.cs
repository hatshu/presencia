using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Presencia.Model
{
   public class UsuarioItem: INotifyPropertyChanged

   {
      public string Nombre { get; set; }
      public DateTime fechaEntrada { get; set; }
      public DateTime fechaSalida { get; set; }
      public string Ausencia { get; set; }
      public DateTime ausenciaEntrada { get; set; }
      public DateTime ausenciaSalida { get; set; }
      public int TotalHoras { get; set; }


      //Para que se detecte cuando se cambia cada elemento
      public event PropertyChangedEventHandler PropertyChanged;

      private void NotifyPropertyChanged(string info)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
      }
   }

}
