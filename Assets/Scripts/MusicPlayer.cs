using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioClip> musicTracks;
    private int lastPlayedIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        if (musicTracks.Count > 0)
        {
            PlayRandomTrack();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayRandomTrack();
        }
    }

    private void PlayRandomTrack()
    {
        if (musicTracks.Count == 0)
        {
            Debug.Log("No music tracks available.");
            return;
        }

        int newIndex;
        do
        {
            newIndex = UnityEngine.Random.Range(0, musicTracks.Count);
        } while (newIndex == lastPlayedIndex && musicTracks.Count > 1);

        lastPlayedIndex = newIndex;
        audioSource.clip = musicTracks[newIndex];
        audioSource.Play();
    }
}