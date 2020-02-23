using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoitanLib;

public class mameTest : MonoBehaviour
{
    [SerializeField]
    GameObject hitEff;
    [SerializeField]
    float speed = 12;
    [SerializeField]
    float stageWidth = 8.64f;
    [SerializeField]
    float stageHeight = 4.8f;
    [SerializeField]
    float charaWidth = 0.5f;
    [SerializeField]
    float charaHeight = 0.5f;
    public Vector3 dir = Vector3.right;
    Vector3 nowPos;
    // Start is called before the first frame update
    void Start()
    {
        nowPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        nowPos += dir * speed * Time.deltaTime;
        if (nowPos.y - charaHeight <= -stageHeight)
        {
            nowPos.y = -stageHeight + charaHeight;
            Death();
        }

        if (nowPos.x + charaWidth >= stageWidth)
        {
            nowPos.x = stageWidth - charaWidth;
            Death();
        }

        if (nowPos.x - charaWidth <= -stageWidth)
        {
            nowPos.x = -stageWidth + charaWidth;
            Death();
        }
        transform.position = nowPos;
    }

    private void Death()
    {
        Destroy(Instantiate(hitEff, nowPos, Quaternion.Euler(0, (dir.x == 1) ? 0 : 180, 0)), 0.5f);//クソ実装

        //Destroy(gameObject);
        gameObject.SetActive(false);
    }

    public void Init(Vector3 _pos, Vector3 _dir)
    {
        transform.position = nowPos = _pos;
        dir = _dir;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        GizmosExtensions2D.DrawWireRect2D(transform.position, charaWidth * 2, charaHeight * 2);
    }
}
