using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffectDestroy : MonoBehaviour
{
    private float time;
    void Start()
    {
        time = 0.0f;
    }

    void Update()
    {
        time += Time.deltaTime;
        if (time >1.0f) {
            Destroy(gameObject);
        }
    }
}
