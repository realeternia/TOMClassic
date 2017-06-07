namespace ConfigDatas
{
    public interface IMonster
    {
        int Id{get;}
        int Star { get; }
        int Level { get; }
        int CardId{get;}
        void AddHp(double addon);
        void AddHpRate(double value);
        double HpRate{get;}
        int Hp{get;}
        int WeaponId{get;}
        int WeaponType { get; }	//1,2,3,4
        IPlayer Owner{get;}
        IPlayer Rival { get; }
        bool IsDefence { get; }
        System.Drawing.Point Position{get;set;}
        IMap Map { get; }
        bool IsLeft { get; }

        IMonsterAuro AddAuro(int buff, int lv, string tar);
        void AddAntiMagic(string type, int value);
        void AddBuff(int buffId, int blevel, double dura);
        void AddItem(int itemId);//战斗中
        void Transform(int monId);
        void AddActionRate(double value);
        void Return(int costChange);
        void AddWeapon(int weaponId, int lv);
        void StealWeapon(IMonster target);
        void BreakWeapon();
        void WeaponReturn();
        void LevelUpWeapon(int lv);
        void AddRandSkill();
        void AddSkill(int id, int rate);
        int GetMonsterCountByRace(int rid);
        int GetMonsterCountByType(int type);
        void AddMissile(IMonster target, string arrow);
		
        AttrModifyData Atk{get;set;}
        AttrModifyData MaxHp { get; set; }

        AttrModifyData Def { get; set; }
        AttrModifyData Mag { get; set; }
        AttrModifyData Spd { get; set; }
        AttrModifyData Hit { get; set; }
        AttrModifyData Dhit { get; set; }
        AttrModifyData Crt { get; set; }
        AttrModifyData Luk { get; set; }

        double CrtDamAddRate { get; set; } //暴击时伤害倍数增加

        bool IsTileMatching { get; }
        bool CanAttack { get; set; }
        bool IsElement(string ele);
        bool IsRace(string rac);
        int AttackType { get; }
        bool HasSkill(int sid);
        void Silent();
        int Attr { get; } //属性
        int Type { get; } //种族

        bool IsNight{get;}
        void ClearDebuff();
        void ExtendDebuff(double count);
        bool HasBuff(int id);
        void SetToPosition(string type, int step);
        void OnMagicDamage(IMonster source, double damage, int element);
        void SuddenDeath();
        void Rebel();//造反
        void AddMaxHp(double val);
        int MovRound { get; } //连续移动回合数，攻击后清除

        void Summon(string type, int id, int count);
        void SummonRandomAttr(string type, int attr);
        void MadDrug(); //交换攻击和血量
        void CureRandomAlien(double rate);
        bool ResistBuffType(int type);
        void EatTomb(IMonster tomb);
        void ClearTarget();
        void AddPArmor(double val);
        void AddMArmor(double val);
        int GetPArmor();
    }
}