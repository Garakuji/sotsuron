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
            // 1) 일시정지 해제
            Time.timeScale = 1f;

            // 2) 버튼 선택 효과음
            AudioManager.Instance.PlayMenuSelect();

            // 3) GameManager 파괴 (OnDestroy에서 Instance=null 이 자동으로 됩니다)
            if (GameManager.Instance != null)
                Destroy(GameManager.Instance.gameObject);

            // 4) 캐릭터 선택 씬 로드
            SceneManager.LoadScene("CharacterSelectScene");
    }

}
