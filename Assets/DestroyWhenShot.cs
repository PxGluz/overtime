using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenShot : MonoBehaviour
{
    public DamageWhileFalling damageWhileFalling;

    private void Start()
    {
        damageWhileFalling.enabled = false;
    }

    public void ReceiveHit()
    {
        damageWhileFalling.enabled = true;
        Destroy(gameObject);
    }
}
