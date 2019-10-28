using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopController : MonoBehaviour
{
    public GameObject PoopSplatter;
    public Vector3 splatterRotation;
    public float splatSize = 3f;
    public float fudgeFactor = -0.1f;

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        DecalController.Instance.AddDecal(contact.point, contact.normal, splatSize, fudgeFactor);

        Splat();
        Destroy(gameObject);
    }

    private void Splat()
    {
        GameObject splatter = Instantiate(PoopSplatter, transform.position, Quaternion.Euler(splatterRotation));
        Destroy(splatter, 3.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Balloon"))
        {
            Splat();
            Destroy(gameObject);
        }
    }
}
