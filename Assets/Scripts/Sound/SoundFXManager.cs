using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    public AudioSource globalAudioObject;

    Settings gameManager;
    string soundPath; // TODO: make settings exist everywhere
    //public float volume = 1f;
    [SerializeField] private AudioSource soundFXObject;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (GameObject.Find("GameManager"))
            gameManager = GameObject.Find("GameManager").GetComponent<Settings>();
        else
            soundPath = "SoundFX";
    }

    public void PrepareSoundFXClip(string audioClip, Transform spawnTransform, float volume, bool isGlobal = false)
    {
        string path;
        if (GameObject.Find("GameManager"))
            path = "Sounds/" + gameManager.soundPath + "/" + audioClip;
        else
            path = "Sounds/" + soundPath + "/" + audioClip;

        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip == null)
        {
            Debug.LogError($"AudioClip not found at path: {path}. Ensure the path is correct and the file is in the Resources folder.");
        }

        if (isGlobal)
        {
            PlayGlobalSound(clip, spawnTransform, volume);
            return;
        }

        PlaySoundFXClip(clip, spawnTransform, volume);

    }

    public void PrepareSoundFXClipArray(string[] audioClip, Transform spawnTransform, float volume)
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

    public void PlayGlobalSound(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.loop = true;

        audioSource.spread = 360;

        audioSource.Play();

        globalAudioObject = audioSource;
    }

    public void DestroyGlobalSound(AudioSource audioSource)
    {
        audioSource.Stop();
        Destroy(audioSource.gameObject);
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
