using ColorZXing;
using System.Drawing;
using System.Drawing.Printing;
using System;


class program
{


    private static int mode = 0;
    static int Main(string[] args)
    {



        if (mode == 0)
        {


            for(int i = 0; i < 5; i++)
            {
                String filePath = $"./assets/CQRCode_{i}.jpg";
                Console.WriteLine(ColorZXingRGB.Decode(filePath));
            }




        }
        else
        {
            String transmission_data;
            try
            {
                transmission_data = args[0];
                if (transmission_data == null)
                {
                    Console.WriteLine("transmission_data is null");
                    return -1;
                }

                if (transmission_data.Length == 0)
                {
                    Console.WriteLine("you didn't input any words!");
                    return 0;
                }




            }
            catch (Exception e)
            {
                Console.WriteLine("error!please check the arguments");
                return -1;
            }

            transmission_data += "%E";

            int n = transmission_data.Length;
            int t = (n-1) / 28+1;


            for(int i = 0; i < t; i++)
            {
                /*                Console.WriteLine("Please Input the string you want to Encode!");
                Console.Write("Input:");*/
                string Input_Line;
                if (i == t - 1)
                {
                    Input_Line = $"%{i}%";
                    Input_Line += transmission_data.Substring(i * 28, transmission_data.Length - i * 28);

                }
                else
                {
                    Input_Line = $"%{i}%";
                    Input_Line += transmission_data.Substring(i * 28,28);
                    
                }
                // int num=Convert.ToInt32(Console.ReadLine());


                if (Input_Line == null || Input_Line == "")
                {
                    Console.WriteLine("The Input_Line is null");
                }
                else
                {
                    
                    var bitmap = ColorZXingRGB.Encode(Input_Line, 1000, 1000, 0);
                    Image image = bitmap;
                    //image.Dispose();
                    //image.Save("./a.jpg");

                    var res = ColorZXingRGB.AddFrame(image, 50);

                    res.Save($"./assets/CQRCode_{i}.jpg");
                    Console.WriteLine($"The CQR_Code_{i}.jpg has been saved!");
                }
            }


        }

        return 0;

    }
}





/**/



