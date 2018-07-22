using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemCard;

namespace TaleofMonsters.Controler.Battle.DataTent
{
    internal interface ISubscribeUser
    {
        void OnMessage(EventMsgQueue.EventMsgTypes type, ActiveCard selectCard, Point location, IMonster mon, IPlayer targetPlayer);
    }

    internal class EventMsgQueue
    {
        public enum EventMsgTypes
        {
            UseCard=1, Summon
        }

        private List<ISubscribeUser> users = new List<ISubscribeUser>();
        public void Subscribe(ISubscribeUser user)
        {
            users.Add(user);
        }

        public void UnSubscribe(ISubscribeUser user)
        {
            users.Remove(user);
        }

        public void Pubscribe(EventMsgTypes type, ActiveCard selectCard, Point location, IMonster mon, IPlayer targetPlayer)
        {
            foreach (var subscribeUser in users)
            {
                subscribeUser.OnMessage(type, selectCard, location, mon, targetPlayer);
            }
        }
    }
}