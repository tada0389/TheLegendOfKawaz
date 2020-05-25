using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GotPoint : MonoBehaviour
{
    TextMeshPro m_text;

    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        //アルファ値を引く
        Color col = m_text.color;
        //消える秒数で割る
        col.a -= Time.deltaTime / 2f;
        m_text.color = col;
        if (col.a <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    
    public static void PointInit(TextMeshPro text, Vector3 pos, Quaternion ang, int score, Transform owner = null)
    {
        var txt = KoitanLib.ObjectPoolManager.GetInstance<TextMeshPro>(text);
        if (txt == null) return;
        txt.transform.position = pos;
        txt.transform.rotation = ang;
        if (score >= 0) txt.text = "+" + score.ToString();
        else txt.text = score.ToString();
        txt.transform.parent = owner;
        //アルファ値を1にする
        Color col = txt.color;
        if (score > 0)
        {
            if (Game.instance.state == Game.STATE.FEVER) col = Color.yellow;
            else col = Color.green;
        }
        else if (score == 0) col = Color.black;
        else col = Color.red;
        
        //col.a = 1f;
        txt.color = col;
        txt.gameObject.SetActive(true);
    }
    
}
