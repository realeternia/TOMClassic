using System.Drawing;

namespace TaleofMonsters.Controler.Resource
{
    class ImageItem
    {
        public Image image;
        public int time;

        public override string ToString()
        {
            return string.Format("{0}x{1} {2}", image != null ? image.Width : 0, image != null ? image.Height : 0, time);
        }
    }
}
