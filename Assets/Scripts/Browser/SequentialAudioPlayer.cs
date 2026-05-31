using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SequentialAudioPlayer : MonoBehaviour
{
    // Drag your audio clips into this list in the Unity Inspector
    [SerializeField] private List<AudioClip> playlist;
    
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Start the sequential playback
        StartCoroutine(PlayPlaylist());
    }

    private IEnumerator PlayPlaylist()
    {
        // Loop through every clip in the list
        for (int i = 0; i < playlist.Count; i++)
        {
            // Assign the current clip to the AudioSource
            audioSource.clip = playlist[i];
            
            // Play the clip
            audioSource.Play();
            
            // Wait until the clip is completely finished playing
            yield return new WaitForSeconds(audioSource.clip.length);
        }
        
        Debug.Log("All audio clips have finished playing!");
    }
}
