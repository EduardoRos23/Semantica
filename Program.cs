using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sintaxis_1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Sintaxis L = new Sintaxis("prueba.cpp"))
                {

                    L.match (Token.Tipos.Numero);
                    L.match(Token.Tipos.OpTermino);
                    L.match (Token.Tipos.Identificador);
                    L.match (";");
                   /*
                    while(!L.finArchivo())
                    {
                        L.nextToken();
                    }
                    */
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
