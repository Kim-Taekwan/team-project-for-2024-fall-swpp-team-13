using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public float bgmVolume = 1.0f;
    public float sfxVolume = 1.0f;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource != null && clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
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

    public void PlayCoinSound() => PlaySFX(coinSound);
    public void PlayRecipeSound() => PlaySFX(recipeSound);
    public void PlayPowerUpSound() => PlaySFX(powerUpSound);
    public void PlayJumpSound() => PlaySFX(jumpSound);
    public void PlayGameClearSound() => PlayBGM(gameClearSound);
    public void PlayGameOverSound() => PlaySFX(gameOverSound);
    public void PlayAttackSound() => PlaySFX(attackSound);
    public void PlayDamagedSound() => PlaySFX(damagedSound);
    public void PlayMoveSound() => PlaySFX(moveSound);
}
