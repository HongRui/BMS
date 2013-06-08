using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    public enum AssetState { 在用,报废, 售出 }  //{ 在用, 报废,售出 }
    public enum AssetType{自有资产,托管资产}
    public class Asset
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("商场")]
        public int MallId { get; set; }
        [Required]
        [DisplayName("类别")]
        public int CatalogId { get; set; }
        
        [Required]
        [MaxLength(50)]
        [DisplayName("资产名称")]
        public string Name { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("序列号")]
        [MaxLength(100)]
        public string SN { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("供应商")]
        [MaxLength(50)]
        public string Supplier { get; set; }
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("资产编号")]
        [MaxLength(100)]
        public string AssetNo { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("NC编号")]
        [MaxLength(50)]
        public string NCNo { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("品牌/规格/型号")]
        [MaxLength(50)]
        public string Spec { get; set; }
        [DisplayName("发票金额")]
        public decimal Price { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("发票号码")]
        [MaxLength(50)]
        public string ReceiptNo { get; set; }
        [DisplayName("购买日期")]
        public DateTime? BuyDate { get; set; }
        [DisplayName("状态")]
        public int State { get; set; }
        [DisplayName("资产类型")]
        public int AssetType { get; set; }
        [DisplayName("故障")]
        public int IsBad { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [DisplayName("备注")]
        [MaxLength(500)]
        public string Remark { get; set; }

        [DisplayName("设备信息")]
        public string DeviceInfo { get; set; }
        [DisplayName("保修截止日期")]
        public DateTime? GuaranteeDate { get; set; }

        public static string LogClass = "设施设备";
    }   
}