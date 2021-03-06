﻿using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.Players.Frag;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Cards;
using TaleofMonsters.Datas.HeroPowers;
using TaleofMonsters.Datas.Skills;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Datas.Equips
{
    public class Equip
    {
        public int TemplateId { get; set; }//装备的id

        private int level;

        private int atk;//实际的攻击力
        private int hp;
        private int attr; //辅助属性

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
            var standardValue = CardAssistant.GetCardModify(3, level, (QualityTypes)equipConfig.Quality, 0);
            atk = (int)(standardValue * (0 + equipConfig.AtkP*0.6) / 100); //200
            hp = (int)(standardValue * (0 + equipConfig.VitP*2.4)*5 / 100); //200
            attr = (int)(standardValue * (0 + equipConfig.Attr * 5) / 100); //200
        }

        internal List<EquipModifier.EquipModifyState> GetEquipAddons(bool getLuck)
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(TemplateId);
            var addons = new List<EquipModifier.EquipModifyState>();
            if (equipConfig.AtkP > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.AtkRate), Value = Math.Max(1, atk) });
            if (equipConfig.VitP > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.HpRate), Value = Math.Max(1, hp) });
            if (equipConfig.Def > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.Def), Value = equipConfig.Def });
            if (equipConfig.Mag > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.Mag), Value = equipConfig.Mag });
            if (equipConfig.Spd > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.Spd), Value = equipConfig.Spd });
            if (equipConfig.Hit > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.Hit), Value = equipConfig.Hit });
            if (equipConfig.Dhit > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.Dhit), Value = equipConfig.Dhit });
            if (equipConfig.Crt > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.Crt), Value = equipConfig.Crt });
            if (equipConfig.Luk > 0 && getLuck)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.Luk), Value = equipConfig.Luk });
            if (equipConfig.Range > 0)
                addons.Add(new EquipModifier.EquipModifyState { Id = (int)(EquipAttrs.Range), Value = equipConfig.Range });

            if (equipConfig.AttrId > 0)
            {
                var attrConfig = ConfigData.GetEquipAddonConfig(equipConfig.AttrId);
                addons.Add(new EquipModifier.EquipModifyState {Id = equipConfig.AttrId, Value = Math.Max(1, (int)(attr* attrConfig.Attr))});
            }
            return addons;
        }

        public Image GetPreview()
        {
            EquipConfig equipConfig = ConfigData.GetEquipConfig(TemplateId);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage(PaintTool.GetTalkColor);
            tipData.AddTextNewLine(string.Format("{0} Lv{1}", equipConfig.Name, level), HSTypes.I2QualityColor(equipConfig.Quality), 20);
            tipData.AddTextNewLine(string.Format("       位置:{0}", HSTypes.I2EquipSlotType(equipConfig.Position)), "White");
            tipData.AddTextNewLine("", "White");

            foreach (var equipAddon in GetEquipAddons(true))
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
                tipData.AddTextLines(tp, "White", 15, false);
            }
            if (equipConfig.CommonSkillId > 0)
            {
                tipData.AddLine();
                tipData.AddImageNewLine(SkillBook.GetSkillImage(equipConfig.CommonSkillId));
                var skillConfig = ConfigData.GetSkillConfig(equipConfig.CommonSkillId);
                string tp = string.Format("{0}(被动)({2}%):{1}", skillConfig.Name, skillConfig.GetDescript(level), equipConfig.CommonSkillRate);
                tipData.AddTextLines(tp, "White", 15, false);
            }
            if (!string.IsNullOrEmpty(equipConfig.Des))
            {
                tipData.AddLine();
                tipData.AddTextNewLine(equipConfig.Des, "Gold");
            }
            tipData.AddImageXY(EquipBook.GetEquipImage(TemplateId), 8, 8, 48, 48, 7, 24, 30, 30);
            return tipData.Image;
        }
    }
}