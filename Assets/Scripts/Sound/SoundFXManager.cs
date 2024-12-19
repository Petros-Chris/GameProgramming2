using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    Settings gameManager;
    //public float volume = 1f;
    [SerializeField] private AudioSource soundFXObject;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        gameManager = GameObject.Find("GameManager").GetComponent<Settings>();
    }

    public void prepareSoundFXClip(string audioClip, Transform spawnTransform, float volume)
    {
        string path = "Sounds/" + gameManager.soundPath + "/" + audioClip;
        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip == null)
        {
            Debug.LogError($"AudioClip not found at path: {path}. Ensure the path is correct and the file is in the Resources folder.");
        }
        PlaySoundFXClip(clip, spawnTransform, volume);

    }

    public void prepareSoundFXClipArray(string[] audioClip, Transform spawnTransform, float volume)
    {
        AudioClip[] clip = new AudioClip[audioClip.Length];
        for (int i = 0; i < audioClip.Length; i++)
        {
            string path = "Sounds/" + gameManager.soundPath + "/" + audioClip[i];
            clip[i] = Resources.Load<AudioClip>(path);
            if (clip == null)
            {
                Debug.LogError($"AudioClip not found at path: {path}. Ensure the path is correct and the file is in the Resources folder.");
            }
        }

        PlayRandomSoundFXClip(clip, spawnTransform, volume);

    }


    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.spatialBlend = 1f;

        audioSource.spread = 360;

        audioSource.maxDistance = 20f;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);

    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {

        int rand = Random.Range(0, audioClip.Length);


        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip[rand];

        audioSource.volume = volume;

        audioSource.spatialBlend = 1f;

        audioSource.spread = 360;

        audioSource.maxDistance = 20f;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);

    }

    public bool chancePlaySound(float chance)
    {
        float randNum = Random.Range(1f, 100f);
        if (randNum < chance)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

}
