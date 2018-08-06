using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data;

namespace TaleofMonsters.Controler.Battle.DataTent
{
    internal interface ISubscribeUser
    {
        void OnMessage(EventMsgQueue.EventMsgTypes type, IPlayer p, IMonster src, IMonster dest, DamageData damage, Point l, int cardId, int cardType, int cardLevel);
    }

    internal class EventMsgQueue
    {
        public enum EventMsgTypes
        {
            UseCard=1, Summon, MonsterDie, EpRecover
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

        public void Pubscribe(EventMsgTypes type, IPlayer p, IMonster src, IMonster dest, DamageData damage, Point l, int cardId, int cardType, int cardLevel)
        {
            foreach (var subscribeUser in users)
            {
                subscribeUser.OnMessage(type, p, src, dest, damage, l, cardId, cardType, cardLevel);
            }
        }
    }
}