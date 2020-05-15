using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gage : MonoBehaviour
{
    [SerializeField]
    LineRenderer BHPBar;
    [SerializeField]
    Ciel m_ciel;

    private int bombNum;
    float gageNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        bombNum = 0;
        gageNum = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (bombNum >= 20)
        {
            bombNum = 0;
            m_ciel.CielRespawner();
        }
        //int gageNum = 0;
        DOTween.To(
        () => gageNum,          // 何を対象にするのか
        num => gageNum = num,   // 値の更新
        bombNum,                  // 最終的な値
        0.2f                  // アニメーション時間
        );

        BHPBar.SetPosition(0, new Vector3(10f * gageNum / 20, 0.0f, 0.0f));
    }

    public void bombNumIncrimenter()
    {
        bombNum++;
    }
}
