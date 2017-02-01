using System.Drawing;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Interface;
using ConfigDatas;
using TaleofMonsters.DataType.Scenes;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class PictureRegion : SubVirtualRegion
    {
        protected PictureRegionCellType type;
        protected int nid;//特征id，比如卡的id，npc的id等等

        public PictureRegion(int id, int x, int y, int width, int height, PictureRegionCellType type, int nid)
            : base(id, x, y, width, height)
        {
            this.nid = nid;
            this.type = type;
            Scale = 1;
        }

        public override void Draw(Graphics g)
        {
            if (nid > 0)
            {
                Image img = null;
                HsActionCallback action = null;
                if (type == PictureRegionCellType.Npc)
                {
                    img = SceneBook.GetSceneNpcImage(nid);
                }
                else if (type == PictureRegionCellType.Item)
                {
                    img = DataType.Items.HItemBook.GetHItemImage(nid);
                    action = () =>
                    {
                        var itemConfig = ConfigData.GetHItemConfig(nid);
                        var pen = new Pen(Color.FromName(HSTypes.I2RareColor(itemConfig.Rare)), 2);
                        g.DrawRectangle(pen, X, Y, Width, Height);
                        pen.Dispose();
                    };
                }
                else if (type == PictureRegionCellType.Equip)
                {
                    img = DataType.Equips.EquipBook.GetEquipImage(nid);
                    action = () =>
                    {
                        var equipConfig = ConfigData.GetEquipConfig(nid);
                        var pen = new Pen(Color.FromName(HSTypes.I2QualityColor(equipConfig.Quality)), 2);
                        g.DrawRectangle(pen, X, Y, Width, Height);
                        pen.Dispose();
                    };
                }
                else if (type == PictureRegionCellType.Card)
                {
                    img = DataType.Cards.CardAssistant.GetCardImage(nid, 60, 60);
                    action = () =>
                    {
                        var cardData = CardConfigManager.GetCardConfig(nid);
                        string cardBorder = DataType.Cards.CardAssistant.GetCardBorder(cardData);
                        g.DrawImage(PicLoader.Read("Border", cardBorder), X + 2, Y + 2, Width - 4, Height - 4);
                    };
                }
                else if (type == PictureRegionCellType.SkillAttr)
                {
                    img = DataType.HeroSkills.HeroSkillAttrBook.GetHeroSkillAttrImage(nid);
                }
                else if (type == PictureRegionCellType.Achieve)
                {
                    img = DataType.Achieves.AchieveBook.GetAchieveImage(nid);
                }
                else if (type == PictureRegionCellType.People)
                {
                    img = DataType.Peoples.PeopleBook.GetPersonImage(nid);
                }
                else if (type == PictureRegionCellType.HeroSkill)
                {
                    img = DataType.HeroSkills.HeroSkillBook.GetHeroSkillImage(nid);
                }
                else if (type == PictureRegionCellType.CardQual)
                {
                    img = HSIcons.GetIconsByEName("gem" + nid);
                }
                else if (type == PictureRegionCellType.Job)
                {
                    img = HSIcons.GetIconsByEName("job" + nid);
                    action = () =>
                    {
                        var jobConfig = ConfigData.GetJobConfig(nid + JobConfig.Indexer.NewBie);
                        Pen pen = new Pen(Color.FromName(jobConfig.Color));
                        g.DrawRectangle(pen, X, Y, Width, Height);
                        pen.Dispose();
                    };
                }
                else if (type == PictureRegionCellType.Bless)
                {
                    img = DataType.Blesses.BlessBook.GetBlessImage(nid);
                }

                if (img != null)
                {
                    if (Scale == 1)
                    {
                        g.DrawImage(img, X, Y, Width, Height);
                    }
                    else
                    {
                        int realWidth = (int)(Width*Scale);
                        int realHeight = (int)(Height * Scale);
                        g.DrawImage(img, X + (Width- realWidth)/2, Y + (Height- realHeight)/2, realWidth, realHeight);
                    }
                }
                if (action != null)
                {
                    action();
                }
            }

            foreach (IRegionDecorator decorator in decorators)
            {
                decorator.Draw(g, X, Y, Width, Height);
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

        public void SetType(PictureRegionCellType value)
        {
            type = value;
        }

        public PictureRegionCellType GetVType()
        {
            return type;
        }

        public float Scale { get; set; } //中心图片缩放
    }

    internal enum PictureRegionCellType
    {
        Npc,
        Item,
        Equip,
        Card,
        SkillAttr,
        Achieve,
        HeroSkill,
        People,
        Task,
        CardQual,
        Job,
        Bless,
    }
}
