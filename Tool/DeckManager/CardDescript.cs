using System;
using System.Collections.Generic;
using System.Text;

namespace DeckManager
{
    [Serializable]
    public struct CardDescript
    {
        public int id;
        public string tip;
        public CardDescript(int id, string tip)
        {
            this.id = id;
            this.tip = tip;
        }

    }
}
