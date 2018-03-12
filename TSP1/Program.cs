using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"1. Wczytaj dane z pliku");
            Console.WriteLine($"2. Losuj dane");
            Console.WriteLine($"3. Testuj");
            var decision = Convert.ToInt32(Console.ReadLine());
            var loader = new Loader();
            switch (decision)
            {
                case 1:
                    Console.WriteLine($"Podaj nazwe pliku:");
                    var name = Console.ReadLine();
                    var data = loader.ReadFromFile($"{name}.txt");
                    if (data.Cities == 0)
                    {
                        Console.ReadKey();
                        return;
                    }
                    var solver = new BBAlgorithm(data);
                    var sw = new Stopwatch();
                    sw.Start();
                    solver.Solve();
                    sw.Stop();
                    Console.WriteLine(sw.Elapsed);
                    break;

                case 2:
                    Console.WriteLine($"Podal liczbe miast: ");
                    var cities = Convert.ToInt32(Console.ReadLine());
                    var dataa = loader.RandomFile(cities);
                    var solver2 = new BBAlgorithm(dataa);
                    var sww = new Stopwatch();
                    sww.Start();
                    solver2.Solve();
                    sww.Stop();
                    Console.WriteLine(sww.Elapsed);
                    break;
                case 3:
                    var testArray = new[] {8, 10, 12, 14, 16, 18, 20};
                    
                    foreach (var test in testArray)
                    {
                        var span = new TimeSpan();
                        using (var file = new StreamWriter($"test{test}.txt"))
                        {
                            var _sw = new Stopwatch();
                            for (var i = 0; i < 100; i++)
                            {
                                _sw.Reset();
                                var _data = loader.RandomFile(test);
                                var _solver = new BBAlgorithm(_data);
                                _sw.Start();
                                _solver.Solve();
                                _sw.Stop();
                                file.WriteLine(_sw.Elapsed);
                                span += _sw.Elapsed;
                            }
                            file.WriteLine($"srednia: {span.Duration()}");
                        }
                    }
                    break;

                default:
                    Console.WriteLine($"wybierz jakas opcje");
                    break;
            }
            
            Console.ReadKey();

        }
    }
}
