using System;
using System.Collections;
using System.Collections.Generic;
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
    public AudioClip damagedSound;
    public AudioClip moveSound;

    //Debugìš©, ì™„ì„±ì‹œ Start ë©”ì„œë“œ ì‚­ì œ
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

    // Scene°ú µ¿ÀÏÇÑ ÀÌ¸§ÀÇ BGMÀ» Àç»ý
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        for (int i = 0; i < bgmList.Count; i++)
        {
            if (arg0.name == bgmList[i].name)
            {
                PlayBGM(bgmList[i]);
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

    public void PlayGameClearSound() => PlayBGM(gameClearSound, false);
    public void PlayGameOverSound() => PlayBGM(gameOverSound, false);
    public void PlayCoinSound() => PlaySFX(coinSound);
    public void PlayRecipeSound() => PlaySFX(recipeSound);
    public void PlayPowerUpSound() => PlaySFX(powerUpSound);
    public void PlayJumpSound() => PlaySFX(jumpSound);
    public void PlayAttackSound() => PlaySFX(attackSound);
    public void PlayDamagedSound() => PlaySFX(damagedSound);
    public void PlayMoveSound() => PlaySFX(moveSound);
}
