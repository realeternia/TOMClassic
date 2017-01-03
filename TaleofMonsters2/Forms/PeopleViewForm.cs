using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms
{
    internal sealed partial class PeopleViewForm : BasePanel
    {
        private const int cardWidth = 60;
        private const int cardHeight = 60;
        private bool showImage;
        private int realTar = -1;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private List<RivalState> people;
        private VirtualRegion virtualRegion;

        private int[] types;

        public PeopleViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            people = new List<RivalState>();
            virtualRegion = new VirtualRegion(this);
            virtualRegion.AddRegion(new PictureRegion(1, 41, 40, 70, 70, PictureRegionCellType.People, 0));
            for (int i = 0; i < 20; i++)
            {
                int xoff = (i%5)*cardWidth+19;
                int yoff = (i / 5) * cardHeight+159;
                SubVirtualRegion region = new PictureAnimRegion(i + 2, xoff, yoff, cardWidth, cardHeight, PictureRegionCellType.People, 0);
                region.AddDecorator(new RegionTextDecorator(0, 45, 9));
                virtualRegion.AddRegion(region);
            }
            types = GetPeopleAvailTypes();
            for (int i = 0; i < types.Length; i++)
            {
                int xoff = i * 26 + 19;
                int yoff = 125;
                virtualRegion.AddRegion(new ButtonRegion(i + 30, xoff, yoff, 24, 24, string.Format("MiniPeopleType{0}.JPG", types[i]), string.Format("MiniPeopleType{0}On.JPG", types[i])));
            }
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
            virtualRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClick);
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            showImage = true;
            this.bitmapButtonFight.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");
            bitmapButtonFight.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonFight.ForeColor = Color.White;
            bitmapButtonFight.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("abl1");
            bitmapButtonFight.IconSize = new Size(18, 18);
            bitmapButtonFight.IconXY = new Point(5, 5);
            bitmapButtonFight.TextOffX = 8;
            if (types.Length>0)
            {
                virtualRegion.SetRegionState(30, RegionState.Rectangled);
                Bind(types[0]);
            }            

            SoundManager.PlayBGM("TOM002.mp3");
            IsChangeBgm = true;

            if (UserProfile.InfoBasic.LastRival > 0)
            {
                FastBind(UserProfile.InfoBasic.LastRival);
            }
        }

        private void Fight()
        {
            if (realTar == -1)
                return;

            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(people[realTar].Pid);
            if (peopleConfig.Emethod == "")
            {
                ControlPlus.MessageBoxEx.Show("目前版暂不开放！");
                return;
            }

            PeopleBook.Fight(peopleConfig.Id, peopleConfig.BattleMap, -1, peopleConfig.Level, null, null, null);
        }

        private static int[] GetPeopleAvailTypes()
        {
            List<int> typeLists = new List<int>();
            foreach (RivalState person in UserProfile.InfoRival.Rivals.Values)
            {
                if (person.Avail)
                {
                    int personType = ConfigData.GetPeopleConfig(person.Pid).Type;
                    if (personType!=0 && !typeLists.Contains(personType))
                    {
                        typeLists.Add(personType);
                    }
                }
            }
            typeLists.Sort();
            return typeLists.ToArray();
        }

        private void Bind(int type)
        {
            people.Clear();
            int off = 2;

            for (int i = 0; i < 20; i++)
            {
                virtualRegion.SetRegionKey(i + 2, 0);
                virtualRegion.SetRegionDecorator(i + 2, 0, "");
            }

            foreach (RivalState person in UserProfile.InfoRival.Rivals.Values)
            {
                if (person.Avail)
                {
                    PeopleConfig personInfo = ConfigData.GetPeopleConfig(person.Pid);
                    if (personInfo.Type == type)
                    {
                        people.Add(person);
                        virtualRegion.SetRegionKey(off, person.Pid);
                        virtualRegion.SetRegionDecorator(off, 0, personInfo.Name);
                        off++;
                    }
                }
            }

            realTar = 0;
            virtualRegion.SetRegionKey(1, people[realTar].Pid);
            Invalidate();
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {

            if (id == 1 && realTar != -1)
            {
                Image image = PeopleBook.GetPreview(people[realTar].Pid);
                tooltip.Show(image, this, x, y);
            }
            else
            {
                tooltip.Hide(this);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void virtualRegion_RegionClick(int id, int x, int y, MouseButtons button)
        {
            if(button == MouseButtons.Left )
            {
                if (id > 1 && id < 30)
                {
                    int tar = id - 2;                   
                    if (tar < people.Count)
                    {
                        realTar = tar;
                        virtualRegion.SetRegionKey(1, people[realTar].Pid);
                        UserProfile.InfoBasic.LastRival = people[realTar].Pid;
                        Invalidate();
                    }
                }
                else if (id >= 30)
                {
                    int realtype = types[id - 30];
                    for (int i = 0; i < types.Length; i++)
                    {
                        virtualRegion.SetRegionState(i + 30, RegionState.Free);    
                    }                    
                    virtualRegion.SetRegionState(id, RegionState.Rectangled);
                    Bind(realtype);
                }
            }

        }

        private void FastBind(int id)
        {
            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);
            Bind(peopleConfig.Type);
            virtualRegion.SetRegionKey(1, id);
            for (int i = 0; i < people.Count; i++)
            {
                if (people[i].Pid == id)
                {
                    realTar = i;
                    break;
                }
            }
        }

        private void bitmapButtonFight_Click(object sender, EventArgs e)
        {
            Fight();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PeopleViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("挑战对手", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            if (showImage)
            {
                virtualRegion.Draw(e.Graphics);

                if(realTar!=-1)
                {
                    int xoff = 35;
                    int yoff = 35;
                    Font fontsong = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                    PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(people[realTar].Pid);
                    e.Graphics.DrawString(string.Format("{0} (Lv{1})", peopleConfig.Name, peopleConfig.Level), fontsong, Brushes.White, xoff + 100, yoff + 10);
                    e.Graphics.DrawString("来自", fontsong, Brushes.DarkGray, xoff+100, yoff+28);
                    e.Graphics.DrawString(peopleConfig.World, fontsong, Brushes.Cyan, xoff + 140, yoff + 28);
                    e.Graphics.DrawString("属性", fontsong, Brushes.DarkGray, xoff+100, yoff+46);
                    for (int i = 0; i < peopleConfig.Deck.Length; i++)
                        e.Graphics.DrawImage(HSIcons.GetIconsByEName(peopleConfig.Deck[i]), xoff + 140 + i * 18, yoff + 46, 16, 16);
                    int win = people[realTar].Win;
                    int loss = people[realTar].Loss;
                    int rate = 0;
                    if (win + loss != 0)
                        rate = win * 100 / (win + loss);
                    e.Graphics.DrawString("胜率", fontsong, Brushes.DarkGray, xoff+100, yoff+64);
                    e.Graphics.DrawString(string.Format("{0:0.0}% ({1}胜{2}负)", rate, win, loss), fontsong, (win == loss) ? Brushes.White : (win > loss ? Brushes.LightGreen : Brushes.Red), xoff+140, yoff+64);
                    fontsong.Dispose();
                }
            }
        }

    }
}