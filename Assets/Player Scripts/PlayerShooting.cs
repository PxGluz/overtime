using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerShooting : MonoBehaviour
{
    // Projectile system:


    // bullet
    public GameObject bullet;

    // bullet force
    //public float shootForce, upwardForce;

    // Gun Stats
    //public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    //public int magazineSize, bulletsPerTap;
    //public bool allowButtonHold;

    public int bulletsleft;
    int bulletsShot;

    // bools
    bool shooting, reloading;
    [HideInInspector]
    public bool readyToShoot;

    // Reference;
    //public Transform attackPoint;
    [HideInInspector]
    private WeaponManager weaponM;

    // Graphics 
    public GameObject muzzleFlash;
    public GameObject AmmoDisplayParent;
    public TextMeshProUGUI ammunitionDisplay;


    // bug fixing :D
    public bool allowInvoke = true;
    public GameObject AttackPointObject;

    private void Start()
    {
        weaponM = Player.m.weaponManager;
        //bulletsleft = weaponM.currentWeapon.gunMagazineSize;
        readyToShoot= true;
    }

    private void Update()
    {

        // stop the shooting script if the player isn't in the shoot attack type
        if (Player.m.AttackType != "ranged")
            return;

        if (!Player.m.weaponManager.weaponIsInPlace)
            return;

        MyInput();

        // Ammo display
        if (weaponM.currentWeapon.gunBulletsPerTap != 0 && weaponM.currentWeapon.gunBulletsPerTap != 0)
            if (ammunitionDisplay != null)
                ammunitionDisplay.SetText(bulletsleft / weaponM.currentWeapon.gunBulletsPerTap + " / " + weaponM.currentWeapon.gunMagazineSize / weaponM.currentWeapon.gunBulletsPerTap);
    }

    private void MyInput()
    {
        // Check if allowed to hold down button and take corresponding input
        if (weaponM.currentWeapon.gunAllowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        // Reloading
        if (Input.GetKeyDown(KeyCode.T) && bulletsleft < weaponM.currentWeapon.gunMagazineSize && !reloading)
            Reload();

        /*
        // Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsleft <= 0)
            Reload();
        */

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
        {
             targetPoint = hit.point;
        }
        else
            targetPoint = ray.GetPoint(75);

        AudioManager.AM.Play("pistolShoot");


        // Calculate direction from attackPoint to targetPoint
        //Vector3 directionWithoutSpread = (targetPoint - weaponM.currentWeapon.shootPoint.position).normalized;
        
        float spreadUp = Random.Range(-1f, 1f) * weaponM.currentWeapon.gunSpread / 10;
        float spreadRight = Random.Range(-1f, 1f) * weaponM.currentWeapon.gunSpread /10;

        // Vector3 directionWithSpread = directionWithoutSpread + new Vector3(spreadUp,spreadRight , 0);

        // Find random point in the same plane as the targetPoint.
        Vector3 spreadPoint = targetPoint
            + Player.m.playerCam.transform.right * spreadRight * (targetPoint - weaponM.currentWeapon.shootPoint.position).magnitude
            + Player.m.playerCam.transform.up * spreadUp * (targetPoint - weaponM.currentWeapon.shootPoint.position).magnitude;
        // Calculate direction as before.
        Vector3 directionWithSpread = (spreadPoint - weaponM.currentWeapon.shootPoint.position).normalized;

        // Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(bullet, weaponM.currentWeapon.shootPoint.position, Quaternion.identity);

        // Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        // Add force to bullet
        Rigidbody bulletRB = currentBullet.GetComponent<Rigidbody>();
        bulletRB.AddForce(directionWithSpread.normalized * weaponM.currentWeapon.gunShootForce, ForceMode.Impulse);
        

        // Set bullet damage
        BulletCollision bulletCollision = currentBullet.GetComponent<BulletCollision>();
        if (bulletCollision != null)
        {
            bulletCollision.bulletDamage = Player.m.weaponManager.currentWeapon.bulletDamage;
        }

        if (muzzleFlash != null)
            Instantiate(muzzleFlash, weaponM.currentWeapon.shootPoint.position, Quaternion.identity);

        bulletsleft--;
        bulletsShot++;

        // Invoke resetShot function ( if not already invoked )
        if (allowInvoke)
        {
            Invoke("ResetShot", weaponM.currentWeapon.gunTimeBetweenShooting);
            allowInvoke = false; 
        }

        // if more than one bulletsPerTap make sure to repeat shoot function
        if (bulletsShot < weaponM.currentWeapon.gunBulletsPerTap && bulletsleft > 0)
            Invoke("Shoot", weaponM.currentWeapon.gunTimeBetweenShots);

    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", weaponM.currentWeapon.gunReloadTime);
    }

    private void ReloadFinished()
    {
        bulletsleft = weaponM.currentWeapon.gunMagazineSize;
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
