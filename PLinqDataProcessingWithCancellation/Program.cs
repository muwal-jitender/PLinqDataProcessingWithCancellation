using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PLinqDataProcessingWithCancellation
{
    class Program
    {
        static CancellationTokenSource _cancelToken = new();
        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("Start any key to start processing");
                Console.ReadKey();
                Console.WriteLine("Processing on thread {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);

                Program p = new();
                Task.Factory.StartNew(p.ProcessIntData);
                Console.WriteLine("Enter Q to quit: ");
                string answer = Console.ReadLine();
                // Does user want to quit?
                if (answer.Equals("Q", StringComparison.OrdinalIgnoreCase))
                {
                    _cancelToken.Cancel();
                    break;
                }
               
            } while (true);
           Console.ReadLine();
        }

        void ProcessIntData()
        {
            Console.WriteLine("Processing on thread {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            // Get a very large array of integers.
            int[] source = Enumerable.Range(1, 1_00_000_000).ToArray();

            // Find the numbers where num % 3 == 0 is true, returned in descending order.
            try
            {
                int[] modThreeIsZero = (
                                        // from num in source
                                        from num in source.AsParallel().WithCancellation(_cancelToken.Token)
                                        where num % 3 == 0
                                        orderby num descending
                                        select num).ToArray();
                Console.WriteLine($"Found { modThreeIsZero.Count()} numbers that match query!");

            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
