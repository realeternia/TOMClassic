using System.Drawing;
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

        private int selectIndex;

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
                        buttonPage.BackColor = Color.Maroon;
                        buttonPage.Cursor = Cursors.Hand;
                        buttonPage.FlatStyle = FlatStyle.Popup;
                        buttonPage.Font = new Font("微软雅黑", 13.5f, FontStyle.Bold, GraphicsUnit.Pixel, ((byte)(134)));
                        buttonPage.ForeColor = Color.Gold;
                        buttonPage.Location = new Point(width - (totalPage - i) * 20+x, 2+y);
                        buttonPage.Name = "buttonJob";
                        buttonPage.Tag = i.ToString();
                        buttonPage.Size = new Size(18, 24);
                        buttonPage.Text = (i + 1).ToString();
                        buttonPage.UseVisualStyleBackColor = false;
                        buttonPage.Click += buttonTag_Click;
                        parent.Controls.Add(buttonPage);
                        buttons[i] = buttonPage;
                    }
                }

                ChangeTarget(0);
            }
        }

        public void SetTarget(int targetId)
        {
            ChangeTarget(targetId);
        }

        void buttonTag_Click(object sender, System.EventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                ChangeTarget(int.Parse(control.Tag.ToString()));
            }
        }

        private void ChangeTarget(int targetId)
        {
            buttons[selectIndex].BackColor = Color.Maroon;
            selectIndex = targetId;
            buttons[selectIndex].BackColor = Color.Green;
            if (PageChange != null)
            {
                PageChange(targetId);
            }
        }
    }
}
