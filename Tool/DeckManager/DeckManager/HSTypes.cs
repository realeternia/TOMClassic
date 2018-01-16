namespace DeckManager
{
    public class HSTypes
    {

        public static string I2QualityColor(int id)
        {
            string[] rt = { "White", "Green", "DodgerBlue", "Violet", "Orange", "Gray", "Gray", "", "", "Yellow" };
            return rt[id];
        }
    }
}