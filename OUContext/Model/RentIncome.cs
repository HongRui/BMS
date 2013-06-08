using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    public class RentIncome
    {
        public static string LogClass = "租金收款";
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int RentBillId { get; set; }
        [DisplayName("收款公司")]
        public int CompanyId { get; set; }
        [DisplayName("租金")]
        public decimal Money { get; set; }
        [DisplayName("收款日期")]
        public DateTime PayDate { get; set; }
        [DisplayName("备注")]
        public string Remark { get; set; }

    }   
}