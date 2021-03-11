using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonController: MonoBehaviour
{
    public float PopCoefficient = 0.91f;
    public float RequiredSpeed = 10f;
    public float OffsetScale = Mathf.PI;
    public GameObject PopPrefab;
    public RuntimeTransformSet balloonSet;

    private Vector3 _startPos;
    private float _arbitraryOffset;

    // Start is called before the first frame update
    void Start()
    {
        _startPos = transform.position;
        _arbitraryOffset = Random.value * OffsetScale;

        balloonSet.Add(transform);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _startPos + new Vector3(0, Mathf.Sin(Time.time + _arbitraryOffset), 0);
    }

    public void Pop()
    {
        GameObject go = Instantiate(PopPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        Destroy(go, 5f);
        gameObject.SetActive(false);
    }
}
