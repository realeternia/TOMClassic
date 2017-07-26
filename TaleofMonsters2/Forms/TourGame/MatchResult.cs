using TaleofMonsters.Core;

namespace TaleofMonsters.Forms.TourGame
{
    public class MatchResult
    {
        [FieldIndex(Index = 1)]
        public int Winner;
        [FieldIndex(Index = 2)]
        public int Loser;

        public MatchResult()
        {
            
        }

        public MatchResult(int wp, int lp)
        {
            Winner = wp;
            Loser = lp;
        }

        public override string ToString()
        {
            return string.Format("{0};{1}", Winner, Loser);
        }

        public static string MatchResultToString(MatchResult result)
        {
            return result.ToString();
        }
    }
}
