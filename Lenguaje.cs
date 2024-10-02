using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Net.Http.Headers;

using Sintaxis_1;
/*
    1. Usar find en lugar del for each
    2. Valiar que no existan varibles duplicadas
    3. Validar que existan las variables en las expressions matematicas
       Asignacion
    4. Asinar una expresion matematica a la variable al momento de declararla
       verificando la semantica
    5. Validar que en el ReadLine se capturen solo numeros (Excepcion)
    6. listaConcatenacion: 30, 40, 50, 12, 0
    7. Quitar comillas y considerar el Write
    8. Emular el for -- 15 puntos
    9. Emular el while -- 15 puntos
*/

namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        private List<Variable> listaVariables;
        private Stack<float> S;
        private Variable.TipoDato tipoDatoExpresion;
        public Lenguaje()
        {
            log.WriteLine("Analizador Sintactico");
            listaVariables = new List<Variable>();
            S = new Stack<float>();
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            log.WriteLine("Analizador Sintactico");
            listaVariables = new List<Variable>();
            S = new Stack<float>();
        }
        // Programa  -> Librerias? Main
        public void Programa()
        {
            if (getContenido() == "using")
            {
                Librerias();
            }
            Main();
            imprimeVariables();
        }
        // Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            listaLibrerias();
            match(";");
            if (getContenido() == "using")
            {
                Librerias();
            }
        }
        // ListaLibrerias -> identificador (.ListaLibrerias)?
        private void listaLibrerias()
        {
            match(Tipos.Identificador);
            if (getContenido() == ".")
            {
                match(".");
                listaLibrerias();
            }
        }
        Variable.TipoDato getTipo(string TipoDato)
        {
            Variable.TipoDato tipo = Variable.TipoDato.Char;
            switch (TipoDato)
            {
                case "int": tipo = Variable.TipoDato.Int; break;
                case "float": tipo = Variable.TipoDato.Float; break;
            }
            return tipo;
        }
        // Variables -> tipo_dato Lista_identificadores;
        private void Variables()
        {
            Variable.TipoDato tipo = getTipo(getContenido());
            match(Tipos.TipoDato);
            listaIdentificadores(tipo);
            match(";");
        }
        private void imprimeVariables()
        {
            log.WriteLine("Lista de variables");
            foreach (Variable v in listaVariables)
            {
                log.WriteLine(v.getNombre() + " (" + v.getTipo() + ") = " + v.getValor());
            }
        }
        // ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void listaIdentificadores(Variable.TipoDato t)
        {
            listaVariables.Add(new Variable(getContenido(), t));
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                listaIdentificadores(t);
            }
        }
        // BloqueInstrucciones -> { listaIntrucciones? }
        private void bloqueInstrucciones(bool ejecutar)
        {
            match("{");
            if (getContenido() != "}")
            {
                listaIntrucciones(ejecutar);
            }
            match("}");
        }
        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void listaIntrucciones(bool ejecutar)
        {
            Instruccion(ejecutar);
            if (getContenido() != "}")
            {
                listaIntrucciones(ejecutar);
            }
        }
        // Instruccion -> Console | If | While | do | For | Variables | Asignacion
        private void Instruccion(bool ejecutar)
        {
            if (getContenido() == "Console")
            {
                console(ejecutar);
            }
            else if (getContenido() == "if")
            {
                If(ejecutar);
            }
            else if (getContenido() == "while")
            {
                While(ejecutar);
            }
            else if (getContenido() == "do")
            {
                Do(ejecutar);
            }
            else if (getContenido() == "for")
            {
                For(ejecutar);
            }
            else if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion(ejecutar);
                match(";");
            }
        }
        // Asignacion -> Identificador = Expresion;
        private void Asignacion(bool ejecutar)
        {
            string variable = getContenido();
            match(Tipos.Identificador);

            var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == variable; });
            float nuevoValor = v.getValor();

            tipoDatoExpresion = Variable.TipoDato.Char;

            if (getContenido() == "=")
            {
                match("=");
                if (getContenido() == "Console")
                {
                    match("Console");
                    match(".");
                    if (getContenido() == "Read")
                    {
                        match("Read");
                        if (ejecutar)
                        {
                            float valor = Console.Read();
                        }
                        // 8
                    }
                    else
                    {
                        match("ReadLine");
                        nuevoValor = float.Parse("" + Console.ReadLine());
                        // 8
                    }
                    match("(");
                    match(")");
                }
                else
                {
                    Expresion();
                    nuevoValor = S.Pop();
                }
            }
            else if (getContenido() == "++")
            {
                match("++");
                nuevoValor++;
            }
            else if (getContenido() == "--")
            {
                match("--");
                nuevoValor--;
            }
            else if (getContenido() == "+=")
            {
                match("+=");
                Expresion();
                nuevoValor += S.Pop();
            }
            else if (getContenido() == "-=")
            {
                match("-=");
                Expresion();
                nuevoValor -= S.Pop();
            }
            else if (getContenido() == "*=")
            {
                match("*=");
                Expresion();
                nuevoValor *= S.Pop();
            }
            else if (getContenido() == "/=")
            {
                match("/=");
                Expresion();
                nuevoValor /= S.Pop();
            }
            else
            {
                match("%=");
                Expresion();
                nuevoValor %= S.Pop();
            }
            // match(";");
            if (analisisSemantico(v, nuevoValor))
            {
                if (ejecutar)
                    v.setValor(nuevoValor);
            }
            else
            {
                // tipoDatoExpresion = 
                throw new Error("Semantico, no puedo asignar un " + tipoDatoExpresion +
                                " a un " + v.getTipo(), log, linea);
            }
            log.WriteLine(variable + " = " + nuevoValor);
        }
        private Variable.TipoDato valorToTipo(float valor)
        {
            if (valor % 1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            else if (valor <= 255)
            {
                return Variable.TipoDato.Char;
            }
            else if (valor <= 65535)
            {
                return Variable.TipoDato.Int;
            }
            return Variable.TipoDato.Float;
        }
        bool analisisSemantico(Variable v, float valor)
        {
            if (tipoDatoExpresion > v.getTipo())
            {
                return false;
            }
            else if (valor % 1 == 0)
            {
                if (v.getTipo() == Variable.TipoDato.Char)
                {
                    if (valor <= 255)
                        return true;
                }
                else if (v.getTipo() == Variable.TipoDato.Int)
                {
                    if (valor <= 65535)
                        return true;
                }
                return false;
            }
            else
            {
                if (v.getTipo() == Variable.TipoDato.Char ||
                    v.getTipo() == Variable.TipoDato.Int)
                    return false;
            }
            return true;
        }
        // If -> if (Condicion) bloqueInstrucciones | instruccion
        // (else bloqueInstrucciones | instruccion)?
        private void If(bool ejecutar)
        {
            match("if");
            match("(");
            bool resultado = Condicion();
            match(")");
            if (getContenido() == "{")
            {
                bloqueInstrucciones(resultado && ejecutar);
            }
            else
            {
                Instruccion(resultado && ejecutar);
            }
            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                {
                    bloqueInstrucciones(!resultado && ejecutar);
                }
                else
                {
                    Instruccion(!resultado && ejecutar);
                }
            }
        }
        // Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion()
        {
            Expresion(); // E1
            string operador = getContenido();
            match(Tipos.OpRelacional);
            Expresion(); // E2
            float R2 = S.Pop();
            float R1 = S.Pop();
            switch (operador)
            {
                case ">": return R1 > R2;
                case ">=": return R1 >= R2;
                case "<": return R1 < R2;
                case "<=": return R1 <= R2;
                case "==": return R1 == R2;
                default: return R1 != R2;
            }
        }
        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool ejecutar)
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                bloqueInstrucciones(ejecutar);
            }
            else
            {
                Instruccion(ejecutar);
            }
        }
        // Do -> do 
        //          bloqueInstrucciones | intruccion 
        //       while(Condicion);
        private void Do(bool ejecutar)
        {
            int cTemp = caracter-3;
            int lTemp = linea;
            bool resultado = false;
            do
            {
                match("do");
                if (getContenido() == "{")
                {
                    bloqueInstrucciones(ejecutar);
                }
                else
                {
                    Instruccion(ejecutar);
                }
                match("while");
                match("(");
                resultado = Condicion() && ejecutar;
                match(")");
                match(";");
                if (resultado)
                {
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cTemp, SeekOrigin.Begin);
                    nextToken();
                }
            } while (resultado);
        }
        // For -> for(Asignacion Condicion; Incremento) 
        //          BloqueInstrucciones | Intruccion
        private void For(bool ejecutar)
        {
            match("for");
            match("(");
            Asignacion(ejecutar);
            match(";");
            Condicion();
            match(";");
            Asignacion(ejecutar);
            match(")");
            if (getContenido() == "{")
            {
                bloqueInstrucciones(ejecutar);
            }
            else
            {
                Instruccion(ejecutar);
            }
        }
        // Console -> Console.(WriteLine|Write) (cadena?);
        private void console(bool ejecutar)
        {
            match("Console");
            match(".");
            if (getContenido() == "WriteLine")
            {
                match("WriteLine");
            }
            else
            {
                match("Write");
            }
            match("(");
            if (getClasificacion() == Tipos.Cadena)
            {
                if (ejecutar)
                {
                    Console.WriteLine(getContenido());
                }
                // Considerar el Write
                // Quitar las comillas
                match(Tipos.Cadena);
                if (getContenido() == "+")
                {
                    listaConcatenacion();
                }
            }
            match(")");
            match(";");
        }
        string listaConcatenacion()
        {
            match("+");
            match(Tipos.Identificador); // Validar que exista la variable
            if (getContenido() == "+")
            {
                listaConcatenacion();
            }
            return "";
        }
        // Main      -> static void Main(string[] args) BloqueInstrucciones 
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
            bloqueInstrucciones(true);
        }
        // Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        // MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OpTermino)
            {
                string operador = getContenido();
                match(Tipos.OpTermino);
                Termino();
                float R1 = S.Pop();
                float R2 = S.Pop();
                switch (operador)
                {
                    case "+": S.Push(R2 + R1); break;
                    case "-": S.Push(R2 - R1); break;
                }
            }
        }
        // Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        // PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OpFactor)
            {
                string operador = getContenido();
                match(Tipos.OpFactor);
                Factor();
                float R1 = S.Pop();
                float R2 = S.Pop();
                switch (operador)
                {
                    case "*": S.Push(R2 * R1); break;
                    case "/": S.Push(R2 / R1); break;
                    case "%": S.Push(R2 % R1); break;
                }
            }
        }
        // Factor -> numero | identificador | (Expresion)
        private void imprimeStack()
        {
            log.WriteLine("Stack:");
            foreach (float e in S.Reverse())
            {
                log.Write(e + " ");
            }
            log.WriteLine();
        }
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                S.Push(float.Parse(getContenido()));
                if (tipoDatoExpresion < valorToTipo(float.Parse(getContenido())))
                {
                    tipoDatoExpresion = valorToTipo(float.Parse(getContenido()));
                }
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == getContenido(); });
                S.Push(v.getValor());
                if (tipoDatoExpresion < v.getTipo())
                {
                    tipoDatoExpresion = v.getTipo();
                }
                match(Tipos.Identificador);
            }
            else
            {
                bool huboCast = false;
                Variable.TipoDato aCastear = Variable.TipoDato.Char;
                match("(");
                if (getClasificacion() == Tipos.TipoDato)
                {
                    huboCast = true;
                    aCastear = getTipo(getContenido());
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
                if (huboCast && aCastear != Variable.TipoDato.Float)
                {
                    tipoDatoExpresion = aCastear;
                    float valor = S.Pop();
                    if (aCastear == Variable.TipoDato.Char)
                    {
                        valor %= 256;
                    }
                    else
                    {
                        valor %= 65536;
                    }
                    S.Push(valor);
                }
            }
        }
    }
}