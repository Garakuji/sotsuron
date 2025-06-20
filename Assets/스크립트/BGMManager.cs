using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }
    public AudioClip bgmClip;    // Inspector에 BGM 파일 할당
    [Range(0f, 1f)] public float volume = 0.5f;

    private AudioSource _source;

    void Awake()
    {
        // 싱글턴 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _source = gameObject.AddComponent<AudioSource>();
            _source.clip = bgmClip;
            _source.loop = true;
            _source.volume = volume;
            _source.playOnAwake = false;
            _source.Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 볼륨을 런타임에 바꿔야 할 때
    /// </summary>
    public void SetVolume(float vol)
    {
        volume = Mathf.Clamp01(vol);
        if (_source != null) _source.volume = volume;
    }
}
