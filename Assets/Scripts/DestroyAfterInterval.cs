using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterInterval : MonoBehaviour
{
    public float interval;
    private void Awake()
    {
        Invoke(nameof(DestroyObject), interval);
    }

    private void DestroyObject() => Destroy(gameObject);
}
