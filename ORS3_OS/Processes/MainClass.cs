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

        public static void Main (String [] args)
        {

            
            Scheduler sc = new Scheduler();

            for(int i=0; i<50; i++)
            {
                sc.NewProcess(i.ToString());
            }
            sc.NewProcess("System");
            sc.NewProcess("System.io");
            //sc.RunThread();
            sc.CPUUtilization();

            Thread reru = new Thread(new ThreadStart(sc.ReadyToRunningStart)); //vrsi se stalno prebacivanje iz stanja ready u running i obrnuto
            Thread rure = new Thread(new ThreadStart(sc.RunningToReadyStart));
            //Thread block = new Thread(new ThreadStart(sc.Block));
            //Thread unblock = new Thread(new ThreadStart(sc.Unblock));

            reru.Start();
            rure.Start();
            //block.Start();
            //unblock.Start();

            

            sc.TaskManager();
            

            
        }

        
    }
}
