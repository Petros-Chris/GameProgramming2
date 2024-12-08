

public class AttackEnemyTowerState : IStateAlly
{
    private AllyAI aiController;

    public StateTypeAlly Type => StateTypeAlly.AttackEnemyFromTower;

    public AttackEnemyTowerState(AllyAI aiController)
    {
        this.aiController = aiController;
    }

    public void Enter() { }

    public void Execute()
    {
        // Enemy doesn't exist
        if (aiController.enemy != null)
        {
            if (aiController.IsEnemyInRange(aiController.AttackRange))
            {
                aiController.transform.LookAt(aiController.enemy);
                aiController.Attack();
            }
            else
            {
                aiController.StateMachine.TransitionToState(StateTypeAlly.Idle);
            }

        }
        else
        {
            aiController.enemy = aiController.GetClosestEnemy();
        }


    }

    public void Exit() { }
}

