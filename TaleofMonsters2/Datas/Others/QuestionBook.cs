using ConfigDatas;
using NarlonLib.Core;
using NarlonLib.Math;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Cards.Spells;
using TaleofMonsters.Datas.Cards.Weapons;
using TaleofMonsters.Datas.Skills;

namespace TaleofMonsters.Datas.Others
{
    internal static class QuestionBook
    {
        public static Question GetQuestion()
        {
            int type = MathTool.GetRandom(8) + 1;
            switch ((QuestionTypes)type)
            {
                case QuestionTypes.MonsterInfo: return GetQuestionMonsterInfo();
                case QuestionTypes.InfoMonster: return GetQuestionInfoMonster();
                case QuestionTypes.WeaponInfo: return GetQuestionWeaponInfo();
                case QuestionTypes.InfoWeapon: return GetQuestionInfoWeapon();
                case QuestionTypes.SpellInfo: return GetQuestionSpellInfo();
                case QuestionTypes.InfoSpell: return GetQuestionInfoSpell();
                case QuestionTypes.SkillInfo: return GetQuestionSkillInfo();
                case QuestionTypes.InfoSkill: return GetQuestionInfoSkill();
            }

            return new Question();
        }

        static Question GetQuestionMonsterInfo()
        {
            string[] eids = { "hp", "race", "star", "type","atk","def"};
            string[] cids = { "初始生命值", "种族", "星级", "属性", "初始攻击", "初始防御" };
            int type = MathTool.GetRandom(eids.Length);

            int monsterId = MonsterBook.GetRandMonsterId();
            Question question = new Question();
            question.info = string.Format("|怪物|Red|{0}||的{1}是?", ConfigData.GetMonsterConfig(monsterId).Name, cids[type]);
            question.ans = new string[4];
            question.ans[MathTool.GetRandom(4)] = MonsterBook.GetAttrByString(monsterId, eids[type]);
            int idx = 0;
            SimpleSet<string> set = new SimpleSet<string>();
            set.Add(MonsterBook.GetAttrByString(monsterId, eids[type]));
            while (idx < 4)
            {
                if (question.ans[idx] != null)
                {
                    idx++;
                    continue;
                }

                int guessId = MonsterBook.GetRandMonsterId();
                string guessStr = MonsterBook.GetAttrByString(guessId, eids[type]);
                if (!set.Has(guessStr))
                {
                    question.ans[idx] = guessStr;
                    set.Add(guessStr);
                    idx++;
                }
            }
            question.result = MonsterBook.GetAttrByString(monsterId, eids[type]);
            return question;
        }

        static Question GetQuestionInfoMonster()
        {
            string[] eids = { "hp", "race", "star", "type", "atk", "def" };
            string[] cids = { "初始生命值", "种族", "星级", "属性", "初始攻击", "初始防御" };
            int type = MathTool.GetRandom(eids.Length);

            MonsterConfig monsterConfig = ConfigData.GetMonsterConfig(MonsterBook.GetRandMonsterId());
            Question question = new Question();
            question.info = string.Format("|以下哪一个怪物的{0}是|Yellow|{1}||?", cids[type], MonsterBook.GetAttrByString(monsterConfig.Id, eids[type]));
            question.ans = new string[4];
            question.ans[MathTool.GetRandom(4)] = monsterConfig.Name;
            int idx = 0;
            SimpleSet<string> set = new SimpleSet<string>();
            set.Add(MonsterBook.GetAttrByString(monsterConfig.Id, eids[type]));
            while (idx < 4)
            {
                if (question.ans[idx] != null)
                {
                    idx++;
                    continue;
                }

                MonsterConfig guessConfig = ConfigData.GetMonsterConfig(MonsterBook.GetRandMonsterId());
                if (!set.Has(guessConfig.Name) && MonsterBook.GetAttrByString(monsterConfig.Id, eids[type]) != MonsterBook.GetAttrByString(guessConfig.Id, eids[type]))
                {
                    question.ans[idx] = guessConfig.Name;
                    set.Add(guessConfig.Name);
                    idx++;
                }
            }
            question.result = monsterConfig.Name;
            return question;
        }

