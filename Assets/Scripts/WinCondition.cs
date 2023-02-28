using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    private Transform enemyParent;

    // Start is called before the first frame update
    void Start()
    {
        enemyParent = GameObject.Find("EnemyParent").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyParent.childCount <= 0)
            Player.m.YouWin();
    }
}
