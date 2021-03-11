using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public Vector3 spinValues;
    public GameObject PopPrefab;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(spinValues * Time.deltaTime);
    }

    public void Pop()
    {
        GameObject go = Instantiate(PopPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        Destroy(go, 5f);
        gameObject.SetActive(false);
    }
}
