using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Tools;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.HeroPowers;
using TaleofMonsters.Datas.Skills;
using TaleofMonsters.Datas.User;

namespace TaleofMonsters.Datas.Equips
{
    internal class Equip
    {
        public int TemplateId { get; set; }//装备的id

        private int level;

        public int Atk { get; set; }//实际的攻击力
        public int Hp { get; set; }

        public int Def { get; set; }
        public int Mag { get; set; }
        public int Spd { get; set; }
        public int Hit { get; set; }
        public int Dhit { get; set; }
        public int Crt { get; set; }
        public int Luk { get; set; }

        public int Range { get; set; }

        public int LpRate { get; set; }
        public int PpRate { get; set; }
        public int MpRate { get; set; }

        public int Dura { get; set; } //实际的耐久值
        public int ExpireTime { get; set; } //过期时间

        public List<RLIdValue> CommonSkillList = new List<RLIdValue>();

        public Equip()
        {
        }

        public Equip(int id)
        {
            TemplateId = id;

            UpgradeToLevel();
        }

        public void UpgradeToLevel()
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(TemplateId);
            LpRate = equipConfig.EnergyRate[0];
            PpRate = equipConfig.EnergyRate[1];
            MpRate = equipConfig.EnergyRate[2];

            level = ConfigData.GetLevelExpConfig(UserProfile.Profile.InfoBasic.Level).TowerLevel;
            var heroData = new Monster(MonsterConfig.Indexer.KingTowerId);
            heroData.UpgradeToLevel(level);
            Atk = heroData.Atk * equipConfig.AtkR / 100;
            Hp = heroData.Hp * equipConfig.VitR / 100;

            Def = equipConfig.Def;
            Mag = equipConfig.Mag;
            Spd = equipConfig.Spd;
            Hit = equipConfig.Hit;
            Dhit = equipConfig.Dhit;
            Crt = equipConfig.Crt;
            Luk = equipConfig.Luk;

            Range = equipConfig.Range;

            if (equipConfig.CommonSkillId > 0)
                CommonSkillList.Add(new RLIdValue {Id = equipConfig.CommonSkillId, Value = equipConfig.CommonSkillRate});
        }

