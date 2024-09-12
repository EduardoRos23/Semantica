using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sintaxis_1
{
    public class Token
    {
        public enum Tipos
        {
            Identificador, Numero, FinSentencia, OpTermino, OpFactor,
            OpLogico, OpRelacional, OpTernario, Asignacion, IncTermino,
            IncFactor, Cadena, Inicio, Fin, Caracter, TipoDato, Ciclo, 
            Condicion
        };
        private string contenido;
        private Tipos clasificacion;
        public Token()
        {
            contenido = "";
        }
        
        public string Contenido{
         get => contenido;
         set => contenido = value;
        
        }

        public Tipos Clasificacion{
            get=> clasificacion;
            set => clasificacion=value;
        }
      /*  public void setContenido(string contenido)
        {
            this.contenido = contenido;
        }
        public void setClasificacion(Tipos clasificacion)
        {
            this.clasificacion = clasificacion;
        }
        public string getContenido()
        {
            return this.contenido;
        }
        public Tipos getClasificacion()
        {
            return this.clasificacion;
        }*/
        
    }
}