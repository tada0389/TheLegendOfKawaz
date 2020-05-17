using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    private TextMeshProUGUI m_text;
    [HideInInspector]
    public static int m_score { private set; get; }
    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<TextMeshProUGUI>();
        m_score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        m_text.text = "Score:" + m_score.ToString("D5") + "Pt";
    }

    public void ScoreAdder(int num)
    {
        m_score += num;
    }
}
