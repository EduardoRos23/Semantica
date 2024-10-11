using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Net.Http.Headers;

using Sintaxis_1;
/*
    1. Usar find en lugar del for each                                  -Listo
    2. Valiar que no existan varibles duplicadas                        -Listo
    3. Validar que existan las variables en las expressions matematicas -Listo
       Asignacion
    4. Asinar una expresion matematica a la variable al momento de declararla -Listo
       verificando la semantica
    5. Validar que en el ReadLine se capturen solo numeros (Excepcion)   -Listo
    6. listaConcatenacion: 30, 40, 50, 12, 0                            -Aun no listo 
    7. Quitar comillas y considerar el Write                           -Listo 
    8. Emular el for -- 15 puntos                                      -No listo
    9. Emular el while -- 15 puntos                                    -No listo
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
            if (Contenido == "using")
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
            if (Contenido == "using")
            {
                Librerias();
            }
        }
        // ListaLibrerias -> identificador (.ListaLibrerias)?
        private void listaLibrerias()
        {
            match(Tipos.Identificador);
            if (Contenido == ".")
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
            Variable.TipoDato tipo = getTipo(Contenido);
            match(Tipos.TipoDato);
            listaIdentificadores(tipo);
            match(";");
        }
        private void imprimeVariables()
        {
            log.WriteLine("Lista de variables");

            for (int i = 0; i < listaVariables.Count(); i++)
            {
                var v = listaVariables.Find(v => v.Nombre == listaVariables[i].Nombre);
                log.WriteLine(v.Nombre + " (" + v.Tipo + ") = " + v.Valor);
            }
        }
        // ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void listaIdentificadores(Variable.TipoDato t)
        {
            //Requerimiento 2
            if (listaVariables.Exists(v => v.Nombre == Contenido))
            {
                throw new Error("Semantico: la variable \"" + Contenido + "\" ya ha sido declarada; linea: " + linea, log);
            }
            listaVariables.Add(new Variable(Contenido, t));
            var variable = listaVariables.Find(delegate (Variable x) { return x.Nombre == Contenido; });
            /*if(variable == null){
                throw new Error("La variable \""+ Contenido + "\" no ha sido declarada; linea:"+linea,log);
            }   */
            match(Tipos.Identificador);
            if (Contenido == "=")
            {
                match("=");
                Expresion();
                float x = S.Pop();
                if (analisisSemantico(variable, x))
                {
                    variable.Valor = x;
                    S.Push(x);
                    log.WriteLine(variable.Nombre + " = " + x);
                }
                else
                {
                    throw new Error("Semántico: No se puede asignar un " + tipoDatoExpresion + "a un" + variable.Tipo + "; linea: ", log);
                }

            }
            if (Contenido == ",")
            {
                match(",");
                listaIdentificadores(t);
            }
        }
        // BloqueInstrucciones -> { listaIntrucciones? }
        private void bloqueInstrucciones(bool ejecutar)
        {
            match("{");
            if (Contenido != "}")
            {
                listaIntrucciones(ejecutar);
            }
            match("}");
        }
        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void listaIntrucciones(bool ejecutar)
        {
            Instruccion(ejecutar);
            if (Contenido != "}")
            {
                listaIntrucciones(ejecutar);
            }
        }
        // Instruccion -> Console | If | While | do | For | Variables | Asignacion
        private void Instruccion(bool ejecutar)
        {
            if (Contenido == "Console")
            {
                console(ejecutar);
            }
            else if (Contenido == "if")
            {
                If(ejecutar);
            }
            else if (Contenido == "while")
            {
                While(ejecutar);
            }
            else if (Contenido == "do")
            {
                Do(ejecutar);
            }
            else if (Contenido == "for")
            {
                For(ejecutar);
            }
            else if (Clasificacion == Tipos.TipoDato)
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
            string variable = Contenido;
            if (!listaVariables.Exists(v => v.Nombre == variable))
            {
                throw new Error("Semantico: la variable \"" + variable + "\" no ha sido declarada; línea: " + linea, log);
            }
            match(Tipos.Identificador);
            var v = listaVariables.Find(delegate (Variable x) { return x.Nombre == variable; });
            float nuevoValor = v.Valor;

            tipoDatoExpresion = Variable.TipoDato.Char;

            if (Contenido == "=")
            {
                match("=");
                if (Contenido == "Console")
                {
                    match("Console");
                    match(".");
                    if (Contenido == "Read")
                    {
                        match("Read");
                        match("(");
                        if (ejecutar)
                        {
                            float valor = Console.Read();
                        }
                        // 8
                    }
                    else
                    {
                        match("ReadLine");
                        match("(");
                        string entrada = ("" + Console.ReadLine());
                        if (!float.TryParse(entrada, out nuevoValor))
                        {
                            throw new Exception("El valor introducido debe ser un número");
                        }
                        // 8
                    }

                    match(")");
                    //match(";");
                }
                else
                {
                    Expresion();
                    nuevoValor = S.Pop();
                }
            }
            else if (Contenido == "++")
            {
                match("++");
                nuevoValor++;
            }
            else if (Contenido == "--")
            {
                match("--");
                nuevoValor--;
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                nuevoValor += S.Pop();
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                nuevoValor -= S.Pop();
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                nuevoValor *= S.Pop();
            }
            else if (Contenido == "/=")
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
                    v.Valor = nuevoValor;
            }
            else
            {
                // tipoDatoExpresion = 
                throw new Error("Semantico, no puedo asignar un " + tipoDatoExpresion +
                                " a un " + v.Tipo + "; linea: " + linea, log);
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
            if (tipoDatoExpresion > v.Tipo)
            {
                return false;
            }
            else if (valor % 1 == 0)
            {
                if (v.Tipo == Variable.TipoDato.Char)
                {
                    if (valor <= 255)
                        return true;
                }
                else if (v.Tipo == Variable.TipoDato.Int)
                {
                    if (valor <= 65535)
                        return true;
                }
                return false;
            }
            else
            {
                if (v.Tipo == Variable.TipoDato.Char ||
                    v.Tipo == Variable.TipoDato.Int)
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
            if (Contenido == "{")
            {
                bloqueInstrucciones(resultado && ejecutar);
            }
            else
            {
                Instruccion(resultado && ejecutar);
            }
            if (Contenido == "else")
            {
                match("else");
                if (Contenido == "{")
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
            string operador = Contenido;
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
            int cTemp = caracter - 3;
            int lTemp = linea;
            bool resultado = false;

            match("while");
            match("(");
            resultado = Condicion() && ejecutar;
            match(")");
            while (resultado)
            {
                if (Contenido == "{")
                {
                    bloqueInstrucciones(ejecutar);
                }
                else
                {
                    Instruccion(ejecutar);
                }

                resultado = Condicion() && ejecutar;
                if (resultado)
                {
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cTemp, SeekOrigin.Begin);
                    nextToken();
                }
            }//

        }
        // Do -> do 
        //          bloqueInstrucciones | intruccion 
        //       while(Condicion);
        private void Do(bool ejecutar)
        {
            int cTemp = caracter - 3;
            int lTemp = linea;
            bool resultado = false;
            do
            {
                match("do");
                if (Contenido == "{")
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
            int cTemp = caracter - 3;
            int lTemp = linea;
            bool resultado = false;

            match("for");
            match("(");
            var varF1 = listaVariables.Find(v => v.Nombre == Contenido);
            if (varF1 == null)
            {
                throw new Exception("La variable dentro del for no ha sido declarada");
            }
            Asignacion(ejecutar);
            match(";");
            resultado = Condicion() && ejecutar;
            match(";");
            var varF2 = listaVariables.Find(v => v.Nombre == Contenido);
            if (varF2 == null)
            {
                throw new Exception("La variable dentro del for no ha sido declarada");
            }
            /*if(varF2.Nombre != varF1.Nombre){
                throw new Exception("No puede haber más de una variable dentro del For");
            }*/
            Asignacion(ejecutar);
            match(")");
            match("{");
            while (resultado)
            {
                if (Contenido == "{")
                {
                    bloqueInstrucciones(ejecutar);
                }
                else
                {
                    Instruccion(ejecutar);
                }
                resultado = Condicion() && ejecutar;
                if (resultado)
                {
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cTemp, SeekOrigin.Begin);
                    nextToken();
                }

            }

        }

        private void console(bool ejecutar)
        {
            match("Console");
            match(".");
            bool b = false;
            string texto;
            if (Contenido == "WriteLine")
            {
                b = true;
                match("WriteLine");
            }
            else if (Contenido == "Write")
            {
                match("Write");
            }
            match("(");
            if (Clasificacion == Tipos.Cadena)
            {
                if (ejecutar)
                {
                    texto = Contenido;
                    texto = texto.Replace("\"", "");
                    match(Tipos.Cadena);
                    if (b)
                    {
                        if (Contenido == "+")
                        {
                            string textoT = listaConcatenacion();
                            if (ejecutar)
                            {
                                texto = texto + textoT;
                                Console.WriteLine(texto.Replace('"', ' ').TrimEnd());
                            }
                        }
                        else
                        {
                            if (ejecutar)
                            {
                                Console.WriteLine(texto.Replace('"', ' ').TrimEnd());
                            }
                        }
                    }
                    else
                    {
                        if (Contenido == "+")
                        {
                            string textoT = listaConcatenacion();
                            if (ejecutar)
                            {
                                texto = texto + textoT;
                                Console.Write(texto.Replace('"', ' ').TrimEnd());
                            }
                        }
                        else
                        {
                            if (ejecutar)
                            {
                                Console.Write(texto.Replace('"', ' ').TrimEnd());
                            }
                        }
                    }


                }

            }
            else{
                float resultado;
                var v = listaVariables.Find(variable => variable.Nombre == Contenido);
                resultado = v.Valor;
                match(Tipos.Identificador);
                if (b)
                {
                    if (Contenido == "+")
                    {
                        String temp = listaConcatenacion();
                        if (ejecutar)
                            Console.WriteLine(resultado + temp);
                    }
                    else
                    {
                        if (ejecutar)
                            Console.WriteLine(resultado);
                    }
                }
                else
                {
                    if (Contenido == "+")
                    {
                        String temp = listaConcatenacion();
                        if (ejecutar)
                            Console.Write(resultado + temp);
                    }
                    else
                    {
                        if (ejecutar)
                            Console.Write(resultado);
                    }
                }

            }
            match(")");
            match(";");
        }
        string listaConcatenacion()
        {
            String texto = "";
            string resultado = "";
            match("+");
            if (Clasificacion == Tipos.Identificador)
            {
                if (!listaVariables.Exists(v => v.Nombre == Contenido))
                {
                throw new Error("Semantico: la variable \""+Contenido+ "\" no ha sido declarada; linea: "+linea,log);
                }
                var v = listaVariables.Find(variable => variable.Nombre == Contenido);
                resultado = v.Valor.ToString();
                match(Tipos.Identificador);
            }
            if (Clasificacion == Tipos.Cadena)
            {
                texto = Contenido;
                texto = texto.Replace("\"","");
                resultado += texto;
                match(Tipos.Cadena);
            }
            if (Contenido == "+")
            {
                resultado += listaConcatenacion();
            }
            return resultado;
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
            if (Clasificacion == Tipos.OpTermino)
            {
                string operador = Contenido;
                match(Tipos.OpTermino);
                Termino();
                float R1 = S.Pop();
                float R2 = S.Pop();
                switch (operador)
                {
                    case "+":
                        S.Push(R2 + R1);
                        tipoDatoExpresion = valorToTipo(R2 + R1);
                        break;
                    case "-":
                        S.Push(R2 - R1);
                        tipoDatoExpresion = valorToTipo(R2 - R1);
                        break;
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
            if (Clasificacion == Tipos.OpFactor)
            {
                string operador = Contenido;
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
        /* private void imprimeStack()
         {
             log.WriteLine("Stack:");

             foreach (float e in S.Reverse())
             {
                 log.Write(e + " ");
             }
             log.WriteLine();
         }*/
        private void Factor()
        {
            if (Clasificacion == Tipos.Numero)
            {
                S.Push(float.Parse(Contenido));
                if (tipoDatoExpresion < valorToTipo(float.Parse(Contenido)))
                {
                    tipoDatoExpresion = valorToTipo(float.Parse(Contenido));
                }
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                var v = listaVariables.Find(delegate (Variable x) { return x.Nombre == Contenido; });
                S.Push(v.Valor);
                if (tipoDatoExpresion < v.Tipo)
                {
                    tipoDatoExpresion = v.Tipo;
                }
                match(Tipos.Identificador);
            }
            else
            {
                bool huboCast = false;
                Variable.TipoDato aCastear = Variable.TipoDato.Char;
                match("(");
                if (Clasificacion == Tipos.TipoDato)
                {
                    huboCast = true;
                    aCastear = getTipo(Contenido);
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