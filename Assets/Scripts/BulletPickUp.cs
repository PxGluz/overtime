using TMPro;
using UnityEngine;

public class BulletPickUp : MonoBehaviour
{
    public int nrOfBullets;

    public GameObject Model;
    public TextMeshProUGUI BulletCountText;
    public float collisionRange;
    public float pickUpRange;
    public float pickUpDuration;
    public float rotationsPerMinute;

    private Vector3 smoothDampVelocityRef;

    private void Start()
    {
        BulletCountText.text = nrOfBullets.ToString();
    }

    void Update()
    {
        Model.transform.Rotate(0, 6.0f * rotationsPerMinute * Time.deltaTime, 0);

        if (Player.m.AttackType.ToLower() != "ranged")
            return;

        if (Player.m.playerShooting.bulletsleft == Player.m.weaponManager.currentWeapon.gunMagazineSize)
            return;

        if (Vector3.Distance(Player.m.playerMovement.GroundCheckSource.transform.position, transform.position) <= pickUpRange)
            transform.position = Vector3.SmoothDamp(transform.position, Player.m.playerMovement.GroundCheckSource.transform.position, ref smoothDampVelocityRef, pickUpDuration);
        else
            return;

        if (Vector3.Distance(Player.m.playerMovement.GroundCheckSource.transform.position, transform.position) <= collisionRange)
        {
            while (Player.m.playerShooting.bulletsleft < Player.m.weaponManager.currentWeapon.gunMagazineSize)
            {
                nrOfBullets--;
                BulletCountText.text = nrOfBullets.ToString();

                Player.m.playerShooting.bulletsleft += Mathf.Max(1, Player.m.weaponManager.currentWeapon.gunBulletsPerTap);
                Mathf.Clamp(Player.m.playerShooting.bulletsleft, 0, Player.m.weaponManager.currentWeapon.gunMagazineSize);

                Player.m.playerShooting.UpdateGunAmmoDisplay();

                if (nrOfBullets <= 0)
                {
                    Destroy(this.gameObject);
                    break;
                }
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pickUpRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, collisionRange);
    }
}
