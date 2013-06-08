using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    public class Client
    {
        public static string LogClass = "客户";
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [DisplayName("简称")]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        [DisplayName("全称")]
        public string FullName { get; set; }
        [DisplayName("联系人")]
        public string Contact { get; set; }
        [DisplayName("联系电话")]
        public string Phone { get; set; }
        [DisplayName("联系地址")]
        public string Address { get; set; }
        [DisplayName("备注")]
        public string Remark { get; set; }

    }   
}