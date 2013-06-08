using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OUDAL;
using Quartz;
using Quartz.Impl;
using Dapper;
namespace BMS.Web.BLL
{
    public class UserSyn
    {
        public string Syn()
        {
            OUContext db=new OUContext();
            var users = (from o in db.SystemUsers select o).ToList();
            string sqlUsers = @" select a.nvarchar8 as company ,a.nvarchar21 as department ,a.nvarchar15 as subdepartment ,a.nvarchar7 as email,a.nvarchar6 as name ,nvarchar20 as loginname,nvarchar10 as title
from AllUserData a where a.tp_ListId ='10236E7F-298D-4247-BEE3-1FEB8738784D' and nvarchar20 is not null and a.nvarchar8 is not null";
            System.Data.SqlClient.SqlConnection conn = SqlHelper.Conn();
            conn.Open();
            var OaUsers = conn.Query(sqlUsers);
            foreach (var oaUser in OaUsers)
            {
                bool found = false;
                string loginname = oaUser.loginname;
                foreach (var systemUser in users)
                {
                    if(loginname==systemUser.WorkNO)
                    {
                        found = true;
                        systemUser.Email = oaUser.email;
                        systemUser.Name = oaUser.name;
                    }
                }
                if(found==false)
                {
                    db.SystemUsers.Add(new SystemUser
                                           {
                                               Email = oaUser.email,
                                               Name = oaUser.name,
                                               Password = "11",
                                               WorkNO = loginname
                                           });
                }
                db.SaveChanges();
            }
            //找不到的账号禁用
            users.ForEach(o =>
            {
                if (o.Id > 1)
                {
                    if ((from u in OaUsers where u.loginname == o.WorkNO select o.Id).ToList().Count == 0)
                    {
                        o.State = 1;
                    }
                }
            });
            db.SaveChanges();
            var departments = (from o in db.Departments select o).ToList();
            
            var sqlDepart =@"select a.nvarchar1 as company ,a.nvarchar3 as department ,a.nvarchar4 as subdepartment ,u1.tp_Title as submanager,u2.tp_Title as manager 
from AllUserData a
join UserInfo u1 on a.int1=u1.tp_deleted --and u1.tp_Login like 'glcp%'
join UserInfo u2 on a.int2=u2.tp_deleted --and u2.tp_Login like 'glcp%'
where a.tp_ListId ='40394CC4-40A8-4B8F-BF08-068DB9E90FB7' ";
            var OaDeparts = conn.Query(sqlUsers);
            var q1 = (from o in OaDeparts select o.company).Distinct();
            //公司一级
            foreach (string o in q1)
            {
                bool found = false;
                foreach (var department in departments)
                {
                    if(department.Name==o)
                    {
                        found = true;
                    }
                }
                if(!found&&(!string.IsNullOrEmpty(o)))
                {
                    db.Departments.Add(new Department {DepartmentType = "公司", Name = o, PId = 1});
                }
            }
            db.SaveChanges();
            //部门一级
            departments = (from o in db.Departments select o).ToList();
            var q2 = (from o in OaDeparts select new {company = o.company, department = o.department}).Distinct();
            foreach (var o in q2)
            {
                bool found = false;
                Department parent = (from d in departments where d.Name == o.company select d).FirstOrDefault();
                if(parent!=null)
                {
                    foreach (var department in departments)
                    {
                        if(department.Name==o.department && department.PId==parent.Id)
                        {
                            found = true;
                        }
                    }
                }
               
                if(!found)
                {
                    
                    db.Departments.Add(new Department {DepartmentType = "部门", Name = o.department, PId = parent.Id});
                }
            }
            db.SaveChanges();
            //子部门一级
            departments = (from o in db.Departments select o).ToList();
            var q3 =
                (from o in OaDeparts
                 select new {company = o.company, department = o.department, subdepartment = o.subdepartment}).Distinct();
            foreach (var o in q3)
            {
                bool found = false;
                Department company = (from d in departments where d.Name == o.company select d).First();
                Department department =
                    (from d in departments where d.Name == o.department && d.PId == company.Id select d).First();
                foreach (var dept in departments)
                {
                    if(dept.Name==o.subdepartment )//-- 子部门名称唯一 && dept.PId==department.Id 
                    {
                        found = true;
                    }
                    
                }
                if(!found)
                {
                    db.Departments.Add(new Department
                                           {DepartmentType = "部门", Name = o.subdepartment, PId = department.Id});
                }
            }
            db.SaveChanges();
            //部门用户
            departments = (from o in db.Departments select o).ToList();
            users = (from o in db.SystemUsers select o).ToList();
            var departUsers = (from o in db.DepartmentUsers select o).ToList();
            foreach (var oaUser in OaUsers)
            {
                var dept = (from o in departments where o.Name == oaUser.subdepartment select o).First();
                var user= (from o in users where o.WorkNO==oaUser.loginname select o).First();
                var du =
                    (from o in departUsers where o.DepartmentId == dept.Id && o.UserId == user.Id select o).
                        FirstOrDefault();
                if(du==null)
                {
                    du=new DepartmentUser{DepartmentId = dept.Id,UserId = user.Id };
                    db.DepartmentUsers.Add(du);
                }
                if (oaUser.title != null && ((oaUser.title as string).Contains("经理") || (oaUser.title as string).Contains("领导")))
                {
                    du.IsManager = 1;
                }
                else
                {
                    du.IsManager = 0;
                }
                var delList =
                    (from o in departUsers where o.DepartmentId != dept.Id && o.UserId == user.Id select o).ToList();
                delList.ForEach(o=>db.DepartmentUsers.Remove(o));
            }
            db.SaveChanges();
            return "OK";
        }
    }
    public class SynchronishJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            UserSyn syn=new UserSyn();
            string result = syn.Syn();
            LogHelper.LogHelper.Info(result);
        }
        public static void Schedule()
        {
            ISchedulerFactory factory = new StdSchedulerFactory();
            IScheduler schedule = factory.GetScheduler();
            schedule.Start();
            JobDetail job = new JobDetail("SynUser", typeof(SynchronishJob));

            Trigger trigger = TriggerUtils.MakeDailyTrigger(12, 02);
            trigger.StartTimeUtc = DateTime.UtcNow;
            trigger.Name = "Syn1";
            schedule.ScheduleJob(job, trigger);

        }
    }
}