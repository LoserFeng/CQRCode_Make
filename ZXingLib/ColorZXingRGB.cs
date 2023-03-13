using System;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing.QrCode;
using System.Runtime.InteropServices;
using static ZXing.RGBLuminanceSource;

namespace ColorZXing
{
    public class ColorZXingRGB
    {
        ///Use the IntPtr method instead of bitmap.GetPixel, it's 6x faster.
        private static void SetBitmap(Bitmap bitmap, byte[] red, byte[] green, byte[] blue)
        {
            // 1.   功能：LockBits方法  将Bitmap锁定到系统内存中，
            //  返回：BitmapData，它包含有关此锁定操作的信息。
            var bmd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            try
            {
                var width = bitmap.Width;
                var height = bitmap.Height;
                //获取或设置 Bitmap 对象的跨距宽度（也称为扫描宽度）
                var stride = bmd.Stride;
                //获取或设置位图中第一个像素数据的地址。相当于在内存中的起始位置
                var scan0 = bmd.Scan0;

                //2.对信息进行编码处理
                //IntPtr Add (IntPtr pointer, int offset);  pointer: 要为其增加偏移量的指针
                //offset： 要增加的偏移量。 返回：新指针  //显示宽度与扫描线宽度的间隙

                for (int y = 0; y < height; y++)
                {
                    var row = IntPtr.Add(scan0, (y * stride));
                    for (int x = 0; x < width; x++)
                    {
                        var imgIndex = IntPtr.Add(row, x * Constants.PixelSize);
                        var index = (y * width + x) * Constants.PixelSize;
                        //Marshal.WriteByte(IntPtr, Byte):将单个字节值写入到非托管内存
                        //      IntPtr：非托管内存中要写入的地址。  Byte：要写入的值。
                       // Marshal.WriteByte(IntPtr.Add(imgIndex, 0), blue[index]);
                       
                        Marshal.WriteByte(IntPtr.Add(imgIndex, 2), red[index]);
                        Marshal.WriteByte(IntPtr.Add(imgIndex, 1), green[index]);
                       

                    }
                }

                for (int i = 0; i < height; i++)
                {
                    var row = IntPtr.Add(scan0, (i * stride));
                    for (int j = 0; j < width; j++)
                    {
                        var imgIndex = IntPtr.Add(row, j * Constants.PixelSize);
                        var index = (i * width + j) * Constants.PixelSize;
                        if (green[index] == 0 && red[index] == 0)
                        {
                            Marshal.WriteByte(IntPtr.Add(imgIndex, 0), (byte)255);
                        }


                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bmd);
            }
        }

        ///Use the IntPtr method instead of bitmap.SetPixel, it's 2x faster.
        private static void GetRGBByteArrayFromBitmap(Bitmap bitmap, byte[] blue, byte[] green, byte[] red)
        {
            var bmd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            var width = bitmap.Width;
            var height = bitmap.Height;
            var stride = bmd.Stride;
            var scan0 = bmd.Scan0;

            for (int y = 0; y < height; y++)
            {
                var row = IntPtr.Add(scan0, (y * stride));
                for (int x = 0; x < width; x++)
                {
                    var imgIndex = IntPtr.Add(row, x * Constants.PixelSize);
                    var index = (y * width + x) * Constants.Gray8PixelSize;

                    blue[index] = Marshal.ReadByte(IntPtr.Add(imgIndex, 0));
                    
                    green[index] = Marshal.ReadByte(IntPtr.Add(imgIndex, 1));
                    red[index] = Marshal.ReadByte(IntPtr.Add(imgIndex, 2));
                }
            };
        }

        /*功能：多参数编码
         *Param：
         *      value：内容
         *      width：长
         *      height：宽
         *      margin：设置二维码边缘留白宽度（值越大留白宽度大，二维码就减小）
         */
        public static Bitmap Encode(string value, int width, int height, int margin)
        {

            int subStringSize = value.Length / 2;
            var str1 = value.Substring(0, subStringSize);
            var str2 = value.Substring(subStringSize, value.Length - subStringSize);


            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                //BarcodeFormat 枚举类型，条码格式
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    //QrCodeEncodingOptions 二维码设置选项，继承于EncodingOptions，主要设置宽，高，编码方式等信息。
                    Height = height,
                    Width = width,
                    Margin = margin
                }
            };

           
            
            var blue = qrCodeWriter.Write(str1);
            var green = qrCodeWriter.Write(str1);
            var red = qrCodeWriter.Write(str2);

            var pixelWidth = green.Width;
            var pixelHeight = green.Height;

            var bitmap = new Bitmap(pixelWidth, pixelHeight, PixelFormat.Format32bppRgb);

            SetBitmap(bitmap, red.Pixels, green.Pixels, blue.Pixels);
            //SetBitmap(bitmap, red.Pixels, green.Pixels, null);
            return bitmap;
        }



        /**
         * 解析函数
         *      1.获取图片的大小，然后生成RGB三个变量
         *      2.
         */
        public static string Decode(Bitmap bitmap)
        {
            var byteSize = bitmap.Width * bitmap.Height * Constants.Gray8PixelSize;

            byte[] blue = new byte[byteSize];
            byte[] green = new byte[byteSize];
            byte[] red = new byte[byteSize];

            GetRGBByteArrayFromBitmap(bitmap, blue, green, red);
            var str1 = ColorZXingBasic.Decode(blue, bitmap.Width, bitmap.Height, BitmapFormat.Gray8);
            Console.WriteLine("blue+" + str1 + "\n");
            var str2 = ColorZXingBasic.Decode(green, bitmap.Width, bitmap.Height, BitmapFormat.Gray8);
            Console.WriteLine("green+" + str2 + "\n");
            var str3 = ColorZXingBasic.Decode(red, bitmap.Width, bitmap.Height, BitmapFormat.Gray8);
            Console.WriteLine("red+" + str3 + "\n");
            return  str2+str3;
        }

        //编码功能
        public static string Decode(byte[] bytes)
        {
            var bitmap = Utils.CreateBitmap(bytes);
            return Decode(bitmap);
        }

        //解码功能
        public static string Decode(Uri url)
        {
            var bitmap = Utils.DownloadBitmap(url);
            return Decode(bitmap);
        }

        public static string Decode(String filePath)
        {
            var bitmap = Utils.GetLocalBitmap(filePath);
            return Decode(bitmap);
        }


        public static Bitmap AddFrame(Image Img, int Margin = 6)
        {
            //位图宽高
            int width = Img.Width + Margin;
            int height = Img.Height + Margin;
            width = (int)((height / 480.0) * 640);  //为了适配640 480的比例
            //System.Console.Write(width);
            int wid_Margin = width - Img.Width;
            Bitmap BitmapResult = new Bitmap(width, height);
            
            Graphics Grp = Graphics.FromImage(BitmapResult);
            SolidBrush b = new SolidBrush(Color.White);//这里修改颜色
            Grp.FillRectangle(b, 0, 0, width, height);
            System.Drawing.Rectangle Rec = new System.Drawing.Rectangle(0, 0, Img.Width, Img.Height);
            //向矩形框内填充Img
            Grp.DrawImage(Img, wid_Margin / 2, Margin / 2, Rec, GraphicsUnit.Pixel);
            //返回位图文件
            Grp.Dispose();
            GC.Collect();
            
            return BitmapResult;
        }
    }
}
