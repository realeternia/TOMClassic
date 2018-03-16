namespace TaleofMonsters.Datas.Formulas
{
    internal static class FormulaBook
    {
        public static double GetPhyDefRate(int def)
        {
            return def * 0.1/(1 + 0.1* def);
        }
        public static double GetMagDefRate(int mag)
        {
            return mag * 0.05 / (1 + 0.05 * mag);
        }
    }
}
