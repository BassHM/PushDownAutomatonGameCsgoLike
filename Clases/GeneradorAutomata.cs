using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;

namespace DesactivadorDeBombas.Clases
{
    public class Transicion
    {
        public int EstadoInicial { get; set; }
        public int EstadoDestino { get; set; }
        public char Simbolo { get; set; }
        public char PopSimboloPila { get; set; }
        public List<char> PushSimboloPila { get; set; }

        public Transicion(int estadoInicial, int estadoFinal, char simbolo, char popSimboloPila, List<char> pushSimboloPila)  // Updated constructor parameter type
        {
            EstadoInicial = estadoInicial;
            EstadoDestino = estadoFinal;
            Simbolo = simbolo;
            PopSimboloPila = popSimboloPila;
            PushSimboloPila = pushSimboloPila;
        }
    }
    public class AutomataPila
    {
        //REGLAS PARA AÑADIR TRANSICIONES.
        //En el caso de que la contraseña sea de máximo 8 caracteres:
        //- todas las transiciones consumen al menos un elemento de la lista
        //- la semilla debe de estar compuesta por número s
        //1 - Si se pretende añadir una transición que añada dos elementos de la pila, verificar que el máximo de caracteres de contraseña no se vea superado.
        //(en este caso el número de transiciones se sumara un + 3 )
        //2 - En caso de que la transición propuesta solo añada un elemento a la pila, entonces el número de transiciones se sumaría +2 (verificar que esto no sobrepase el número máximo de transiciones)
        //3 - Por último, existen las tansiciones que no añaden nada a la pila y existen en caso de queden restos en la pila + 1
        public static int MaximoDeCaracteresContrasenia = 8;
        private List<Transicion> Transiciones = new List<Transicion> { };
        private int EstadoActual;
        private int EstadoFinal;
        private List<int> Estados = new List<int> { };
        //Posibles simbolos de la pila
        List<char> CaracteresPila = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        //Pila
        Stack Stack = new Stack();
        //Simbolo inicial de la pila
        char Zeta = 'Z';
        private List<int> Semilla = new List<int> { };
        public AutomataPila(List<int> semilla)
        {
            this.Semilla = semilla;
            //Vamos a generar el maximo de transiciones posibles, donde cada transicion es un caracter de la contraseña
            //El número de transiciones es igual al número de caracteres de la contraseña
            //Todas las transiciones consumen un único elemento de la pila
            //Las transiciones pueden introducir 0, 1 o 2 elementos en la pila
            //Una vez realizadas todas las transiciones, la pila debe de estar vacía para que la contraseña sea correcta

            //Para generar las transiciones, vamos a generar un número aleatorio entre 0 y 2, que será el número de elementos que se introducirán en la pila
            //Caso 0: Si el número de elementos a introducir es 0, entonces la transición no añade nada a la pila
            //Caso 1: Si el número de elementos a introducir es 1, entonces la transición añade un elemento a la pila
            //Caso 2: Si el número de elementos a introducir es 2, entonces la transición añade dos elementos a la pila

            //Antes de generar una transición, se debe de comprobar que el número de elementos a introducir no sobrepase el número de caracteres de la contraseña:
            //Caso 0: Las tranciciones que introducen ningun elemento consumen un digito de la contraseña 
            //Caso 1: En caso de que la transición introduzca un elemento, entonces consumirá dos digitos de la contraseña (el primero para la transición y el segundo para el elemento que se introduce)
            //Caso 2: En el caso de que se introduzcan dos elementos a la pila, entonces se consumirán tres digitos de la contraseña (el primero para la transición, el segundo para el primer elemento que se introduce y el tercero para el segundo elemento que se introduce)

            //La semilla está conformada por transiciones iniciales del tipo 0. A mas grande la semilla, las transiciones serán más simples porque la pila estará mas llena que con una semilla pequeña (recordemos que con una pila mas llena se necesitan más transiciones para consumirla y vaciarla)

            //Simbolo inicial de la pila
            Stack.Push(Zeta);

            //El enemigo que planta la bomba, genera una contraseña con una semilla de los digitos que desee, pero no puede superar el máximo de caracteres de la contraseña
            if(semilla.Count > MaximoDeCaracteresContrasenia)
            {
                throw new Exception("La semilla no puede superar el máximo de caracteres de la contraseña");
            }

            //Estado inicial: 0
            Estados.Add(0);
            EstadoActual = 0;
            char ultimoCaracter = Zeta;
            for (int indexDigito = 0; indexDigito < semilla.Count; indexDigito++)
            {
                Transiciones.Add(new Transicion(EstadoActual, EstadoActual + 1, semilla[indexDigito].ToString()[0], ultimoCaracter, new List<char> { CaracteresPila[semilla[indexDigito]], ultimoCaracter }));
                ultimoCaracter = CaracteresPila[semilla[indexDigito]];
                Stack.Push(ultimoCaracter);
                Estados.Add(indexDigito + 1);
                EstadoActual = indexDigito + 1;
            }
            int numeroDeTransiciones = 0;
            //Generamos las transiciones restantes que consumen un digito de la contraseña y que solo se mueven hacia si mismos
            //Generamos las transiciones restantes que consumen un digito de la contraseña y que solo se mueven desde el estado actual al mismo estado actual
            //Al generar las transiciones restantes, se debe de comprobar que el número de transiciones no sobrepase el número máximo de transiciones
            //Debemos asegurarnos que al final de todas las transiciones, la pila esté vacía (IMPORTANTE)
            int numeroDeElementosAIntroducir;
            while (numeroDeTransiciones <= MaximoDeCaracteresContrasenia && Stack.Count > 1)
            {
                //Generamos un número aleatorio entre 0 y 2
                Random random = new Random();
                numeroDeElementosAIntroducir = random.Next(0, 3);
                Console.WriteLine(numeroDeElementosAIntroducir + "   " + numeroDeElementosAIntroducir + "   " + Stack.Count); 
                //Caso 0: Si el número de elementos a introducir es 0, entonces la transición no añade nada a la pila
                if (numeroDeElementosAIntroducir == 0 && Stack.Count >= 2)
                {
                    //Generamos un número aleatorio entre 0 y 9
                    int indiceCaracterPila = random.Next(0, 9);
                    Transiciones.Add(new Transicion(EstadoActual, EstadoActual, CaracteresPila[indiceCaracterPila], Stack.Peek().ToString()[0], new List<char> {  }));
                    numeroDeTransiciones++;
                    Stack.Pop();
                }
                //Caso 1: Si el número de elementos a introducir es 1, entonces la transición añade un elemento a la pila
                else if (numeroDeElementosAIntroducir == 1 && (MaximoDeCaracteresContrasenia - numeroDeTransiciones + 2) > Stack.Count)
                {
                    //Generamos 2 transiciones, una que añade un elemento a la pila y otra que no añade nada a la pila
                    //Generamos un número aleatorio entre 0 y 9 para el caracter que se lee en la transición
                    int indiceCaracterPila = random.Next(0, 9);
                    //Generamos un número aleatorio entre 0 y 9 para el caracter que se introduce en la pila
                    Transiciones.Add(new Transicion(EstadoActual, EstadoActual, CaracteresPila[random.Next(0, 9)], Stack.Peek().ToString()[0], new List<char> { CaracteresPila[indiceCaracterPila] }));
                    Stack.Pop();
                    Stack.Push(CaracteresPila[indiceCaracterPila]);
                    numeroDeTransiciones++;
                    //Generamos un número aleatorio entre 0 y 9
                    Transiciones.Add(new Transicion(EstadoActual, EstadoActual, CaracteresPila[random.Next(0, 9)], Stack.Peek().ToString()[0], new List<char> { }));
                    numeroDeTransiciones++;
                    Stack.Pop();
                }
                //Caso 2: Elimina un elemento de la lista y añade dos, por lo tanto debe de añadir 2 transiciones más que que consumen esos 2 elementos
                //Caso 1: Si el número de elementos a introducir es 1, entonces la transición añade un elemento a la pila
                else if (numeroDeElementosAIntroducir == 2 && (MaximoDeCaracteresContrasenia - numeroDeTransiciones + 3) > Stack.Count)
                {
                    //Generamos 2 transiciones, una que añade un elemento a la pila y otra que no añade nada a la pila
                    //Generamos un número aleatorio entre 0 y 9 para el caracter que se lee en la transición
                    int indiceCaracterPila = random.Next(0, 9);
                    int indiceCaracterPila2 = random.Next(0, 9);
                    //Generamos un número aleatorio entre 0 y 9 para el caracter que se introduce en la pila
                    Transiciones.Add(new Transicion(EstadoActual, EstadoActual, CaracteresPila[random.Next(0, 9)], Stack.Peek().ToString()[0], new List<char> { CaracteresPila[indiceCaracterPila2], CaracteresPila[indiceCaracterPila] }));
                    Stack.Pop();
                    Stack.Push(CaracteresPila[indiceCaracterPila]);
                    Stack.Push(CaracteresPila[indiceCaracterPila2]);
                    numeroDeTransiciones++;
                    //Generamos un número aleatorio entre 0 y 9
                    Transiciones.Add(new Transicion(EstadoActual, EstadoActual, CaracteresPila[random.Next(0, 9)], Stack.Peek().ToString()[0], new List<char> { }));
                    numeroDeTransiciones++;
                    Stack.Pop();
                    //Generamos un número aleatorio entre 0 y 9
                    Transiciones.Add(new Transicion(EstadoActual, EstadoActual, CaracteresPila[random.Next(0, 9)], Stack.Peek().ToString()[0], new List<char> { }));
                    numeroDeTransiciones++;
                    Stack.Pop();
                }
            }
            //Añadimos transicicion que lleva al estado final
            Transiciones.Add(new Transicion(EstadoActual, EstadoActual + 1, ' ', Stack.Peek().ToString()[0], new List<char> { }));
            EstadoFinal = EstadoActual + 1;
            //Vaciamos la pila y la volvemos a llenar con la semilla
            Stack.Clear();
            Stack.Push(Zeta);
            foreach(char caracter in semilla)
            {
                Stack.Push(CaracteresPila[caracter]);
            }
            EstadoActual = semilla.Count;
        }
        public void ReinicarAutomata()
        {
            //Vaciamos la pila y la volvemos a llenar con la semilla
            Stack.Clear();
            Stack.Push(Zeta);
            foreach (char caracter in Semilla)
            {
                Stack.Push(CaracteresPila[caracter]);
            }
            EstadoActual = Semilla.Count;
        }
        public void ImprimirTransiciones()
        {
            foreach (Transicion transicion in Transiciones)
            {
                Console.Write($"S(q{transicion.EstadoInicial}, {transicion.Simbolo}, {transicion.PopSimboloPila}) = " + "{" + $"q{transicion.EstadoDestino}, {PushSimbolosToString(transicion.PushSimboloPila)}" + "}\n");
            }
        }
        public string ObtenerTransicionesTerrorista()
        {
            string transiciones = "";
            if (Transiciones.Count > 0)
            {
                for (int i = 0; i < Semilla.Count; i++)
                {
                    transiciones+=($"S({Transiciones[i].EstadoInicial}, {Transiciones[i].Simbolo}, {Transiciones[i].PopSimboloPila}) = " + "{" + $"{Transiciones[i].EstadoDestino}, {PushSimbolosToString(Transiciones[i].PushSimboloPila)}" + "}"+ Environment.NewLine);
                }
                return transiciones;
            }
            return "Sin transiciones...";
            
        }
        public string ObtenerTransicionesAntiterrorista()
        {
            string transiciones = "";
            List<Transicion> temp = new List<Transicion> { };
            for(int i = Semilla.Count; i < Transiciones.Count; i++)
            {
                temp.Add(Transiciones[i]);
            }
            Shuffle(temp);
            if (Transiciones.Count > 0)
            {
                for (int i = 0; i < temp.Count; i++)
                {
                    transiciones += ($"S(q{temp[i].EstadoInicial}, {temp[i].Simbolo}, {temp[i].PopSimboloPila}) = " + "{" + $"q{temp[i].EstadoDestino}, {PushSimbolosToString(temp[i].PushSimboloPila)}" + "}" + Environment.NewLine);
                }
                return transiciones;
            }
            return "Sin transiciones...";

        }
        static void Shuffle<T>(List<T> lista)
        {
            Random random = new Random();
            int n = lista.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                // Intercambiar los elementos en las posiciones i y j
                T temp = lista[i];
                lista[i] = lista[j];
                lista[j] = temp;
            }
        }
        public string ObtenerPila()
        {
            string pila = "";
            foreach (char simbolo in Stack)
            {
                pila += simbolo + Environment.NewLine;
            }
            return pila;
        }

