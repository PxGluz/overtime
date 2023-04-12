using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{

    public GameObject bulletMiss, bulletHit, enemyDeath;

    public void CreateParticle(Vector3 contactPoint, Vector3 direction, string particleType = "bulletMiss")
    {
        GameObject toBeInstantiated = new GameObject();
        switch (particleType)
        {
            case "bulletMiss":
                toBeInstantiated = bulletMiss;
                break;
            case "bulletHit":
                toBeInstantiated = bulletHit;
                break;
            case "enemyDeath":
                toBeInstantiated = enemyDeath;
                break;
        }

        GameObject particle = Instantiate(toBeInstantiated, contactPoint, Quaternion.identity);
        particle.transform.eulerAngles = Quaternion.FromToRotation(particle.transform.forward, direction).eulerAngles;
    }

}
