using System.Collections;
using UnityEngine;


public class EnemyRanged : MonoBehaviour
{

    private EnemyMaster enemy;
    public EnemyAmmoReloadDisplay enemyAmmoReloadDisplay;

    [Header("Variables: ")]
    public GameObject enemyBullet;
    public GameObject muzzleFlash;
    public Transform gunPosition;
    public float reloadCooldown = 2f;
    public float gunDrawTime = 1f;

    [Header("For debugging: ")]
    public int bulletsleft;
    private int bulletsShot;
    private bool reloading = false;
    private bool readyToShoot = true;
    private bool isRaisingArms = false;

    // Reference;
    [HideInInspector]
    public Transform shootPoint;


    // bug fixing :D
    private bool allowInvoke = true;

    private void Start()
    {
        enemy = GetComponentInParent<EnemyMaster>();
        enemy.animator.SetLayerWeight(1, 1);
    }

    private void Update()
    {

        if (enemy.enemyMovement.canSeePlayer)
        {
            if (!isRaisingArms && !enemy.animator.GetBool("isAiming"))
            {
                isRaisingArms = true;
                Invoke(nameof(RaiseArmsUp), gunDrawTime);
                enemy.animator.SetBool("isAiming", true);
            }
        }
        else
            enemy.animator.SetBool("isAiming", false);

        if (isRaisingArms)
            return;

        if (readyToShoot && !reloading && bulletsleft <= 0)
            StartCoroutine(Reload(reloadCooldown));

        // Shooting
        if (enemy.enemyMovement.canSeePlayer && readyToShoot && !reloading && bulletsleft > 0)
        {
            bulletsShot = 0;
            enemy.StunEnemy(enemy.myWeaponClass.gunBulletsPerTap * enemy.myWeaponClass.gunTimeBetweenShots + 0.5f);
            Shoot();
        }
    }

    private void RaiseArmsUp() { isRaisingArms = false; }

    private void Shoot()
    {
        enemy.soundManager.Play("pistolShoot");

        readyToShoot = false;

        Vector3 targetPoint = Player.m.crouchLogic.HeadTransform.position;

        float spreadUp = Random.Range(-1f, 1f) * enemy.myWeaponClass.gunSpread / 10;
        float spreadRight = Random.Range(-1f, 1f) * enemy.myWeaponClass.gunSpread / 10;

        // Find random point in the same plane as the targetPoint.
        Vector3 spreadPoint = targetPoint
            + Player.m.crouchLogic.HeadTransform.right * spreadRight * (targetPoint - shootPoint.position).magnitude
            + Player.m.crouchLogic.HeadTransform.up * spreadUp * (targetPoint - shootPoint.position).magnitude;

        // Calculate direction as before.
        Vector3 directionWithSpread = (spreadPoint - shootPoint.position).normalized;

        // Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(enemyBullet, shootPoint.position, Quaternion.identity);

        EnemyBullet bullet = currentBullet.GetComponent<EnemyBullet>();
        bullet.bulletDamage = enemy.myWeaponClass.bulletDamage;

        // Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        // Add force to bullet
        Rigidbody bulletRB = currentBullet.GetComponent<Rigidbody>();
        bulletRB.AddForce(directionWithSpread.normalized * enemy.myWeaponClass.gunShootForce, ForceMode.Impulse);

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
            Invoke("ResetShot", enemy.myWeaponClass.gunTimeBetweenShooting);
            allowInvoke = false;
        }

        // if more than one bulletsPerTap make sure to repeat shoot function
        if (bulletsShot < enemy.myWeaponClass.gunBulletsPerTap && bulletsleft > 0)
            Invoke("Shoot", enemy.myWeaponClass.gunTimeBetweenShots);

    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private IEnumerator Reload(float duration)
    {
        reloading = true;

        enemyAmmoReloadDisplay.SliderSetActive(true);

        float time = 0.0f;

        do
        {
            time += Time.deltaTime;

            enemyAmmoReloadDisplay.UpdateSliderValue(time / duration);

            yield return 0;

        } while (time < duration);

        enemyAmmoReloadDisplay.SliderSetActive(false);

        bulletsleft = enemy.myWeaponClass.gunMagazineSize;
        reloading = false;
    }

}
