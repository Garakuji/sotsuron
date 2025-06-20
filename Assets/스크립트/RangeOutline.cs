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
        // ��� ǥ�ø� ���� ������ ����
        transform.localScale = Vector3.one * radius;
    }

    /// <summary>
    /// ǥ�ø� �����ϰ�, ���� �ð��� ������ onCharged �̺�Ʈ�� ȣ��
    /// </summary>
    public void BeginCharge()
    {
        StopAllCoroutines();
        StartCoroutine(DisplayAndInvoke());
    }

    private IEnumerator DisplayAndInvoke()
    {
        // ���� �ð� ���� �״�� ǥ��
        yield return new WaitForSeconds(displayDuration);
        onCharged?.Invoke();
    }
}
