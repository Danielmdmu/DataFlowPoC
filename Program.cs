using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataFlowPoC
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Proof of Concept for Sync-Manager with TPL Dataflow");

            var manager = new SyncManager(new TimeSpan(hours: 0, minutes: 0, seconds: 30), 5);

            var endApp = false; Console.WriteLine("Options:");

            Console.WriteLine("'c' for Collection");
            Console.WriteLine("'f' for Filter");
            Console.WriteLine("'q' for Quit");

            while (!endApp)
            {
                var key = Console.ReadKey(true);
                Console.WriteLine($"Eingabe = {key.Key}");
                Console.WriteLine("---------------------------------------");

                switch (key.Key)
                {
                    case ConsoleKey.C:
                        manager.AddSyncAction(new SyncAction(SyncTarget.Collection));
                        break;

                    case ConsoleKey.F:
                        manager.AddSyncAction(new SyncAction(SyncTarget.Filter));
                        break;

                    case ConsoleKey.Q:
                        endApp = true;
                        break;

                    default:
                        continue;
                }
            }

            return;
        }
    }
}