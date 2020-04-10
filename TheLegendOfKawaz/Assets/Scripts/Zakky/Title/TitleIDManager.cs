using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleIDManager : MonoBehaviour
{
    //public GameObject titleState;
    public int ChoiceID
    {
        get;
        set;
    }

    // Use this for initialization
    void Start()
    {
        ChoiceID = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //ストーリーテキストから戻る
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1) || ActionInput.GetButtonDown(ActionCode.Back)) &&
            TitleState.m_state == TitleState.State.Story)
        {
            TitleState.m_state = TitleState.State.Select;
        }

        //セレクトモードじゃなければ動かない
        if (TitleState.m_state != TitleState.State.Select) return;

        //ボタンのID切り替え
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || ActionInput.GetButtonDown(ButtonCode.Up))
        {
            ChoiceID--;
            if (ChoiceID < 0)
                ChoiceID = 2;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || ActionInput.GetButtonDown(ButtonCode.Down))
        {
            ChoiceID++;
            if (ChoiceID > 2)
                ChoiceID = 0;
        }
        
    }
}