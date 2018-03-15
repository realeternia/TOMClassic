using System.Collections.Generic;
using JLM.NetSocket;

namespace GameServer.Logic
{
    public class PlayerManager
    {
        private Dictionary<int, NetClient> socketDict = new Dictionary<int, NetClient>();
        private Dictionary<string, int> playerDict = new Dictionary<string, int>();

        public void AddPlayer(int socketId, NetClient socket)
        {
            socketDict[socketId] = socket;
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

        public void SetName(int socketId, string name)
        {
            if (playerDict.ContainsKey(name))
            {
                socketDict[playerDict[name]].Close("kick");
            }

            playerDict[name] = socketId;
        }
    }
}