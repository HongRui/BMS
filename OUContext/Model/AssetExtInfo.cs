using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    
    public class AssetExtInfo
    {
        
        public int Id { get; set; }
        [ForeignKey("Id")] 
        public Asset Asset { get; set; }
        [DisplayName("设备信息")]
        public string DeviceInfo { get; set; }
        [DisplayName("保修截止日期")]
        public DateTime GuaranteeDate { get; set; }
        
    }   
}