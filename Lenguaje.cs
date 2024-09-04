using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Sintaxis_1;

namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
       private  List<Variable> listaVariables;
        private  Stack<float> S; 
        public Lenguaje()
        {
         listaVariables  = new List<Variable>();
         S = new Stack<float>();
        }

        public Lenguaje(string nombre) : base(nombre)
        {
            listaVariables  = new List<Variable>();
            S = new Stack<float>();
        }
        public void Programa()
        {
            if (getContenido() == "using")
            {
                Librerias();
            }
            Main();
            imprimeVariable();
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
        Variable.TipoDato getTipo(string TipoDato)
        {
            Variable.TipoDato tipo = Variable.TipoDato.Char;
            switch (TipoDato)
            {
                case "int": tipo = Variable.TipoDato.Int; break;
                case "Float": tipo = Variable.TipoDato.Float;break;
                
            }
            return tipo;
        }

        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            
            Variable.TipoDato tipo = getTipo(getContenido());
            match(Tipos.TipoDato);
            listaIdentificadores(tipo);
            match(";");
            
        }

        private void imprimeVariable()
        {
            foreach(Variable v in listaVariables)
            {
                log.WriteLine(v.getNombre()+"()"+v.getTipo()+")="+v.getValor() );
            }
        }
        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void listaIdentificadores(Variable.TipoDato t)
        {
            listaVariables.Add(new Variable(getContenido(),t));
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                listaIdentificadores(t);
            }
        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void bloqueInstrucciones()
        {
            match("{");
            if (getContenido() != "}")
            {
                listaInstrucciones();
            }
            match("}");
        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void listaInstrucciones()
        {
            Instruccion();
            if (getContenido() != "}")
            {
                listaInstrucciones();
            }
        }
        //Instruccion -> Console | If | While | do | For | Variables | Asignacion
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
                While();
            }
            else if (getContenido() == "do")
            {
                Do();
            }
            else if (getContenido() == "for")
            {
                For();

            }
            else if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
            }
        }
        //Asignacion -> Identificador = Expresion;
        private void Asignacion()
        {
            string Variable = getContenido();
            match(Tipos.Identificador);
            match("=");
            Expresion();
            match(";");
            imprimeStack();
            log.WriteLine(Variable+"="+S.Pop());
        }
        //If -> if (Condicion) bloqueInstrucciones | instruccion (else bloqueInstrucciones | instruccion)?

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
        //Condicion -> Expresion operadorRelacional Expresion
        private void Condicion()
        {
            Expresion();
            match(Tipos.OpRelacional);
            Expresion();
        }
        //While -> while(Condicion) bloqueInstrucciones | instruccion
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
        //Do -> do bloqueInstrucciones | intruccion while(Condicion);
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
        //For -> for(Asignacion Condicion; Incremento) BloqueInstrucciones | Intruccion 
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
        //Incremento -> Identificador ++ | --
        public void Incremento()
        {
            match(Tipos.Identificador);
            if (getContenido() == "++")
            {
                match("++");
            }
            else
            {
                match("--");
            }
        }
        //Console -> Console.(WriteLine|Write) (cadena); | Console.(Read | ReadLine) ();
        private void Console()
        {
            match("Console");
            match(".");
            if (getContenido() == "WriteLine" || getContenido() == "Write")
            {
                match(getContenido());
                match("(");
                if (getClasificacion() == Tipos.Cadena)
                {
                    match(Tipos.Cadena);
                }
                match(")");

            }
            else
            {
                if (getContenido() == "ReadLine")
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
        //Main      -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            bloqueInstrucciones();
        }
        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OpTermino)
            {
                string operador = getContenido();
                match(Tipos.OpTermino);
                Termino();
                float R1 = S.Pop();
                float R2 = S.Pop();
                switch(operador)
                {
                    case "+":S.Push(R1+R2); break;
                    case "-":S.Push(R1-R2); break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            
            if (getClasificacion() == Tipos.OpFactor)
            {
                string operador = getContenido();
                match(Tipos.OpFactor);
                Factor();
                float R1 = S.Pop();
                float R2 = S.Pop();
                switch(operador)
                {
                    case "*":S.Push(R1*R2); break;
                    case "/":S.Push(R1/R2); break;
                    case "%":S.Push(R1%R2); break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void imprimeStack()
        {
            log.WriteLine("Stack:");
            foreach(float e in S.Reverse())
            {
                log.Write(e+" ");
            }
            log.WriteLine();
        }
        private void Factor()
        {
            
            if (getClasificacion() == Tipos.Numero)
            {
                S.Push(float.Parse(getContenido()));
                match(Tipos.Numero);

            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                if(getClasificacion() == Tipos.TipoDato)
                {
                    match(Tipos.TipoDato);
                    match("(");
                    match(")");
                }
                Expresion();
                match(")");
            }
        }
    }


}



/*
Librerias -> using ListaLibrerias; Librerias?


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