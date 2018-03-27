using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.Players.Frag;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Cards;
using TaleofMonsters.Datas.HeroPowers;
using TaleofMonsters.Datas.Skills;

namespace TaleofMonsters.Datas.Equips
{
    public class Equip
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

        public List<RLIdValue> CommonSkillList = new List<RLIdValue>();

        public Equip()
        {
        }

        public Equip(int id)
        {
            TemplateId = id;

            EquipConfig equipConfig = ConfigData.GetEquipConfig(TemplateId);
            if (equipConfig.CommonSkillId > 0)
                CommonSkillList.Add(new RLIdValue { Id = equipConfig.CommonSkillId, Value = equipConfig.CommonSkillRate });

            UpgradeToLevel(1);
        }

        public void UpgradeToLevel(int lv)
        {
            level = lv;

            EquipConfig equipConfig = ConfigData.GetEquipConfig(TemplateId);
            LpRate = equipConfig.EnergyRate[0];
            PpRate = equipConfig.EnergyRate[1];
            MpRate = equipConfig.EnergyRate[2];

            var standardValue = CardAssistant.GetCardModify(3, level, (CardQualityTypes)equipConfig.Quality, 0);
            Atk = (int)(standardValue * (0 + equipConfig.AtkP*0.6) / 100); //200
            Hp = (int)(standardValue * (0 + equipConfig.VitP*2.4) / 100 * 5); //200

            Def = equipConfig.Def;
            Mag = equipConfig.Mag;
            Spd = equipConfig.Spd;
            Hit = equipConfig.Hit;
            Dhit = equipConfig.Dhit;
            Crt = equipConfig.Crt;
            Luk = equipConfig.Luk;

            Range = equipConfig.Range;
        }

        internal List<EquipModifier.EquipModifyState> GetEquipAddons()
        {
            var addons = new List<EquipModifier.EquipModifyState>();
            if (Atk > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.AtkRate) , Value = Atk });
            if (Hp > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.HpRate), Value = Hp });
            if (Def > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.Def), Value = Def });
            if (Mag > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.Mag), Value = Mag });
            if (Spd > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.Spd), Value = Spd });
            if (Hit > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.Hit), Value = Hit });
            if (Dhit > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.Dhit), Value = Dhit });
            if (Crt > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.Crt), Value = Crt });
            if (Luk > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.Luk), Value = Luk });
            if (Range > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.Range), Value = Range });
            return addons;
        }

        public Image GetPreview()
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(TemplateId);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(string.Format("{0} Lv{1}", equipConfig.Name, level), HSTypes.I2QualityColor(equipConfig.Quality), 20);
            tipData.AddTextNewLine(string.Format("       位置:{0}", HSTypes.I2EquipSlotType(equipConfig.Position)), "White");
            tipData.AddTextNewLine("", "White");

            foreach (var equipAddon in GetEquipAddons())
            {
                EquipAddonConfig eAddon = ConfigData.GetEquipAddonConfig(equipAddon.Id);
                tipData.AddTextNewLine(string.Format(eAddon.Format, equipAddon.Value), HSTypes.I2EaddonColor(eAddon.Rare));
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
                if (equipConfig.Des.Length > 15)
                {
                    tipData.AddTextNewLine(equipConfig.Des.Substring(0, 14), "Lime");
                    tipData.AddTextNewLine(equipConfig.Des.Substring(14), "Lime");
                }
                else
                {
                    tipData.AddTextNewLine(equipConfig.Des, "Lime");
                }
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
                if (tp.Length > 15)
                {
                    tipData.AddText(tp.Substring(0, 14), "White");
                    tipData.AddTextNewLine(tp.Substring(14), "White");
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
                if (tp.Length > 15)
                {
                    tipData.AddText(tp.Substring(0, 14), "White");
                    tipData.AddTextNewLine(tp.Substring(14), "White");
                }
                else
                {
                    tipData.AddText(tp, "White");
                }
            }
            tipData.AddImageXY(EquipBook.GetEquipImage(TemplateId), 8, 8, 48, 48, 7, 24, 30, 30);
            return tipData.Image;
        }
    }
}