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

        public Lenguaje(string nombre="prueba.cpp")
        {
            listaVariables  = new List<Variable>();
            S = new Stack<float>();
        }
        public void Programa()
        {
            if (Contenido == "using")
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
            if (Contenido == "using")
            {
                Librerias();
            }
        }
        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (Contenido == ".")
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
            
            Variable.TipoDato tipo = getTipo(Contenido);
            match(Tipos.TipoDato);
            ListaIdentificadores(tipo);
            match(";");
            
        }

        private void imprimeVariable()
        {
            foreach(Variable v in listaVariables)
            {
                log.WriteLine(v.Nombre+"()"+v.Tipo+")="+v.Valor );
                //Console.
            }
        }
        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoDato t)
        {
            listaVariables.Add(new Variable(Contenido,t));
            match(Tipos.Identificador);
            if (Contenido == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void bloqueInstrucciones()
        {
            match("{");
            if (Contenido != "}")
            {
                listaInstrucciones();
            }
            match("}");
        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void listaInstrucciones()
        {
            Instruccion();
            if (Contenido != "}")
            {
                listaInstrucciones();
            }
        }
        //Instruccion -> Console | If | While | do | For | Variables | Asignacion
        private void Instruccion()
        {
            if (Contenido == "Console")
            {  
                Console1();
            }
            else if (Contenido == "if")
            {
                If();
            }
            else if (Contenido == "while")
            {
                While();
            }
            else if (Contenido == "do")
            {
                Do();
            }
            else if (Contenido == "for")
            {
                For();

            }
            else if (Clasificacion == Tipos.TipoDato)
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
            string Variable = Contenido;
            
            match(Tipos.Identificador);
            switch(Contenido){
                
                case "++":match("++"); break;
                case "--":match("--"); break;
                case "=": match("=");Expresion();break;
                case "+=": match("+=");Expresion(); break;
                case "-=": match("-=");Expresion(); break;
                case "*=": match("*=");Expresion(); break;
                case "/=": match("/=");Expresion(); break;
                case "%=": match("%=");Expresion(); break;
            }
            match(";");
            SizeVar(Variable);
            //log.WriteLine(Variable+"="+S.Pop());
        }
        private void SizeVar(string var){
          if(TipoV(var)=="Char" && Valor(var)>255){
           throw new Error(" Semantico: la variable "+var + " está fuera del rango del tipo Char", log);
          }
          if(TipoV(var)=="Int" && Valor(var)>65535){
           throw new Error(" Semantico: la variable "+var + " está fuera del rango del tipo Int", log);
          }
        }
          public float Valor(string var){
            foreach(Variable n in listaVariables){
             if(n.Nombre == var){
                return n.Valor;
             }
             
            }
            return 0;
          }
          public string TipoV(string var){
            foreach(Variable n in listaVariables){
             if(n.Nombre == var){
                return n.Tipo.ToString();
             }
             
            }
            return "";
          }
        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            bloqueInstrucciones();
            if (Contenido == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }

            if (Contenido == "else")
            {
                match("else");
                if (Contenido == "{")
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
            if (Contenido == "{")
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
            if (Contenido == "{")
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
            if (Contenido == "{")
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
            if (Contenido == "++")
            {
                match("++");
            }
            else
            {
                match("--");
            }
        }
        //Console -> Console.(WriteLine|Write) (cadena); | Console.(Read | ReadLine) ();
        private void Console1()
        {
            match("Console");
            match(".");
            if (Contenido == "WriteLine" || Contenido == "Write")
            {
                match(Contenido);
                match("(");
                if (Clasificacion == Tipos.Cadena)
                {
                    match(Tipos.Cadena);
                }
                match(")");

            }
            else
            {
                if (Contenido == "ReadLine")
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
            imprimeStack();
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
            if (Clasificacion == Tipos.OpTermino)
            {
                string operador = Contenido;
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
            
            if (Clasificacion == Tipos.OpFactor)
            {
                string operador = Contenido;
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
            
            if (Clasificacion == Tipos.Numero)
            {
                S.Push(float.Parse(Contenido));
                match(Tipos.Numero);

            }
            else if (Clasificacion == Tipos.Identificador)
            {
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                if(Clasificacion == Tipos.TipoDato)
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

