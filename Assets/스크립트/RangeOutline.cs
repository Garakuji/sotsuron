using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class RangeOutline : MonoBehaviour
{
    [Header("Outline Settings")]
    [Tooltip("The radius of the attack area to display immediately")]
    public float radius = 1f;

    [Tooltip("Delay before invoking onCharged (seconds)")]
    public float displayDuration = 1f;

    public UnityEvent onCharged;

    private void Awake()
    {
        // 즉시 표시를 위한 스케일 설정
        transform.localScale = Vector3.one * radius;
    }

    /// <summary>
    /// 표시를 시작하고, 일정 시간이 지나면 onCharged 이벤트를 호출
    /// </summary>
    public void BeginCharge()
    {
        StopAllCoroutines();
        StartCoroutine(DisplayAndInvoke());
    }

    private IEnumerator DisplayAndInvoke()
    {
        // 일정 시간 동안 그대로 표시
        yield return new WaitForSeconds(displayDuration);
        onCharged?.Invoke();
    }
}
