using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presencia.Model
{
   class LockAuditTrail : INotifyPropertyChanged
   {

      public string Id_Event { get; set; }
      public string Dt_Audit { get; set; }
      public int InsertOrder { get; set; }
      public int Id_User { get; set; }
      public int NCopy { get; set; }
      public int Id_Function { get; set; }
      public int InsertionCounter { get; set; }
      public  int CardCode { get; set; }




      //Para que se detecte cuando se cambia cada elemento
      public event PropertyChangedEventHandler PropertyChanged;

      private void NotifyPropertyChanged(string info)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
      }
   }

}
