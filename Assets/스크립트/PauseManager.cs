using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Pause Menu UI")]
    [Tooltip("ESC�� ����� UI Panel")]
    public GameObject pauseCanvas;

    private bool isPaused = false;

    void Awake()
    {
        if (pauseCanvas == null)
            Debug.LogError("PauseManager: pauseCanvas�� �г��� �Ҵ��ϼ���!");
        pauseCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseCanvas.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;

        Cursor.visible = isPaused;
        Cursor.lockState = isPaused
            ? CursorLockMode.None
            : CursorLockMode.Locked;
    }

    // Resume ��ư
    public void OnResume()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.menuSelectClip);
        TogglePause();
    }

    // Quit ��ư (���� �޴��� ���ư���)

    public void OnQuitToMenu()
    {
            // 1) �Ͻ����� ����
            Time.timeScale = 1f;

            // 2) ��ư ���� ȿ����
            AudioManager.Instance.PlayMenuSelect();

            // 3) GameManager �ı� (OnDestroy���� Instance=null �� �ڵ����� �˴ϴ�)
            if (GameManager.Instance != null)
                Destroy(GameManager.Instance.gameObject);

            // 4) ĳ���� ���� �� �ε�
            SceneManager.LoadScene("CharacterSelectScene");
    }

}
