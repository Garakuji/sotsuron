using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM")]
    public AudioClip bgmClip;
    [Range(0f, 1f)] public float bgmVolume = 0.25f;

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

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // (1) QuitButton 다시 바인딩
        var quitGO = GameObject.Find("Canvas/Button");
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
        // ★Hierarchy 경로는 프로젝트에 맞게 바꿔주세요.
        // 예) Canvas/SettingsPanel/BGMSlider
        var sliderGO = GameObject.Find("Canvas/Slider");
        if (sliderGO != null && sliderGO.TryGetComponent<Slider>(out var slider))
        {
            _bgmSlider = slider;
            // 초기값 동기화
            _bgmSlider.value = bgmVolume;
            // 리스너 재등록
            _bgmSlider.onValueChanged.RemoveAllListeners();
            _bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = bgmVolume;
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
    public void PlayMenuSelect() => PlaySFX(menuSelectClip);

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
