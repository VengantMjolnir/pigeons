using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonFloat : MonoBehaviour
{
    public float OffsetScale = Mathf.PI;
    private Vector3 _startPos;
    private float _arbitraryOffset;

    // Start is called before the first frame update
    void Start()
    {
        _startPos = transform.position;
        _arbitraryOffset = Random.value * OffsetScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _startPos + new Vector3(0, Mathf.Sin(Time.time + _arbitraryOffset), 0);
    }
}
