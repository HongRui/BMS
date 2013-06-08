using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    public enum MaterialInType {采购,调入,调出,损益}
    public class MaterialIn
    {
        public int Id { get; set; }

        public int MallId { get; set; }
        [Required]
        [DisplayName("单号")]
        public string Code { get; set; }
        [DisplayName("经办人")]
        public int PersonId { get; set; }
        [DisplayName("入库类型")]
        public int InType { get; set; }
        [Required]
        [DisplayName("入库日期")]
        public DateTime InDate { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("供应商")]
        public string Supplier { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("备注")]
        [MaxLength(500)]
        public string Remark { get; set; }
        [DisplayName("入库部门")]
        public int DepartmentId { get; set; }

    }   
}