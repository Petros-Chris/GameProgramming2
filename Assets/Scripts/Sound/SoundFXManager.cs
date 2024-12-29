using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    public AudioSource globalAudioObject;
    Settings gameManager;
    private string soundPath = "SoundFX";
    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioSource musicObject;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        soundFXObject = Resources.Load<AudioSource>("PreFabs/Sounds/SoundFXObject");
        musicObject = Resources.Load<AudioSource>("PreFabs/Sounds/MusicObject");
        gameManager = GameObject.Find("DifficultyController").GetComponent<Settings>();
    }

    public void PrepareSoundFXClip(string audioClip, Transform spawnTransform, float volume, bool isGlobal = false, bool isLoop = false, bool isMusic = false)
    {
        string path;
        if (GameObject.Find("GameManager"))
            path = "Sounds/" + gameManager.pathForSound + "/" + audioClip;
        else
            path = "Sounds/" + soundPath + "/" + audioClip;

        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip == null)
        {
            Debug.LogError($"AudioClip not found at path: {path}. Ensure the path is correct and the file is in the Resources folder.");
        }

        if (isGlobal && isLoop)
        {
            PlayGlobalSound(clip, spawnTransform, volume);
            return;
        }
        else if (isGlobal && !isLoop)
        {
            if (!isMusic)
            {
                PlayGlobalMusic(clip, spawnTransform, volume);
                return;
            }

            PlayGlobalSoundNoLoop(clip, spawnTransform, volume);
        }

        PlaySoundFXClip(clip, spawnTransform, volume);
    }

    public void PrepareSoundFXClipArray(string[] audioClip, Transform spawnTransform, float volume)
    {
        AudioClip[] clip = new AudioClip[audioClip.Length];
        for (int i = 0; i < audioClip.Length; i++)
        {
            string path = "Sounds/" + gameManager.pathForSound + "/" + audioClip[i];
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
        AudioSource audioSource = Instantiate(musicObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.loop = true;

        audioSource.spread = 360;

        audioSource.Play();

        globalAudioObject = audioSource;
    }

    public void PlayGlobalMusic(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(musicObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.spread = 360;

        audioSource.Play();

        globalAudioObject = audioSource;
    }

    public void DestroyGlobalSound(AudioSource audioSource)
    {
        audioSource.Stop();
        Destroy(audioSource.gameObject);
    }

    public void PlayGlobalSoundNoLoop(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.spread = 360;

        audioSource.Play();

        globalAudioObject = audioSource;
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

    public bool ChanceToPlaySound(float chance)
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
