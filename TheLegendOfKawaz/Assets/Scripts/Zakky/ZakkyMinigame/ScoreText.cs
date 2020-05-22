using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreText : MonoBehaviour
{
    private TextMeshProUGUI m_text;
    [HideInInspector]
    public int m_score { private set; get; }
    private float m_scoreTween;
    private static int m_highScore = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<TextMeshProUGUI>();
        m_score = 0;
        m_scoreTween = 0;
    }

    // Update is called once per frame
    void Update()
    {
        DOTween.To(
        () => m_scoreTween,          // 何を対象にするのか
        num => m_scoreTween = num,   // 値の更新
        m_score,                  // 最終的な値
        1f                  // アニメーション時間
        );
        m_text.text = "Score:       " + Mathf.RoundToInt(m_scoreTween).ToString("D5") + "Pt\n"
            + "HighScore:" + m_highScore.ToString("D5") + "Pt";

        if (m_highScore < m_score) m_highScore = m_score;
    }

    public void ScoreAdder(int num)
    {
        m_score += num;
    }
}