        public bool CadenaAceptada(string cadena)
        {
            //Recorremos la cadena
            int indexCadena = 0;
            foreach(Transicion transicion in Transiciones)
            {
                if(transicion.EstadoInicial == EstadoActual)
                {
                    if(transicion.PopSimboloPila == Stack.Peek().ToString()[0] && transicion.Simbolo == cadena[indexCadena])
                    {
                        //Imprmir la pila
                        Console.WriteLine("Pila: ");
                        foreach(char simbolo in Stack)
                        {
                            Console.Write(simbolo);
                        }
                        EstadoActual = transicion.EstadoDestino;
                        Stack.Pop();
                        transicion.PushSimboloPila.Reverse();
                        foreach (char simbolo in transicion.PushSimboloPila)
                        {
                            Stack.Push(simbolo);
                        }
                        indexCadena++;
                        if(EstadoActual == EstadoFinal)
                        {
                            return true;
                        }
                    }
                }
            }
            //Vaciar pila e insertar la semilla nuevamente
            Stack.Clear();
            Stack.Push(Zeta);

            return false;
        }
        public string PushSimbolosToString(List<char> pushSimbolos)
        {
            string pushSimbolosString = "";
            foreach(char simbolo in pushSimbolos)
            {
                pushSimbolosString += simbolo;
            }
            return pushSimbolosString;
        }

    }
}
