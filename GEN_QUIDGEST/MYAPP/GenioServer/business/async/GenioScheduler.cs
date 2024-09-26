using System;
using System.Collections.Generic;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;
using System.Linq;


namespace CSGenio.business.async
{
    using Unit = String;
    using Process = CSGenioAs_apr;    


    public class GenioScheduler
    {

        public GenioScheduler(JobFinder jobFinder)
        {
            _jobFinder = jobFinder;
        }
    
        private JobFinder _jobFinder;

        private List<GenioWork> works = new List<GenioWork>();
        //private List<CSGenioAprman> inMaintenance = new List<CSGenioAprman>();


        public GenioWork GetWork(List<Process> processos, PersistentSupport sp, User user)
        {
            UpdateWorks(processos, sp, user);

            for (int i = 0; i < works.Count; i++)
            {
                //Se está em fila de espera tentamos subi-lo na lista de execução
                if (works[i].Process.ValRtstatus == ArrayS_prstat.E_FE_2 && works[i].FulfillRequirements(sp, user))
                {

                    for (int j = i - 1; ; j--)
                    {
                        if (j == -1)
                            //If evaluating the first process, that means it's executable right now.
                            return works[i];
                        else if (Collision(works[i].Job, works[j].Job, sp))
                            break;
                        else
                            //Move up the list, continue checking for collisions
                            continue;
                    }
                    //If it hasn't returned, this process cannot be executed. Move to next
                    continue;
                }
            }

            return null;
        }

        public bool Collision(GenioExecutableJob first, GenioExecutableJob second, PersistentSupport sp)
        {
            PartitionPolicy firstPolicy = first.GetPartitionPolicy(second);
            PartitionPolicy secondPolicy = second.GetPartitionPolicy(first);

            if (firstPolicy.IsGlobal || secondPolicy.IsGlobal)
            {
                return true;
            }
            else
            {
                List<Unit> firstList = firstPolicy.GetSubUnits(sp);
                List<Unit> secondList = secondPolicy.GetSubUnits(sp);
                return firstList.Intersect(secondList).Any();
            }            
        }

        private int OlderFirst(GenioWork first, GenioWork second)
        {
            if (first.Process.ValId > second.Process.ValId)
                return 1;
            else if (first.Process.ValId == second.Process.ValId)
                return 0;
            else
                return -1;
        }

        private int HighestPriority(GenioWork first, GenioWork second)
        {
            if (first.Priority > second.Priority)
                return 1;
            else if (first.Priority == second.Priority)
                return OlderFirst(first, second);
            else
                return -1;
        }

        private void UpdateWorks(List<Process> processos, PersistentSupport sp, User user)
        {
            //If a process isn't in the list it has finished. Remove it.
            var finished = works.Select(x => x.Process.ValCodascpr)
                .Except(processos.Select(x => x.ValCodascpr));
            if (finished.Count() > 0)
                works.RemoveAll(x => finished.Contains(x.Process.ValCodascpr));

            //Add new process and update existing ones            
            foreach (Process process in processos)
            {
                GenioWork existing = works.Find(x => x.Process.ValCodascpr == process.ValCodascpr);
                if (existing != null)
                {
                    if (existing.Process.ValExternal == 1 && existing.Process.ValRtstatus != process.ValRtstatus)
                    {
                        existing.Process.ValRtstatus = process.ValRtstatus;
                    }
                }
                else
                {
                    GenioExecutableJob job = _jobFinder.ObtainJob(process);
                    job.FillArguments(sp, user, process);
                    job.SetPartitionPolicies();
                    works.Add(new GenioWork(process, job));
                }
            }

            var executing = works.Where(w => SchedulerBroker.IsExecuting(w.Process));
            var notExecuting = works.Where(w => !SchedulerBroker.IsExecuting(w.Process)).ToList();
            notExecuting.Sort(HighestPriority);

            works = executing.Union(notExecuting).ToList();
        }
    }

    class InvalidProcessException : BusinessException
    {
        public InvalidProcessException(string message, string localErro, string causaErro)
            : base(message, localErro, causaErro)
        {
        }
    }

    public enum TimeUnit
    {
        Seconds,
        Minutes,
        Hours
    }

    public static class MonitorUtils
    {
        public static string GetTimeUnitAsString(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return "segundos";
                case TimeUnit.Minutes:
                    return "minutos";
                case TimeUnit.Hours:
                    return "horas";
            }
            return string.Empty;
        }

        public static bool CompareTimeDiff(TimeSpan span, double val, TimeUnit unit)
        {
            bool result = false;

            switch (unit)
            {
                case TimeUnit.Seconds:
                    result = span.TotalSeconds > val;
                    break;
                case TimeUnit.Minutes:
                    result = span.TotalMinutes > val;
                    break;
                case TimeUnit.Hours:
                    result = span.TotalHours > val;
                    break;
                default:
                    break;
            }

            return result;
        }

        public static double GetUnitTimeSpan(TimeUnit unit, TimeSpan span)
        {
            switch (unit)
            {
                case TimeUnit.Seconds:
                    return span.TotalSeconds;
                case TimeUnit.Minutes:
                    return span.TotalMinutes;
                case TimeUnit.Hours:
                    return span.TotalHours;
            }
            return span.Minutes;
        }

        public static TimeUnit GetTimeUnit(string unit)
        {
            switch (unit)
            {
                case "H":
                    return TimeUnit.Hours;
                case "M":
                    return TimeUnit.Minutes;
                case "S":
                    return TimeUnit.Seconds;
                default:
                    return TimeUnit.Seconds;
            }
        }
    }


}