using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public float visionAngle;
    public float visionDistance;
    void Start()
    {
        visionAngle /= 2;
    }

    void Update()
    {
        Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * visionDistance, new Color(0, 1, 0));
    }

    public bool IsInView(GameObject obj)
    {
        Vector3 dir = obj.transform.position - gameObject.transform.position;
        if (Physics.Raycast(gameObject.transform.position, Vector3.Normalize(dir), out RaycastHit hitInfo, visionDistance))
        {
            if (hitInfo.collider.gameObject.Equals(obj))
            {
                if (Mathf.Abs(Vector3.Angle(gameObject.transform.forward, Vector3.Normalize(dir))) <= visionAngle)
                {
                    Debug.DrawRay(gameObject.transform.position, dir, new Color(0, 0, 1));
                    return true;
                }
                return false;
            }
            return false;
        }
        return false;
    }
}
