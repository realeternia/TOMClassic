using TaleofMonsters.Controler.Battle.Data.MemCard;

namespace TaleofMonsters.Core.Interface
{
    internal interface ICardList
    {
        void DisSelectCard();
        void UpdateSlot(ActiveCard[] pCards);
        ActiveCard GetSelectCard();
        void SetSelectId(int value);
        int GetSelectId();
        int GetCapacity();
        ActiveCard[] GetAllCard();
    }
}
