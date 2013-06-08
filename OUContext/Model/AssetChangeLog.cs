using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    public class AssetChangeLog
    {
        public int Id { get; set; }
        [Required]
        public int AssetId { get; set; }        
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("事由")]
        public string Title { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("说明")]
        public string Detail { get; set; }
        [DisplayName("变动时间")]
        public DateTime ChangeDate { get; set; }
    }
}
