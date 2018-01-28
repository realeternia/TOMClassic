namespace ConfigDatas
{
    public interface IMonsterAction
    {
        bool IsNight { get; }
        bool IsTileMatching { get; }
        bool IsElement(string ele);
        bool IsRace(string rac);

        void Disappear();
        void AddBuff(int buffId, int blevel, double dura);
        void AddItem(int itemId);//战斗中
        void Transform(int monId);

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
        void AddMissile(IMonster target, int attr, double damage, string arrow);
       
        void ClearDebuff();
        void ExtendDebuff(double count);
        bool HasBuff(int id);
        void SetToPosition(string type, int step);

        void SuddenDeath();
        void Rebel();//造反

        void Summon(string type, int id, int count);
        void SummonRandomAttr(string type, int attr);
        void MadDrug(); //交换攻击和血量
        void CureRandomAlien(double rate);
        bool ResistBuffType(int type);
        void EatTomb(IMonster tomb);
        void Silent();
        IMonsterAuro AddAuro(int buff, int lv, string tar);
        void AddPArmor(double val);
        void AddMArmor(double val);
        int GetPArmor();
    }
}