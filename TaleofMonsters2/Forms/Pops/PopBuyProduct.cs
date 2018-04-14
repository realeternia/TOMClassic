using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms.Pops
{
    internal partial class PopBuyProduct : BasePanel
    {
        private int itemid;
        private int itemprice;
        private int count;
        private string fontcolor;
        private VirtualRegion vRegion;
        private ImageToolTip toolTip =SystemToolTip.Instance;

        public PopBuyProduct()
        {
            InitializeComponent();
            BackgroundImage = PicLoader.Read("System", "DeckChoose.PNG");
            vRegion = new VirtualRegion(this);
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        private void PopBuyProduct_Load(object sender, EventArgs e)
        {
            count = 1;
            textBoxTotal.Text = (count * itemprice).ToString();
            fontcolor = HSTypes.I2RareColor(ConfigDatas.ConfigData.GetHItemConfig(itemid).Rare);
        }

        void virtualRegion_RegionLeft()
        {
            toolTip.Hide(this);
        }

        void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            Image image = null;
            image = HItemBook.GetPreview(itemid);
            toolTip.Show(image, this, 108, 44);
        }

        private void MessageBoxEx_Paint(object sender, PaintEventArgs e)
        {
            vRegion.Draw(e.Graphics);

            var itemname = ConfigDatas.ConfigData.GetHItemConfig(itemid).Name;
            Font font = new Font("微软雅黑", 10*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            Brush brush = new SolidBrush(Color.FromName(fontcolor));
            e.Graphics.DrawString(itemname, font, brush, 134, 52);
            brush.Dispose();
            e.Graphics.DrawString("数量", font,Brushes.White ,69, 106);
            e.Graphics.DrawString("总价", font, Brushes.White, 69, 138);
            font.Dispose();
            e.Graphics.DrawImage(HSIcons.GetIconsByEName("res8"), 212, 140, 16, 16);
        }

        public static void Show(int id, int price, BasePanel p)
        {
            PopBuyProduct mb = new PopBuyProduct();
            mb.itemid = id;
            mb.itemprice = price;
            mb.ParentPanel = p;
            mb.vRegion.AddRegion(new PictureRegion(1, 68, 44, 40, 40, PictureRegionCellType.Item, id));
            PanelManager.DealPanel(mb);
            p.SetBlacken(true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (UserProfile.InfoBag.PayDiamond(itemprice * count))
            {
                UserProfile.InfoBag.AddItem(itemid, count);
                Close();
                ParentPanel.SetBlacken(false);
            }
            else
            {
                MainTipManager.AddTip(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughDimond), "Red");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
            ParentPanel.SetBlacken(false);
        }

        private void textBoxCount_KeyPress(object sender, KeyPressEventArgs e)
        {
             if (e.KeyChar >= 32 && !char.IsDigit(e.KeyChar))
             {
                 e.Handled = true;
             }
        }

        private void textBoxCount_TextChanged(object sender, EventArgs e)
        {
            if (textBoxCount.Text == "")
                textBoxCount.Text = @"1";
            count = int.Parse(textBoxCount.Text);
            if (count>=999)
            {
                count = 999;
                textBoxCount.Text = @"999";
            }
            else if (count <= 0)
            {
                count = 1;
                textBoxCount.Text = @"1";
            }
            textBoxTotal.Text = (count * itemprice).ToString();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (count < 999)
            {
                count++;
                textBoxCount.Text = count.ToString();
                textBoxTotal.Text = (count*itemprice).ToString();
            }
        }

        private void buttonMinus_Click(object sender, EventArgs e)
        {
            if (count>1)
            {
                count--;
                textBoxCount.Text = count.ToString();
                textBoxTotal.Text = (count * itemprice).ToString();
            }
        }

    }
}