using System.Collections.Generic;
using System.Drawing;

namespace DeckManager
{
    public class ImageCache
    {
        private const string pathParent = "../../PicResource/";
        private static Dictionary<int, Image> cacheDict = new Dictionary<int, Image>();

        public static Image GetImage(int cardId)
        {
            if (cacheDict.ContainsKey(cardId))
                return cacheDict[cardId];

            var cardConfig = CardConfigManager.GetCardConfig(cardId);
            var img = Image.FromFile(string.Format("{0}{1}/{2}.JPG", pathParent, cardConfig.GetImageFolderName(), cardConfig.Icon));
            cacheDict[cardId] = img;
            return img;
        }
    }
}