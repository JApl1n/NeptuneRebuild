using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareGun : MonoBehaviour
{
    public GameObject flarePrefab;
    public Transform firingPoint;
    public float firingForce = 20f;
    public Vector3 offset = new Vector3(0f, 0f, 0f);
    
    private bool canFire = true;
    private float timer = 0f;
    private float reloadTime = 10f;

    private void Update() {
        timer += Time.deltaTime;
        if (timer >= reloadTime) {
            canFire = true;
            timer = 0f; 
            // FireFlare(); //comment out to remove automatic firing
        }
    }



    public void FireFlare() {
        if (canFire) {
            // Instantiate the flare projectile at the firing point
            firingPoint = this.transform;
            GameObject flare = Instantiate(flarePrefab, (firingPoint.position + offset), firingPoint.rotation);
            
            // Apply force to the flare projectile
            Rigidbody flareRb = flare.GetComponent<Rigidbody>();
            if (flareRb != null) {
                flareRb.AddForce(firingPoint.forward * firingForce, ForceMode.Impulse);
            }
            canFire = false;
        }
    }
}
