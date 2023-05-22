using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public GameObject BrickDeathEffect;

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Brock") {
            Destroy(other.gameObject);
            Debug.Log("Çarptı");
        }
    }
}
