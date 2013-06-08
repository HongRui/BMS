using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    public class RentContractRoom
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        [DisplayName("房屋")]
        public int RoomId { get; set; }
        [DisplayName("产证面积")]
        public decimal Dim { get; set; }
        [DisplayName("租赁面积")]
        public decimal RentDim { get; set; }
        [DisplayName("租金")]
        public decimal RentPrice { get; set; }
        [DisplayName("起租日期")]
        public DateTime RentBeginDate { get; set; }
        [DisplayName("到期日期")]
        public DateTime RentEndDate { get; set; }
        [DisplayName("备注")]
        public string Remark { get; set; }

    }
    public class ViewRentContractRoom
    {
        public static string sql = @"select rent.id,rent.dim,rent.rentdim,rent.rentprice,rent.remark,r.id as roomid,b.name+' '+r.name  as name  from rooms r join buildings b on b.id=r.buildingid
join rentcontractrooms rent on rent.roomid=r.id  
";
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string Name { get; set; }
        public decimal Dim { get; set; }
        public decimal RentDim { get; set; }
        public string Remark { get; set; }
        public decimal RentPrice { get; set; }
        public bool IsRemoved { get; set; }
    }
}