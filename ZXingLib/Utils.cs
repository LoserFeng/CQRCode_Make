using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
namespace ColorZXing
{
    public class Utils
    {
        public static int GetGrayScale(int r, int g, int b)
        {
            return (r + g + b) / 3;
        }

        public static int GetGrayScale(Color color)
        {
            return (color.R + color.G + color.B) / 3;
        }

        public static bool IsDarkColor(Color color)
        {
            return GetGrayScale(color) < 128;
        }

        //功能：保存文件
        //filePath：文件名
        public static void WriteBitMap(Bitmap bitmap, string filePath, ImageFormat format)
        {
            using (var fs = File.Create(filePath))
            {
                bitmap.Save(fs, format);
            }
        }

        public static Bitmap ReadBitMap(string filePath)
        {
            var bitmap = new Bitmap(filePath);
            return bitmap;
        }

        public static Bitmap CreateBitmap(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return new Bitmap(ms);
            }
        }

        public static Bitmap DownloadBitmap(Uri url)
        {
            var webClient = new WebClient();
            var bytes = webClient.DownloadData(url.AbsoluteUri);
            return CreateBitmap(bytes);
        }


        public static byte[] GetFileBytes(String filePath)
        {

            byte[] byteArray = null;
            if (File.Exists(filePath))
                byteArray = File.ReadAllBytes(filePath);
            else
            {
                Console.WriteLine("file not exist!");
                return null;
            }
            return byteArray;
        }
        public static Bitmap GetLocalBitmap(String filePath)
        {

            byte[] bytes = GetFileBytes(filePath);
            if(bytes == null)
            {
                return null;
            }else
            {
                return CreateBitmap(bytes);
            }

        }
    }
}
