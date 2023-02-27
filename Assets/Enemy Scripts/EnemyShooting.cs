using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject bullet;
    public GameObject weapon;
    public GameObject weaponDrop;

    [Header("Stats")]
    public int currentBullets;
    [Tooltip("How many bullets are in a full magazine")]
    public int bullets;
    public float gunForwardForce;
    public float gunUpwardForce;

    [Header("Other references")]
    public GameObject weaponPlace;
    public GameObject bulletPlace;

    private void Start() 
    {
        GameObject newWeapon = Instantiate(weapon, weaponPlace.transform.position, new Quaternion());
        newWeapon.transform.LookAt(weaponPlace.transform.position - weaponPlace.transform.forward);
        newWeapon.transform.localScale = newWeapon.transform.localScale / 2;
        newWeapon.transform.parent = weaponPlace.transform;
    }

    public void Shoot(Transform target, float damage)
    {
        // If the enemy has no bullets left, he reloads and misses a shooting oportunity.
        if (currentBullets == 0) {
            currentBullets = bullets;
            Debug.Log($"{gameObject.name} has reloaded");
        } else {
            GameObject newBullet = Instantiate(bullet, bulletPlace.transform.position, new Quaternion());

            EnemyBullet enemyBullet = newBullet.GetComponent<EnemyBullet>();
            enemyBullet.bulletDamage = damage;
            
            newBullet.transform.LookAt(target);
            Rigidbody bulletRB = newBullet.GetComponent<Rigidbody>();
            Vector3 direction = Vector3.Normalize(target.position - bulletPlace.transform.position);
            bulletRB.AddForce(direction * gunForwardForce, ForceMode.Impulse);
            bulletRB.AddForce(Vector3.up * gunUpwardForce, ForceMode.Impulse);

            currentBullets--;
        }
    }
}
