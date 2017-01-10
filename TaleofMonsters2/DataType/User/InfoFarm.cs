using System;
using NarlonLib.Core;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User.Db;

namespace TaleofMonsters.DataType.User
{
    public class InfoFarm
    {
        [FieldIndex(Index = 1)] public DbFarmState[] DbFarmState;

        public InfoFarm()
        {
            DbFarmState = new DbFarmState[GameConstants.PlayFarmCount];
            DbFarmState[0] = new DbFarmState(0, 0);
            for (int i = 1; i < DbFarmState.Length; i++)
            {
                DbFarmState[i] = new DbFarmState(-1, 0);
            }
        }

        public DbFarmState GetFarmState(int id)
        {
            return DbFarmState[id];
        }

        public void SetFarmState(int id, DbFarmState state)
        {
            DbFarmState[id] = state;
        }

        public int GetFarmAvailCount()
        {
            int count = 0;
            foreach (DbFarmState state in DbFarmState)
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
                if (DbFarmState[i].Type == 0)
                {
                    DbFarmState[i].Type = type;
                    DbFarmState[i].Time = TimeTool.DateTimeToUnixTime(DateTime.Now) + dura;
                    return true;
                }
            }
            return false;
        }
    }
}
