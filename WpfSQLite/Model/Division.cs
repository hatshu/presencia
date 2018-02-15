using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presencia.Model
{
   class Division : INotifyPropertyChanged
   {


      public string Header { get; set; }
      public ObservableCollection<ElementoListaResumenFinal> Content { get; set; }
      public string HorasTotales { get; set; }

      //Para que se detecte cuando se cambia cada elemento
      public event PropertyChangedEventHandler PropertyChanged;

      private void NotifyPropertyChanged(string info)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
      }
   }
}
