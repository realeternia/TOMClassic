using System;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.Peoples;

namespace TaleofMonsters.DataType.Mazes
{
    internal class MazeItem
    {
        public int Id { get; private set; }
        public MazeItemConfig MazeItemConfig;
        public int mlevel;
        private int[] infos;
        private string type;

        public MazeItem(int id, int level)
        {
            mlevel = level;
            this.Id = id;
            MazeItemConfig = ConfigData.GetMazeItemConfig(id);
            type = MazeItemConfig.Type;
            infos = new int[MazeItemConfig.Info.Length];
            Array.Copy(MazeItemConfig.Info, infos, infos.Length);

            if (type == "ritem")
            {
                type = "item";
                infos[0] = HItemBook.GetRandRareMid(infos[0]);
            }
            else if (type == "rmon")
            {
                type = "mon";
                infos[0] = infos[NarlonLib.Math.MathTool.GetRandom(0, infos.Length)];
                infos[1] = 0;
            }
            else if (type == "rresource")
            {
                type = "resource";
                infos[0] = NarlonLib.Math.MathTool.GetRandom(2, 8);
            }
        }

        public bool IsOn()
        {
            int[] cond = MazeItemConfig.Cond;
            if (cond.Length != 2)
            {
                return true;
            }

            int dtype = cond[0];
            int dinfo = cond[1];
            if (dtype == 0)
            {
                return UserProfile.InfoTask.GetTaskStateById(dinfo) == 0;
            }
            if (dtype == 1)
            {
                return UserProfile.InfoTask.GetTaskStateById(dinfo) == 1;
            }

            return false;
        }

        public void Effect(Forms.BasePanel panel, string tile, HsActionCallback success, HsActionCallback fail)
        {
            if (type == "item")
            {
                UserProfile.InfoBag.AddItem(infos[0], 1);
                HItemConfig itemConfig = ConfigData.GetHItemConfig(infos[0]);
                panel.AddFlowCenter(string.Format("获得{0}x1", itemConfig.Name), HSTypes.I2RareColor(itemConfig.Rare));
            }
            else if (type == "task")
            {
                UserProfile.InfoTask.SetTaskStateById(infos[0], 2);
                panel.AddFlowCenter(string.Format("任务{0}达成", ConfigData.GetTaskConfig(infos[0]).Name), "White");
            }
            else if (type == "mon")
            {
                PeopleBook.Fight(infos[0], tile, -1, mlevel + infos[1], PeopleFightReason.Other, success, fail,null);
                return;
            }
            else if (type == "gold")
            {
                UserProfile.InfoBag.AddResource(GameResourceType.Gold, (uint)infos[0]);
                panel.AddFlowCenter(string.Format("获得黄金x{0}", infos[0]), HSTypes.I2ResourceColor(0));
            }
            else if (type == "resource")
            {
                UserProfile.InfoBag.AddResource((GameResourceType)(infos[0] - 1), 1);
                panel.AddFlowCenter(string.Format("获得{1}x{0}", 1, HSTypes.I2Resource(infos[0] - 1)), HSTypes.I2ResourceColor(infos[0] - 1));
            }
            string word = MazeItemConfig.Word;
            if (word != "")
            {
                panel.AddFlowCenter(word, "White");
            }
            success();
        }

        public void DrawIcon(Graphics g, Rectangle dest, int frame)
        {
            string moveicon = "";
            switch (type)
            {
                case "mon": moveicon = ConfigData.GetPeopleConfig(infos[0]).Figue;break;
                case "resource": moveicon = string.Format("res{0}", infos[0]); break;
                case "gold": moveicon = "res1"; break;
                case "task": moveicon = "trap1"; break;
            }

            if (moveicon != "")
            {
                Image moveImage = MazeBook.GetMoveImage(moveicon);
                if (moveImage != null)
                {
                    int wid = moveImage.Width / 4;
                    Rectangle targetRect = new Rectangle(dest.X + (38 - wid) / 2, dest.Y, wid, moveImage.Height);
                    g.DrawImage(moveImage, targetRect, wid * frame, 0, wid, moveImage.Height, GraphicsUnit.Pixel);                   
                    if (type == "mon")
                    {
                        int lv = mlevel + infos[1];
                        Font font = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                        g.DrawString("lv" + lv, font, Brushes.Maroon, targetRect.X + 6, targetRect.Y + 28);
                        g.DrawString("lv" + lv, font, Brushes.White, targetRect.X + 5, targetRect.Y + 27);
                        font.Dispose();
                    }
                }
            }
            else
            {
                Rectangle smallDest = new Rectangle(dest.X + 7, dest.Y + 7, dest.Width - 14, dest.Height - 14);
                if (type == "block")
                {
                    Image block = PicLoader.Read("Map", "block.PNG");
                    g.DrawImage(block, smallDest);
                    block.Dispose();
                }
                else if (type == "item")
                {
                    g.DrawImage(HItemBook.GetHItemImage(infos[0]), smallDest, 4, 4, 56, 56, GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(HSIcons.GetIconsByEName("res1"), smallDest, 0, 0, 32, 32, GraphicsUnit.Pixel);
                }
            }
        }
    }
}
