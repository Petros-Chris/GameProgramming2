public class TowerAttack : AllyAI
{
    void Start()
    {
        StateMachine = new StateMachineAlly();
        StateMachine.AddState(new IdleStateAlly(this));
        StateMachine.AddState(new AttackEnemyTowerState(this));
        StateMachine.TransitionToState(StateTypeAlly.AttackEnemyFromTower);
    }

    void Update()
    {
        if (StateMachine == null)
        {
            Start();
        }
        else
        {
            StateMachine.Update();
        }
    }
}
