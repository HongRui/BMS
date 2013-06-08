using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace OUDAL
{
    public enum RoomTypeEnum{房屋,广告位}
    public enum  OwnnerTypeEnum{自有,委托}
    public class Room
    {
        public static string LogClass = "房屋";
        public int Id { get; set; }
        [DisplayName("物业所有权")]
        public OwnnerTypeEnum OwnnerType { get; set; }
        [DisplayName("房屋类型")]
        public RoomTypeEnum RoomType { get; set; }
        [DisplayName("楼栋")]
        public int BuildingId { get; set; }
        [Required]
        [MaxLength(50)]
        [DisplayName("房号")]
        public string Name { get; set; }
        [DisplayName("产证面积")]
        public decimal Dim { get; set; }
        [DisplayName("租赁面积")]
        public decimal RentDim { get; set; }
        [DisplayName("备注")]
        public string Remark { get; set; }

    }   
    public class RoomView
    {
        public static string sql = @"select r.* ,b.name as building ,'aaa' as client from rooms r join buildings b on b.id=r.buildingid
left outer join rentcontractrooms rent on rent.roomid=r.id  
";
//         @"select r.* ,b.name as building ,c.name as client from rooms r join buildings b on b.id=r.buildingid
//left outer join rentcontractrooms rent on rent.roomid=r.id and rent.state=1 and rent.rentbegindate <getdate() left outer join clients c on c.id=rent.clientid 
//";
        public int Id { get; set; }
        public OwnnerTypeEnum OwnnerType { get; set; }
        public RoomTypeEnum RoomType { get; set; }
        public string Building { get; set; }
        public string Name { get; set; }
        public decimal Dim { get; set; }
        public decimal RentDim { get; set; }
        public string Remark { get; set; }
        public string Client { get; set; }
    }
}