        static Question GetQuestionWeaponInfo()
        {
            string[] eids = { "atf", "skill", "star", "attr" };
            string[] cids = { "初始攻/防", "技能", "星级", "属性" };
            int type = MathTool.GetRandom(eids.Length);

            int weaponId = WeaponBook.GetRandWeaponId();
            Question question = new Question();
            question.info = string.Format("|武器|Green|{0}||的{1}是?", ConfigData.GetWeaponConfig(weaponId).Name, cids[type]);
            question.ans = new string[4];
            string weaponStr = WeaponBook.GetAttrByString(weaponId, eids[type]);
            question.ans[MathTool.GetRandom(4)] = weaponStr;
            int idx = 0;
            SimpleSet<string> set = new SimpleSet<string>();
            set.Add(weaponStr);
            while (idx < 4)
            {
                if (question.ans[idx] != null)
                {
                    idx++;
                    continue;
                }

                int guessId = WeaponBook.GetRandWeaponId();
                string guessStr = WeaponBook.GetAttrByString(guessId, eids[type]);
                if (!set.Has(guessStr))
                {
                    question.ans[idx] = guessStr;
                    set.Add(guessStr);
                    idx++;
                }
            }
            question.result = weaponStr;
            return question;
        }

        static Question GetQuestionInfoWeapon()
        {
            string[] eids = { "atf", "skill", "star", "attr" };
            string[] cids = { "初始攻/防", "技能", "星级", "属性" };
            int type = MathTool.GetRandom(eids.Length);

            int weaponId = WeaponBook.GetRandWeaponId();
            Question question = new Question();
            question.info = string.Format("|以下哪一个武器的{0}是|Yellow|{1}||?", cids[type], WeaponBook.GetAttrByString(weaponId, eids[type]));
            question.ans = new string[4];
            question.ans[MathTool.GetRandom(4)] = ConfigData.GetWeaponConfig(weaponId).Name;
            int idx = 0;
            SimpleSet<string> set = new SimpleSet<string>();
            set.Add(WeaponBook.GetAttrByString(weaponId, eids[type]));
            while (idx < 4)
            {
                if (question.ans[idx] != null)
                {
                    idx++;
                    continue;
                }

                int guessId = WeaponBook.GetRandWeaponId();
                if (!set.Has(ConfigData.GetWeaponConfig(guessId).Name) && WeaponBook.GetAttrByString(guessId, eids[type]) != WeaponBook.GetAttrByString(weaponId, eids[type]))
                {
                    question.ans[idx] = ConfigData.GetWeaponConfig(guessId).Name;
                    set.Add(ConfigData.GetWeaponConfig(guessId).Name);
                    idx++;
                }
            }
            question.result = ConfigData.GetWeaponConfig(weaponId).Name;
            return question;
        }

        static Question GetQuestionSpellInfo()
        {
            string[] eids = { "star", "des"};
            string[] cids = { "星级", "描述"};
            int type = MathTool.GetRandom(eids.Length);

            int spellId = SpellBook.GetRandSpellId();
            Question question = new Question();
            question.info = string.Format("|魔法|Blue|{0}||的{1}是?", ConfigData.GetSpellConfig(spellId).Name, cids[type]);
            question.ans = new string[4];
            string attrStr = SpellBook.GetAttrByString(spellId, eids[type]);
            question.ans[MathTool.GetRandom(4)] = attrStr;
            int idx = 0;
            SimpleSet<string> set = new SimpleSet<string>();
            set.Add(attrStr);
            while (idx < 4)
            {
                if (question.ans[idx] != null)
                {
                    idx++;
                    continue;
                }

                int guessId = SpellBook.GetRandSpellId();
                string guessStr = SpellBook.GetAttrByString(guessId, eids[type]);
                if (!set.Has(guessStr))
                {
                    question.ans[idx] = guessStr;
                    set.Add(guessStr);
                    idx++;
                }
            }
            question.result = attrStr;
            return question;
        }

