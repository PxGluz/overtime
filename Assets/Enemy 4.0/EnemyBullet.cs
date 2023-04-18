using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [HideInInspector]
    public float bulletDamage;

    private void Start()
    {
        if (Vector3.Distance(transform.position, Player.m.crouchLogic.HeadTransform.position) < 0.5f)
        {
            Player.m.TakeDamage(bulletDamage);
            Destroy(gameObject);
        }

        Invoke(nameof(Expire), 10f);
    }
    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward * 2f, Color.yellow);

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 2f))
        {
            HandleLayerLogic(hit);
        }
    }

    private void HandleLayerLogic(RaycastHit hit)
    {
        DestroyWhenShot destroyWhenShot = hit.collider.gameObject.GetComponent<DestroyWhenShot>();
        if (destroyWhenShot != null)
            destroyWhenShot.ReceiveHit();

        switch (LayerMask.LayerToName(hit.collider.gameObject.layer))
        {
            case "Interactable":
                return;

            case "Player":
                Player.m.TakeDamage(bulletDamage);
                break;

            case "Explosive":
                hit.collider.gameObject.GetComponent<ExplosiveBarrel>().ReceiveHit();
                break;
        }

        if (LayerMask.LayerToName(hit.collider.gameObject.gameObject.layer) != "Enemy")
        {
            Destroy(gameObject);
        }
    }

    private void Expire() => Destroy(gameObject);
}
