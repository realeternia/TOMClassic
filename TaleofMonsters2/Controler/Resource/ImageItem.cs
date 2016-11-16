using System.Drawing;

namespace TaleofMonsters.Controler.Resource
{
    internal class ImageItem
    {
        public Image Image;
        public int Time;

        public override string ToString()
        {
            return string.Format("{0}x{1} {2}", Image != null ? Image.Width : 0, Image != null ? Image.Height : 0, Time);
        }
    }
}
