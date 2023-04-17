using UnityEngine;

public class ParticleManager : MonoBehaviour
{

    public GameObject bulletMiss, bulletHit;

    public void CreateParticle(Vector3 contactPoint, Vector3 direction, string particleType = "bulletMiss")
    {
        //GameObject toBeInstantiated = new GameObject();
        GameObject toBeInstantiated = null;
        switch (particleType)
        {
            case "bulletMiss":
                toBeInstantiated = bulletMiss;
                break;
            case "bulletHit":
                toBeInstantiated = bulletHit;
                break;
        }
        GameObject particle = Instantiate(toBeInstantiated, contactPoint, Quaternion.identity);
        particle.transform.eulerAngles = -Quaternion.FromToRotation(particle.transform.forward, direction).eulerAngles;
    }

}
