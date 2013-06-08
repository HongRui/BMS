using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    public class MaterialInItem
    {
        public int Id { get; set; }
        
        [DisplayName("物资")]
        public int MaterialId { get; set; }

        public int InId { get; set; }
        [DisplayName("单价")]
        public decimal UnitPrice { get; set; }
        [DisplayName("数量")]
        public decimal Num { get; set; }
    }   
}