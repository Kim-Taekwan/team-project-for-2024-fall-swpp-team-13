using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public float bgmVolume = 0.7f;
    public float sfxVolume = 0.7f;

    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public List<AudioClip> bgmList = new List<AudioClip>();

    [Header("Sound Effects")]
    public AudioClip coinSound;
    public AudioClip recipeSound;
    public AudioClip powerUpSound;
    public AudioClip jumpSound;
    public AudioClip gameClearSound;
    public AudioClip gameOverSound;
    public AudioClip attackSound;
    public AudioClip playerDamagedSound;
    public AudioClip moveSound;
    public AudioClip powerupLostSound;
    public AudioClip sparkSound;
    public AudioClip cliffSound;
    public AudioClip rewardSound;
    public AudioClip moveSceneSound;
    public AudioClip enemyDamagedSound;
    public AudioClip shootCarrotSound;
    public AudioClip sweetPotatoSound;
    public AudioClip chiliPepperSound;
    public AudioClip moveButtonSound;
    public AudioClip decisionButtonSound;
    public AudioClip flourOnFloorSound;
    public AudioClip flourOnScreenSound;
    public AudioClip cancelUISound;
    public AudioClip warpSound;
    public AudioClip brokenSound;
    public AudioClip electricWhiskSound;
    public AudioClip clapSound;


    private void Start()
    {
        if (bgmSource != null && bgmSource.clip != null)
        {
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }
    }


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Play the bgm having the same name with scene
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        for (int i = 0; i < bgmList.Count; i++)
        {
            if (arg0.name == bgmList[i].name)
            {
                PlayBGM(bgmList[i]);
                return;
            }
        }                
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    public void PlayBGM(AudioClip clip, bool isLoop = true)
    {
        if (bgmSource != null && clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.loop = isLoop;
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            GameObject go = new GameObject(clip.name);
            AudioSource audioSource = go.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = sfxVolume;
            audioSource.Play();
            Destroy(go, clip.length);
        }        
    }

    public void PlayLoopSFX(AudioClip clip, int instanceID)
    {
        if (sfxSource != null && clip != null)
        {
            if (GameObject.Find(clip.name + instanceID) != null)
            {
                return;
            }
            GameObject go = new GameObject(clip.name + instanceID);
            AudioSource audioSource = go.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.clip = clip;
            audioSource.volume = sfxVolume;
            audioSource.Play();
        }
    }

    public void PlaySpatialSFX(AudioClip clip, GameObject go)
    {
        if (sfxSource != null && clip != null && go != null)
        {            
            AudioSource audioSource = go.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.spatialBlend = 1.0f;
            audioSource.maxDistance = 50.0f;
            audioSource.clip = clip;
            audioSource.volume = sfxVolume;
            audioSource.Play();
        }
    }

    public void StopSpatialSFX(GameObject go)
    {
        if (go != null)
        {
            AudioSource audioSource = go.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }
    }

    public void StopLoopSFX(AudioClip clip, int instanceID)
    {
        if (sfxSource != null && clip != null)
        {
            GameObject loopSFX = GameObject.Find(clip.name + instanceID);
            if (loopSFX != null)
            {
                Destroy(loopSFX);
            }
        }
    }

    public void StopAllSounds()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }

        if (sfxSource != null && sfxSource.isPlaying)
        {
            sfxSource.Stop();
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    public void PlayGameClearSound() => PlayBGM(gameClearSound, false);
    public void PlayGameOverSound() => PlayBGM(gameOverSound, false);
    public void PlayCoinSound() => PlaySFX(coinSound);
    public void PlayRecipeSound() => PlaySFX(recipeSound);
    public void PlayPowerUpSound() => PlaySFX(powerUpSound);
    public void PlayJumpSound() => PlaySFX(jumpSound);
    public void PlayAttackSound() => PlaySFX(attackSound);
    public void PlayDamagedSound() => PlaySFX(playerDamagedSound);
    public void PlayMoveSound(int instanceID) => PlayLoopSFX(moveSound, instanceID);
    public void StopMoveSound(int instanceID) => StopLoopSFX(moveSound, instanceID);
    public void PlayPowerupLostSound() => PlaySFX(powerupLostSound);
    public void PlaySparkSound() => PlaySFX(sparkSound);
    public void PlayCliffSound() => PlaySFX(cliffSound);
    public void PlayRewardSound() => PlaySFX(rewardSound);
    public void PlayMoveSceneSound() => PlaySFX(moveSceneSound);
    public void PlayEnemyDamagedSound() => PlaySFX(enemyDamagedSound);
    public void PlayShootCarrotSound() => PlaySFX(shootCarrotSound);
    public void PlaySweetPotatoSound() => PlaySFX(sweetPotatoSound);
    public void PlayChiliPepperSound() => PlaySFX(chiliPepperSound);
    public void PlayMoveButtonSound() => PlaySFX(moveButtonSound);
    public void PlayDecisionButtonSound() => PlaySFX(decisionButtonSound);
    public void PlayFlourOnFloorSound() => PlaySFX(flourOnFloorSound);
    public void PlayFlourOnScreenSound() => PlaySFX(flourOnScreenSound);
    public void PlayCancelUISound() => PlaySFX(cancelUISound);
    public void PlayWarpSound() => PlaySFX(warpSound);
    public void PlayBrokenSound() => PlaySFX(brokenSound);
    public void PlayElectricWhiskSound(GameObject go) => PlaySpatialSFX(electricWhiskSound, go);
    public void StopElectricWhiskSound(GameObject go) => StopSpatialSFX(go);
    public void PlayClapSound() => PlaySFX(clapSound);
}
