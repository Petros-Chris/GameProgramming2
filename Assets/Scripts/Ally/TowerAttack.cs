public class TowerAttack : AllyAI
{
    void Start()
    {
        StateMachine = new StateMachineAlly();
        StateMachine.AddState(new AttackEnemyTowerState(this));

        StateMachine.TransitionToState(StateTypeAlly.AttackEnemyFromTower);
    }

    void Update()
    {
        StateMachine.Update();
    }
}
