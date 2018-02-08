using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presencia.Model
{
   class Ausencia
   {
      public int proceso { get; set; }
      public string usuario { get; set; }
      public string Tipo { get; set; }
      public string Dia { get; set; }
      public string FechaInicio { get; set; }
      public string FechaFin { get; set; }
      public bool cancelado { get; set; }
      public string Comentarios { get; set; }
      public bool localizadoEnSalto { get; set; }

      
   }

}
