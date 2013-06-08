using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    public class AssetUser
    {
        public int Id { get; set; }
        [Required]
        public int AssetId { get; set; }
        
        public int? UserId { get; set; }
        public int DepartmentId { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("使用场所")]
        [MaxLength(50)]
        public string Location { get; set; }
        [DisplayName("领用时间")]
        public DateTime BeginDate { get; set; }
        [DisplayName("交还时间")]
        public DateTime? EndDate { get; set; }
        [DisplayName("使用类型")]
        [MaxLength(50)]
        public string UseType { get; set; }
    }
    
}
