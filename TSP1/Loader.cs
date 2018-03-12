using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP1
{
    class Loader
    {
        public Data ReadFromFile(string fileName)
        {
            try
            {
                using (var sr = new StreamReader(fileName))
                {
                    var cities = Convert.ToInt32(sr.ReadLine());
                    var array = new int[cities][];
                    for (var i = 0; i < cities; i++)
                    {
                        array[i] = new int[cities];
                    }
                    for (var i = 0; i < cities; i++)
                    {
                        var line = sr.ReadLine();
                        if (line == null) continue;
                        var numbers = line.Split(' ');
                        for (var j = 0; j < cities; j++)
                            array[i][j] = Convert.ToInt32(numbers[j]);
                    }
                    
                    ///////////////////////////////////////////////////////////////////////////////////////
                    // Wyświetlanie macierzy kosztów
                    for (var i = 0; i < cities; i++)
                    {
                        Console.WriteLine($"");
                        for (var j = 0; j < cities; j++)
                            Console.Write($"{array[i][j]} ");
                    }
                    ////////////////////////////////////////////////////////////////////////////////////////
                    return new Data(cities, array);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new Data(0, null);
            }
        }
        public Data RandomFile(int cities)
        {
            var random = new Random();
            var array = new int[cities][];
            for (var i = 0; i < cities; i++)
            {
                array[i] = new int[cities];
            }
            for (var i = 0; i < cities; i++)
            for (var j = 0; j < cities; j++)
            {
                if (i == j)
                    array[i][j] = -1;
                else
                    array[i][j] = random.Next(0, 101);
            }

            for (var i = 0; i < cities; i++)
            {
                Console.WriteLine($"");
                for (var j = 0; j < cities; j++)
                    Console.Write($"{array[i][j]} ");
            }
            return new Data(cities, array);
        }
    }
}
