using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP1
{
    class BBAlgorithm
    {
        private Data Data;  // Informacje o danej instancji: macierz kosztów , liczba miast, początkowe ograniczenie
        private int UpperBound; // Górne ograniczenie
        public Dictionary<int, Vertex> Visited;
        public SortedSet<Vertex> NotVisited;    // Posortowane rosnąco według dolnego ograniczenia wierchołki które pominęliśmy ponieważ wtedy nie były najkorzystniejszą drogą
        private Vertex BestRoad;    // Wierzchołek który jest końcem najlepszego rozwiązania
        private Stopwatch Watch;    // Zegarek liczący czas aby gdy minie minuta przerwać algorytm i zwrócić do tej pory najlepszą drogę
        private int CalculateCost(Dictionary<int, Vertex> visited, Vertex vertex)   // Obliczenie drogi na podstawie odwiedzonych wierzchołków
        {
            var cost = 0;
            var keys = visited.Keys.ToArray();  // Przypisanie kluczy ze słownika do nowej tablicy. Kluczami w tym słowniku są numery odwiedzonych dotychczas wierzchołków
            for (var i = 0; i < visited.Keys.Count - 1; i++)    // Przegląd wszystkich kluczy ze słownika przekazanego w parametrze
                cost += Data.TspArray[keys[i]][keys[i + 1]];    // Dodanie do kosztu odpowiedzniej wagi krawędzi
            cost += Data.TspArray[keys[keys.Length - 1]][vertex.Id];    // Dodanie przedostatniej krawędzi
            cost += Data.TspArray[vertex.Id][0];    // Dodanie krawędzi powrotnej
            return cost;    // Zwrócenie kosztu całej drogi
        }
        private void FindBetterSolution(SortedSet<Vertex> notVisited) // Funkcja przeszukuje wszystkie wcześniej pominięte obiecujące wierzchołki w celu znalezienia lepszego rozwiązania
        {
            var notVisit = new SortedSet<Vertex>(new VertexComparer()); // Stworzenie nowego SortedSet aby móc do niego dodać obiecującyh potomków 
                                                                        // których pominiemy ze względu na istnienie bardziej korzystnego połączenia niż do niego.
                                                                        // Póżniej ten SortedSet zostanie przekazany jako parametr do tej funkcji w wywołaniu rekurencyjnym
            foreach (var vertex in notVisited)  // Dla każdego pominiętego wcześniej wierzchołka
            {
                if (vertex.LowerBound > UpperBound) // Jeżeli dolne ograniczenie jest większe niż górne nie ma sensu dalej przeglądać
                                                    // Ponieważ ta kolekcja jest sortowana rosnąco według dolnego ograniczenia, dlatego każdy kolejny wierzchołek ma coraz większe ograniczenia
                                                    // które jeszcze bardziej będzie przekraczać górne ograniczenie
                    break;
                var level = vertex.Level;
                var nextVertex = vertex;
                while (level < Data.Cities - 1) // Schodzenie w "dół" w celu znalezieniu rozwiązana
                {
                    nextVertex.SetPossibleConnections(Data, nextVertex.Parents);    // Obliczenie możliwych połączeń dla danego wierzchołka
                    AddToNotVisited(nextVertex, notVisit);  // Dodanie wszyskich potomków oprócz pierwszego (który jest najkorzystniejszy i do niego będzie poprowadzone połączenie)
                                                                // do kolekcji aby w rekurencji móc je przeszukać
                    nextVertex = nextVertex.PossibleConnections.First();    // Przypisanie najlepszego połączenia jako następny wierzchołek do sprawdzenia
                    if (nextVertex.LowerBound > UpperBound) break;  // Jeżeli dolne ograniczenie następnego najbardziej obeicującego potomka jest większe niż górne ograniczenie przerwij przeszukiwanie
                    level = nextVertex.Level;  
                }
                if (nextVertex.Parents.Count == Data.Cities - 1)    // Jeżeli algorytm doszedł do liścia oblicz drogę na podsatwie odwiedzonych wierzchołków
                {
                    var up = CalculateCost(nextVertex.Parents, nextVertex); // Wywołanie funkci obliczającej drogę
                    if (up < UpperBound)    // Jeżeli droga jest lepsza niż górne ograniczenie przypisz ją jako nowe górne ograniczenie
                    {
                        UpperBound = up;
                        BestRoad = nextVertex;  // Oraz ostatni wierzchołek dodaj jako ostani wierzcholek najlepszej drogi
                    }
                }
                if (Watch.ElapsedMilliseconds >= 60000) // Jeżeli algorytm działa dłużej niż minute, przerwij jego działanie
                {
                    Console.WriteLine($"Minuta minela, przerywam dzialanie");
                    return;
                }
            }
            
            if (notVisit.Count != 0)    // Jeżeli jest conajmniej 1 pominięty wierzchołek, wywołaj tę samą metode z kolekcją przechowującą te wierzchołki
                FindBetterSolution(notVisit);
        }   
        private void AddToNotVisited(Vertex vertex, SortedSet<Vertex> notVisited)   // Dodanie do kolekcji pominiętych wierzchołków, wierzchołek przekazany w parametrach
        {
            foreach (var ver in vertex.PossibleConnections.Skip(1)) // Każde możliwe połączenie z pominięciem pierwszgo które jest najlepsze dodaj do kolekcji wierzchołków pominiętych
            {
                notVisited.Add(ver);
            }
        }
        private void ShowBestRoad() // Wyświetlnie rozwiązania
        {
            Console.Write($"\nKoszt: {UpperBound}\n");
            Console.Write($"Droga: ");
            foreach (var parentsKey in BestRoad.Parents.Keys)
            {
                Console.Write($"{parentsKey} ");
            }
            Console.Write($"{BestRoad.Id}\n");

        }
        public void Solve() // Funkcja znajdująca pierwsze rozwiązanie
        {
            Watch = new Stopwatch();
            Watch.Start();
            Data.SetLowerBoundTable();  // Obliczenie minimów dla każdego wiersza z macierzy kosztów
            var root = new Vertex(Data.LowerBound);     // Stworzenie korzenia, tutaj zawsze będzie nim pierwszy wierzchołek
            Visited = new Dictionary<int, Vertex>();
            NotVisited = new SortedSet<Vertex>(new VertexComparer());   // Inicjalizacja kolekcji przechowującej aktualnie pominięte wierchołki ze względu na istnienie lepszych
            Visited.Add(root.Id, root);
            root.SetPossibleConnections(Data, root.Parents);   // Obliczenie możliwych połączeń dla korzenia
            AddToNotVisited(root, NotVisited);  // Dodanie do kolekcji wszystkich połączeń z pominięciem pierwszego które jest najlepsze
            var nextVertex = root.PossibleConnections.First();  // Przypisanie najlepszego połączenia jako kolejny wierzchołek do odwiedzenia
            var level = nextVertex.Level;   // Schodzenie w dół drzewa
            Visited.Add(nextVertex.Id,nextVertex);
            while (level<Data.Cities-1) // Dopóki nie zejdzeiemy na sam dół powtarzaj
            {
                nextVertex.SetPossibleConnections(Data, nextVertex.Parents);
                AddToNotVisited(nextVertex, NotVisited);
                nextVertex = nextVertex.PossibleConnections.First();
                level = nextVertex.Level;
                Visited.Add(nextVertex.Id, nextVertex);
            }
            UpperBound = CalculateCost(nextVertex.Parents, nextVertex);     // Przypisanie jako górne ograniczenie pierwszego rozwiązania
            BestRoad = nextVertex;
            FindBetterSolution(NotVisited);   // Wywyołanie funkcji szukającej lepszego rozwiązania
            ShowBestRoad(); // Wyświetlenie najlepszego rozwiązania
            Watch.Stop();
        }
        public BBAlgorithm(Data data)
        {
            Data = data;
        }
    }
}
    