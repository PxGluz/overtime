using UnityEngine;

public class Grenade : MonoBehaviour
{
    ThrownProjectile thrownProjectile;
    public float radius = 10;
    public float damage = 10;
    public float pushForce = 10;
    public float explosionDelay = 2;

    private void Start()
    {
        thrownProjectile = GetComponent<ThrownProjectile>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (thrownProjectile.isInPickUpState)
            return;

        Invoke(nameof(Explode), explosionDelay);
    }

    private void Explode()
    {
        ExplosionManager.instance.Explode(gameObject.transform.position, radius, damage, pushForce);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(gameObject.transform.position, radius);
    }
}
