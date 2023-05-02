using UnityEngine;

public class WinCondition : MonoBehaviour
{
    void Update()
    {
        int aliveCount = 0;
        foreach (Transform child in transform)
            if (child.TryGetComponent(out EnemyMaster eMaster) && !eMaster.isDead)
                aliveCount++;
        if (aliveCount == 0)
            Player.m.YouWin();
    }
}
