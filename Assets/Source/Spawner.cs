using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public GameObject initialObject;
    public float spawnWaitTime = 10f;

    private GameObject _spawnedObject;
    private bool spawning = false;

    public void Start()
    {
        _spawnedObject = initialObject;
    }

    public void Update()
    {
        if (_spawnedObject == null && !spawning)
        {
            StartCoroutine(SpawnObjectRoutine());
        }
    }

    public IEnumerator SpawnObjectRoutine()
    {
        spawning = true;
        yield return new WaitForSeconds(spawnWaitTime);
        _spawnedObject = Instantiate(objectToSpawn, transform);
        _spawnedObject.transform.localPosition = Vector3.zero;
        spawning = false;
    }
}
