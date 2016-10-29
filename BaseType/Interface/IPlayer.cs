namespace ConfigDatas
{
    public interface IPlayer
    {
        bool IsLeft{get;}
        int Job { get; }
        float Mp { get; }
        float Lp { get; }
        float Pp { get; }  
        void AddMp(double addon);//spell使用
        void AddLp(double addon);
        void AddPp(double addon);
        void AddMana(IMonster mon, int type, double addon);//skill使用

        void AddSpike(int id);
        void RemoveSpike(int id);
        void DeleteRandomCardFor(IPlayer p, int levelChange);
        void CopyRandomCardFor(IPlayer p, int levelChange);
        void GetNextNCard(IMonster mon, int n);
        void ConvertCard(int count, int cardId, int levelChange);
        void AddCard(IMonster mon, int cardId, int level);
        void DeleteAllCard();
        void DeleteSelectCard();
        void RecostSelectCard();
        int CardNumber { get; }
        void CopyRandomNCard(int n, int spellid);
        void CardLevelUp(int n, int type);
        void AddRandomCard(IMonster mon, int type, int lv); //按类型给一张随机卡牌，怪物，武器
        void AddRandomCardJob(IMonster mon, int jobId, int lv); //按职业给一张随机卡牌
        void AddRandomCardRace(IMonster mon, int race, int lv); //按种族给一张随机卡牌

        void AddMonster(int cardId, int level, System.Drawing.Point location);
        void ExchangeMonster(IMonster target, int lv);
        void AddResource(int type, int number);
        void AddTrap(int id, int lv, double rate, int dam);
        void AddSpellMissile(IMonster target, ISpell spell, System.Drawing.Point mouse, string effect);
        void AddSpellRowMissile(ISpell spell, int count, System.Drawing.Point mouse, string effect);

        void AddSpellEffect(double rate);
    }
}