using System.Windows.Forms;

namespace ControlPlus
{
    public class NLPageSelector
    {
        public delegate void ChangePageEventHandler(int pg);
        public event ChangePageEventHandler PageChange;
        private Button[] buttons;

        private int totalPage;
        private int x;
        private int y;
        private int width;
        private Control parent;

        public NLPageSelector(Control uc,int x,int y,int width)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            parent = uc;
        }

        public int TotalPage
        {
            set
            {
                totalPage = value;
                if (buttons != null)
                {
                    foreach (Button button in buttons)
                    {
                        parent.Controls.Remove(button);
                    }
                }

                if (totalPage>1)
                {
                    buttons = new Button[totalPage];
                    for (int i = 0; i < totalPage; i++)
                    {
                        Button buttonPage = new Button();
                        buttonPage.BackColor = System.Drawing.Color.Maroon;
                        buttonPage.Cursor = System.Windows.Forms.Cursors.Hand;
                        buttonPage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
                        buttonPage.Font = new System.Drawing.Font("微软雅黑", 13.5f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
                        buttonPage.ForeColor = System.Drawing.Color.Gold;
                        buttonPage.Location = new System.Drawing.Point(width - (totalPage - i) * 20+x, 2+y);
                        buttonPage.Name = "buttonJob";
                        buttonPage.Tag = i.ToString();
                        buttonPage.Size = new System.Drawing.Size(18, 24);
                        buttonPage.Text = (i + 1).ToString();
                        buttonPage.UseVisualStyleBackColor = false;
                        buttonPage.Click += new System.EventHandler(buttonJob_Click);
                        parent.Controls.Add(buttonPage);
                        buttons[i] = buttonPage;
                    }
                }
            }
        }

        void buttonJob_Click(object sender, System.EventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                int page = int.Parse(control.Tag.ToString());
                if (PageChange != null)
                {
                    PageChange(page);
                }
            }
        }
    }
}
