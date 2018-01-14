namespace ConfigDatas
{
    public interface IMonster
    {
        int Id{get;}
        int Star { get; }
        int Level { get; }
        int CardId { get; }
        void AddHp(double addon);
        void AddHpRate(double value);
        void DecHp(double addon);
        void RepairHp(double addon);
        double HpRate{get;}
        int Hp{get;}
        int WeaponId{get;}
        int WeaponType { get; }	//1,2,3,4
        IPlayer Owner{get;}
        IPlayer Rival { get; }
        bool IsDefence { get; }
        System.Drawing.Point Position{get;}
        IMap Map { get; }
        IMonsterAction Action { get; }
        bool IsLeft { get; }

        AttrModifyData Atk { get; set; }
        AttrModifyData MaxHp { get; set; }

        AttrModifyData Def { get; set; }
        AttrModifyData Mag { get; set; }
        AttrModifyData Spd { get; set; }
        AttrModifyData Hit { get; set; }
        AttrModifyData Dhit { get; set; }
        AttrModifyData Crt { get; set; }
        AttrModifyData Luk { get; set; }
        int Cure { get; }

        double CrtDamAddRate { get; set; } //暴击时伤害倍数增加


        bool CanAttack { get; set; }

        int AttackType { get; }
        bool HasSkill(int sid);

        int Attr { get; } //属性
        int Type { get; } //种族

        int MovRound { get; } //连续移动回合数，攻击后清除
        void AddActionRate(double value);
        void AddMaxHp(double val);
        void AddAntiMagic(string type, int value);
        void OnMagicDamage(IMonster source, double damage, int element);
        void OnSpellDamage(double damage, int element);
        void OnSpellDamage(double damage, int element, double vibrate);
        void ClearTarget();
    }
}