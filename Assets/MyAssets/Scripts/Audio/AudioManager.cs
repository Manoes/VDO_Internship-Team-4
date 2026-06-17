using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Play On Start")]
    [SerializeField] private bool playGameplayMusicOnStart = false;
    [SerializeField] private bool playStartSceneMusicOnStart = false;

    [Header("Music")]
    [SerializeField] private AudioClip[] gameplayMusic;
    [SerializeField, Range(0f, 1f)] private float gameplayMusicVolume = 1f;

    [SerializeField] private AudioClip startSceneMusic;
    [SerializeField, Range(0f, 1f)] private float startSceneMusicVolume = 1f;

    [Header("SFX")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField, Range(0f, 1f)] private float buttonClickVolume = 1f;

    [SerializeField] private AudioClip[] coconutHitSounds;
    [SerializeField, Range(0f, 1f)] private float coconutHitVolume = 1f;

    [SerializeField] private AudioClip gameOverSound;
    [SerializeField, Range(0f, 1f)] private float gameOverVolume = 1f;

    [SerializeField] private AudioClip jumpSound;
    [SerializeField, Range(0f, 1f)] private float jumpVolume = 1f;

    [SerializeField] private AudioClip[] bananaCollectSounds;
    [SerializeField, Range(0f, 1f)] private float bananaCollectVolume = 1f;

    [SerializeField] private AudioClip walkingSound;
    [SerializeField, Range(0f, 1f)] private float walkingVolume = 1f;

    void Start()
    {
        if (playGameplayMusicOnStart)
            PlayRandomGameplayMusic();

        if (playStartSceneMusicOnStart)
            PlayStartSceneMusic();
    }

    public void PlayRandomGameplayMusic()
    {
        PlayMusic(GetRandomClip(gameplayMusic), gameplayMusicVolume);
    }

    public void PlayStartSceneMusic()
    {
        PlayMusic(startSceneMusic, startSceneMusicVolume);
    }

    private void PlayMusic(AudioClip clip, float volume)
    {
        if (musicSource == null || clip == null) return;

        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSound, buttonClickVolume);
    }

    public void PlayCoconutHit()
    {
        PlaySFX(GetRandomClip(coconutHitSounds), coconutHitVolume);
    }

    public void PlayGameOver()
    {
        PlaySFX(gameOverSound, gameOverVolume);
    }

    public void PlayJump()
    {
        PlaySFX(jumpSound, jumpVolume);
    }

    public void PlayBananaCollect()
    {
        PlaySFX(GetRandomClip(bananaCollectSounds), bananaCollectVolume);
    }

    public void PlayWalking()
    {
        PlaySFX(walkingSound, walkingVolume);
    }

    private void PlaySFX(AudioClip clip, float volume)
    {
        if (sfxSource == null || clip == null) return;

        sfxSource.PlayOneShot(clip, volume);
    }

    private AudioClip GetRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return null;

        return clips[Random.Range(0, clips.Length)];
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
}