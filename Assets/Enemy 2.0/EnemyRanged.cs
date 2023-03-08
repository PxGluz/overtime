using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static WeaponManager;

public class EnemyRanged : MonoBehaviour
{

    private EnemyMaster enemy;
    
    public GameObject enemyBullet;
    public Transform gunPosition;
    public float reloadCooldown = 2f;

    public int bulletsleft;
    private int bulletsShot;
    private bool  reloading;
    private bool readyToShoot;

    // Reference;
    [HideInInspector]
    public Transform shootPoint;

    // Graphics 
    public GameObject muzzleFlash;

    // bug fixing :D
    private bool allowInvoke = true;

    private void Start()
    {
        enemy = GetComponentInParent<EnemyMaster>();
        enemy.animator.SetLayerWeight(1, 1);

        if (enemy.enemyType.ToString() != "Ranged")
        {
            this.enabled = false;
            return;
        }

        readyToShoot = true;

        bulletsleft = enemy.WeaponClass.gunMagazineSize;
    }

    public bool isInTransition;

    private void Update()
    {

        if (enemy.enemyMovement.canSeePlayer)
        {
            if (!isInTransition && !enemy.animator.GetBool("isAiming")) {
                isInTransition = true;
                Invoke(nameof(RaiseArmsUp), 1f);
                enemy.animator.SetBool("isAiming", true);
            }
        }
        else
            enemy.animator.SetBool("isAiming", false);

        if (isInTransition)
            return;

        if ( readyToShoot && !reloading && bulletsleft <= 0)
            Reload();

        // Shooting
        if ( enemy.enemyMovement.canSeePlayer && readyToShoot && !reloading && bulletsleft > 0)
        {
            bulletsShot = 0;
            enemy.StunEnemy(enemy.WeaponClass.gunBulletsPerTap * enemy.WeaponClass.gunTimeBetweenShots +0.5f);
            Shoot();
        }
    }

    private void RaiseArmsUp() { isInTransition= false; }

    private void Shoot()
    {
        readyToShoot = false;

        // Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = (Player.m.headPosition.position - shootPoint.position).normalized;

        float spreadUp = Random.Range(-1f, 1f) * enemy.WeaponClass.gunSpread / 10;
        float spreadRight = Random.Range(-1f, 1f) * enemy.WeaponClass.gunSpread / 10;

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(spreadUp, spreadRight, 0);

        // Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(enemyBullet, shootPoint.position, Quaternion.identity);

        // Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        // Add force to bullet
        Rigidbody bulletRB = currentBullet.GetComponent<Rigidbody>();
        bulletRB.AddForce(directionWithSpread.normalized * enemy.WeaponClass.gunShootForce, ForceMode.Impulse);


        // Set bullet damage
        BulletCollision bulletCollision = currentBullet.GetComponent<BulletCollision>();
        if (bulletCollision != null)
        {
            bulletCollision.bulletDamage = Player.m.weaponManager.currentWeapon.bulletDamage;
        }

        if (muzzleFlash != null)
            Instantiate(muzzleFlash, shootPoint.position, Quaternion.identity);

        bulletsleft--;
        bulletsShot++;

        // Invoke resetShot function ( if not already invoked )
        if (allowInvoke)
        {
            Invoke("ResetShot", enemy.WeaponClass.gunTimeBetweenShooting);
            allowInvoke = false;
        }

        // if more than one bulletsPerTap make sure to repeat shoot function
        if (bulletsShot < enemy.WeaponClass.gunBulletsPerTap && bulletsleft > 0)
            Invoke("Shoot", enemy.WeaponClass.gunTimeBetweenShots);

    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadCooldown);
    }

    private void ReloadFinished()
    {
        bulletsleft = enemy.WeaponClass.gunMagazineSize;
        reloading = false;
    }

}