using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
   
    public class MaterialOut
    {
        public int Id { get; set; }
        
        public int MallId { get; set; }
        [Required]
        [DisplayName("单号")]
        public string Code { get; set; }
        [DisplayName("领用部门")]
        public int DepartmentId { get; set; }
        [DisplayName("领用人")]
        public int PersonId { get; set; }
        [Required]
        [DisplayName("出库日期")]
        public DateTime OutDate { get; set; }
        
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("备注")]
        [MaxLength(500)]
        public string Remark { get; set; }

    }   
}