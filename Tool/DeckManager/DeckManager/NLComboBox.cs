using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DeckManager
{
    public partial class NLComboBox : ComboBox
    {
        public NLComboBox()
        {
            InitializeComponent();

            this.DrawMode = DrawMode.OwnerDrawVariable;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

         protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // Draw the background 
            //e.DrawBackground();

            if (e.Index ==-1)
            {
                return;
            }

            if ((e.State & DrawItemState.Selected) != 0)
            {
                //渐变画刷
                LinearGradientBrush brush = new LinearGradientBrush(e.Bounds, Color.Brown,
                                                 Color.SandyBrown, LinearGradientMode.Vertical);
                //填充区域
                //Rectangle borderRect = new Rectangle(0, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                e.Graphics.FillRectangle(brush, e.Bounds);
            }
            else
            {
                SolidBrush brush = new SolidBrush(BackColor);
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            // Get the item text    
            string text = Items[e.Index].ToString();
             var colr = ForeColor;
             if (text.Contains("|"))
             {
                 var datas = text.Split('|');
                 colr = Color.FromName(datas[0]);
                 text = datas[1];
             }

            // Determine the forecolor based on whether or not the item is selected    
            Brush b = new SolidBrush(colr);
            // Draw the text    
            e.Graphics.DrawString(text, Font, b, e.Bounds.X, e.Bounds.Y+2);
            b.Dispose();
        }

        protected override void WndProc(ref Message m)
        {

            base.WndProc(ref m);

            if (m.Msg == 0xF)

            {

                Graphics graph = this.CreateGraphics();

                Pen pen = new Pen(BackColor, 2.0f);

                graph.DrawRectangle(pen, base.ClientRectangle);
                pen.Dispose();
            }

        }

        public string TargetText
        {
            get { var txt = SelectedItem.ToString();
                if (txt.Contains("|"))
                {
                    txt = txt.Split('|')[1];
                }
                return txt;
            }
        }
    }
}
