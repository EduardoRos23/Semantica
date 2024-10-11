using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Sintaxis_1
{
    //Requerimiento: Número de linea donde se encuentra el error
    public class Error : Exception
    {
        public Error(string mensaje, StreamWriter log) : base("Error: " + mensaje)
        {
            log.WriteLine("Error: " + mensaje);
        }

         public Error(string mensaje, StreamWriter log, int linea) : base("Error: " + mensaje)
        {
            log.WriteLine("Error: " + mensaje+ "en la linea: "+linea);
        }

    
    }
}