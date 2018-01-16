using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using ConfigDatas;

namespace DeckManager
{
    public partial class Form1 : Form
    {
        private CardDeck deck;
        private Image[] images = new Image[30];
        private string path;
        private const string pathParent = "../../PicResource/";

        private bool isDirty = true;
        private Bitmap cacheImage;

        private int leftSelectIndex = -1;

        public Form1(string path)
        {
            AllowDrop = true;
            InitializeComponent();
            this.path = path;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string version = FileVersionInfo.GetVersionInfo(System.Windows.Forms.Application.ExecutablePath).FileVersion;
            Text = string.Format("卡片组织器 v{0}", version);

            if (path != "null")
                LoadFromFile(path);

            ConfigDatas.ConfigData.LoadData();
        }

        private void LoadFromFile(string txt)
        {
            path = txt;
            try
            {
                deck = new CardDeck();
                deck.MakeLoad(txt, 1);
                UpdateImages();
            }
            catch (Exception e)
            {
                MessageBox.Show("错误的文件格式"+e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateImages()
        {
            for (int i = 0; i < 30; i++)
            {
                int cardId = deck.GetCardId(i).Id;
                if (cardId == 0)
                {
                    images[i] = null;
                    continue;
                }

                if (cardId < 52000000)
                {
                    var config = ConfigData.GetMonsterConfig(cardId);
                    images[i] = Image.FromFile(string.Format("{0}Monsters/{1}.JPG", pathParent, config.Icon));
                }
                else if (cardId < 53000000)
                {
                    var config = ConfigData.GetWeaponConfig(cardId);
                    images[i] = Image.FromFile(string.Format("{0}Weapon/{1}.JPG", pathParent, config.Icon));
                }
                else
                {
                    var config = ConfigData.GetSpellConfig(cardId);
                    images[i] = Image.FromFile(string.Format("{0}Spell/{1}.JPG", pathParent, config.Icon));
                }
            }
            panel1.Invalidate();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            var wid = panel1.Width/3;
            var het = panel1.Height / 10;
            Font ft = new Font("宋体", 9);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (images[j * 3 + i] != null)
                        e.Graphics.DrawImage(images[j * 3 + i], i * wid, j * het, wid, het);
                    if (deck != null)
                    {
                        var cardData = deck.GetCardId(j * 3 + i);
                        if (cardData.Id > 0)
                        {
                            var cardConfig = CardConfigManager.GetCardConfig(cardData.Id);
                            var brush = new SolidBrush(Color.FromName(HSTypes.I2QualityColor((int)cardConfig.Quality)));
                            e.Graphics.DrawString(cardConfig.Name, ft, brush, i * wid, j * het);
                            brush.Dispose();
                        }
                        else
                        {
                            e.Graphics.DrawString(cardData.Type, ft, Brushes.White, i * wid, j * het);
                        }
                    }
                }
            }
            if (leftSelectIndex >= 0)
            {
                int nx = leftSelectIndex%3;
                int ny = leftSelectIndex/3;
                Pen p = new Pen(Color.LightGreen, 5);
                e.Graphics.DrawRectangle(p, nx*wid, ny*het, wid, het);
                p.Dispose();
            }
            ft.Dispose();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            int index = 0;
            int wid = 50;
            int heg = 50;
            int xCount = panel2.Width / wid;
            if (isDirty)
            {
               var cardIdList = new List<int>();
                foreach (var monsterConfig in ConfigData.MonsterDict.Values)
                    if (monsterConfig.IsSpecial == 0)
                        cardIdList.Add(monsterConfig.Id);
                foreach (var weaponConfig in ConfigData.WeaponDict.Values)
                    if (weaponConfig.IsSpecial == 0)
                        cardIdList.Add(weaponConfig.Id);
                foreach (var spellConfig in ConfigData.SpellDict.Values)
                    if (spellConfig.IsSpecial == 0)
                        cardIdList.Add(spellConfig.Id);

                panel2.Height = (cardIdList.Count / xCount + 1) * 50;
                cacheImage = new Bitmap(panel2.Width, panel2.Height);

                Graphics g = Graphics.FromImage(cacheImage);
                Font ft = new Font("宋体", 9);
                foreach (var cardId in cardIdList)
                {
                    var cardConfig = CardConfigManager.GetCardConfig(cardId);
                    var img = Image.FromFile(string.Format("{0}{1}/{2}.JPG", pathParent, cardConfig.GetImageFolderName(), cardConfig.Icon));
                    g.DrawImage(img, (index % xCount) * wid, (index / xCount) * heg, wid, heg);
                    var brush = new SolidBrush(Color.FromName(HSTypes.I2QualityColor((int)cardConfig.Quality)));
                    g.DrawString(cardConfig.Name, ft, brush, (index % xCount) * wid, (index / xCount) * heg);
                    brush.Dispose();
                    index++;
                }
                ft.Dispose();
                isDirty = false;
            }

            if (cacheImage != null)
            {
                e.Graphics.DrawImage(cacheImage, 0, 0, cacheImage.Width, cacheImage.Height);
            }
        }


        private void panel1_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", path);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            LoadFromFile(path);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            isDirty = true;
            panel2.Invalidate();
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            leftSelectIndex = -1;
            var wid = panel1.Width / 3;
            var het = panel1.Height / 10;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (e.X >= i*wid && e.X <= (i + 1)*wid && e.Y >= het*j && e.Y <= het*(j + 1))
                    {
                        leftSelectIndex = 3*j + i;
                        panel1.Invalidate();
                        break;
                    }
                }
            }
        }

        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void panel2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
        }
    }
}