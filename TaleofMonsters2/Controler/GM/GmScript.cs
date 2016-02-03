using System.IO;
using TaleofMonsters.Controler.Battle;

namespace TaleofMonsters.Controler.GM
{
    internal class GmScript
    {
        public static void MonsterVsBatch()
        {
            StreamWriter sw = new StreamWriter("F://a.txt");
            for (int i = 10001; i < 10305; i++)
            {
                float winCount = 0;
                for (int j = 10001; j < 10305; j++)
                {
                    var result = CardFastBattle.Instance.StartGame(i, j,0);
                    if (result == CardFastBattleResult.LeftWin)
                    {
                        winCount++;
                    }
                    else if (result == CardFastBattleResult.Draw)
                    {
                        winCount += 0.5f;
                    }
                }
                sw.WriteLine(winCount / 305);
            }
            sw.Close();
        }
    }
}
