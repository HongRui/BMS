using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    public enum SupplierState { 潜在,往来, 黑名单 }  //{ 在用, 报废,售出 }
    public enum SupplierLevel { A, B, C, D, E, F }
    public enum SupplierType{生产商,代理商,经销商}
    public enum SupplierProductLevel{高端,中端,低端}
    public class Supplier
    {
        public static string LogClass = "供应商";
        public int Id { get; set; }
        [DisplayName("状态")]
        public int State { get; set; }
        [DisplayName("等级")]
        public int Level { get; set; }
        [DisplayName("供应商类型")]
        public int Type { get; set; }
        [Required]
        [MaxLength(50)]
        [DisplayName("简称/登录名")]
        public string Name { get; set; }
        [Required]
        [DisplayName("全称")]
        [MaxLength(100)]
        public string FullName { get; set; }
        
        [DisplayName("集团名称")]
        [MaxLength(100)]
        public string GroupName { get; set; }
        [Required]
        [DisplayName("营业执照注册号")]
        [MaxLength(100)]
        public string Licence { get; set; }
        [Required]
        [DisplayName("税务登记号")]
        [MaxLength(100)]
        public string TaxNo { get; set; }
        [Required]
        [DisplayName("企业性质")]
        [MaxLength(100)]
        public string QYXZ { get; set; }
        [DisplayName("合作渠道")]
        [MaxLength(50)]
        public string QuDao { get; set; }
        [DisplayName("合作渠道说明")]
        [MaxLength(100)]
        public string QuDaoOther { get; set; }
        [Required]
        [DisplayName("组织代码号")]
        [MaxLength(100)]
        public string OrgNo { get; set; }
        [Required]
        [DisplayName("开户银行")]
        [MaxLength(100)]
        public string Bank { get; set; }
        [Required]
        [DisplayName("银行账号")]
        [MaxLength(100)]
        public string BankAccount { get; set; }
        [Required]
        [DisplayName("法人代表")]
        [MaxLength(100)]
        public string LegalPerson { get; set; }
        [Required]
        [DisplayName("注册资金")]
        [MaxLength(100)]
        public string RegCapital { get; set; }
        [Required]
        [DisplayName("注册地址")]
        [MaxLength(100)]
        public string RegAddress { get; set; }
        [Required]
        [DisplayName("注册日期")]
        public DateTime RegDate { get; set; }
        [Required]
        [DisplayName("经营地址")]
        [MaxLength(100)]
        public string Address { get; set; }
        [Required]
        [DisplayName("邮政编码")]
        [MaxLength(100)]
        public string Zip { get; set; }
        [DisplayName("企业网址")]
        [MaxLength(100)]
        public string Website { get; set; }
        [Required]
        [DisplayName("总机")]
        [MaxLength(100)]
        public string Phone { get; set; }
        //以下两个字段今后要转移到子表中
        [Required]
        [DisplayName("联系人")]
        [MaxLength(100)]
        public string Contact { get; set; }
        [Required]
        [DisplayName("职务")]
        [MaxLength(100)]
        public string ContactDute { get; set; }
        [Required]
        [DisplayName("联系电话")]
        [MaxLength(100)]
        public string ContactPhone { get; set; }
        [DisplayName("传真")]
        [MaxLength(100)]
        public string ContactFax { get; set; }
        [Required]
        [DisplayName("手机")]
        [MaxLength(100)]
        public string ContactMobile { get; set; }
        [DisplayName("电子邮件")]
        [MaxLength(100)]
        public string ContactEmail { get; set; }

        [DisplayName("品牌")]
        [MaxLength(100)]
        public string Brand { get; set; }
        [DisplayName("注册商标")]
        [MaxLength(100)]
        public string Trademark { get; set; }
        [DisplayName("销售渠道覆盖地区(省会城市)")]
        public string SaleZone { get; set; }
        [DisplayName("产品/服务")]
        public string Product { get; set; }
        [DisplayName("产品定位")]
        public int ProductLevel { get; set; }
        [DisplayName("代表客户及项目")]
        public string  Customer{ get; set; }
        [DisplayName("与我司往来情况")]
        public string LvDiProject { get; set; }
        [DisplayName("企业简介")]
        public string Remark { get; set; }

        [DisplayName("供应商分类")]
        [MaxLength(50)]
        public string Catalog { get; set; }
        [DisplayName("供应商编号")]
        [MaxLength(50)]
        public string Code { get; set; }
        [DisplayName("审批结果")]
        public string AuditRemark { get; set; }
        [DisplayName("登录密码")]
        [Required]
        [MaxLength(50)]
        public string Password { get; set; }
        /// <summary>
        /// 给没有权限者一个查看密码
        /// </summary>
        [DisplayName("操作密码")]
        [MaxLength(50)]
        public string AccessCode { get; set; }
        /// <summary>
        /// 添加accesscode
        /// </summary>
        /// <param name="db"></param>
        public void Save(OUContext db)
        {
            if(AccessCode==null)
            {
                Random random=new Random();
                byte[] bytes = new byte[30];
                StringBuilder sb=new StringBuilder();
                for(int i=0;i<30;i++)
                {
                    bytes[i] = (byte) random.Next(97, 122);
                }
                AccessCode = Encoding.Default.GetString(bytes);
            }
            if (Id == 0) db.Suppliers.Add(this);
        }
        public static string GetNameById(int id)
        {
            if (id == 0) return "";
            OUContext db = new OUContext();
            Supplier s = db.Suppliers.Find(id);
            if (s != null) return s.Name;
            else return "";
        }
       
    }  
    
}