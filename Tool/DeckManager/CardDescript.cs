using System;
using System.Collections.Generic;
using System.Text;

namespace DeckManager
{
    [Serializable]
    public struct CardDescript
    {
        public int id;
        public int level;
        public int rare;
        public CardDescript(int id, int level, int rare)
        {
            this.id = id;
            this.level = level;
            this.rare = rare;
        }

    }
}
