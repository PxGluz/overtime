using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class PlayerShooting : MonoBehaviour
{
    // Projectile system:

    // bullet
    public GameObject bullet;

    // bullet force
    public float shootForce, upwardForce;

    // Gun Stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    int bulletsleft, bulletsShot;

    // bools
    bool shooting, readyToShoot, reloading;

    // Reference;
    public Transform attackPoint;

    // Graphics 
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;


    // bug fixing :D
    public bool allowInvoke = true;
    public GameObject AttackPointObject;

    private void Awake()
    {
        bulletsleft = magazineSize;
        readyToShoot= true;
    }

    private void Update()
    {

        // stop the shooting script if the player isn't in the shoot attack type
        if (Player.m.AttackType != "shoot")
            return;

        MyInput();

        // Ammo display
        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsleft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
    }

    private void MyInput()
    {
        // Check if allowed to hold down button and take corresponding input
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        // Reloading
        if (Input.GetKeyDown(KeyCode.R) && bulletsleft < magazineSize && !reloading)
            Reload();
        // Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsleft <= 0)
            Reload();

        // Shooting
        if (readyToShoot && shooting && !reloading && bulletsleft > 0)
        {
            // Set bullets shot to 0
            bulletsShot = 0;

            Shoot();
        }


    }

    private void Shoot()
    {
        readyToShoot= false;

        // Find the exact hit position using raycast
        Ray ray = Player.m.MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Just a ray through the middle of your camera
        RaycastHit hit;

        // check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        //AttackPointObject.transform.position = targetPoint;

        // Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = (targetPoint - attackPoint.position).normalized;



        // The spred system works like shit

        //Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);



        // Calculate new direction with spread
        //Vector3 directionWithSpread = (ray.GetPoint(75) - attackPoint.position).normalized + new Vector3(x,y,0);
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        // Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);

        // Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        // Add force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(Player.m.MainCamera.transform.up * upwardForce, ForceMode.Impulse);

        // Set bullet damage
        BulletCollision bulletCollision = currentBullet.GetComponent<BulletCollision>();
        if (bulletCollision != null)
        {
            bulletCollision.bulletDamage = Player.m.weaponManager.currentWeapon.gunDamage;
        }

        if (muzzleFlash != null)
            Instantiate(muzzleFlash,attackPoint.position, Quaternion.identity);

        bulletsleft--;
        bulletsShot++;

        // Invoke resetShot function ( if not already invoked )
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false; 
        }

        // if more than one bulletsPerTap make sure to repeat shoot function
        if (bulletsShot < bulletsPerTap && bulletsleft > 0)
            Invoke("Shoot", timeBetweenShots);

    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished",reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsleft = magazineSize;
        reloading = false;
    }





























    // Raycast system:
    /*

    public KeyCode shootKey = KeyCode.Mouse0;
    public float shootingCooldown;
    public float shootingDistance = 10f;
    private Transform playerCameraTransform;
    private bool canShoot = true;
    public float baseDamage = 50f;
    void Start()
    {
        playerCameraTransform = Player.m.MainCamera.transform;
    }

    void Update()
    {

        // stop the shooting script if the player isn't in the shoot attack type
        if (Player.m.AttackType != "shoot")
            return;

        if (Input.GetKey(shootKey))
        {
            TryShooting();
        }
    }

    // Function called on hit. Change it for something to happen.
    void HitEffects(RaycastHit hitInfo)
    {
        //Debug.DrawRay(playerCameraTransform.position, hitInfo.point - playerCameraTransform.position, new Color(0, 0, 1), 2);
        //Debug.Log(hitInfo.collider.gameObject.name + " " + hitInfo.point);
        switch (LayerMask.LayerToName(hitInfo.collider.gameObject.layer))
        {
            case "Enemy":
                if (hitInfo.collider.gameObject.name == "Body")
                {
                    hitInfo.collider.gameObject.GetComponentInParent<EnemyStats>().ReceiveHit(baseDamage);
                }
                else if (hitInfo.collider.gameObject.name == "Head")
                {
                    hitInfo.collider.gameObject.GetComponentInParent<EnemyStats>().ReceiveHit(baseDamage * 2);
                }
                break;
            case "Explosive":
                hitInfo.collider.gameObject.GetComponent<ExplosiveBarrel>().ReceiveHit();
                break;

        }
    }

    void TryShooting()
    {
        if (canShoot)
        {
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hitInfo, shootingDistance))
            {
                HitEffects(hitInfo);
            }
            canShoot = false;
            Invoke("ResetShooting", shootingCooldown);
        }
    }

    void ResetShooting()
    {
        canShoot = true;
    }

    */
}