        static Question GetQuestionInfoSpell()
        {
            string[] eids = { "star", "des" };
            string[] cids = { "星级", "描述" };
            int type = MathTool.GetRandom(eids.Length);

            int spellId = SpellBook.GetRandSpellId();
            string attrStr = SpellBook.GetAttrByString(spellId, eids[type]);
            Question question = new Question();
            question.info = string.Format("|以下哪一个魔法的{0}是|Yellow|{1}||?", cids[type], attrStr);
            question.ans = new string[4];
            question.ans[MathTool.GetRandom(4)] = ConfigData.GetSpellConfig(spellId).Name;
            int idx = 0;
            SimpleSet<string> set = new SimpleSet<string>();
            set.Add(attrStr);
            while (idx < 4)
            {
                if (question.ans[idx] != null)
                {
                    idx++;
                    continue;
                }

                int guessId = SpellBook.GetRandSpellId();
                string guessStr = SpellBook.GetAttrByString(guessId, eids[type]);
                if (!set.Has(ConfigData.GetSpellConfig(guessId).Name) && guessStr != attrStr)
                {
                    question.ans[idx] = ConfigData.GetSpellConfig(guessId).Name;
                    set.Add(ConfigData.GetSpellConfig(guessId).Name);
                    idx++;
                }
            }
            question.result = ConfigData.GetSpellConfig(spellId).Name;
            return question;
        }

        static Question GetQuestionSkillInfo()
        {
            string[] eids = { "type", "des" };
            string[] cids = { "类型", "描述" };
            int type = MathTool.GetRandom(eids.Length);

            SkillConfig luk = ConfigData.GetSkillConfig(SkillBook.GetRandSkillId());
            Question question = new Question();
            question.info = string.Format("|技能|Gold|{0}||的{1}是?", luk.Name, cids[type]);
            question.ans = new string[4];
            string attrType = SkillBook.GetAttrByString(luk.Id, eids[type]);
            question.ans[MathTool.GetRandom(4)] = attrType;
            int idx = 0;
            SimpleSet<string> set = new SimpleSet<string>();
            set.Add(SkillBook.GetAttrByString(luk.Id, eids[type]));
            while (idx < 4)
            {
                if (question.ans[idx] != null)
                {
                    idx++;
                    continue;
                }

                SkillConfig guess = ConfigData.GetSkillConfig(SkillBook.GetRandSkillId());
                string guessType = SkillBook.GetAttrByString(guess.Id, eids[type]);
                if (!set.Has(guessType))
                {
                    question.ans[idx] = guessType;
                    set.Add(guessType);
                    idx++;
                }
            }
            question.result = attrType;
            return question;
        }

        static Question GetQuestionInfoSkill()
        {
            string[] eids = { "type", "des" };
            string[] cids = { "类型", "描述" };
            int type = MathTool.GetRandom(eids.Length);

            SkillConfig luk = ConfigData.GetSkillConfig(SkillBook.GetRandSkillId());
            Question question = new Question();
            string attrType = SkillBook.GetAttrByString(luk.Id, eids[type]);
            question.info = string.Format("|以下哪一个技能的{0}是|Yellow|{1}||?", cids[type], attrType);
            question.ans = new string[4];
            question.ans[MathTool.GetRandom(4)] = luk.Name;
            int idx = 0;
            SimpleSet<string> set = new SimpleSet<string>();
            set.Add(attrType);
            while (idx < 4)
            {
                if (question.ans[idx] != null)
                {
                    idx++;
                    continue;
                }

                SkillConfig guess = ConfigData.GetSkillConfig(SkillBook.GetRandSkillId());
                string guessType = SkillBook.GetAttrByString(guess.Id, eids[type]);
                if (!set.Has(guess.Name) && guessType != attrType)
                {
                    question.ans[idx] = guess.Name;
                    set.Add(guess.Name);
                    idx++;
                }
            }
            question.result = luk.Name;
            return question;
        }
    }
}