        public Image GetPreview()
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(TemplateId);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(equipConfig.Name, HSTypes.I2QualityColor(equipConfig.Quality), 20);
            tipData.AddTextNewLine(string.Format("       位置:{0}", HSTypes.I2EquipSlotType(equipConfig.Position)), "White");
            tipData.AddTextNewLine("", "White");
            if (Atk > 0)
            {
                EquipAddonConfig eAddon = ConfigData.GetEquipAddonConfig((int) (EquipAttrs.AtkRate+ 1));
                tipData.AddTextNewLine(string.Format(eAddon.Format, equipConfig.AtkR), HSTypes.I2EaddonColor(eAddon.Rare));
            }
            if (Hp > 0)
            {
                EquipAddonConfig eAddon = ConfigData.GetEquipAddonConfig((int)(EquipAttrs.HpRate + 1));
                tipData.AddTextNewLine(string.Format(eAddon.Format, equipConfig.VitR), HSTypes.I2EaddonColor(eAddon.Rare));
            }
            if (Def > 0)
            {
                EquipAddonConfig eAddon = ConfigData.GetEquipAddonConfig((int)(EquipAttrs.Def + 1));
                tipData.AddTextNewLine(string.Format(eAddon.Format, equipConfig.Def), HSTypes.I2EaddonColor(eAddon.Rare));
            }
            if (Mag > 0)
            {
                EquipAddonConfig eAddon = ConfigData.GetEquipAddonConfig((int)(EquipAttrs.Mag + 1));
                tipData.AddTextNewLine(string.Format(eAddon.Format, equipConfig.Mag), HSTypes.I2EaddonColor(eAddon.Rare));
            }
            if (Spd > 0)
            {
                EquipAddonConfig eAddon = ConfigData.GetEquipAddonConfig((int)(EquipAttrs.Spd + 1));
                tipData.AddTextNewLine(string.Format(eAddon.Format, equipConfig.Spd), HSTypes.I2EaddonColor(eAddon.Rare));
            }
            if (Hit > 0)
            {
                EquipAddonConfig eAddon = ConfigData.GetEquipAddonConfig((int)(EquipAttrs.Hit + 1));
                tipData.AddTextNewLine(string.Format(eAddon.Format, equipConfig.Hit), HSTypes.I2EaddonColor(eAddon.Rare));
            }
            if (Dhit > 0)
            {
                EquipAddonConfig eAddon = ConfigData.GetEquipAddonConfig((int)(EquipAttrs.Dhit + 1));
                tipData.AddTextNewLine(string.Format(eAddon.Format, equipConfig.Dhit), HSTypes.I2EaddonColor(eAddon.Rare));
            }
            if (Crt > 0)
            {
                EquipAddonConfig eAddon = ConfigData.GetEquipAddonConfig((int)(EquipAttrs.Crt + 1));
                tipData.AddTextNewLine(string.Format(eAddon.Format, equipConfig.Crt), HSTypes.I2EaddonColor(eAddon.Rare));
            }
            if (Luk > 0)
            {
                EquipAddonConfig eAddon = ConfigData.GetEquipAddonConfig((int)(EquipAttrs.Luk + 1));
                tipData.AddTextNewLine(string.Format(eAddon.Format, equipConfig.Luk), HSTypes.I2EaddonColor(eAddon.Rare));
            }
            if (Range > 0)
            {
                EquipAddonConfig eAddon = ConfigData.GetEquipAddonConfig((int)(EquipAttrs.Range + 1));
                tipData.AddTextNewLine(string.Format(eAddon.Format, equipConfig.Range), HSTypes.I2EaddonColor(eAddon.Rare));
            }
            if (equipConfig.SlotId != null && equipConfig.SlotId.Length > 0)
            {
                string availStr = "开启:";
                foreach (var availId in equipConfig.SlotId)
                    availStr += ConfigData.GetEquipSlotConfig(availId).Name + " ";
                tipData.AddTextNewLine(availStr, "Lime");
            }
            if (!string.IsNullOrEmpty(equipConfig.Des))
            {
                tipData.AddTextNewLine(equipConfig.Des, "Lime");
            }
            if (equipConfig.EnergyRate[0] != 0 || equipConfig.EnergyRate[1] != 0 || equipConfig.EnergyRate[2] != 0)
            {
                tipData.AddLine();
                tipData.AddTextNewLine("能量回复比率修正", "White");
                tipData.AddTextNewLine(string.Format(" LP {0} ", equipConfig.EnergyRate[0].ToString().PadLeft(3,' ')), "Gold");
                tipData.AddBarTwo(100, equipConfig.EnergyRate[0], Color.Yellow, Color.Gold);
                tipData.AddTextNewLine(string.Format(" PP {0} ", equipConfig.EnergyRate[1].ToString().PadLeft(3, ' ')), "Red");
                tipData.AddBarTwo(100, equipConfig.EnergyRate[1], Color.Pink, Color.Red);
                tipData.AddTextNewLine(string.Format(" MP {0} ", equipConfig.EnergyRate[2].ToString().PadLeft(3, ' ')), "Blue");
                tipData.AddBarTwo(100, equipConfig.EnergyRate[2], Color.Cyan, Color.Blue);
            }
            if (equipConfig.DungeonAttrs != null && equipConfig.DungeonAttrs.Length > 0)
            {
                tipData.AddLine();
                tipData.AddTextNewLine("副本判定属性", "White");
                if (equipConfig.DungeonAttrs[0] != 0)
                    tipData.AddTextNewLine(string.Format(" 力量 {0}{1}", equipConfig.DungeonAttrs[0] > 0 ? "+":"",
                        equipConfig.DungeonAttrs[0]), "IndianRed");
                if (equipConfig.DungeonAttrs[1] != 0)
                    tipData.AddTextNewLine(string.Format(" 敏捷 {0}{1}", equipConfig.DungeonAttrs[1] > 0 ? "+" : "",
                        equipConfig.DungeonAttrs[1]), "LawnGreen");
                if (equipConfig.DungeonAttrs[2] != 0)
                    tipData.AddTextNewLine(string.Format(" 智慧 {0}{1}", equipConfig.DungeonAttrs[2] > 0 ? "+" : "",
                        equipConfig.DungeonAttrs[2]), "DeepSkyBlue");
                if (equipConfig.DungeonAttrs[3] != 0)
                    tipData.AddTextNewLine(string.Format(" 感知 {0}{1}", equipConfig.DungeonAttrs[3] > 0 ? "+" : "",
                        equipConfig.DungeonAttrs[3]), "MediumPurple");
                if (equipConfig.DungeonAttrs[4] != 0)
                    tipData.AddTextNewLine(string.Format(" 耐力 {0}{1}", equipConfig.DungeonAttrs[4] > 0 ? "+" : "",
                        equipConfig.DungeonAttrs[4]), "Bisque");
            }
            if (equipConfig.HeroSkillId > 0)
            {
                tipData.AddLine();
                tipData.AddImageNewLine(HeroPowerBook.GetImage(equipConfig.HeroSkillId));
                var skillConfig = ConfigData.GetHeroPowerConfig(equipConfig.HeroSkillId);
                string tp = string.Format("{0}:{1}", skillConfig.Name, skillConfig.Des);
                if (tp.Length > 12)
                {
                    tipData.AddText(tp.Substring(0, 11), "White");
                    tipData.AddTextNewLine(tp.Substring(11), "White");
                }
                else
                {
                    tipData.AddText(tp, "White");
                }
            }
            if (equipConfig.CommonSkillId > 0)
            {
                tipData.AddLine();
                tipData.AddImageNewLine(SkillBook.GetSkillImage(equipConfig.CommonSkillId));
                var skillConfig = ConfigData.GetSkillConfig(equipConfig.CommonSkillId);
                string tp = string.Format("{0}(被动)({2}%):{1}", skillConfig.Name, skillConfig.GetDescript(level), equipConfig.CommonSkillRate);
                if (tp.Length > 12)
                {
                    tipData.AddText(tp.Substring(0, 11), "White");
                    tipData.AddTextNewLine(tp.Substring(11), "White");
                }
                else
                {
                    tipData.AddText(tp, "White");
                }
            }
            tipData.AddLine();
            if (Dura > 0)//实例化了
                tipData.AddTextNewLine(string.Format("耐久:{0}/{1}", Dura, equipConfig.Durable), "White");
            else
                tipData.AddTextNewLine(string.Format("最大耐久:{0}", equipConfig.Durable), "White");
            if (ExpireTime > 0)//存在过期
            {
                var expireTime = TimeTool.UnixTimeToDateTime(ExpireTime);
                if (DateTime.Now >= expireTime.AddSeconds(-60))
                    tipData.AddTextNewLine("即将过期", "Red");
                else
                {
                    var timeDiffer = expireTime - DateTime.Now;
                    tipData.AddTextNewLine(string.Format("过期:{0}天{1}时{2}分", timeDiffer.Days, timeDiffer.Hours, timeDiffer.Minutes), "White");
                }
            }
            tipData.AddLine();
            tipData.AddTextNewLine(string.Format("出售价格:{0}", equipConfig.Value), "Yellow");
            tipData.AddImage(HSIcons.GetIconsByEName("res1"));
            tipData.AddImageXY(EquipBook.GetEquipImage(TemplateId), 8, 8, 48, 48, 7, 24, 30, 30);
            return tipData.Image;
        }
    }
}