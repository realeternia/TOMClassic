using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Log;
using TaleofMonsters.Core.Loader;

namespace TaleofMonsters.Controler.Battle.Data.MemMissile
{
    internal class MissileBook
    {
        static Dictionary<string, Image> effectType = new Dictionary<string, Image>();
        static Dictionary<string, MissileConfig> cachedConfigDict;

        public static Image GetImage(int id, bool isYFlip)
        {
            var key = isYFlip ? id.ToString() + "f" : id.ToString();
            if (!effectType.ContainsKey(key))
            {
                var img = PicLoader.Read("Missile", string.Format("{0}.PNG", id));
                if (img == null)
                    img = PicLoader.Read("Missile", "10.PNG");
                if (isYFlip)
                    img.RotateFlip(RotateFlipType.RotateNoneFlipY);
                effectType.Add(key, img);
            }
            return effectType[key];
        }

        public static MissileConfig GetConfig(string name)
        {
            if (cachedConfigDict == null)
            {
                cachedConfigDict = new Dictionary<string, MissileConfig>();
                foreach (var missile in ConfigData.MissileDict.Values)
                    cachedConfigDict[missile.TypeName] = missile;
            }

            MissileConfig configData;
            if (cachedConfigDict.TryGetValue(name, out configData))
                return configData;

            NLog.Warn("MissileBook.GetConfig error name={0}", name);
            return cachedConfigDict["null"];
        }
    }
}
