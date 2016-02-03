using System.Collections.Generic;
using System.Drawing;
using NarlonLib.Core;
using System;
using TaleofMonsters.Controler.Loader;

namespace TaleofMonsters.Controler.Resource
{
    static class ImageManager
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

            return images.ContainsKey(path) && images[path].image != null;
        }

        static public Image GetImage(string path)
        {
            images[path].time = TimeTool.DateTimeToUnixTime(DateTime.Now);
            return images[path].image ?? nullImage;
        }

        static public void AddImage(string path, Image img)
        {
            ImageItem item = new ImageItem();
            item.image = img;
            item.time = TimeTool.DateTimeToUnixTime(DateTime.Now);
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
                if (item.image!=null)
                {
                    int size = item.image.Width*item.image.Height;
                    int time = 60*10000/size;
                    if (item.time < now - time)
                    {
                        item.image.Dispose();
                        item.image = null;
                        count--;
                    }
                }
            }
        }
    }
}
