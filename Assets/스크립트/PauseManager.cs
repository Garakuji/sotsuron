using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Pause Menu UI")]
    [Tooltip("ESC로 토글할 UI Panel")]
    public GameObject pauseCanvas;

    private bool isPaused = false;

    void Awake()
    {
        if (pauseCanvas == null)
            Debug.LogError("PauseManager: pauseCanvas에 패널을 할당하세요!");
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

    // Resume 버튼
    public void OnResume()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.menuSelectClip);
        TogglePause();
    }

    // Quit 버튼 (메인 메뉴로 돌아가기)
    public void OnQuitToMenu()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.menuSelectClip);
        // "CharacterSelect" 씬 이름에 맞춰 변경
        SceneManager.LoadScene("CharacterSelectScene");
    }
}
