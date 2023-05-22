using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] musicClips; // Oynatılacak müzik parçalarının listesi
    private AudioSource audioSource; // Ses kaynağı

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }


    private void Start()
    {
        PlayRandomMusic();
    }


    private void PlayRandomMusic()
    {
        if (musicClips.Length == 0)
        {
            Debug.LogWarning("Müzik parçaları tanımlanmamış!");
            return;
        }

        int randomIndex = Random.Range(0, musicClips.Length);
        AudioClip randomClip = musicClips[randomIndex];

        audioSource.clip = randomClip;
        audioSource.Play();
    }

    public void PlayNextMusic()
    {
        audioSource.Stop();
        PlayRandomMusic();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

        // Update is called once per frame
        void Update()
    {
        
    }
}
