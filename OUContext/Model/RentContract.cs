using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    public enum RentPriceTypeEnum{日,月,抽成}
    public enum RentContractStateEnum{意向合同,正式合同,终止合同}
    //转公司功能暂无
    public class RentContract
    {
        public static string LogClass = "租赁合同";
        public int Id { get; set; }
        public RoomTypeEnum RoomType { get; set; }
        [DisplayName("客户")]
        public int ClientId { get; set; }
        [DisplayName("状态")]
        public RentContractStateEnum State{ get; set; }
        [MaxLength(50)]
        [DisplayName("序号")]
        public string SerialNum { get; set; }
        [Required]
        [MaxLength(50)]
        [DisplayName("合同号")]
        public string ContractCode { get; set; }
       

        [DisplayName("租金计算")]
        public RentPriceTypeEnum RentPriceType { get; set; }
        [DisplayName("租金")]
        public decimal RentPrice { get; set; }
        [DisplayName("起租日期")]
        public DateTime RentBeginDate { get; set; }
        [DisplayName("到期日期")]
        public DateTime RentEndDate { get; set; }
        [DisplayName("租赁保证金")]
        public decimal Deposit { get; set; }
        [DisplayName("免租天数")]
        public int FreeDays { get; set; }
        [DisplayName("账单开始日")]
        public int BillDay { get; set; }
        [DisplayName("账单月份")]
        public int BillMonth { get; set; }
        [DisplayName("备注")]
        public string Remark { get; set; }

    }   
}