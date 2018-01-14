using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.Loader;

namespace TaleofMonsters.Core
{
    class HSCursor
    {
        delegate void ChangeSubMethod(Cursor cursor);

        private void ChangeSub(Cursor cursor)
        {
            if (parent.InvokeRequired)
            {
                ChangeSubMethod aus = new ChangeSubMethod(ChangeSub);
                parent.Invoke(aus, new Cursor[] { cursor });
            }
            else
            {
                parent.Cursor = cursor;
            }
        }

        private Control parent;

        public string Name { get; set; }

        public HSCursor(Control control)
        {
            parent = control;
            Name = "null";
        }

        public void ChangeCursor(string cname)
        {
            if (cname != Name)
                ChangeCursor("System.Cursor", string.Format("{0}.PNG", cname), 0, 0);
        }

        public void ChangeCursor(string path, string cname)
        {
            if (cname != Name)
                ChangeCursor(path, cname, 0, 0);
        }

        public void ChangeCursor(string path, string cname, int width, int height)
        {
            string keyname = cname.Substring(0, cname.IndexOf('.'));
            if (keyname != Name)
            {
                Name = keyname;
                Image img = PicLoader.Read(path, cname);
                if (img == null)
                    img = PicLoader.Read("system.cursor", "default.PNG");
                SetCursorSize(img, new Point(0, 0), width, height);
            }
        }

        private void SetCursorSize(Image cursor, Point hotPoint, int width, int height)
        {
            int hotX = hotPoint.X;
            int hotY = hotPoint.Y;
            Bitmap myNewCursor = new Bitmap(cursor.Width * 2 - hotX, cursor.Height * 2 - hotY);
            Graphics g = Graphics.FromImage(myNewCursor);
            g.Clear(Color.FromArgb(0, 0, 0, 0));
            if (width == 0 && height == 0)
                g.DrawImage(cursor, cursor.Width - hotX, cursor.Height - hotY, cursor.Width, cursor.Height);
            else
                g.DrawImage(cursor, width - hotX, height - hotY, width, height);
            ChangeSub(new Cursor(myNewCursor.GetHicon()));
            g.Dispose();
            myNewCursor.Dispose();
        }
    }
}
