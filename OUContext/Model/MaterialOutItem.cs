using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    public class MaterialOutItem
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public int OutId { get; set; }
        [DisplayName("数量")]
        public decimal Num { get; set; }
    }
    public class MaterialOutItemEdit
    {
        public MaterialOutItem Item { get; set; }
        public string Msg { get; set; }
    }
}