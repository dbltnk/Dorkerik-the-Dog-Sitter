using System.Collections;
using UnityEngine;

public class VisitorSpawner : MonoBehaviour
{
    [SerializeField] private GameObject visitorPrefab;
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private float spawnIntervalMin = 4f;
    [SerializeField] private float spawnIntervalMax = 8f;

    private AudioSource _audioSource;

    private void Awake() => _audioSource = GetComponent<AudioSource>();

    private void Start() => StartCoroutine(SpawnVisitorCoroutine());

    private IEnumerator SpawnVisitorCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));
            InstantiateVisitor();
        }
    }

    private void InstantiateVisitor()
    {
        var visitorInstance = Instantiate(visitorPrefab, transform.position, Quaternion.identity, GameObject.Find("Hoomans").transform);
        _audioSource.PlayOneShot(spawnSound);
    }
}