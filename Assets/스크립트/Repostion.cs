using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Repostion : MonoBehaviour
{
    private Collider2D _coll;

    private void Awake()
    {
        _coll = GetComponent<Collider2D>();
        if (_coll == null)
            Debug.LogError("[Repostion] Collider2D가 없습니다!");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 1) collision이 유효한지
        if (collision == null || !collision.CompareTag("Area"))
            return;

        // 2) GameManager 및 player가 유효한지
        if (GameManager.Instance == null || GameManager.Instance.player == null)
            return;

        // 3) 이동 컴포넌트가 붙어 있는지
        var moveComp = GameManager.Instance.player.GetComponent<move_test>();
        if (moveComp == null)
            return;

        // 이제 안전하게 모든 값을 꺼내서 처리
        Vector3 playerPos = GameManager.Instance.player.position;
        Vector3 myPos = transform.position;

        float dirX = playerPos.x - myPos.x;
        float dirY = playerPos.y - myPos.y;
        float diffX = Mathf.Abs(dirX);
        float diffY = Mathf.Abs(dirY);

        // 1 또는 -1 로 정규화
        dirX = dirX > 0 ? 1f : -1f;
        dirY = dirY > 0 ? 1f : -1f;

        // 플레이어 입력 벡터
        Vector3 playerDir = moveComp.inputVec;

        switch (gameObject.tag)
        {
            case "Ground":
                if (diffX > diffY)
                    transform.Translate(Vector3.right * dirX * 80f);
                else
                    transform.Translate(Vector3.up * dirY * 80f);
                break;

            case "Enemy":
                // coll.enabled를 체크하려면 Awake에서 캐싱된 _coll 사용
                if (_coll != null && _coll.enabled)
                {
                    transform.Translate(
                        playerDir * 40f
                        + new Vector3(
                            Random.Range(-3f, 3f),
                            Random.Range(-3f, 3f),
                            0f
                          )
                    );
                }
                break;
        }
    }
}
