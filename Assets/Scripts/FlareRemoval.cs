using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareRemoval : MonoBehaviour
{
    [SerializeField] private float lifespan = 9f;
    private float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > lifespan) {
            DestroyObject(gameObject);
        }
    }
}
