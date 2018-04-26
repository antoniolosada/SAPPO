using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * El algoritmo en la primera iteración localiza los nodos adyacentes del nodo actual y recupera la distancia a cada nodo adyacente
 * En cada iteración posterior localiza los nodos adyacentes desde cada
 */
namespace Trayectoria
{
    struct Enlace
    {
        public int NodoPadre;
        public int NodoHijo;

        public Enlace(int Padre, int Hijo)
        {
            NodoPadre = Padre;
            NodoHijo = Hijo;
        }
    };
    class Dijkstra
    { //1 Declaración de variables a utilizar
        private int rango = 0;
        private int[,] L; // matriz de adyacencia
        private int[,] T; // matriz de adyacencia
        private int[] C; // arreglo de nodos
        public int[] D; // arreglo de distancias
        private int trango = 0;
        public int n_nodos = 0;
        private Stack<Enlace> Pila = new Stack<Enlace>();
        // Algoritmo Dijkstra
        public Dijkstra(int paramRango, int[,] paramArreglo)
        {
            L = new int[paramRango, paramRango];
            C = new int[paramRango];
            D = new int[paramRango];
            rango = paramRango;

            for (int i = 0; i < rango; i++)
            {
                for (int j = 0; j < rango; j++)
                {
                    L[i, j] = paramArreglo[i, j];
                }
            }

            for (int i = 0; i < rango; i++)
            {
                C[i] = i;
            }
            C[0] = -1;
            for (int i = 1; i < rango; i++)
            {
                D[i] = L[0, i];
            }
        }

        // Rutina de solución Dijkstra
        public void SolDijkstra()
        {
            int minValor = Int32.MaxValue;
            int minNodo = 0;

            for (int i = 0; i < rango; i++)
            {
                if (C[i] == -1)
                    continue;
                if (D[i] > 0 && D[i] < minValor)
                {
                    minValor = D[i];
                    minNodo = i;
                }
            }
            C[minNodo] = -1;

            for (int i = 0; i < rango; i++)
            {
                if (L[minNodo, i] < 0) // si no existe arco
                    continue;
                if (D[i] < 0) // si no hay un peso asignado
                {
                    D[i] = minValor + L[minNodo, i];
                    Console.WriteLine(minNodo + ",(" + i + ")");
                    Pila.Push(new Trayectoria.Enlace(minNodo, i));
                    continue;
                }
                if ((D[minNodo] + L[minNodo, i]) < D[i])
                {
                    D[i] = minValor + L[minNodo, i];
                    Console.WriteLine(minNodo + ",(" + i + ")");
                    Pila.Push(new Trayectoria.Enlace(minNodo, i));
                }
            }
        }

        // Función de implementación del algoritmo
        public void CorrerDijkstra()
        {
            for (trango = 1; trango < rango; trango++)
            {
                SolDijkstra();
                Console.WriteLine("lteracion No." + trango);
                Console.WriteLine("Matriz de distancias: ");
                for (int i = 0; i < rango; i++)
                    Console.Write(i + " ");

                Console.WriteLine(" ");

                for (int i = 0; i < rango; i++)
                    Console.Write(D[i] + " ");

                Console.WriteLine(" ");
                Console.WriteLine(" ");
            }
        }
        public Stack<int> Ruta(int NodoFin)
        {
            Stack<int> Trayectoria = new Stack<int>();
            while (true)
            {
                Enlace Union;
                try
                {
                    Union = Pila.Pop();
                }
                catch (Exception ex)
                {
                    Trayectoria.Push(NodoFin);
                    return Trayectoria;
                }
                if (Union.NodoHijo == NodoFin)
                {
                    Trayectoria.Push(NodoFin);
                    NodoFin = Union.NodoPadre;
                }
                if ((Union.NodoPadre == 0) || (Pila.Count == 0))
                {
                    Trayectoria.Push(NodoFin);
                    return Trayectoria;
                }
            }
        }

    }
}
