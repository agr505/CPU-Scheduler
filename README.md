# CPU-Scheduler
Developed three CPU scheduling algorithms for simulating how computer operating systems schedule processes.
I implemented FCFS (First Come First Serve) non-preemptive, SJF (Shortest Job First) non-preemptive, and MLFQ (Multi Level Feedback Queue)
preemptive. After processes complete a CPU burst, they  then goes in I/O and once they complete I/O they are scheduled by the algorithm.
Average wait time, turnaround time, CPU utilization, and response time for each algorithm are calculated. The MLFQ implementation embodies
object oriented principles by containing a class MLFQ_Queue which inherits from the generic Queue<T> and each MLFQ_Queue contains 
references to other MLFQ_Queue's relative to their respective priorities. The two higher priority queue’s use Round Robin with different
time quantums for their own scheduling while the lowest priority queue uses FCFS for scheduling.
