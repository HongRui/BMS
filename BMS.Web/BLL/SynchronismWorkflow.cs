using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OUDAL;
using Quartz;
using Quartz.Impl;
using Common.Logging;
namespace BMS.Web.BLL.SynchronismWorkflow
{
    public class DepartUserView
    {
        public int DepartmentId;
        public int UserId;
 
    }
    public class SynchronismWorkflow
    {  
        public string  Import()
        {
            return "";

        }
    }
    public class SynchronishJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            SynchronismWorkflow sw = new SynchronismWorkflow();
            string result = sw.Import();
            LogHelper.LogHelper.Info(result);
        }
        public static void Schedule()
        {
            ISchedulerFactory factory = new StdSchedulerFactory();
            IScheduler schedule = factory.GetScheduler();
            schedule.Start();
            JobDetail job = new JobDetail("SynochronishOU", typeof(SynchronishJob));

            Trigger trigger = TriggerUtils.MakeHourlyTrigger(2);
            trigger.StartTimeUtc = DateTime.UtcNow;
            trigger.Name = "Syn";
            schedule.ScheduleJob(job, trigger);
        }
    }
   
}
