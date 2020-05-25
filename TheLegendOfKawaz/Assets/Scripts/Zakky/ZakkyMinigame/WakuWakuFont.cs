using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class WakuWakuFont : MonoBehaviour
{
    bool fiverCheck;
    Sequence seq;
    // Start is called before the first frame update
    void Start()
    {
        fiverCheck = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.instance.state == Game.STATE.FEVER)
        {
            if (!fiverCheck)
            {
                fiverCheck = true;
                transform.DOKill();
                transform.DOScale(Vector2.one * 2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            }
        }
        else
        {
            if (fiverCheck)
            {
                fiverCheck = false;
                transform.DOKill();
                transform.DOScale(Vector2.one, 0.5f);
            }
        }
        
    }
}
