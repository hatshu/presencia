using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Presencia.Model
{
   public class SearchItem: INotifyPropertyChanged

   {
      public string Nombre { get; set; }
      public int Id { get; set; }
      public int CardCode { get; set; }
      public List<int> AllCardCodes { get; set; }
      public DateTime FechaInicio { get; set; }
      public DateTime FechaFin { get; set; }

      //Para que se detecte cuando se cambia cada elemento
      public event PropertyChangedEventHandler PropertyChanged;

      private void NotifyPropertyChanged(string info)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
      }
   }

}
