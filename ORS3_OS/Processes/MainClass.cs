using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.Threading;
using System.Timers;

namespace ORS3_OS.Processes
{
    class MainClass
    {
        public static List<string> RAM = new List<string>();
        public static List<string> HardDriveMemory = new List<string>();
        public static int MAX_RAM = 20;
        
        public static void Main(String[] args)
        {
            Scheduler sc = new Scheduler();

            for (int i = 0; i < 50; i++)
            {
                sc.NewProcess("P"+i.ToString());
            }
            sc.NewProcess("System");
            sc.NewProcess("System.io");
            sc.CPUUtilization();

            Thread reru = new Thread(new ThreadStart(sc.ReadyToRunningStart)); //vrsi se stalno prebacivanje iz stanja ready u running i obrnuto
            Thread rure = new Thread(new ThreadStart(sc.RunningToReadyStart));
            Thread block = new Thread(new ThreadStart(sc.BlockStart));
            Thread unblock = new Thread(new ThreadStart(sc.UnblockStart));

            reru.Start();
            rure.Start();
            block.Start();
            unblock.Start();

            while (true)
            {
                Console.WriteLine("Input command:");
                String com = Console.ReadLine();
                if (com.Equals("TaskManager"))
                {
                    sc.TaskManager();
                }
                else if (com.Equals ("Exit"))
                {
                    System.Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Unknown command!");
                }
                Thread.Sleep(100);
            }

        }

    }
}
