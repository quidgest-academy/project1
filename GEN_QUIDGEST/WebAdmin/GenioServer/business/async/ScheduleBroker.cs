using System;
using System.Collections.Generic;
using CSGenio.framework;
using CSGenio.persistence;
using System.Linq;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business.async
{
    using Process = CSGenioAs_apr; 
    /// <summary>
    /// Class responsible for distributing jobs to every worker.
    /// It's a singleton class, only one instance can exist at a time.
    /// Be careful, every public method must deal with possible concurrency.
    /// </summary>
    public class SchedulerBroker
    {

        private SchedulerBroker(JobFinder jobFinder)
        {
            scheduler = new GenioScheduler(jobFinder);
            lastCheck = new Dictionary<string, DateTime>();
            process = new List<Process>();
        }

        public static void SetupBroker(JobFinder jobFinder)
        {
            instance = new SchedulerBroker(jobFinder);
        }

        /// <summary>
        /// 
        /// </summary>
        private List<Process> process = new List<Process>(); //Beware, not all fields have data, check ObtainProcess()
        
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, List<Process>> allProcess = new Dictionary<string, List<Process>>();
        
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, DateTime> lastCheck;

        /// <summary>
        /// 
        /// </summary>
        private Object lockProcess = new Object();

        private GenioScheduler scheduler;

        /// <summary>
        /// Finds the next process that can be executed and returns the corresponding job.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>A job to be executed or null if no work can be executed</returns>
        public IGenioWork GetWork(User user)
        {
            PersistentSupport sp = null;
            try
            {
                sp = PersistentSupport.getPersistentSupport(user.Year);
                sp.openTransaction();

                //If the scheduler is not working, no one gets new work.
                //CSGenioAglob glob = GlobalFunctions.SearchListUnique<CSGenioAglob>(sp, null, user, false);                
                //CSGenioAglob glob = null;

                lock (lockProcess)
                {
                    process = GetProcess(sp, user);
                                        
                    //if (glob.ValDesbloqu == 1)
                    KillUnresponsive(user);

                    if (CanWork() == false)
                    {
                        //If there is no work to be done, return null
                        return null;
                    }

                    GenioWork mostUrgent = scheduler.GetWork(process, sp, user);
                    if (mostUrgent != null)
                    {
                        //Se conseguirmos marcar o processo retornamos
                        GenioProcessManager manager = GenioProcessManager.PersistProcessManager(user);
                        if (manager.AllocateProcess(mostUrgent.Process))
                        {
                            return mostUrgent;
                        }
                        else
                        {
                            //Someone changed the state of the process being allocated.  We will simply return null;
                        }
                    }
                    sp.closeTransaction();
                    return null;
                }

            }
            catch (InvalidProcessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                sp.rollbackTransaction();
                //Se houver problemas damos exceção
                string message = Translations.Get("MSG_ERROR_OBTAIN_NEXT_PROCESS", user.Language);
                throw new BusinessException(message, "Scheduler.GetWork", ex.Message);
            }
            finally
            {
                sp.closeTransaction();
            }
        }

        private void KillUnresponsive(User user)
        {
            //var unit = MonitorUtils.GetTimeUnit(glob.ValDesblqun);
            var unit = MonitorUtils.GetTimeUnit("S");
            var manager = GenioProcessManager.PersistProcessManager(user);
                       
            //double val = glob.ValDesblqpr;
            double val = 120;
            if (Configuration.ExistsProperty("inactivitytime"))
                val = Convert.ToDouble(Configuration.GetProperty("inactivitytime"));

            foreach (var proc in process.Where(NotResponding))
            {
                var passedTime = DateTime.Now - proc.ValLastupdt;
                bool overtime = MonitorUtils.CompareTimeDiff(passedTime, val, unit);

                if (overtime)
                {
                    string msg = Translations.Get("MSG_ABORT_PROCESS_AUTO", user.Language);
                    int t = (int)MonitorUtils.GetUnitTimeSpan(unit, passedTime);
                    msg = string.Format(msg, t, MonitorUtils.GetTimeUnitAsString(unit));
                    //proc.ValUnblock = 1;
                    manager.AbortProcess(proc, msg);
                    manager.NotifyProcess(proc);
                }
            }
        }


        //Checks if the scheduler is turned on.
        private bool Shutdown()
        {
            string sched = Environment.GetEnvironmentVariable("Schedulerswith");
            if (sched == null)
                return false;

            return sched.Equals("off");
            //return glob.ValSchedoff == 1;
        }

        /// <summary>
        /// Validate if it is possible to continue work
        /// </summary>
        /// <param name="glob"></param>
        /// <returns></returns>
        /// private bool CanWork(CSGenioAglob glob)
        private bool CanWork()
        {
            //string concurrencyType = Environment.GetEnvironmentVariable("concurrencytype");
            string concurrencyType = null;

            if (Configuration.ExistsProperty("concurrencytype"))
                concurrencyType = Configuration.GetProperty("concurrencytype");
            
            if (concurrencyType != null)
            {
                if(concurrencyType == "L")
                {
                    int convertedMaxProc = 1;
                    //string maxprocess = Environment.GetEnvironmentVariable("maxprocess");
                    if (Configuration.ExistsProperty("maxprocess"))
                        convertedMaxProc = Conversion.string2Int(Configuration.GetProperty("maxprocess"));

                    int numProcessos = process.Count(IsExecuting);
                    if (numProcessos >= convertedMaxProc)
                        return false;
                    else
                        return true;
                }
                else if (concurrencyType == "I")
                {
                    return true;
                }
            }
            else
            {
                //Por defeito faz o caso de 'Apenas 1'
                return !process.Exists(IsExecuting);
            }

            //if (glob.ValConcorre == ArrayAconcorr.E_L_2)
            //{
            //    int numProcessos = process.Count(EmExecucao);
            //    if (numProcessos >= glob.ValMax_proc)
            //        return false;
            //    else
            //        return true;
            //}
            //else if (glob.ValConcorre == ArrayAconcorr.E_I_3)
            //{
            //    return true;
            //}
            //else
            //{
            //    //Por defeito faz o caso de 'Apenas 1'
            //    return !process.Exists(EmExecucao);
            //}
            return true;
        }

        /// <summary>
        /// Get the list of all valid/available process for execution
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private List<Process> GetProcess(PersistentSupport sp, User user)
        {
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500); // 0.5 seconds
            if (!lastCheck.ContainsKey(user.Year))
                lastCheck[user.Year] = DateTime.MinValue;

            if (DateTime.Now - lastCheck[user.Year] > duration)
            {
                List<Process> results = Process.searchList(sp, user,
                    CriteriaSet.And()
                        .Equal(Process.FldFinished, 0)
                        .Equal(Process.FldZzstate, 0)
                        .SubSet(CriteriaSet.NotAnd()
                            .Equal(Process.FldRtstatus, ArrayS_prstat.E_AC_8)
                            .Equal(Process.FldRtstatus, ArrayS_prstat.E_NR_6))
                        //Não count com processos em fila de espera que estão em manutenção 
                        //(os em execução têm de ser considerados to a concorrência)
                        //.SubSet(CriteriaSet.NotAnd()
                        //    .Equal(Process.FldStatus, ArrayS_prstat.E_FE_2)
                            //.Exists(new SelectQuery()
                            //    .Select(new SqlValue(1), "exists")
                            //    .From(CSGenioAprman.AreaPRMAN)
                            //    .Where(CriteriaSet.And()
                            //        .Equal(CSGenioAprman.FldTipoproc, CSGenioAlogp0.FldTipoproc)
                            //        .In(CSGenioAprman.FldModomanu, new string[] { ArrayAmanproc.E_E_2, ArrayAmanproc.E_AE_3 }))
                            //        )
                        //)
                    );

                lastCheck[user.Year] = DateTime.Now;
                allProcess[user.Year] = results;
                return results;
            }
            else
            {
                //Se não passou time suficente retornamos o que está em memoria.
                return allProcess[user.Year];
            }
        }

        private static bool IsWaiting(Process processo)
        {
            return processo.ValRtstatus == ArrayS_prstat.E_FE_2;
        }

        public static bool IsExecuting(Process processo)
        {
            return processo.ValRtstatus == ArrayS_prstat.E_AG_3 ||
                processo.ValRtstatus == ArrayS_prstat.E_EE_1 ||
                processo.ValRtstatus == ArrayS_prstat.E_AC_8;
        }

        private static bool NotResponding(Process processo)
        {
            return processo.ValRtstatus == ArrayS_prstat.E_NR_6;
        }

        /* Esta classe é Singleton, só pode haver uma instancia! */
        private static SchedulerBroker instance = null;
        private SchedulerBroker()
        {
            lastCheck = new Dictionary<string, DateTime>();
            process = new List<Process>();
        }

        public static SchedulerBroker GetBroker()
        {
            lock(typeof(SchedulerBroker))
            {
                if (instance == null)
                    instance = new SchedulerBroker(new ReflectionJobFinder());
                return instance;
            }            
        }

        /// <summary>
        /// Indicates that a process has terminated successfully.
        /// </summary>
        /// <param name="processo">The terminated process</param>
        public void TerminatedProcess(Process processo)
        {
            lock (lockProcess)
            {
                //Finds a process in the cache and updates the state. No need to go to the DB.
                process.RemoveAll(x => x.ValCodascpr == processo.ValCodascpr);
            }
        }
    }
}