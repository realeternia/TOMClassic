using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal sealed partial class BuyPieceForm : BasePanel
    {
        internal class NpcPieceData
        {
            public int Id;
            public int Count;
            public bool Used;
            public bool IsEmpty()
            {
                return Id == 0;
            }
        }

        private List<NpcPieceData> changes;
        private PieceItem[] pieceControls;
        private ColorWordRegion colorWord;

        public BuyPieceForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonRefresh.ImageNormal = PicLoader.Read("ButtonBitmap", "PlusButton.JPG");
            bitmapButtonRefresh.NoUseDrawNine = true;
            this.bitmapButtonFresh.ImageNormal = PicLoader.Read("ButtonBitmap", "FreshButton.JPG");
            bitmapButtonFresh.NoUseDrawNine = true;
            this.bitmapButtonDouble.ImageNormal = PicLoader.Read("ButtonBitmap", "MoneyButton.JPG");
            bitmapButtonDouble.NoUseDrawNine = true;
            colorWord = new ColorWordRegion(12, 38, 384, "微软雅黑", 11, Color.White);
            colorWord.Bold = true;
            colorWord.Text = "|每|Red|6小时||随机更新5条购买素材，素材的|Lime|背景颜色||决定素材的最高品质。";
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);

            pieceControls = new PieceItem[8];
            for (int i = 0; i < 8; i++)
            {
                pieceControls[i] = new PieceItem(this, 8 + (i % 2) * 192, 111 + (i / 2) * 55, 193, 56);
                pieceControls[i].Init(i);
            }
            RefreshInfo();
            OnFrame(0);
        }

        private void RefreshInfo()
        {
            GetPieceData();
            for (int i = 0; i < 8; i++)
            {
                pieceControls[i].RefreshData();
            }
            bitmapButtonRefresh.Visible = changes.Count < 8;
        }


        private void pictureBoxCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bitmapButtonRefresh_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx2.Show("是否花5钻石增加一条素材?") == DialogResult.OK)
            {
                if (UserProfile.InfoBag.PayDiamond(5))
                {
                    AddPieceData();
                    RefreshInfo();
                }
            }
        }

        private void bitmapButtonFresh_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx2.Show("是否花10钻石刷新所有素材?") == DialogResult.OK)
            {
                if (UserProfile.InfoBag.PayDiamond(10))
                {
                    RefreshAllPieceData();
                    RefreshInfo();
                }
            }
        }
        private void bitmapButtonDouble_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx2.Show("是否花5钻石翻倍所有素材数量?") == DialogResult.OK)
            {
                if (UserProfile.InfoBag.PayDiamond(5))
                {
                    DoubleAllPieceData();
                    RefreshInfo();
                }
            }
        }


        private void BuyPieceForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("素材", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            colorWord.Draw(e.Graphics);
            foreach (PieceItem ctl in pieceControls)
            {
                ctl.Draw(e.Graphics);
            }
        }


        private List<NpcPieceData> GetPieceData()
        {
            changes = new List<NpcPieceData>();
            for (int i = 0; i < 5; i++)
            {
                changes.Add(CreatePieceMethod(i));
            }
            return changes;
        }

        private void AddPieceData()
        {
            if (changes.Count < 8)
            {
                changes.Add(CreatePieceMethod(changes.Count));
            }
        }

        public NpcPieceData GetPieceData(int index)
        {
            if (changes.Count > index)
            {
                return changes[index];
            }
            return new NpcPieceData();
        }

        public void RemovePieceData(int index)
        {
            if (changes.Count > index)
            {
                changes[index].Used = true;
            }
        }

        private void RefreshAllPieceData()
        {
            int count = changes.Count;
            changes.Clear();
            for (int i = 0; i < count; i++)
            {
                changes.Add(CreatePieceMethod(i));
            }
        }

        private void DoubleAllPieceData()
        {
            foreach (var memNpcPieceData in changes)
            {
                memNpcPieceData.Count *= 2;
            }
        }
        private NpcPieceData CreatePieceMethod(int index)
        {
            NpcPieceData piece = new NpcPieceData();
            int rare = MathTool.GetRandom(Math.Max(index / 2, 1), index / 2 + 3);
            piece.Id = HItemBook.GetRandRareItemId(rare);
            piece.Count = MathTool.GetRandom((8 - rare) / 2, 8 - rare);

            return piece;
        }
    }
}