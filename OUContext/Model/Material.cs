using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    public class Material
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("商场")]
        public int MallId { get; set; }
        [DisplayName("保管部门")]
        public int DepartmentId { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required]
        [DisplayName("类别")]
        public int CatalogId { get; set; }
        [Required]
        [MaxLength(50)]
        [DisplayName("名称")]
        public string Name { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("品牌/规格")]
        [MaxLength(50)]
        public string Spec { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("备注")]
        [MaxLength(500)]
        public string Remark { get; set; }

    }   
}