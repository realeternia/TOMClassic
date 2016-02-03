using System;
using System.Drawing;
using NarlonLib.Math;

namespace NarlonLib.Drawing
{
    public class ImagePixelTool
    {
        public static void Effect(Bitmap source, ImagePixelEffects effect, int level)
        {
            int imageWidth = source.Width;
            int imageHeight = source.Height;

            Rectangle rect = new Rectangle(0, 0, imageWidth, imageHeight);
            System.Drawing.Imaging.BitmapData bmpData = source.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, source.PixelFormat);
            IntPtr ptr = bmpData.Scan0;

            int count = imageWidth * imageHeight * 4;
            byte[] pixelValues = new byte[count];
            System.Runtime.InteropServices.Marshal.Copy(ptr, pixelValues, 0, count);

            switch (effect)
            {
                case ImagePixelEffects.Lightness: Lightness(pixelValues, imageWidth, imageHeight, level); break;
                case ImagePixelEffects.Darkness: Darkness(pixelValues, imageWidth, imageHeight, level); break;
                case ImagePixelEffects.Gray: Gray(pixelValues, imageWidth, imageHeight, level); break;
                case ImagePixelEffects.Redden: Redden(pixelValues, imageWidth, imageHeight, level); break;
                case ImagePixelEffects.Bluen: Bluen(pixelValues, imageWidth, imageHeight, level); break;
                case ImagePixelEffects.Greenen: Greenen(pixelValues, imageWidth, imageHeight, level); break;
                case ImagePixelEffects.Invisible: Invisible(pixelValues, imageWidth, imageHeight, level); break;
                case ImagePixelEffects.Reverse: Reverse(pixelValues, imageWidth, imageHeight, level); break;
                case ImagePixelEffects.RandomSwitch: RandomSwitch(pixelValues, imageWidth, imageHeight, level); break;
                case ImagePixelEffects.RandomRowMove: RandomRowMove(pixelValues, imageWidth, imageHeight, level); break;
                case ImagePixelEffects.RandomColumnMove: RandomColumnMove(pixelValues, imageWidth, imageHeight, level); break;
                case ImagePixelEffects.RandomPoint: RandomPoint(pixelValues, imageWidth, imageHeight, level); break;
                case ImagePixelEffects.Statue: Statue(pixelValues, imageWidth, imageHeight, level); break;
            }

            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, ptr, count);
            source.UnlockBits(bmpData);
        }

        private static void Lightness(byte[] pixelValues, int imageWidth, int imageHeight, int level)
        {
            int index = 0;
            for (int i = 0; i < imageHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    pixelValues[index] = SafeAdd(pixelValues[index], 30 * level); //B
                    index++;
                    pixelValues[index] = SafeAdd(pixelValues[index], 30 * level); //G
                    index++;
                    pixelValues[index] = SafeAdd(pixelValues[index], 30 * level); //R
                    index++;
                    index++; //A
                }
            }
        }

        private static void Darkness(byte[] pixelValues, int imageWidth, int imageHeight, int level)
        {
            int index = 0;
            for (int i = 0; i < imageHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    pixelValues[index] = SafeAdd(pixelValues[index], -30 * level); //B
                    index++;
                    pixelValues[index] = SafeAdd(pixelValues[index], -30 * level); //G
                    index++;
                    pixelValues[index] = SafeAdd(pixelValues[index], -30 * level); //R
                    index++;
                    index++; //A
                }
            }
        }

        private static void Reverse(byte[] pixelValues, int imageWidth, int imageHeight, int level)
        {
            int index = 0;
            for (int i = 0; i < imageHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    pixelValues[index] = (byte) (pixelValues[index] + (255 - pixelValues[index]*2)*level/5);
                    index++;
                    pixelValues[index] = (byte) (pixelValues[index] + (255 - pixelValues[index]*2)*level/5);
                    index++;
                    pixelValues[index] = (byte) (pixelValues[index] + (255 - pixelValues[index]*2)*level/5);
                    index++;
                    index++; //A
                }
            }
        }

        private static void Gray(byte[] pixelValues, int imageWidth, int imageHeight, int level)
        {
            int index = 0;
            for (int i = 0; i < imageHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    int b = pixelValues[index];
                    int g = pixelValues[index + 1];
                    int r = pixelValues[index + 2];
                    int val = (g + b + r)/3;

                    pixelValues[index] = (byte)(pixelValues[index] + (val - pixelValues[index]) * level / 5);
                    index++;
                    pixelValues[index] = (byte)(pixelValues[index] + (val - pixelValues[index]) * level / 5);
                    index++;
                    pixelValues[index] = (byte)(pixelValues[index] + (val - pixelValues[index]) * level / 5);
                    index++;
                    index++; //A
                }
            }
        }

        private static void Redden(byte[] pixelValues, int imageWidth, int imageHeight, int level)
        {
            int index = 0;
            for (int i = 0; i < imageHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    pixelValues[index] = SafeAdd(pixelValues[index], -20 * level); //B
                    index++;
                    pixelValues[index] = SafeAdd(pixelValues[index], -20 * level); //G
                    index++;
                    pixelValues[index] = SafeAdd(pixelValues[index], 30 * level); //R
                    index++;
                    index++; //A
                }
            }
        }

        private static void Greenen(byte[] pixelValues, int imageWidth, int imageHeight, int level)
        {
            int index = 0;
            for (int i = 0; i < imageHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    pixelValues[index] = SafeAdd(pixelValues[index], -20 * level); //B
                    index++;
                    pixelValues[index] = SafeAdd(pixelValues[index], 30 * level); //G
                    index++;
                    pixelValues[index] = SafeAdd(pixelValues[index], -20 * level); //R
                    index++;
                    index++; //A
                }
            }
        }

        private static void Invisible(byte[] pixelValues, int imageWidth, int imageHeight, int level)
        {
            int index = 0;
            for (int i = 0; i < imageHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    index += 3;
                    pixelValues[index] = pixelValues[index] == 0 ? (byte)0 : (byte)(level * 40); //B
                    index++; //A
                }
            }
        }

        private static void Bluen(byte[] pixelValues, int imageWidth, int imageHeight, int level)
        {
            int index = 0;
            for (int i = 0; i < imageHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    pixelValues[index] = SafeAdd(pixelValues[index], 30 * level); //B
                    index++;
                    pixelValues[index] = SafeAdd(pixelValues[index], -20 * level); //G
                    index++;
                    pixelValues[index] = SafeAdd(pixelValues[index], -20 * level); //R
                    index++;
                    index++; //A
                }
            }
        }

        private static void RandomSwitch(byte[] pixelValues, int imageWidth, int imageHeight, int level)
        {
            int index = 0;
            for (int i = 0; i < imageHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    int yoff = i + MathTool.GetRandom(level*3);
                    if (yoff>=imageHeight)
                    {
                        yoff = imageHeight - 1;
                    }
                    int xoff = j + MathTool.GetRandom(level * 3);
                    if (xoff >= imageWidth)
                    {
                        xoff = imageWidth - 1;
                    }
                    int tindex = (yoff*imageWidth + xoff)*4;

                    pixelValues[index] = pixelValues[tindex];
                    index++;
                    pixelValues[index] = pixelValues[tindex+1];
                    index++;
                    pixelValues[index] = pixelValues[tindex+2];
                    index++;
                    pixelValues[index] = pixelValues[tindex + 3];
                    index++; //A
                }
            }
        }

        private static void RandomRowMove(byte[] pixelValues, int imageWidth, int imageHeight, int level)
        {
            for (int i = 0; i < imageHeight; i++)
            {
                int direct = MathTool.GetRandom(2);
                int len = MathTool.GetRandom(level*3);
                if (direct ==0) //右移
                {
                    for (int j = imageWidth - 1; j >=0; j--)
                    {
                        int xoff = j - len;
                        if (xoff < 0)
                        {
                            xoff = 0;
                        }
                        int index = (i * imageWidth + j) * 4;
                        int tindex = (i * imageWidth + xoff) * 4;
                        pixelValues[index] = pixelValues[tindex];
                        pixelValues[index + 1] = pixelValues[tindex + 1];
                        pixelValues[index + 2] = pixelValues[tindex + 2];
                        pixelValues[index + 3] = pixelValues[tindex + 3];
                    }
                }
                else
                {
                    for (int j = 0; j < imageWidth; j++)
                    {
                        int xoff = j + len;
                        if (xoff >= imageWidth)
                        {
                            xoff = imageWidth - 1;
                        }
                        int index = (i * imageWidth + j) * 4;
                        int tindex = (i * imageWidth + xoff) * 4;
                        pixelValues[index] = pixelValues[tindex];
                        pixelValues[index+1] = pixelValues[tindex + 1];
                        pixelValues[index+2] = pixelValues[tindex + 2];
                        pixelValues[index + 3] = pixelValues[tindex + 3];
                    }
                }
            }
        }

        private static void RandomColumnMove(byte[] pixelValues, int imageWidth, int imageHeight, int level)
        {
            for (int i = 0; i < imageWidth; i++)
            {
                int direct = MathTool.GetRandom(2);
                int len = MathTool.GetRandom(level * 3);
                if (direct == 0)
                {
                    for (int j = imageHeight - 1; j >= 0; j--)
                    {
                        int yoff = j - len;
                        if (yoff < 0)
                        {
                            yoff = 0;
                        }
                        int index = (j * imageWidth + i) * 4;
                        int tindex = (yoff * imageWidth + i) * 4;
                        pixelValues[index] = pixelValues[tindex];
                        pixelValues[index + 1] = pixelValues[tindex + 1];
                        pixelValues[index + 2] = pixelValues[tindex + 2];
                        pixelValues[index + 3] = pixelValues[tindex + 3];
                    }
                }
                else
                {
                    for (int j = 0; j < imageHeight; j++)
                    {
                        int yoff = j + len;
                        if (yoff >= imageHeight)
                        {
                            yoff = imageHeight - 1;
                        }
                        int index = (j * imageWidth + i) * 4;
                        int tindex = (yoff * imageWidth + i) * 4;
                        pixelValues[index] = pixelValues[tindex];
                        pixelValues[index + 1] = pixelValues[tindex + 1];
                        pixelValues[index + 2] = pixelValues[tindex + 2];
                        pixelValues[index + 3] = pixelValues[tindex + 3];
                    }
                }
            }
        }

        private static void RandomPoint(byte[] pixelValues, int imageWidth, int imageHeight, int level)
        {
            int index = 0;
            for (int i = 0; i < imageHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    if (MathTool.GetRandom(100) > level*10)
                    {
                        index += 4;
                        continue;
                    }
                    int pv = MathTool.GetRandom(255);
                    pixelValues[index] = (byte)pv; //B
                    index++;
                    pixelValues[index] = (byte)pv; //G
                    index++;
                    pixelValues[index] = (byte)pv; //R
                    index++;
                    index++; //A
                }
            }
        }

        private static void Statue(byte[] pixelValues, int imageWidth, int imageHeight, int level)
        {
            int index = 0;
            for (int i = 0; i < imageHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    int yoff = i + 1;
                    if (yoff >= imageHeight)
                    {
                        yoff = imageHeight - 1;
                    }
                    int xoff = j + 1;
                    if (xoff >= imageWidth)
                    {
                        xoff = imageWidth - 1;
                    }
                    int tindex = (yoff * imageWidth + xoff) * 4;

                    pixelValues[index] = (byte)System.Math.Abs(pixelValues[index] - pixelValues[tindex] * level / 5 + level * 25);
                    index++;
                    pixelValues[index] = (byte)System.Math.Abs(pixelValues[index] - pixelValues[tindex + 1] * level / 5 + level * 25);
                    index++;
                    pixelValues[index] = (byte)System.Math.Abs(pixelValues[index] - pixelValues[tindex + 2] * level / 5 + level * 25);
                    index++;
                    index++; //A
                }
            }
        }

        public static byte SafeAdd(byte value, int add)
        {
            if (value+add>255)
            {
                return 255;
            }
            if (value + add < 0)
            {
                return 0;
            }
            return (byte)(value + add);
        }
    }
}
