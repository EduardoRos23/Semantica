using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sintaxis_1;
//El Hazel no sabe ni que esta haciendo.
namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        public Lenguaje()
        {

        }

        public Lenguaje(string nombre) : base(nombre)
        {

        }
        public void Programa()
        {
            if (getContenido() == "using")
            {
                Librerias();
            }
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            Main();
        }

        //Programa  -> Librerias? Variables? Main

        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(";");
            if (getContenido() == "using")
            {
                Librerias();
            }
        }
        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (getContenido() == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }


        private void Variables()
        {
            match(Tipos.TipoDato);
            listaIdentificadores();
            match(";");
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
        }
        private void listaIdentificadores()
        {
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                listaIdentificadores();
            }
        }
        private void bloqueInstrucciones()
        {
            match("{");
            if (getContenido() != "}")
            {
                listaInstrucciones();
            }
            match("}");
        }
        private void listaInstrucciones()
        {
            Instruccion();
            if (getContenido() != "}")
            {
                listaInstrucciones();
            }
        }
        private void Instruccion()
        {
            if (getContenido() == "Console")
            {
                Console();
            }
            else if (getContenido() == "if")
            {
                If();
            }
            else if (getContenido() == "while")
            {
                while();
            }
            else if (getContenido() == "do")
            {
                Do();
            }
            else if (getContenido() == "for")
            {
                For();

            }
            else
            {
                Asignacion();
            }
        }

        private void Asignacion()
        {
            match(Tipos.Identificador);
            match("=");
            Expresion();
            match(";");
        }
        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            bloqueInstrucciones();
            if (getContenido() == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }

            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                {
                    bloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }
        }

        private void Condicion()
        {
            Expresion();
            match(Tipos.OpRelacional);
            Expresion();
        }

        public void While()
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }

        public void Do()
        {
            match("do");
            if (getContenido() == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }

        public void For()
        {
            match("for");
            match("(");
            Asignacion();
            Condicion();
            match(";");
            Incremento();
            match(")");
            if (getContenido() == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }

        public void Incremento()
        {
            match(Tipos.Identificador);
            if(getContenido()=="++")
            {
                match("++");
            }
            else
            {
                match("--");
            }
        }

        private void Console()
        {
            match("Console");
            match(".");
            if(getContenido() == "WriteLine" || getContenido() == "Write")
            {
                match(getContenido());
                match("(");
                if(getClasificacion()==Tipos.Cadena)
                {
                match(Tipos.Cadena);
                }
                match(")");
                
            }
            else
            {
                if(getContenido() == "ReadLine")
                {
                    match("ReadLine");
                }
                else
                {
                    match("Read");
                }
                match("(");
                match(")");
            }
            match(";");
        }

    }


}



/*
Librerias -> using ListaLibrerias; Librerias?
Variables -> tipo_dato Lista_identificadores; Variables?

ListaIdentificadores -> identificador (,ListaIdentificadores)?
BloqueInstrucciones -> { listaIntrucciones? }
ListaInstrucciones -> Instruccion ListaInstrucciones?

Instruccion -> Console | If | While | do | For | Asignacion
Asignacion -> Identificador = Expresion;

If -> if (Condicion) bloqueInstrucciones | instruccion
     (else bloqueInstrucciones | instruccion)?

Condicion -> Expresion operadorRelacional Expresion

While -> while(Condicion) bloqueInstrucciones | instruccion
Do -> do 
        bloqueInstrucciones | intruccion 
      while(Condicion);
For -> for(Asignacion Condicion; Incremento) 
       BloqueInstrucciones | Intruccion 
Incremento -> Identificador ++ | --

Console -> Console.(WriteLine|Write) (cadena); |
           Console.(Read | ReadLine) ();

Main      -> static void Main(string[] args) BloqueInstrucciones 

Expresion -> Termino MasTermino
MasTermino -> (OperadorTermino Termino)?
Termino -> Factor PorFactor
PorFactor -> (OperadorFactor Factor)?
Factor -> numero | identificador | (Expresion)
*/