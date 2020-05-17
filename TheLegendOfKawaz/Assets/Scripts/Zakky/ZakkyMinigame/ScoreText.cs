using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    private TextMeshProUGUI m_text;
    [HideInInspector]
    public int m_score { private set; get; }
    private static int m_highScore = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<TextMeshProUGUI>();
        m_score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        m_text.text = "Score:       " + m_score.ToString("D5") + "Pt\n"
            + "HighScore:" + m_highScore.ToString("D5") + "Pt";
    }

    public void ScoreAdder(int num)
    {
        m_score += num;
        if (m_highScore < m_score) m_highScore = m_score;
    }
}
