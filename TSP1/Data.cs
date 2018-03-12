using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP1
{
    class Data
    {
        public int Cities;  // Liczba miast
        public int[][] TspArray;    // Tablica dwuwymiarowa przechowująca macierz kosztów
        public int[] LowerBoundTable;   // Tablica przechowująca minima z każdego wiersza macierzy kosztów
        public int LowerBound;  // Początkowe dolne ograniczenie dla danej instancji
        public void SetLowerBoundTable()    // Metoda obliczająca minimum z każdego wiersza macierzy kosztów
        {
            var cities = Cities;
            LowerBoundTable = new int[cities];
            for (int i = 0; i < cities; i++)
            {
                var min = i==0 ? TspArray[i][1] : TspArray[i][0];   // Jeżeli i = 0 przypisujemy do min [i][1] a nie [i][0] poniewaz w [i][0] znajduje się -1 co oznacza przekątną
                for (int j = 1; j < cities; j++)
                {
                    var cost = TspArray[i][j];
                    if (cost!=-1 && cost < min) // Jeżeli waga nie jest na przekątnej i jest mniejsza od dotychczasowego min przypisz do min
                        min = cost;
                }
                LowerBoundTable[i] = min;   // Wpisanie do tablicy minimum
            }
            var sum = 0;
            for (var i = 0; i < cities; i++)    //Sumowanie wszystkich minimów w celu obliczenia początkowego dolnego ograniczenia
            {
                sum += LowerBoundTable[i];
            }
            LowerBound = sum;
        }
        public Data(int cities, int [][]tspArray)
        {
            Cities = cities;
            TspArray = tspArray;
        }
    }
}
