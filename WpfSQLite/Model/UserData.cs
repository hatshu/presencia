﻿using System;
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
      public int CardCode { get; set; }
      public string FechaEvento { get; set; }
      public string TipoEvento { get; set; }
      public DateTime Entrada { get; set; }
      public DateTime Salida { get; set; }
      public string Ausencia { get; set; }
      public DateTime AusenciaEntrada { get; set; }
      public DateTime AusenciaSalida { get; set; }
      public DateTime TotalHoras { get; set; }
      public string Comentarios { get; set; }


      //Para que se detecte cuando se cambia cada elemento
      public event PropertyChangedEventHandler PropertyChanged;

      private void NotifyPropertyChanged(string info)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
      }
   }
}

