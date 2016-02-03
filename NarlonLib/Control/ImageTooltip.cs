using System.Drawing;
using System.Windows.Forms;

namespace NarlonLib.Control
{
    public class ImageToolTip : ToolTip
    {
        Image image;
        private int info;

        private void InitializeComponent()
        {
            // 
            // ImageToolTip
            // 
            this.Popup += new System.Windows.Forms.PopupEventHandler(this.MyToolTip_Popup);
            this.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.MyToolTip_Draw);

        }
        public ImageToolTip()
        {
            InitializeComponent();
            base.OwnerDraw = true;
            base.AutoPopDelay = 0;
        }
        public void Show(Image img, IWin32Window window ,int x,int y)
        {
            if (this.image != null)
                Hide(window);
            image = img;
            base.Show(" ", window, x, y, 300000);
        }

        public new void Show(string text, IWin32Window window, int x, int y)
        {
            if (this.image != null)
                Hide(window);
            image = Drawing.DrawTool.GetImageByString(text, 200);

            base.Show(" ", window, x, y, 300000);
        }

        public void Show(Image img, IWin32Window window, int x, int y, int sinfo)
        {
            if (this.image != null)
                Hide(window);
            image = img;
            info = sinfo;
            Show(" ", window, x, y, 300000);
        }

        public new void Hide(IWin32Window window) {
            base.Hide(window);
            if (this.image != null)
                this.image.Dispose();
        }

        public void Hide(IWin32Window window, int sinfo)
        {
            if (sinfo == info)
            {
                base.Hide(window);
                if (this.image != null)
                    this.image.Dispose();
            }
        } 

        private void MyToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            this.BackColor = Color.Black;
            if (image != null)
                e.Graphics.DrawImage(image, 0, 0);
        }

        private void MyToolTip_Popup(object sender, PopupEventArgs e)
        {
            e.ToolTipSize = new Size(image.Width, image.Height);
        }

    }
}
