using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryText : MonoBehaviour
{
    //public GameObject titleState;
    //子オブジェクトでやる(activeがfalseでアクセス不可を防ぐため)
    public GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //StoryStateのとき表示
        if (TitleState.m_state == TitleState.State.Story)
        {
            canvas.SetActive(true);
        }
        else
        {
            canvas.SetActive(false);
        }
    }
}
