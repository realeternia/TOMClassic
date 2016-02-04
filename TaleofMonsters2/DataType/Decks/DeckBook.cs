using System.Collections.Generic;
using System.IO;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.Decks
{
    internal static class DeckBook
    {
        private static List<DeckCard> deck;

        public static DeckCard[] GetDeckByName(string name, int level)
        {
            deck = new List<DeckCard>();
            StreamReader sr = new StreamReader(DataLoader.Read("Deck", string.Format("{0}.dek", name)));

            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                string[] datas = sr.ReadLine().Split('\t');

                var readLevel = byte.Parse(datas[1]);//todo 需要做差异化，暂时只做角色等级/10
                var card = new DeckCard(int.Parse(datas[0]), (byte)(level/10+1), 0);
                deck.Add(card);
            }
            sr.Close();

            return deck.ToArray();
        }

        public static bool HasCard(string name, int id)
        {
            GetDeckByName(name, 1);
            foreach (DeckCard des in deck)
            {
                if (des.BaseId == id)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
