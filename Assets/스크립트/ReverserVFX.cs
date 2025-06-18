using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ReverseVFX : MonoBehaviour
{
    private ParticleSystem _ps;
    private float _duration;
    private float _currentTime;
    private bool _isReversing;

    void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
        _duration = _ps.main.duration;
    }

    void OnEnable()
    {
        // 즉시 전체 재생 상태로 셋업
        _ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _ps.Clear(true);

        _currentTime = _duration;
        _ps.Simulate(_currentTime, withChildren: true, restart: true);

        // 무한 역재생 시작
        _isReversing = true;
    }

    void Update()
    {
        if (!_isReversing) return;

        // 시간 감소
        _currentTime -= Time.deltaTime;
        if (_currentTime <= 0f)
        {
            // 끝에 도달하면 다시 시작으로 리셋 (무한 루프)
            _currentTime = _duration;
        }

        // 역재생 시뮬레이션
        _ps.Simulate(_currentTime, withChildren: true, restart: false);
    }
}
