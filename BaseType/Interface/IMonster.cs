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
        IPlayer Owner{get;}
        IPlayer Rival { get; }
        bool IsHero{get;}
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
        void StealWeapon(IMonster target);
        void BreakWeapon();
        void WeaponReturn();
        void AddRandSkill();
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

        bool IsTileMatching { get; }
        bool CanAttack { get; set; }
        bool IsElement(string ele);
        bool IsRace(string rac);
        int AttackType { get; }
        bool HasSkill(int sid);
        void ForgetSkill();
        int Attr { get; } //属性
        int Type { get; } //种族

        bool IsNight{get;}
        bool HasScroll { get; }//是否拿着卷轴 		
        void ClearDebuff();
        void ExtendDebuff(double count);
        bool HasBuff(int id);
        void SetToPosition(string type);
        void OnMagicDamage(double damage, int element);
        void SuddenDeath();
        void Rebel();//造反
        void AddMaxHp(double val);

        void Summon(int type, int id);
        void MadDrug(); //交换攻击和血量
    }
}