﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presencia.Model
{
   class IdUser : INotifyPropertyChanged
   {
      public string Nombre { get; set; }
      public int Id { get; set; }
      public int CardCode { get; set; }
      public string Area { get; set; }
      public int IdIdinet { get; set; }
      public List<int>AllCardCodes { get; set; }

      //Para que se detecte cuando se cambia cada elemento
      public event PropertyChangedEventHandler PropertyChanged;

      private void NotifyPropertyChanged(string info)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
      }
   }
}
