using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Repostion : MonoBehaviour
{
    Collider2D coll;

    private void Awake()
    {
        coll= GetComponent<Collider2D>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;

        // 플레이어 위치
        Vector3 playerPos = GameManager.Instance.player.position;
        Vector3 myPos = transform.position;

        float dirX = playerPos.x - myPos.x;
        float dirY = playerPos.y - myPos.y;
        float diffX = Mathf.Abs(dirX);
        float diffY = Mathf.Abs(dirY);

        // move_test 스크립트에서 입력 벡터 가져오기
        var moveComp = GameManager.Instance.player.GetComponent<move_test>();
        Vector3 playerDir = (moveComp != null) ? (Vector3)moveComp.inputVec : Vector3.zero;

        // 1 또는 -1 로 정규화
        dirX = dirX > 0 ? 1f : -1f;
        dirY = dirY > 0 ? 1f : -1f;

        switch (gameObject.tag)
        {
            case "Ground":
                if (diffX > diffY)
                    transform.Translate(Vector3.right * dirX * 80f);
                else
                    transform.Translate(Vector3.up * dirY * 80f);
                break;

            case "Enemy":
                if (coll.enabled)
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
