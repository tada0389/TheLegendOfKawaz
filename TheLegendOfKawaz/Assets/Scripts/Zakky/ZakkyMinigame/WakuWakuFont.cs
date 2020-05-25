using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class WakuWakuFont : MonoBehaviour
{
    bool fiverCheck;
    Sequence seq;
    TextMeshProUGUI m_text;
    Color prevColor;
    // Start is called before the first frame update
    void Start()
    {
        fiverCheck = false;
        m_text = GetComponent<TextMeshProUGUI>();
        prevColor = m_text.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.instance.state == Game.STATE.FEVER)
        {
            if (!fiverCheck)
            {
                fiverCheck = true;
                m_text.color = Color.yellow;
                transform.DOKill();
                transform.DOScale(Vector2.one * 2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            }
        }
        else
        {
            if (fiverCheck)
            {
                fiverCheck = false;
                m_text.color = prevColor;
                transform.DOKill();
                transform.DOScale(Vector2.one, 0.5f);
            }
        }
        
    }
}
