using UnityEngine;

public class DestroyWhenShot : MonoBehaviour
{
    public DamageWhileFalling damageWhileFalling;

    public void ReceiveHit()
    {
        damageWhileFalling.isActivated = true;
        Destroy(gameObject);
    }
}
