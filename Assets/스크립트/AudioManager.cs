using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM")]
    public AudioClip bgmClip;
    [Range(0f, 1f)] public float bgmVolume = 0.1f;

    [Header("SFX")]
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Player VO")]
    public AudioClip[] playerHitClips;
    public AudioClip levelUpClip;
    public AudioClip playerDeathClip;

    [Header("Enemy VO")]
    public AudioClip[] enemyHitClips;

    [Header("UI VO")]
    public AudioClip menuSelectClip;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    // 바인딩할 슬라이더
    private Slider _bgmSlider;
    private Slider _sfxSlider;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.clip = bgmClip;
            musicSource.loop = true;
            musicSource.volume = bgmVolume;
            musicSource.Play();

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.volume = sfxVolume;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // (1) QuitButton 다시 바인딩
        var quitGO = GameObject.Find("Canvas/QuitButton");
        if (quitGO != null && quitGO.TryGetComponent<Button>(out var quitBtn))
        {
            quitBtn.onClick.RemoveAllListeners();
            quitBtn.onClick.AddListener(() =>
            {
                PlayMenuSelect();
                QuitGame();
            });
        }

        // (2) BGM Slider 다시 바인딩
        var bgmSliderGO = GameObject.Find("Canvas/BGMSlider");
        if (bgmSliderGO != null && bgmSliderGO.TryGetComponent<Slider>(out var bgmSlider))
        {
            _bgmSlider = bgmSlider;
            _bgmSlider.value = bgmVolume;
            _bgmSlider.onValueChanged.RemoveAllListeners();
            _bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        // (3) SFX Slider 다시 바인딩
        var sfxSliderGO = GameObject.Find("Canvas/SFXSlider");
        if (sfxSliderGO != null && sfxSliderGO.TryGetComponent<Slider>(out var sfxSlider))
        {
            _sfxSlider = sfxSlider;
            _sfxSlider.value = sfxVolume;
            _sfxSlider.onValueChanged.RemoveAllListeners();
            _sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayRandomSFX(AudioClip[] clips)
    {
        if (clips != null && clips.Length > 0)
            PlaySFX(clips[Random.Range(0, clips.Length)]);
    }

    public void PlayPlayerHit() => PlayRandomSFX(playerHitClips);
    public void PlayLevelUp() => PlaySFX(levelUpClip);
    public void PlayPlayerDeath() => PlaySFX(playerDeathClip);
    public void PlayEnemyHit() => PlayRandomSFX(enemyHitClips);

    public void PlayMenuSelect()
    {
        if (menuSelectClip == null) return;
        sfxSource.PlayOneShot(menuSelectClip, 0.5f);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
