namespace ConfigDatas
{
    public interface IPlayer
    {
        bool IsLeft{get;}
        float Mp { get; }
        float Lp { get; }
        float Pp { get; }  
        void AddMp(double addon);//spell使用
        void AddLp(double addon);
        void AddPp(double addon);
        void AddMana(IMonster mon, int type, double addon);//skill使用

        int LpCost { get; set; }
        int MpCost { get; set; }
        int PpCost { get; set; }

        int RoundCardPlus { get; set; }//每回合抽卡加成，默认为0

        void DeleteRandomCardFor(IPlayer p, int levelChange);
        void CopyRandomCardFor(IPlayer p, int levelChange);
        void GetNextNCard(int n);
        void ConvertCard(int count, int cardId, int levelChange);
        void AddCard(int cardId, int level);
        void DeleteAllCard();
        void DeleteSelectCard();
        void RecostSelectCard();
        int GetCardNumber();
        void CopyRandomNCard(int n, int spellid);
        void CardLevelUp(int n, int type);
		
        void AddResource(int type, int number);
        void AddTrap(int id, int lv, double rate, int dam);

        void AddSpellEffect(double rate);
    }
}