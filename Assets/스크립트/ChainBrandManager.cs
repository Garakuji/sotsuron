using UnityEngine;

public class ChainBrandManager : MonoBehaviour
{
    public static ChainBrandManager Instance;

    public GameObject applyVFXPrefab;
    public GameObject transmitVFXPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayApplyVFX(Vector3 position)
    {
        if (applyVFXPrefab == null) return;

        GameObject fx = Instantiate(applyVFXPrefab, position, Quaternion.identity);
        fx.SetActive(true);
    }

    public void PlayTransmitVFX(Vector3 position)
    {
        if (transmitVFXPrefab == null) return;

        GameObject fx = Instantiate(transmitVFXPrefab, position, Quaternion.identity);
        fx.SetActive(true);
    }
}
