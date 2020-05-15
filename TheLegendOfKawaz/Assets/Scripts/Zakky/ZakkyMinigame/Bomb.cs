using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // コレ重要

public class Bomb : MonoBehaviour
{
    //[SerializeField]
    //private Vector2 iniVelo;
    [SerializeField]
    private float maxVelo;
    [SerializeField]
    BaseParticle bomFX;
    //[SerializeField]
    Gage m_gage;

    private Rigidbody2D m_rigidbody2D;
    [HideInInspector]
    public float bounce;
    [HideInInspector]
    public BombSpawner m_bombSpawner;
    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_gage = GameObject.Find("Gage").GetComponent<Gage>();
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
            //Destroy(col.gameObject);
            col.gameObject.SetActive(false);
            //爆破もする
            //Destroy(gameObject);
            TadaLib.EffectPlayer.Play(bomFX, transform.position, Vector3.zero);
            gameObject.SetActive(false);
        }
        else if (col.tag == "KawazWall" || col.tag == "Player") //プレイヤーか井戸に当たったらまけ
        {
            //Debug.Log("まけ");
            //Time.timeScale = 0f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (col.tag == "KawaztanShot")
        {
            //m_bombSpawner.brokenBombsSum++;
            m_gage.bombNumIncrimenter();
            //Debug.Log(m_bombSpawner.brokenBombsSum.ToString());
            TadaLib.EffectPlayer.Play(bomFX, transform.position, Vector3.zero);
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        else if (col.tag == "ToumeiStage")
        {

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
