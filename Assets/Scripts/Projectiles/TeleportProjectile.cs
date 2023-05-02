using UnityEngine;


public class TeleportProjectile : MonoBehaviour
{
    public LayerMask teleportProjectileCollisionLayer;
    public float playerHeight;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    bool hasHit = false;
    RaycastHit downHitRaycast;
    RaycastHit upHitRaycast;

    private void Update()
    {
        if (hasHit)
        {
            float newScale = this.transform.localScale.x - Time.deltaTime * 0.7f;
            this.transform.localScale = new Vector3(newScale, newScale, newScale);

            if (transform.localScale.x <= 0)
                Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasHit)
        {
            hasHit = true;

            AudioManager.AM.Play("teleportLand");

            Physics.Raycast(transform.position, Vector3.down, out downHitRaycast, Mathf.Infinity, teleportProjectileCollisionLayer);

            Physics.Raycast(transform.position, Vector3.up, out upHitRaycast, Mathf.Infinity, teleportProjectileCollisionLayer);

            float teleportPositionY = transform.position.y;

            if (Vector3.Distance(upHitRaycast.point, downHitRaycast.point) < Player.m.playerHeight)
            {
                teleportPositionY = (upHitRaycast.point.y + downHitRaycast.point.y) / 2 + Player.m.playerHeight / 4;
                Player.m.crouchLogic.enterCrouchInstantly();
                print("tight spot");
            }

            else if (Vector3.Distance(transform.position, downHitRaycast.point) < Player.m.playerHeight / 2)
            {
                teleportPositionY = downHitRaycast.point.y + Player.m.playerHeight / 2;
                print("hit ground");
            }

            else if (Vector3.Distance(transform.position, upHitRaycast.point) < Player.m.playerHeight / 2)
            {
                teleportPositionY = upHitRaycast.point.y - Player.m.playerHeight / 2;
                print("hit tavan");
            }

            Player.m.transform.position = new Vector3(transform.position.x, teleportPositionY, transform.position.z);

            rb.isKinematic = true;
        }
    }

}
