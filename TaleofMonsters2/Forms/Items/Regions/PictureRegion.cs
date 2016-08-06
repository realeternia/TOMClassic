using System.Drawing;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Interface;
using ConfigDatas;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class PictureRegion : SubVirtualRegion
    {
        protected VirtualRegionCellType type;
        protected int nid;//特征id，比如卡的id，npc的id等等

        public PictureRegion(int id, int x, int y, int width, int height, int info, VirtualRegionCellType type, int nid)
            : base(id, x, y, width, height, info)
        {
            this.nid = nid;
            this.type = type;
        }

        public override void Draw(Graphics g)
        {
            if (nid > 0)
            {
                if (type == VirtualRegionCellType.Npc)
                {
                    g.DrawImage(DataType.NPCs.NPCBook.GetPersonImage(nid), x, y, width, height);
                }
                else if (type == VirtualRegionCellType.Item)
                {
                    g.DrawImage(DataType.Items.HItemBook.GetHItemImage(nid), x, y, width, height);
                }
                else if (type == VirtualRegionCellType.Equip)
                {
                    g.DrawImage(DataType.Equips.EquipBook.GetEquipImage(nid), x, y, width, height);
                }
                else if (type == VirtualRegionCellType.Card)
                {
                    var cardData = CardConfigManager.GetCardConfig(nid);
                    int attr = cardData.Attr;//属性决定包边颜色
                    g.FillRectangle(PaintTool.GetBrushByAttribute(attr), x + 1, y + 1, width - 2, height - 2);
                    g.DrawImage(DataType.Cards.CardAssistant.GetCardImage(nid, 60, 60), x + 2, y + 2, width - 4, height - 4);
                    string cardBorder = DataType.Cards.CardAssistant.GetCardBorder(cardData);
                    g.DrawImage(PicLoader.Read("Border", cardBorder), x + 2, y + 2, width - 4, height - 4);
                }
                else if (type == VirtualRegionCellType.SkillAttr)
                {
                    g.DrawImage(DataType.HeroSkills.HeroSkillAttrBook.GetHeroSkillAttrImage(nid), x, y, width, height);
                }
                else if (type == VirtualRegionCellType.Achieve)
                {
                    g.DrawImage(DataType.Achieves.AchieveBook.GetAchieveImage(nid), x, y, width, height);
                }
                else if (type == VirtualRegionCellType.People)
                {
                    g.DrawImage(DataType.Peoples.PeopleBook.GetPersonImage(nid), x, y, width, height);
                }
                else if (type == VirtualRegionCellType.HeroSkill)
                {
                    g.DrawImage(DataType.HeroSkills.HeroSkillBook.GetHeroSkillImage(nid), x, y, width, height);
                }
                else if (type == VirtualRegionCellType.CardQual)
                {
                    g.DrawImage(HSIcons.GetIconsByEName("gem" + nid), x, y, width, height);
                }
                else if (type == VirtualRegionCellType.Job)
                {
                    if (nid>0)
                    {
                        var jobConfig = ConfigData.GetJobConfig(nid + JobConfig.Indexer.NewBie);
                        Brush brush = new SolidBrush(Color.FromName(jobConfig.Color));
                        g.FillRectangle(brush, x, y, width, height);
                        g.DrawImage(HSIcons.GetIconsByEName("job" + nid), x, y, width, height);
                        brush.Dispose();
                    }
                }
                else if (type == VirtualRegionCellType.Task)
                {
                    string sicon = "oth3";
                    TaskConfig taskConfig = ConfigData.GetTaskConfig(nid);
                    int tstate = DataType.User.UserProfile.InfoTask.GetTaskStateById(nid);
                    if (tstate > 0)
                    {
                        SolidBrush sb = new SolidBrush(Color.FromName(HSTypes.I2TaskStateColor(tstate)));
                        g.FillRectangle(sb, x + 2, y + 2, width - 4, height - 4);
                        sb.Dispose();
                        sicon = taskConfig.Icon;
                    }
                    if (taskConfig.Main == 1)
                        g.DrawRectangle(Pens.Red, x + 2, y + 2, width - 4, height - 4);
                    g.DrawImage(HSIcons.GetIconsByEName(sicon), x, y, width, height);
                }
            }

            foreach (IRegionDecorator decorator in decorators)
            {
                decorator.Draw(g);
            }
        }

        public override void SetKeyValue(int value)
        {
            base.SetKeyValue(value);
            nid = value;
        }

        public override int GetKeyValue()
        {
            return nid;
        }

        public override void SetType(VirtualRegionCellType value)
        {
            type = value;
        }
    }
}
