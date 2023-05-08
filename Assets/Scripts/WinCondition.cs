using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    private bool shouldCheck = true;
    void Update()
    {
        if (shouldCheck)
        {
            int aliveCount = 0;
            foreach (Transform child in transform)
                if (child.TryGetComponent(out EnemyMaster eMaster) && !eMaster.isDead)
                    aliveCount++;
            if (aliveCount == 0)
            {
                Player.m.YouWin();
                shouldCheck = false;
            }
        }
    }
}
