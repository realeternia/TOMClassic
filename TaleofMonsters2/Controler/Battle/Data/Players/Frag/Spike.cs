namespace TaleofMonsters.Controler.Battle.Data.Players.Frag
{
    internal class Spike
    {
        public int Id { get; set; }//配置表id
        public bool RemoveOnUseMonster { get; set; }
        public bool RemoveOnUseWeapon { get; set; }
        public bool RemoveOnUseSpell { get; set; }
        public bool CanTimeOut { get; set; }
        public float RoundLeft { get; set; }
    }
}
