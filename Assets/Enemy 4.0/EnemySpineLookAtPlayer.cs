using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemySpineLookAtPlayer : MonoBehaviour
{
    public float minimumX, maximumX;
    public EnemyMaster enemy;

    void LateUpdate()
    {
        if (!enemy.enemyMovement.canSeePlayer)
            return;

        Vector3 direction = (Player.m.transform.position - transform.position).normalized;

        // calculate the rotation angle based on the direction vector
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        // only modify the x rotation angle
        float xAngle = targetRotation.eulerAngles.x;
        xAngle = ClampAngle(xAngle, minimumX, maximumX);

        // create a new quaternion with the clamped x angle and the original y and z angles
        Quaternion clampedRotation = Quaternion.Euler(xAngle, transform.eulerAngles.y, transform.eulerAngles.z);

        // set the object's rotation to the clamped quaternion
        transform.rotation = clampedRotation;
    }

    float ClampAngle(float angle, float from, float to)
    {
         // accepts e.g. -80, 80
         if (angle < 0f) angle = 360 + angle;
         if (angle > 180f) return Mathf.Max(angle, 360+from);
         return Mathf.Min(angle, to);
    }
}
