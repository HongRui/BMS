using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    public class RentBill
    {
        public static string LogClass = "租赁账单";
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int ContractDetailId { get; set; }
        [DisplayName("租金计算")]
        public RentPriceTypeEnum RentPriceType { get; set; }

        [DisplayName("租金")]
        public decimal RentPrice { get; set; }
        [DisplayName("起租日期")]
        public DateTime BillBeginDate { get; set; }
        [DisplayName("到期日期")]
        public DateTime BillEndDate { get; set; }
        [DisplayName("备注")]
        public string Remark { get; set; }

    }   
}