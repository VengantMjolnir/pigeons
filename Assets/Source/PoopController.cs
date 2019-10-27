using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopController : MonoBehaviour
{
    public GameObject PoopSplatter;
    public Vector3 splatterRotation;

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        GameObject splatter = Instantiate(PoopSplatter, transform.position, Quaternion.Euler(splatterRotation));
        Destroy(splatter, 1.0f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Balloon"))
        {
            other.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
