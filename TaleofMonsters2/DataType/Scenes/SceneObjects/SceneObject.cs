using System.Drawing;
using TaleofMonsters.Controler.Loader;

namespace TaleofMonsters.DataType.Scenes.SceneObjects
{
    internal class SceneObject
    {
        public int Id { get; protected set; }

        public int X { get; set; }

        public int Y { get; set; } 

        public int Width { get; set; } //可能会被缩放

        public int Height { get; set; }//可能会被缩放

        public string Name { get; protected set; }

        public virtual string Figue { get; protected set; }

        public virtual void Draw(Graphics g, int target)
        {
            Font font = new Font("微软雅黑", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            Image head = PicLoader.Read("NPC", string.Format("{0}.PNG", Figue));
            int ty = Y + 50;
            if (target == Id)
            {
                g.DrawImage(head, X - 5, ty - 5, Width*5/4, Height*5/4);
                g.DrawString(Name, font, Brushes.Black, X + 5, ty + Height - 33);
                g.DrawString(Name, font, Brushes.Yellow, X + 2, ty + Height - 30);
            }
            else
            {
                g.DrawImage(head, X, ty, Width, Height);
                g.DrawString(Name, font, Brushes.Black, X + 3, ty + Height - 33);
                g.DrawString(Name, font, Brushes.White, X, ty + Height - 30);
            }
            head.Dispose();
            font.Dispose();
        }

        public virtual void CheckClick()
        {
        }
    }
}
