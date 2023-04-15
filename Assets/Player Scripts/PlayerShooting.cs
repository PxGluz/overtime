using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bullet;

    public int bulletsleft;
    int bulletsShot;

    // bools
    bool shooting, reloading;
    [HideInInspector]
    public bool readyToShoot;

    // Reference;
    [HideInInspector]
    private WeaponManager weaponM;

    // Graphics 
    public GameObject muzzleFlash;
    public GameObject ammunitionDisplay;

    // bug fixing :D
    public bool allowInvoke = true;
    public GameObject AttackPointObject;

    private void Start()
    {
        weaponM = Player.m.weaponManager;
        readyToShoot= true;
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // stop the shooting script if the player isn't in the shoot attack type
        if (Player.m.AttackType != "ranged")
            return;

        if (!Player.m.weaponManager.weaponIsInPlace)
            return;

        MyInput();

        // Ammo display
        if (weaponM.currentWeapon.ammoBar != null)
            weaponM.currentWeapon.ammoBar.transform.localScale = new Vector3((float)bulletsleft / (float)weaponM.currentWeapon.gunMagazineSize, 1, 1);
    }

    private void MyInput()
    {
        // Check if allowed to hold down button and take corresponding input
        if (weaponM.currentWeapon.gunAllowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        // Reloading
        if (Input.GetKeyDown(KeyCode.T) && bulletsleft < weaponM.currentWeapon.gunMagazineSize && !reloading)
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
        AudioManager.AM.Play("pistolShoot");

        Player.m.crossHairLogic.ActivateCrossHairEffect();

        readyToShoot = false;

        // Find the exact hit position using raycast
        Ray ray = Player.m.MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Just a ray through the middle of your camera
        RaycastHit hit;

        // check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
             targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        // Calculate direction from attackPoint to targetPoint  
        float spreadUp = Random.Range(-1f, 1f) * weaponM.currentWeapon.gunSpread / 10;
        float spreadRight = Random.Range(-1f, 1f) * weaponM.currentWeapon.gunSpread /10;

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


}
