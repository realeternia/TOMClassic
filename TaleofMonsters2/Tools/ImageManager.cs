using System;
using System.Collections.Generic;
using System.Drawing;
using NarlonLib.Tools;
using TaleofMonsters.Core.Loader;

namespace TaleofMonsters.Tools
{
    internal static class ImageManager
    {
        internal class ImageItem
        {
            public Image Image;
            public int Time;
            public bool Persist;

            public override string ToString()
            {
                return string.Format("{0}x{1} {2}", Image != null ? Image.Width : 0, Image != null ? Image.Height : 0, Time);
            }
        }

        private static Image nullImage;
        private static Dictionary<string, ImageItem> images = new Dictionary<string, ImageItem>();
        private static int lastCompressTime;
        private static int count;

        static ImageManager()
        {
            nullImage = PicLoader.Read("System", "Null.JPG");
        }

        public static bool HasImage(string path)
        {
            if (TimeTool.DateTimeToUnixTime(DateTime.Now) > lastCompressTime + 60 && count > 100)
            {
                Compress();
                lastCompressTime = TimeTool.DateTimeToUnixTime(DateTime.Now);
            }

            return images.ContainsKey(path) && images[path].Image != null;
        }

        public static Image GetImage(string path)
        {
            images[path].Time = TimeTool.DateTimeToUnixTime(DateTime.Now);
            return images[path].Image ?? nullImage;
        }

        public static void AddImage(string path, Image img, bool isPersist = false)
        {
            ImageItem item = new ImageItem
            {
                Image = img,
                Time = TimeTool.DateTimeToUnixTime(DateTime.Now),
                Persist = isPersist
            };
            images[path] = item;
            count++;
        }

        public static void Compress()
        {
            int now = TimeTool.DateTimeToUnixTime(DateTime.Now);
            foreach (var pickImg in images.Values)
            {
                if (pickImg.Image == null || pickImg.Persist)
                    continue;

                int size = pickImg.Image.Width*pickImg.Image.Height;
                int time = 60*10000/size;
                if (pickImg.Time < now - time)
                {
                    pickImg.Image.Dispose();
                    pickImg.Image = null;
                    count--;
                }
            }
        }
    }
}
