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
        int MaxHp{get;set;}
        int Hp{get;}
        int WeaponId{get;}
        IPlayer Owner{get;}
        IPlayer Rival { get; }
        bool IsHero{get;}		
        System.Drawing.Point Position{get;set;}
        IMap Map { get; }
        bool IsLeft { get; }

        IMonsterAuro AddAuro(int buff, int lv, string tar);
        void AddAntiMagic(string type, int value);
        void AddBuff(int buffId, int blevel, double dura);
        void AddItem(int itemId);//战斗中
        void Transform(int monId);
        void AddCardRate(int monId, int rate);
        void AddResource(int type,int count);
        void AddActionRate(double value);
        void StealWeapon(IMonster target);
        void BreakWeapon();
        void WeaponReturn(int type);
        void AddRandSkill();
        void AddSpecialMark(SkillMarks mark);
        int GetMonsterCountByRace(int rid);
        int GetMonsterCountByType(int type);
        void AddMissile(IMonster target, string arrow);
		
        AttrModifyData Atk{get;set;}

        bool IsTileMatching { get; }
        bool CanAttack { get; set; }
        bool IsElement(string ele);
        bool IsRace(string rac);
        bool IsMagicAtk { get; }
        int AttackType { get; }
        bool HasSkill(int sid);
        void ForgetSkill();
        int Attr { get; }
        int Type { get; } //分类

        int SkillParm{get;set;}
        bool DropAdd{get;set;}
 		
        bool IsNight{get;}
        bool HasScroll { get; }//是否拿着卷轴
        int CardNumber{get;} 	
 		
        void ClearDebuff();
        void ExtendDebuff(double count);
        bool HasBuff(int id);
        void SetToPosition(string type);
        void OnMagicDamage(int damage, int element);
        void SuddenDeath();

        void Summon(int type, int id);
    }
}