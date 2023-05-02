using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    [HideInInspector]
    public float bulletDamage = 0;
    [HideInInspector]
    public string myDamageType;
    private void Start()
    {
        Invoke(nameof(Expire), 10f);
    }

    private void Update()
    {
        Player.m.AnnounceEnemy(transform.position, Player.m.EnemyAnnoucedByWeaponRange);
    }

    private Vector3 lastPos;
    private void FixedUpdate()
    {
        Debug.DrawRay(lastPos, transform.forward * Vector3.Distance(lastPos, transform.position), Color.yellow);

        RaycastHit hit;

        if (Physics.Raycast(lastPos, transform.forward, out hit, Vector3.Distance(lastPos, transform.position)))
        {

            HandleLayerLogic(hit);
        }
        lastPos = transform.position;
    }

    public void HandleLayerLogic(RaycastHit hit)
    {
        if (LayerMask.LayerToName(hit.collider.gameObject.gameObject.layer) == "Player")
            return;

        DestroyWhenShot destroyWhenShot = hit.collider.gameObject.GetComponent<DestroyWhenShot>();
        if (destroyWhenShot != null)
            destroyWhenShot.ReceiveHit();

        switch (LayerMask.LayerToName(hit.collider.gameObject.layer))
        {
            case "Interactable":
                return;

            case "Enemy":
                EnemyMaster enemy = hit.collider.gameObject.GetComponentInParent<EnemyMaster>();
                if (enemy != null)
                {
                    if (hit.collider.gameObject.name == "spine.006")
                    {
                        enemy.TakeDamage(bulletDamage, hit.collider.gameObject, transform.forward * 25f, hit.point, true);
                    }
                    else
                    {
                        enemy.TakeDamage(bulletDamage, hit.collider.gameObject, transform.forward * 25f, hit.point);
                    }
                }
                break;

            case "Explosive":
                hit.collider.gameObject.GetComponent<ExplosiveBarrel>().ReceiveHit();
                break;
            default:
                Player.m.particleManager.CreateParticle(hit.point, -transform.forward);
                break;

        }
        print(LayerMask.LayerToName(hit.collider.gameObject.layer));
        print(hit.collider.gameObject.name);

        Destroy(gameObject);

    }

    private void Expire()
    {
        Destroy(gameObject);
    }

}
