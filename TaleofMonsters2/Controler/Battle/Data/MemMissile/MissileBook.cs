using System.Collections.Generic;
using System.Drawing;
using TaleofMonsters.Controler.Loader;

namespace TaleofMonsters.Controler.Battle.Data.MemMissile
{
    internal class MissileBook
    {
        static Dictionary<string, Image> effectType = new Dictionary<string, Image>();

        static public Image GetImage(string name)
        {
            if (!effectType.ContainsKey(name))
            {
                effectType.Add(name, PicLoader.Read("Missile", string.Format("{0}.PNG", name)));
            }
            //todo 给一个默认的图
            return effectType[name];
        }
    }
}
