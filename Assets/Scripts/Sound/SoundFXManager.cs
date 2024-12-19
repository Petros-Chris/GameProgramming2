using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    Settings gameManager;
    //public float volume = 1f;
    [SerializeField] private AudioSource soundFXObject;
    private void Awake(){
        if(instance == null){
            instance = this;
        }
        gameManager = GameObject.Find("GameManager").GetComponent<Settings>();
    }

    public void prepareSoundFXClip(string audioClip, Transform spawnTransform, float volume)
    {
        string path = "Sounds/" + gameManager.soundPath +"/"+ audioClip;
        AudioClip clip = Resources.Load<AudioClip>(path);
         if (clip == null)
    {
        Debug.LogError($"AudioClip not found at path: {path}. Ensure the path is correct and the file is in the Resources folder.");
    }
        PlaySoundFXClip(clip,spawnTransform, volume);

    }
    

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume){
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength  = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);

    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume){

        int rand = Random.Range(0, audioClip.Length);


        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip[rand];

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength  = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);

    }
    
}
