using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLFQ_ProcessScheduler
{
    class Program
    {
        static void Main(string[] args)
        {

            Process p1 = new Process("P1", new List<int> { 4, 24, 5, 73, 3, 31, 5, 27, 4, 33, 6, 43, 4, 64, 5, 19, 2 });
            Process p2 = new Process("P2", new List<int> { 18, 31, 19, 35, 11, 42, 18, 43, 19, 47, 18, 43, 17, 51, 19, 32, 10 });
            Process p3 = new Process("P3", new List<int> { 6, 18, 4, 21, 7, 19, 4, 16, 5, 29, 7, 21, 8, 22, 6, 24, 5 });
            Process p4 = new Process("P4", new List<int> { 17, 42, 19, 55, 20, 54, 17, 52, 15, 67, 12, 72, 15, 66, 14 });
            Process p5 = new Process("P5", new List<int> { 5, 81, 4, 82, 5, 71, 3, 61, 5, 62, 4, 51, 3, 77, 4, 61, 3, 42, 5 });
            Process p6 = new Process("P6", new List<int> { 10, 35, 12, 41, 14, 33, 11, 32, 15, 41, 13, 29, 11 });
            Process p7 = new Process("P7", new List<int> { 21, 51, 23, 53, 24, 61, 22, 31, 21, 43, 20 });
            Process p8 = new Process("P8", new List<int> { 11, 52, 14, 42, 15, 31, 17, 21, 16, 43, 12, 31, 13, 32, 15 });
            MLFQ_Queue queue1 = new MLFQ_Queue(1);
            MLFQ_Queue queue2 = new MLFQ_Queue(2);
            MLFQ_Queue queue3 = new MLFQ_Queue(3);
            //Initialize queue references to each other and set lower priority queue references
            queue1.queue1 = queue1;
            queue1.queue2 = queue2;
            queue1.queue3 = queue3;
            queue1.lowerpriority = queue2;
            queue2.queue1 = queue1;
            queue2.queue2 = queue2;
            queue2.queue3 = queue3;
            queue2.lowerpriority = queue3;
            queue3.queue1 = queue1;
            queue3.queue2 = queue2;
            queue3.queue3 = queue3;
            queue3.lowerpriority = null;
            //Add all processes to queue1
            queue1.Enqueue(p1);
            queue1.Enqueue(p2);
            queue1.Enqueue(p3);
            queue1.Enqueue(p4);
            queue1.Enqueue(p5);
            queue1.Enqueue(p6);
            queue1.Enqueue(p7);
            queue1.Enqueue(p8);
            processesinio = new List<Process>();
            bool finished = false;
            while (finished == false)
            {

                executequeue1(queue1);//Execute the Processes in queue1 using Round Robin with TQ 6
                executequeue2(queue2);//Execute the Processes in queue2 using Round Robin with TQ 11
                executequeue3(queue3);//Execute the Processes in queue3 using FCFS
                if (queue1.Count == 0 && queue2.Count == 0 && queue3.Count == 0 && numprocesscomplete != 8 && processesinio.Count != 0)//Execute if CPU is idle
                {
                    cpuidle(queue1);
                }
                if (queue1.Count == 0 && queue2.Count == 0 && queue3.Count == 0 && numprocesscomplete != 8 && processesinio.Count == 0)//Condition if all processes have completed
                {
                    printresults(p1, p2, p3, p4, p5, p6, p7, p8);
                    finished = true;
                }
            }

        }
        public static void executequeue1(MLFQ_Queue queue)
        {

            reverttohigherpriority = 0;//Reset priority variable
            while (queue.Count != 0)
            {

                Process p = queue.Dequeue();
                printexecution(p, queue, queue.queue2, queue.queue3);
                if (p.firstrun == true)
                {
                    p.responsetime = p.wait;
                    p.firstrun = false;
                }
                roundrobin(queue, p);//Execute the Processes in queue1 using Round Robin with TQ 6


            }
        }
        public static void executequeue2(MLFQ_Queue queue)
        {
            while (queue.Count != 0 && reverttohigherpriority != 1)//Run while queue1 does not have any processes
            {
                Process p = null;

                p = queue.Dequeue();
                printexecution(p, queue.queue1, queue, queue.queue3);

                if (p.firstrun == true)
                {
                    p.responsetime = p.wait;
                    p.firstrun = false;
                }
                roundrobin(queue, p);//Execute the Processes in queue2 using Round Robin with TQ 11

            }
        }
        public static void executequeue3(MLFQ_Queue queue)
        {
            while (queue.Count != 0 && reverttohigherpriority != 1 && reverttohigherpriority != 2)//Run while queue1 and queue2 do not have any processes
            {
                Process p = queue.Dequeue();
                printexecution(p, queue.queue1, queue.queue2, queue);
                if (p.firstrun == true)
                {
                    p.responsetime = p.wait;
                    p.firstrun = false;
                }
                fcfs(queue, p);//Execute the Processes in queue3 using FCFS

            }

        }
        public static void cpuidle(MLFQ_Queue queue1)//Handles CPU Idle incrementing Processes in I/O Burst counters
        {
            if (processesinio.Count != 0)
            {
                int n = 0;
                while (processesinio.ElementAt(n).iocounter < processesinio.ElementAt(n).ioburst)
                {
                    cpu_idle++;

                    totaltime++;

                    processesinio.ElementAt(n).iocounter++;
                    if (n == processesinio.Count - 1)
                    {
                        n = 0;
                    }
                    else
                    {
                        n++;
                    }

                }
                queue1.Enqueue(processesinio.ElementAt(n));
                processesinio.RemoveAt(n);
            }

        }
        public static void fcfs(MLFQ_Queue queue, Process p)
        {
            int cpuburst = 0;
            if (p.cpuburstleft == 0)
            {
                cpuburst = p.data[0];
                p.data.RemoveAt(0);
            }
            for (int i = cpuburst; i < cpuburst; i++)//Loop until CPU Burst Completes
            {
                p.cpuburstleft = cpuburst - i;
                totalallcpu++;
                totaltime++;

                for (int k = 0; k < processesinio.Count; k++)//Increment Processes in I/O Burst counters
                {
                    processesinio.ElementAt(k).iocounter++;


                    if (processesinio.ElementAt(k).iocounter == processesinio.ElementAt(k).ioburst)//Check to see if a Process has completed its I/O burst
                    {
                        processesinio.ElementAt(k).ioburst = 0;
                        processesinio.ElementAt(k).iocounter = 0;
                        queue.queue1.Enqueue(processesinio.ElementAt(k));
                        processesinio.RemoveAt(k);

                    }


                }

                incrementwaittimes(queue);


            }

            if (p.data.Count != 0)//Checks to see if the process has not completed and if so puts it in I/O
            {
                p.ioburst = p.data[0];
                p.totalio = p.totalio + p.ioburst;
                p.data.RemoveAt(0);
                processesinio.Add(p);
                Console.WriteLine("\nCompleted Execution= NO");
                Console.WriteLine("\n---------------------------------------------------\n\n\n");
            }
            else//Checks to see if the process has completed 
            {
                p.turnaroundtime = p.totalcpu + p.totalio + p.wait;
                numprocesscomplete++;
                Console.WriteLine("\nCompleted Execution= YES");
                Console.WriteLine("\n---------------------------------------------------\n\n\n");
            }
            if (queue.queue1.Count != 0)//Causes queue3 to stop executing its Processes because queue1 has a process
            {


                reverttohigherpriority = 1;
                return;
            }
            else if (queue.queue2.Count != 0)//Causes queue3 to stop executing its Processes because queue2 has a process
            {


                reverttohigherpriority = 2;
                return;
            }

        }
        public static void roundrobin(MLFQ_Queue queue, Process p)
        {
            int cpuburst = 0;
            if (p.data.Count == 0)
            {
                p.turnaroundtime = p.totalcpu + p.totalio + p.wait;
                numprocesscomplete++;
            }
            else if (p.cpuburstleft == 0)//If there is no CPU burst left from a previous execution 
            {
                cpuburst = p.data[0];
                p.data.RemoveAt(0);
            }
            else//Continue the CPU burst where the process left off
            {

                cpuburst = p.cpuburstleft;
                p.cpuburstleft = 0;
            }
            int i = 0;

            int tq = 0;
            int rununtil = 0;
            if (queue.queuenumber == 1)//Assign TQ to 6 if this is queue1
            {
                tq = 6;

            }
            else if (queue.queuenumber == 2)//Assign TQ to 11 if this is queue2
            {
                tq = 11;
            }
            if (tq >= cpuburst)//If CPU Burst is less than TQ only run until CPU burst length
            {
                rununtil = cpuburst;
                p.cpuburstleft = 0;
            }
            else//Otherwise run until TQ
            {
                rununtil = tq;
                p.cpuburstleft = cpuburst - tq;
            }
            for (i = 0; i < rununtil; i++)//Loop until CPU Burst Completes or until TQ finishes depending on which is less
            {


                totalallcpu++;
                totaltime++;

                for (int k = 0; k < processesinio.Count; k++)//Increment Processes in I/O Burst counters
                {
                    processesinio.ElementAt(k).iocounter++;


                    if (processesinio.ElementAt(k).iocounter == processesinio.ElementAt(k).ioburst)//Check to see if a Process has completed its I/O burst
                    {
                        processesinio.ElementAt(k).ioburst = 0;
                        processesinio.ElementAt(k).iocounter = 0;
                        queue.queue1.Enqueue(processesinio.ElementAt(k));
                        processesinio.RemoveAt(k);

                    }


                }

                incrementwaittimes(queue);

            }
            if (p.cpuburstleft != 0)//If the process did not finish before TQ ended, move it to a lower priority queue
            {
                queue.lowerpriority.Enqueue(p);
            }
            else if (p.data.Count != 0 && p.cpuburstleft == 0)// Checks to see if the process has not completed and if so puts it in I/O
            {
                p.ioburst = p.data[0];
                p.totalio = p.totalio + p.ioburst;
                p.data.RemoveAt(0);
                processesinio.Add(p);
                Console.WriteLine("\nCompleted Execution= NO");
                Console.WriteLine("\n---------------------------------------------------\n\n\n");
            }
            else if (p.data.Count == 0 && p.cpuburstleft == 0)
            {
                p.turnaroundtime = p.totalcpu + p.totalio + p.wait;
                numprocesscomplete++;
                Console.WriteLine("\nCompleted Execution= YES");
                Console.WriteLine("\n---------------------------------------------------\n\n\n");
            }
            if (queue.queuenumber == 2)
            {
                if (queue.queue1.Count != 0)//Causes queue2 to stop executing its Processes because queue1 has a process
                {

                    reverttohigherpriority = 1;
                    return;
                }
            }
        }
        public static void printexecution(Process p, MLFQ_Queue queue1, MLFQ_Queue queue2, MLFQ_Queue queue3)
        {
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("Execution Time is " + totaltime + "            " + p.name + " is Running");
            if (queue1.Count != 0)
            {
                for (int j = 0; j < queue1.Count; j++)
                {
                    if (queue1.ElementAt(j).data.Count != 0)
                    {
                        Console.WriteLine(queue1.ElementAt(j).name + " CPU Burst Length " + queue1.ElementAt(j).data.First() + " is inside of Queue 1" + "\n");
                    }
                }
            }
            if (queue2.Count != 0)
            {
                for (int k = 0; k < queue2.Count; k++)
                {
                    if (queue2.ElementAt(k).data.Count != 0)
                    {
                        Console.WriteLine(queue2.ElementAt(k).name + " CPU Burst Length " + queue2.ElementAt(k).data.First() + " is inside of Queue 2" + "\n");
                    }
                }
            }
            if (queue3.Count != 0)
            {
                for (int f = 0; f < queue3.Count; f++)
                {
                    if (queue3.ElementAt(f).data.Count != 0)
                    {
                        Console.WriteLine(queue3.ElementAt(f).name + " CPU Burst Length " + queue3.ElementAt(f).data.First() + " is inside of Queue 3" + "\n");
                    }
                }
            }
            int timeremaining = 0;
            Console.WriteLine("Processes in I/O\n");
            for (int y = 0; y < processesinio.Count; y++)
            {
                timeremaining = processesinio.ElementAt(y).ioburst - processesinio.ElementAt(y).iocounter;
                Console.WriteLine(processesinio.ElementAt(y).name + " Time Remaining in I/O " + timeremaining + "\n");
            }

        }
        public static void printresults(Process p1, Process p2, Process p3, Process p4, Process p5, Process p6, Process p7, Process p8)
        {

            int totalwait = p1.wait + p2.wait + p3.wait + p4.wait + p5.wait + p6.wait + p7.wait + p8.wait;
            double waitave = totalwait / 8;
            Console.WriteLine("\n\n\n----------------------------------------\nResults\n\n");
            Console.WriteLine("Total Time= " + totaltime);
            float cpuutilization = totalallcpu / totaltime;
            cpuutilization = cpuutilization * 100;
            Console.WriteLine("\nCPU Utilization= " + cpuutilization + "%");
            Console.WriteLine("\nWait Times\n");
            Console.WriteLine(" p1= " + p1.wait + " p2= " + p2.wait + " p3= " + p3.wait + " p4= " + p4.wait + " p5= " + p5.wait + " p6= " + p6.wait + " p7= " + p7.wait + " p8= " + p8.wait);
            Console.WriteLine("Average Wait Time= " + waitave);


            float totalturnaroundtime = p1.turnaroundtime + p2.turnaroundtime + p3.turnaroundtime + p4.turnaroundtime + p5.turnaroundtime + p6.turnaroundtime + p7.turnaroundtime + p8.turnaroundtime;
            float turnaroundave = totalturnaroundtime / 8;
            Console.WriteLine("\nTurnaround Times\n");
            Console.WriteLine("p1= " + p1.turnaroundtime + " p2= " + p2.turnaroundtime + " p3= " + p3.turnaroundtime + " p4= " + p4.turnaroundtime + " p5= " + p5.turnaroundtime + " p6= " + p6.turnaroundtime + " p7= " + p7.turnaroundtime + " p8= " + p8.turnaroundtime);
            Console.WriteLine("\nAverage Turnaround Time= " + turnaroundave);

            Console.WriteLine("\nResponse Times\n");
            Console.WriteLine("p1= " + p1.responsetime + " p2= " + p2.responsetime + " p3= " + p3.responsetime + " p4= " + p4.responsetime + " p5= " + p5.responsetime + " p6= " + p6.responsetime + " p7= " + p7.responsetime + " p8= " + p8.responsetime);
            float totalresponsetime = p1.responsetime + p2.responsetime + p3.responsetime + p4.responsetime + p5.responsetime + p6.responsetime + p7.responsetime + p8.responsetime;
            float responseave = totalresponsetime / 8;
            Console.WriteLine("\nAverage Response Time= " + responseave);





        }

        public static void incrementwaittimes(MLFQ_Queue queue)
        {


            if (queue.queuenumber == 1)
            {
                for (int j = 0; j < queue.Count; j++)
                {
                    queue.ElementAt(j).wait++;
                }
                for (int j = 0; j < queue.queue2.Count; j++)
                {

                    queue.queue2.ElementAt(j).wait++;
                }
                for (int j = 0; j < queue.queue3.Count; j++)
                {
                    queue.queue3.ElementAt(j).wait++;
                }
            }
            else if (queue.queuenumber == 2)
            {
                for (int j = 0; j < queue.Count; j++)
                {
                    queue.ElementAt(j).wait++;
                }
                for (int j = 0; j < queue.queue1.Count; j++)
                {

                    queue.queue1.ElementAt(j).wait++;
                }
                for (int j = 0; j < queue.queue3.Count; j++)
                {
                    queue.queue3.ElementAt(j).wait++;
                }
            }
            else if (queue.queuenumber == 3)
            {
                for (int j = 0; j < queue.Count; j++)
                {
                    queue.ElementAt(j).wait++;
                }
                for (int j = 0; j < queue.queue1.Count; j++)
                {

                    queue.queue1.ElementAt(j).wait++;
                }
                for (int j = 0; j < queue.queue2.Count; j++)
                {
                    queue.queue2.ElementAt(j).wait++;
                }
            }

        }
        public static List<Process> processesinio;
        public static float totaltime = 0;
        public static float totalallcpu = 0;
        public static int reverttohigherpriority = 0;
        public static int numprocesscomplete = 0;
        public static float cpu_idle = 0;

    }
    class MLFQ_Queue : Queue<Process>
    {
        public int queuenumber;
        public MLFQ_Queue queue1;
        public MLFQ_Queue queue2;
        public MLFQ_Queue queue3;
        public MLFQ_Queue lowerpriority;
        public MLFQ_Queue(int number)
        {
            queuenumber = number;
        }

    }
    class Process
    {
        public Process(String n, List<int> d)
        {
            name = n;
            data = d;
        }
        public String name;
        public List<int> data;
        public int cpuburstleft = 0;
        public int wait = 0;
        public int ioburst = 0;
        public int iocounter = 0;

        public bool firstrun = true;
        public int responsetime = 0;

        public int turnaroundtime = 0;
        public int totalcpu = 0;
        public int totalio = 0;

    }
}
