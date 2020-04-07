using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleState : MonoBehaviour
{
    // Start is called before the first frame update
    //ステートを管理
    [HideInInspector]
    public enum State
    {
        Select,
        Story,
        Decided
    }
    //ステージステートを入れとく
    [HideInInspector]
    public static State m_state;
    void Start()
    {
        //最初はスタート状態から
        m_state = State.Select;
    }

    // Update is called once per frame
    //void Update()
    //{
    //}
}
