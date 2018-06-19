namespace TaleofMonsters.Controler.Battle.Data.Players.AIs
{
    public interface IAIState
    {
        void OnEnter();
        void OnExit();

        void OnTimePast(float time);
        void OnTowerHited(double towerHpRate);

        AIStates State { get; }
    }
}