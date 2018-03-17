using System.Collections.Generic;
using JLM.NetSocket;

namespace GameServer.Logic
{
    public class PlayerManager
    {
        private Dictionary<int, GamePlayer> socketDict = new Dictionary<int, GamePlayer>();
        private Dictionary<string, int> playerDict = new Dictionary<string, int>();

        public void AddPlayer(int socketId, NetClient socket)
        {
            socketDict[socketId] = new GamePlayer(socket);
        }

        public void RemovePlayer(int socketId)
        {
            socketDict.Remove(socketId);
            foreach (var pData in playerDict)
            {
                if (pData.Value == socketId)
                {
                    playerDict.Remove(pData.Key);
                    return;
                }
            }
        }

        public GamePlayer GetPlayer(int socketId)
        {
            GamePlayer player = null;
            socketDict.TryGetValue(socketId, out player);
            return player;
        }

        public void SetName(int socketId, string name)
        {
            if (playerDict.ContainsKey(name))
                socketDict[playerDict[name]].Close("kick");

            playerDict[name] = socketId;
            socketDict[socketId].Passport = name;
        }
    }
}