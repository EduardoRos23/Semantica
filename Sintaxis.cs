using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sintaxis_1
{
    
    public class Sintaxis : Lexico
    {
        int lineaError=0;
        public Sintaxis()
        {
            nextToken();
        }
        public Sintaxis(string nombre) : base(nombre)
        {
            nextToken();
        }
        public void match(string espera)
        {
            lineaError++;
            if (Contenido == espera)
            {
                
                nextToken();
            }
            else
            {
                Console.Write("Linea "+lineaError+" ");
                throw new Error("Sintaxis: se espera un "+espera,log);
            }
        }
        public void match(Tipos espera)
        {
            if (Clasificacion == espera)
            {
                nextToken();
            }
            else
            {
                throw new Error("Sintaxis: se espera un "+espera,log);
            }
        }
    }
}