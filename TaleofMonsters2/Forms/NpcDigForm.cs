using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Control;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Achieves;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Scenes;

namespace TaleofMonsters.Forms
{
    internal sealed partial class NpcDigForm : BasePanel
    {
        private string[] say;
        private int npcId;
        private List<DigInfo> digs = new List<DigInfo>();
        private int timePass;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private ColorWordRegion colorWord;
        private VirtualRegion virtualRegion;
        private bool isOn;

        public int NpcId
        {
            set { npcId = value; }
        }

        public NpcDigForm()
        {
            InitializeComponent();
            colorWord = new ColorWordRegion(22, 77, 268, "宋体", 10, Color.White);
            virtualRegion = new VirtualRegion(this);
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);

            bitmapButtonPick.ImageNormal = PicLoader.Read("ButtonBitmap", "PickButton.JPG");
            this.bitmapButtonClose2.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            NpcConfig npcConfig = ConfigData.GetNpcConfig(npcId);

            say = npcConfig.Say.Split(':');
            InitInfo(npcConfig.Answer);
        }

        private void InitInfo(string info)
        {
            colorWord.Text = say[1];
            string[] answer = info.Split('|');
            for (int i = 0; i < answer.Length; i++)
            {
                string[] datas = answer[i].Split(':');
                DigInfo dinfo = new DigInfo();
                dinfo.itemid = int.Parse(datas[0]);
                dinfo.percent = int.Parse(datas[1]);
                digs.Add(dinfo);

                virtualRegion.AddRegion(new PictureAnimRegion(i + 1, 25 + 30 * i, 203, 25, 25, PictureRegionCellType.Item, dinfo.itemid));
            }
        }

        internal override void OnFrame(int tick)
        {
            base.OnFrame(tick);

            DigEnd();
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (id >= 1)
            {
                Image image = HItemBook.GetPreview(key);
                tooltip.Show(image, this, x, y);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }   

        private void bitmapButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bitmapButtonDig_Click(object sender, EventArgs e)
        {
            if (UserProfile.InfoBasic.DigCount >= 20)
            {
                if (MessageBoxEx2.Show("是否花10钻石增加20次采集次数?") == DialogResult.OK)
                {
                    if (UserProfile.InfoBag.PayDiamond(10))
                    {
                        UserProfile.InfoBasic.DigCount = 0;
                        AddFlowCenter("采集次数重置", "Lime");
                        Invalidate();
                        return;
                    }
                }
            }

            if (UserProfile.InfoBasic.DigCount < 20)
            {
                isOn = true;
                timePass = 0;
            }
        }

        private void DigEnd()
        {
            if (!isOn)
            {
                return;
            }

            timePass ++;
            if (timePass >= 20)
            {
                isOn = false;
                UserProfile.InfoBasic.DigCount++;
                timePass = 0;
                int digadd = 0;
                int digget = MathTool.GetRandom(100);

                foreach (DigInfo dig in digs)
                {
                    digadd += dig.percent;
                    if (digget < digadd)
                    {
                        UserProfile.InfoBag.AddItem(dig.itemid, 1);
                        HItemConfig itemConfig = ConfigData.GetHItemConfig(dig.itemid);
                        AddFlow(string.Format("{0}x1", itemConfig.Name), HSTypes.I2RareColor(itemConfig.Rare), 200, 180);
                        UserProfile.Profile.FinishPick(ConfigData.GetNpcConfig(npcId).Func);

                        AchieveBook.CheckByCheckType("pick");
                        
                        return;
                    }
                }

                AddFlow("什么都没有...", "Yellow", 200, 180);
                return;
            }
            Invalidate();
        }

        private void NpcDigForm_Paint(object sender, PaintEventArgs e)
        {
            if (npcId <= 0)
                return;

            Image bgImage = PicLoader.Read("System", "TalkBack.PNG");
            e.Graphics.DrawImage(bgImage, 0, 0, bgImage.Width, bgImage.Height);
            bgImage.Dispose();
            e.Graphics.DrawImage(SceneBook.GetSceneNpcImage(npcId), 24, 0, 70, 70);

            Font font = new Font("宋体", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(ConfigData.GetNpcConfig(npcId).Name, font, Brushes.Chocolate, 131, 50);
            font.Dispose();

            font = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(string.Format("今日采集{0}/20", UserProfile.InfoBasic.DigCount), font, Brushes.PaleGreen, 209, 256);
            font.Dispose();

            if (timePass > 0)
            {
                LinearGradientBrush b1 = new LinearGradientBrush(new Rectangle(25, 238, 160, 8), Color.White, Color.Lime, LinearGradientMode.Vertical);
                int v = 160 * timePass / 20;
                e.Graphics.FillRectangle(b1, 25, 238, v, 8);
                b1.Dispose();
            }

            colorWord.Draw(e.Graphics);

            virtualRegion.Draw(e.Graphics);
        }
    }

    public struct DigInfo
    {
        public int itemid;
        public int percent;
    }
}