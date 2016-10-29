using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Skills;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.DataType.Cards.Weapons;

namespace TaleofMonsters.Forms.MagicBook
{
    internal sealed partial class MonsterSkillViewForm : BasePanel
    {
        private const int cardWidth = 50;
        private const int cardHeight = 50;
        private int xCount = 10;
        private int yCount = 8;
        private int cardCount;
        private int totalCount;
        private int page;
        private bool show;
        private int tar = -1;
        private int sel = -1;
        private List<int> skills;
        private Bitmap tempImage;
        private bool isDirty = true;
        private CardDetail cardDetail;
        private string skillDesStr = ""; //显示在下方的技能说明

        public MonsterSkillViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonNext.ImageNormal = PicLoader.Read("ButtonBitmap", "NextButton.JPG");
            bitmapButtonNext.NoUseDrawNine = true;
            this.bitmapButtonPre.ImageNormal = PicLoader.Read("ButtonBitmap", "PreButton.JPG");
            bitmapButtonPre.NoUseDrawNine = true;
            tempImage = new Bitmap(cardWidth*xCount, cardHeight*yCount);
            comboBoxType.SelectedIndex = 0;
        }

        internal override void Init(int width, int height)
        {
            #region 智能修正面板元素数量
            int borderX = 783 - cardWidth * xCount;
            int borderY = 585 - cardHeight * yCount;

            xCount = (width - borderX) * 7 / 10 / cardWidth; //0.8作为一个边缘因子
            yCount = (height - borderY) * 8 / 10 / cardHeight;
            cardCount = xCount * yCount;

            Width = xCount * cardWidth + borderX;
            Height = yCount * cardHeight + borderY;
            #endregion

            base.Init(width, height);
            show = true;

            cardDetail = new CardDetail(this, cardWidth * xCount + 65, 35, cardHeight * yCount + 93 + cardHeight);
            nlClickLabel1.Location = new Point(75, cardHeight * yCount + 100 + cardHeight);
            nlClickLabel1.Size = new Size(cardWidth * xCount - 20, 63);

            ChangeCards("全部种类");
        }

        internal override void OnFrame(int tick)
        {
            cardDetail.OnFrame();
        }

        private void UpdateButtonState()
        {
            bitmapButtonPre.Enabled = page > 0;
            bitmapButtonNext.Enabled = (page + 1) * cardCount < totalCount;
        }

        private void ChangeCards(string type)
        {
            page = 0;
            totalCount = 0;
            tar = -1;
            sel = -1;
            cardDetail.SetInfo(-1);
            #region 数据装载
            List<IntPair> things = new List<IntPair>();
            foreach (SkillConfig skill in ConfigData.SkillDict.Values)
            {
                if ((skill.Type == type || type == "全部种类"))
                {
                    IntPair mt = new IntPair();
                    mt.Type = skill.Id;
                    mt.Value = skill.Id;
                    things.Add(mt);
                    totalCount++;
                }
            }
            things.Sort(new CompareBySid());

            skills = new List<int>();
            foreach (IntPair mt in things)
            {
                skills.Add(mt.Value);
            }
            #endregion
            UpdateButtonState();
            isDirty = true;
            Invalidate(new Rectangle(65, 35, cardWidth * xCount + 200, 630));
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            string type = comboBoxType.SelectedItem.ToString();
            ChangeCards(type);
        }

        private void buttonPre_Click(object sender, EventArgs e)
        {
            page--;
            if (page < 0)
            {
                page++;
                return;
            }
            tar = -1;
            sel = -1;
            cardDetail.SetInfo(-1);
            UpdateButtonState();
            isDirty = true;
            Invalidate(new Rectangle(65, 35, cardWidth * xCount+200, 630));
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            page++;
            if (page > (totalCount - 1) / cardCount)
            {
                page--;
                return;
            }
            tar = -1;
            sel = -1;
            cardDetail.SetInfo(-1);
            UpdateButtonState();
            isDirty = true;
            Invalidate(new Rectangle(65, 35, cardWidth * xCount+200, 630));
        }

        private void MonsterSkillViewForm_Click(object sender, EventArgs e)
        {
            if (tar != -1)
            {
                sel = tar;
                Skill skill = new Skill(skills[tar]);
                skillDesStr = string.Format("{0}:{1}", skill.SkillConfig.Name, skill.Descript);
                Invalidate(new Rectangle(65, cardHeight*yCount + 37 + cardHeight + 21, 500, 20));
                nlClickLabel1.ClearLabel();
                foreach (int mid in MonsterBook.GetSkillMids(skill.Id))
                {
                    nlClickLabel1.AddLabel(ConfigData.GetMonsterConfig(mid).Name, mid);
                }
                foreach (int wid in WeaponBook.GetSkillWids(skill.Id))
                {
                    nlClickLabel1.AddLabel(ConfigData.GetWeaponConfig(wid).Name, wid);
                }
                nlClickLabel1.Invalidate();
            }
        }

