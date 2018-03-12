using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace TSP1
{
    class Vertex 
    {
        public int Id;  // Numer wierzchołka
        public int Level;   // Poziom na którym się znajduję wierzchołek. Zaczynając od korzenia drzewa stanów. Licząć od 0.
        public int LowerBound { get; private set; } // Dolne ograniczenie danego wierzchołka
        public SortedSet<Vertex> PossibleConnections { get; private set; }  // Posortowane rosnąco możliwe połączenia z danego wierzchołka według dolnego ograniczenia.    
                                                                            // Pierwszy element zawsze będzie najlepszym połączeniem
        public Dictionary<int,Vertex> Parents { get; private set; } // Słownik który zawiera odwiedzone wcześniej wierzchołki, zanim dotarliśmy do aktualnego.
        private int SetLowerBound(int[] lowerBoundTable, int cost)  // Obliczenie dolnego ograniczenia dla danego wierzchołka
        {
            var diff = cost - lowerBoundTable[Level];   // Obliczenie różnicy pomiędzy wagą krawędzi a minimum z odpowieniego wiersza macierzy kosztów
            return diff;    // Zwrócenie różnicy
        }
        public void SetPossibleConnections(Data data, Dictionary<int,Vertex> visited)   // Obliczenie możliwych połączeń z tego wierzchołka
        {
            for (var i = 0; i < data.Cities; i++)
            {
                if (Id == i || visited.ContainsKey(i)) continue;    // Jeżeli id==i oznacza to że chcielibyśmy pójść do wierzchołka w którym aktualnie jesteśmy, dlatego pomijamy
                                                                    // Oraz jeżeli potencjalny wierzchołek znajduję się już we wcześniej odwiedzonych, pomijamy
                var lowerBound = LowerBound + SetLowerBound(data.LowerBoundTable, data.TspArray[Id][i]);    // Obliczenie dolnego ograniczenia dla następnego wierzchołka 
                                                                                                            // na podsatawie ograniczenia w obecnym wierzchołku
                PossibleConnections.Add(new Vertex(i, Level + 1, lowerBound, this));  // Dodanie do SortedSet nowego wierzchołka
                                                                                      // Powtarzamy dopóki nie dodamy wszystkih możliwych połączeń
            }
        }
        public Vertex(int lowerBound)
        {
            Id = 0;
            Level = 0;
            LowerBound = lowerBound;
            PossibleConnections = new SortedSet<Vertex>(new VertexComparer());
            Parents=new Dictionary<int, Vertex>();
        }
        public Vertex(int id, int level, int lowerBound, Vertex vertex)
        {
            Id = id;
            Level = level;
            LowerBound = lowerBound;
            PossibleConnections = new SortedSet<Vertex>(new VertexComparer());
            Parents = new Dictionary<int, Vertex>();
            if (vertex.Parents != null)
            {
                foreach (var vertexParent in vertex.Parents)
                {
                    Parents.Add(vertexParent.Key,vertexParent.Value);
                }
            }  
            Parents.Add(vertex.Id,vertex);
        }
    }
}
