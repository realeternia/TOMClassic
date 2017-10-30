using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Interface;
using ConfigDatas;
using NarlonLib.Control;
using TaleofMonsters.Controler.Battle;
using TaleofMonsters.DataType.Blesses;
using TaleofMonsters.DataType.Equips;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.Peoples;
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
                if (type == PictureRegionCellType.Item)
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
                    var size = 40;
                    if (Width > 100)
                        size = 100;
                    else if (Width > 60)
                        size = 60;
                    img = DataType.Cards.CardAssistant.GetCardImage(nid, size, size);
                    action = () =>
                    {
                        var cardData = CardConfigManager.GetCardConfig(nid);
                        string cardBorder = DataType.Cards.CardAssistant.GetCardBorder(cardData);
                        g.DrawImage(PicLoader.Read("Border", cardBorder), X, Y, Width, Height);
                    };
                }
                else if (type == PictureRegionCellType.Gismo)
                {
                    img = DataType.Scenes.DungeonBook.GetGismoImage(nid);
                }
                else if (type == PictureRegionCellType.People)
                {
                    img = DataType.Peoples.PeopleBook.GetPersonImage(nid);
                }
                else if (type == PictureRegionCellType.HeroSkill)
                {
                    img = DataType.HeroSkills.HeroPowerBook.GetImage(nid);
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
                else if (type == PictureRegionCellType.SceneQuest)
                {
                    img = SceneQuestBook.GetSceneQuestImage(nid);
                }
                if (img != null)
                {
                    if (Scale == 1)
                    {
                        if (Enabled)
                            g.DrawImage(img, X, Y, Width, Height);
                        else
                            g.DrawImage(img, new Rectangle(X, Y, Width, Height), 0,0,
                                img.Width,img.Height,GraphicsUnit.Pixel, HSImageAttributes.ToGray);
                    }
                    else
                    {
                        int realWidth = (int)(Width*Scale);
                        int realHeight = (int)(Height * Scale);
                        g.DrawImage(img, X + (Width - realWidth) / 2, Y + (Height - realHeight) / 2, realWidth, realHeight);
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

        public override void ShowTip(ImageToolTip tooltip, Control form, int x, int y)
        {
            var regionType = GetVType();
            if (regionType == PictureRegionCellType.Item)
            {
                Image image = HItemBook.GetPreview(nid);
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == PictureRegionCellType.Equip)
            {
                Equip equip = new Equip(nid);
                Image image = equip.GetPreview();
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == PictureRegionCellType.Bless)
            {
                Image image = BlessBook.GetPreview(nid);
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == PictureRegionCellType.Gismo)
            {
                Image image = DungeonBook.GetPreview(nid);
                tooltip.Show(image, form, x, y);
            }
            else if (regionType == PictureRegionCellType.People)
            {
                Image image = PeopleBook.GetPreview(nid);
                tooltip.Show(image, form, x, y);
            }
        }
    }

    internal enum PictureRegionCellType
    {
        Item,
        Equip,
        Card,
        Gismo,
        HeroSkill,
        People,
        CardQual,
        Job,
        Bless,
        SceneQuest,
    }
}
