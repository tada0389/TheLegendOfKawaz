using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    //[SerializeField]
    //private Vector2 iniVelo;
    [SerializeField]
    private float maxVelo;
    private Rigidbody2D m_rigidbody2D;
    public float bounce;
    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        //m_rigidbody2D.velocity = IniVelo;
    }

    // Update is called once per frame
    void Update()
    {
        //if (m_rigidbody2D.velocity.magnitude > maxVelo)
        {
            Vector2 vec = m_rigidbody2D.velocity;
            //vec.x = Mathf.Sign(vec.x) * maxVelo;
            if (vec.y < -maxVelo) vec.y = -maxVelo;
            m_rigidbody2D.velocity = vec;
            //m_rigidbody2D.velocity = m_rigidbody2D.velocity.normalized * maxVelo;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //床に当たったら床ぶっ壊す
        if (col.tag == "KawazCeil")
        {
            Destroy(col.gameObject);
            //爆破もする
            Destroy(gameObject);
        }
        else if (col.tag == "KawazWall" || col.tag == "Player") //プレイヤーか井戸に当たったらまけ
        {
            Debug.Log("まけ");
            Time.timeScale = 0f;
        }
        else if (col.tag == "KawaztanShot")
        {
            Destroy(gameObject);
        }
        else
        {
            //速度反転
            //col.gameObject.GetComponent<Rigidbody2D>().velocity = -col.gameObject.GetComponent<Rigidbody2D>().velocity;
            Vector3 vec = m_rigidbody2D.velocity;
            vec.y = bounce;
            m_rigidbody2D.velocity = vec;
        }
    }
}
