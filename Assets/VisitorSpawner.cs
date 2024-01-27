using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitorSpawner : MonoBehaviour
{
    public GameObject visitorPrefab;
    public float spawnIntervalMin = 1.0f;
    public float spawnIntervalMax = 3.0f;

    void Start()
    {
        StartCoroutine(CoSpawnVisitor());
    }

    IEnumerator CoSpawnVisitor()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));
            SpawnVisitor();
        }
    }

    void SpawnVisitor()
    {
        Instantiate(visitorPrefab, transform.position, Quaternion.identity);
    }
}
