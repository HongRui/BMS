using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    
    public class AssetMaintain
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        [DisplayName("负责人")]
        public int? PersonId { get; set; }
        [DisplayName("计划日期")]
        public DateTime? PlanDate { get; set; }
        [DisplayName("执行日期")]
        public DateTime? DoneDate { get; set; }
        [MaxLength(100)]
        [DisplayName("保养计划说明")]
        public string PlanDetail { get; set; }
        [MaxLength(100)]
        [DisplayName("保养结果说明")]
        public string DoneDetail { get; set; }
    }   
}