        private void nlClickLabel1_SelectionChange(Object value)
        {
            cardDetail.SetInfo((int)value);
            Invalidate(new Rectangle(65, 35, cardWidth * xCount + 200, 630));
        }

        private void MonsterSkillViewForm_MouseMove(object sender, MouseEventArgs e)
        {
            int truex = e.X - 65;
            int truey = e.Y - 35 - cardHeight;
            if (truex > 0 && truex < xCount * cardWidth && truey > 0 && truey < yCount * cardHeight)
            {
                int temp = truex / cardWidth + truey / cardHeight * xCount + cardCount * page;
                if (temp != tar)
                {
                    tar = temp < totalCount ? temp : -1;
                    Invalidate(new Rectangle(65, 35, cardWidth * xCount, cardHeight * yCount + cardHeight));
                }
            }
            else
            {
                if (tar != -1)
                {
                    tar = -1;
                    Invalidate(new Rectangle(65, 35, cardWidth * xCount, cardHeight * yCount + cardHeight));
                }
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MonsterSkillViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 全技能 ", font, Brushes.White, 320, 8);
            font.Dispose();

            cardDetail.Draw(e.Graphics);
            e.Graphics.FillRectangle(Brushes.DarkGray, 67, 37, cardWidth * xCount - 4, 44);
            Image img = PicLoader.Read("System", "SearchBack.JPG");
            e.Graphics.DrawImage(img, 70, 40, cardWidth * xCount - 10, 38);
            img.Dispose();

            font = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString("分类", font, Brushes.White, 86, 51);
            font.Dispose();

            font = new Font("宋体", 10 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontblack = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(245, 244, 242)), 65, cardHeight * yCount + 35 + cardHeight, cardWidth * xCount, 93);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(190, 175, 160)), 65, cardHeight * yCount + 35 + cardHeight, cardWidth * xCount, 20);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(190, 175, 160)), 65, cardHeight * yCount + 75 + cardHeight, cardWidth * xCount, 20);
            e.Graphics.DrawString("技能说明", fontblack, Brushes.White, 65, cardHeight * yCount + 37 + cardHeight);
            if (!string.IsNullOrEmpty(skillDesStr))
            {
                e.Graphics.DrawString(skillDesStr, font, Brushes.SaddleBrown, 65+5, cardHeight * yCount + 37 + cardHeight + 21);
            }
            e.Graphics.DrawString("所有生物", fontblack, Brushes.White, 65, cardHeight * yCount + 77 + cardHeight);
            fontblack.Dispose();
            font.Dispose();

            if (show)
            {
                int pages = totalCount / cardCount + 1;
                int cardLimit = (page < pages - 1) ? cardCount : (totalCount % cardCount);
                int former = cardCount * page + 1;
                if (isDirty)
                {
                    tempImage.Dispose();
                    tempImage = new Bitmap(cardWidth * xCount, cardHeight * yCount);
                    Graphics g = Graphics.FromImage(tempImage);
                    for (int i = former - 1; i < former + cardLimit - 1; i++)
                    {
                        var cardImg = SkillBook.GetSkillImage(skills[i]);
                        if (cardImg == null)
                        {
                            continue;
                        }
                        g.DrawImage(cardImg, (i % xCount) * cardWidth, ((i / xCount) % yCount) * cardHeight, cardWidth, cardHeight);
                    }
                    g.Dispose();
                    isDirty = false;
                }
                e.Graphics.DrawImage(tempImage, 65, 35 + cardHeight);

                if (sel != -1 && sel < totalCount)
                {
                    SolidBrush yellowbrush = new SolidBrush(Color.FromArgb(80, Color.Lime));
                    int x = (sel%xCount)*cardWidth + 65;
                    int y = ((sel / xCount) % yCount) * cardHeight + 35 + cardHeight;
                    e.Graphics.FillRectangle(yellowbrush, x, y, cardWidth, cardHeight);
                    yellowbrush.Dispose();

                    Pen yellowpen = new Pen(Brushes.Lime, 3);
                    e.Graphics.DrawRectangle(yellowpen, x, y, cardWidth, cardHeight);
                    yellowpen.Dispose();
                }
                if (tar != -1 && tar < totalCount)
                {
                    int x = (tar % xCount) * cardWidth + 65;
                    int y = ((tar / xCount) % yCount) * cardHeight + 35 + cardHeight;
                    SolidBrush yellowbrush = new SolidBrush(Color.FromArgb(80, Color.Yellow));
                    e.Graphics.FillRectangle(yellowbrush, x, y, cardWidth, cardHeight);
                    yellowbrush.Dispose();

                    Pen yellowpen = new Pen(Brushes.Yellow, 3);
                    e.Graphics.DrawRectangle(yellowpen, x, y, cardWidth, cardHeight);
                    yellowpen.Dispose();
                }
            }
        }
    }


}