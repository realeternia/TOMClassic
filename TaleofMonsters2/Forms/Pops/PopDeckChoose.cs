using System;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Maps;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.Forms.Pops
{
    internal partial class PopDeckChoose : Form
    {
        private int tile;
        private bool comfirm;
        private Image img;

        public PopDeckChoose()
        {
            InitializeComponent();
            BackgroundImage = PicLoader.Read("System", "DeckChoose.PNG");
            FormBorderStyle = FormBorderStyle.None;
        }

        private void MessageBoxEx_Paint(object sender, PaintEventArgs e)
        {
            if (img != null)
            {
                e.Graphics.DrawImage(img, 38, 62, 100, 100);
            }

            Font font = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(string.Format("地形:{0}", tile == -1 ? "默认" : ConfigDatas.ConfigData.GetTileConfig(tile).Cname), font, Brushes.White, 63, 147);
            font.Dispose();
        }

        public static bool Show(string map, int tile, string[] datas)
        {
            PopDeckChoose mb = new PopDeckChoose();
            mb.tile = tile;
            mb.img = BattleMapBook.GetMapImage(map, tile);
            foreach (string s in datas)
                mb.comboBox1.Items.Add(s);
            mb.comboBox1.SelectedIndex = UserProfile.InfoCard.DeckId;
            mb.ShowDialog();
            return mb.comfirm;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            comfirm = true;
            UserProfile.InfoCard.DeckId = comboBox1.SelectedIndex;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}