using System;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;
using ThoughtWorks.QRCode.Codec;
namespace BMS.Web.Controllers
{
    public class QRCodeController : Controller
    {
        //
        // GET: /QRCode/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Get()
        {
            HttpContextBase context = this.HttpContext;
            string str = context.Request.ServerVariables["HTTP_REFERER"];
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;

            qrCodeEncoder.QRCodeScale = 4;

            qrCodeEncoder.QRCodeVersion = 7;

            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

            System.Drawing.Bitmap bmp;
            String data = str;
            bmp = qrCodeEncoder.Encode(data);


            MemoryStream ms = new MemoryStream();
            try
            {
                bmp.Save(ms, ImageFormat.Png);
            }
            finally
            {
                //显式释放资源 
                bmp.Dispose();
                ms.Dispose();
            }
            byte[] bytes = ms.ToArray();
            return File(bytes, @"image/png");
        }
        public ActionResult GetQRCode(string str)
        {
            HttpContextBase context = this.HttpContext;
            if(string.IsNullOrEmpty(str))str = context.Request.ServerVariables["HTTP_REFERER"];
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;

            qrCodeEncoder.QRCodeScale = 4;

            qrCodeEncoder.QRCodeVersion = 7;

            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

            System.Drawing.Bitmap bmp;
            String data = str;
            bmp = qrCodeEncoder.Encode(data);


            MemoryStream ms = new MemoryStream();
            try
            {
                bmp.Save(ms, ImageFormat.Png);
            }
            finally
            {
                //显式释放资源 
                bmp.Dispose();
                ms.Dispose();
            }
            byte[] bytes = ms.ToArray();
            return File(bytes, @"image/png");
        }

    }
}
