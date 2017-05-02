using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJF_ProcessScheduler
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
            readyqueue = new List<Process>();
            //Add all processes to readyqueue
            readyqueue.Add(p1);
            readyqueue.Add(p2);
            readyqueue.Add(p3);
            readyqueue.Add(p4);
            readyqueue.Add(p5);
            readyqueue.Add(p6);
            readyqueue.Add(p7);
            readyqueue.Add(p8);
            processesinio = new List<Process>();

            bool shortest;

            int k = 1; ;
            while (numprocesscomplete != 8)//Run until all processes complete
            {
                //Find the Process with the Shortest CPU Burst Length
                if (readyqueue.Count != 0)
                {

                    Process shortestprocess = readyqueue.ElementAt(0);

                    k = 1;
                    shortest = false;
                    while (!shortest)
                    {

                        if (readyqueue.Count == 1)//If there is one Process in the readyqueue than that is the Shortest Process
                        {
                            shortest = true;
                            shortestprocess = readyqueue.ElementAt(0);
                            readyqueue.RemoveAt(0);

                        }
                        else if (k != readyqueue.Count - 1 && shortestprocess.data.First() > readyqueue.ElementAt(k).data.First())//If the Process is smaller than the "Shortest Process" then reassign the "Shortest Process"
                        {
                            shortest = false;
                            shortestprocess = readyqueue.ElementAt(k);

                        }
                        else if (k == readyqueue.Count - 1 && shortestprocess.data.First() < readyqueue.ElementAt(k).data.First())//If it is comparing the last Process in the ready queue to the "Smallest Process" and the "Smallest Process" is smaller then it is the smallest
                        {
                            shortest = true;
                        }
                        else if (k == readyqueue.Count - 1 && shortestprocess.data.First() > readyqueue.ElementAt(k).data.First())//If it is comparing the last Process in the ready queue to the "Smallest Process" and the "Smallest Process" is bigger then it is the last Process is the smallest
                        {
                            shortest = true;
                            shortestprocess = readyqueue.ElementAt(k);
                        }
                        else if (k != readyqueue.Count - 1 && shortestprocess.data.First() == readyqueue.ElementAt(k).data.First())//If there is a tie than the Process that entered the readyqueue first is the "Smallest Process"
                        {
                            if (shortestprocess.timeenteringqueue > readyqueue.ElementAt(k).timeenteringqueue)
                            {
                                shortest = false;
                                shortestprocess = readyqueue.ElementAt(k);
                            }
                        }
                        else if (k == readyqueue.Count - 1 && shortestprocess.data.First() == readyqueue.ElementAt(k).data.First())//If there is a tie than the Process that entered the readyqueue first is the "Smallest Process", this condition is for comparing with the last element in the readyqueue
                        {
                            if (shortestprocess.timeenteringqueue > readyqueue.ElementAt(k).timeenteringqueue)
                            {
                                shortest = true;
                                shortestprocess = readyqueue.ElementAt(k);

                            }
                            else
                            {
                                shortest = true;

                            }

                        }

                        k++;
                    }
                    if (shortestprocess.firstrun == true)
                    {
                        shortestprocess.responsetime = shortestprocess.wait;
                        shortestprocess.firstrun = false;
                    }

                    readyqueue.Remove(shortestprocess);
                    executeprocess(shortestprocess);//Execute the Shortest Process
                }
                else if (processesinio.Count != 0)//Handles CPU Idle incrementing Processes in I/O Burst counters
                {
                    int n = 0;
                    while (processesinio.ElementAt(n).iocounter < processesinio.ElementAt(n).ioburst)
                    {
                        cpuidle++;
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
                    if (processesinio.ElementAt(n).data.Count == 0)
                    {
                        processesinio.ElementAt(n).turnaroundtime = processesinio.ElementAt(n).totalcpu + processesinio.ElementAt(n).totalio + processesinio.ElementAt(n).wait;
                        numprocesscomplete++;
                        processesinio.RemoveAt(n);

                    }
                    else
                    {
                        readyqueue.Add(processesinio.ElementAt(n));
                        processesinio.RemoveAt(n);
                    }
                }
            }
            printresults(p1, p2, p3, p4, p5, p6, p7, p8);


        }
        public static void printexecution(Process p)
        {
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("Execution Time is " + totaltime + "            " + p.name + " is Running");
            Console.WriteLine("Processes in Ready Queue\n");
            for (int j = 0; j < readyqueue.Count; j++)
            {
                Console.WriteLine(readyqueue.ElementAt(j).name + " CPU Burst Length " + readyqueue.ElementAt(j).data.First() + "\n");
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
        public static void executeprocess(Process p)
        {
            int cpuburst = p.data.First();
            p.totalcpu = p.totalcpu + cpuburst;
            p.data.RemoveAt(0);
            printexecution(p);
            for (int i = 0; i < cpuburst; i++)//Loop until CPU Burst Completes
            {
                totalallcpu++;
                totaltime++;

                for (int k = 0; k < processesinio.Count; k++)//Increment Processes in I/O Burst counters
                {
                    processesinio.ElementAt(k).iocounter++;


                    if (processesinio.ElementAt(k).iocounter == processesinio.ElementAt(k).ioburst)//Check to see if a Process has completed its I/O burst
                    {
                        if (processesinio.ElementAt(k).data.Count == 0)//If I/O was the last action needed to do by the Process than the Process has completed
                        {
                            p.turnaroundtime = p.totalcpu + p.totalio + p.wait;
                            numprocesscomplete++;
                            processesinio.RemoveAt(k);
                            Console.WriteLine("\nCompleted Execution= YES");
                            Console.WriteLine("\n---------------------------------------------------\n\n\n");

                        }
                        else//If I/O was not the last action needed to do by the Process then it is added to the readyqueue and removed from the I/O list
                        {

                            processesinio.ElementAt(k).ioburst = 0;
                            processesinio.ElementAt(k).iocounter = 0;
                            processesinio.ElementAt(k).timeenteringqueue = totaltime;

                            readyqueue.Add(processesinio.ElementAt(k));

                            processesinio.RemoveAt(k);
                        }
                    }

                }

                for (int j = 0; j < readyqueue.Count; j++)//Increments Processes' Wait times
                {
                    readyqueue.ElementAt(j).wait++;

                }

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
        }

        public static List<Process> processesinio;
        public static List<Process> readyqueue;
        public static int totaltime = 0;
        public static int numprocesscomplete = 0;
        public static int round = 0;
        public static float totalallcpu = 0;
        public static float cpuidle = 0;
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
        public int wait = 0;
        public int ioburst = 0;
        public int iocounter = 0;
        public int timeenteringqueue = 0;

        public bool firstrun = true;
        public int responsetime = 0;

        public int turnaroundtime = 0;
        public int totalcpu = 0;
        public int totalio = 0;
    }
}
