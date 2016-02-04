using System;
using System.IO;
using NarlonLib.Core;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Interface;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.DataType.User
{
    public class InfoFarm
    {
         [FieldIndex(Index = 1)]
        public FarmState[] FarmState;

        public InfoFarm()
        {
            FarmState = new FarmState[GameConstants.PlayFarmCount];
            FarmState[0] = new FarmState(0, 0);
            for (int i = 1; i < FarmState.Length; i++)
            {
                FarmState[i] = new FarmState(-1, 0);
            }
        }

        public FarmState GetFarmState(int id)
        {
            return FarmState[id];
        }

        public void SetFarmState(int id, FarmState state)
        {
            FarmState[id] = state;
        }

        public int GetFarmAvailCount()
        {
            int count = 0;
            foreach (FarmState state in FarmState)
            {
                if (state.Type != -1)
                {
                    count++;
                }
            }
            return count;
        }

        public bool UseSeed(int type, int dura)
        {
            for (int i = 0; i < 9; i++)
            {
                if (FarmState[i].Type == 0)
                {
                    FarmState[i].Type = type;
                    FarmState[i].Time = TimeTool.DateTimeToUnixTime(DateTime.Now) + dura;
                    return true;
                }
            }
            return false;
        }
    }
}
