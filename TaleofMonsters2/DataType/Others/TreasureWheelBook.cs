using System.Collections.Generic;
using NarlonLib.Math;
using TaleofMonsters.Forms;
using TaleofMonsters.Forms.CMain;

namespace TaleofMonsters.DataType.Others
{
    internal class TreasureWheelBook
    {
        public static void Show(int wheelGroupId)
        {
            List<int> wheelIds = new List<int>();
            foreach (var treasureWheelConfig in ConfigDatas.ConfigData.TreasureWheelDict)
            {
                if (treasureWheelConfig.Value.Group == wheelGroupId)
                    wheelIds.Add(treasureWheelConfig.Key);
            }

            if (wheelIds.Count > 0)
            {
                TreasureWheelForm form = new TreasureWheelForm();
                form.WheelId = wheelIds[MathTool.GetRandom(wheelIds.Count)];
                PanelManager.DealPanel(form);
            }
        }
    }
}