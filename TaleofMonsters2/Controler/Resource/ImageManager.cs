using System.Collections.Generic;
using System.Drawing;
using NarlonLib.Core;
using System;
using TaleofMonsters.Controler.Loader;

namespace TaleofMonsters.Controler.Resource
{
    internal static class ImageManager
    {
        private static Image nullImage;
        static Dictionary<string, ImageItem> images = new Dictionary<string, ImageItem>();
        private static int lastCompressTime;
        private static int count;

        static ImageManager()
        {
            nullImage = PicLoader.Read("System", "Null.JPG");
        }

        static public bool HasImage(string path)
        {
            if (TimeTool.DateTimeToUnixTime(DateTime.Now) > lastCompressTime + 60 && count > 100)
            {
                Compress();
                lastCompressTime = TimeTool.DateTimeToUnixTime(DateTime.Now);
            }

            return images.ContainsKey(path) && images[path].Image != null;
        }

        static public Image GetImage(string path)
        {
            images[path].Time = TimeTool.DateTimeToUnixTime(DateTime.Now);
            return images[path].Image ?? nullImage;
        }

        static public void AddImage(string path, Image img)
        {
            ImageItem item = new ImageItem();
            item.Image = img;
            item.Time = TimeTool.DateTimeToUnixTime(DateTime.Now);
            if (images.ContainsKey(path))
            {
                images[path] = item;
            }
            else
            {
                images.Add(path, item);     
            }
            count++;
        }

        static public void Compress()
        {
            int now = TimeTool.DateTimeToUnixTime(DateTime.Now);
            foreach (ImageItem item in images.Values)
            {
                if (item.Image!=null)
                {
                    int size = item.Image.Width*item.Image.Height;
                    int time = 60*10000/size;
                    if (item.Time < now - time)
                    {
                        item.Image.Dispose();
                        item.Image = null;
                        count--;
                    }
                }
            }
        }
    }
}
