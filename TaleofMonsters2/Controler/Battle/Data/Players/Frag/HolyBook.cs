using System.Collections.Generic;

namespace TaleofMonsters.Controler.Battle.Data.Players.Frag
{
    internal class HolyBook
    {
        private List<string> holyWordList = new List<string>(); //圣言，一些特殊效果的指令

        public void AddWord(string word)
        {
            if (!holyWordList.Contains(word))
                holyWordList.Add(word);
        }

        public bool HasWord(string word)
        {
            return holyWordList.Contains(word);
        }
    }
}