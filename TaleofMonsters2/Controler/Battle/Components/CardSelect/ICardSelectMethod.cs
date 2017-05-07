using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.Players;

namespace TaleofMonsters.Controler.Battle.Components.CardSelect
{
    internal interface ICardSelectMethod
    {
        CardSelector Selector { set; }
        void Init(Player p);
        void RegionClicked(int id);
        ActiveCard[] GetCards();
        void OnStartButtonClick();
    }
}