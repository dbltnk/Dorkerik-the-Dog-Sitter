using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VisitorSpawner : MonoBehaviour
{
    public GameObject visitorPrefab;
    private float spawnIntervalMin = 2f;
    private float spawnIntervalMax = 4f;
    private AudioSource audioSource;
    public AudioClip spawnSound;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

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
        GameObject vis = Instantiate(visitorPrefab, transform.position, Quaternion.identity);
        vis.transform.SetParent(GameObject.Find("Hoomans").transform);
        audioSource.PlayOneShot(spawnSound);
    }
}