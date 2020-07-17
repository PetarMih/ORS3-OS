using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace ORS3_OS.Processes
{
    class MainClass
    {
        static void main (String [] args)
        {

         
            Scheduler sc = new Scheduler();

            for(int i=0; i<100; i++)
            {
                Process p = new Process(i.ToString());
                sc.AddProcess(p);
            }

            while (true)
            {
                sc.RunningToReady();
                sc.ReadyToRunning();
                sc.Block();
            }
        }
    }
}
