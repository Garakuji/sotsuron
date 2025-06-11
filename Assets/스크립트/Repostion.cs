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

        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector3 myPos = transform.position;
        float dirX = playerPos.x - myPos.x;
        float dirY = playerPos.y - myPos.y;

        float diffX = Mathf.Abs(dirX);
        float diffY = Mathf.Abs(dirY);

        Vector3 playerDir = GameManager.Instance.player.inputVec;

        dirX = dirX > 0 ? 1 : -1;
        dirY = dirY > 0 ? 1 : -1;

        switch (transform.tag)
        {
            case "Ground":
                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * 80);
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 80);
                }
                break;
            case "Enemy":
                if (coll.enabled)
                {
                    transform.Translate(playerDir * 40 + new Vector3(Random.Range(-3f,3f), Random.Range(-3f, 3f), 0f));
                }
                break;
        }

    }
}
