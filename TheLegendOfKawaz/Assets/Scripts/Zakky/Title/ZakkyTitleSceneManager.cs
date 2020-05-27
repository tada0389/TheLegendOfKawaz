using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KoitanLib;

public class ZakkyTitleSceneManager : MonoBehaviour
{
    public GameObject titleIDManager;
    //public GameObject titleState;
    public GameObject mySceneManager;
    public GameObject[] iconID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ボタン押されてたらシーン遷移を確かめる
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || ActionInput.GetButtonDown(ActionCode.Decide))
        {
            IsLoadScene();
        }
    }

    public void IsLoadScene()
    {
        if (TitleState.m_state == TitleState.State.Select)
        {
            iconID[titleIDManager.GetComponent<TitleIDManager>().ChoiceID].GetComponent<IconID>().starEffectSpawner.StarEffect(iconID[titleIDManager.GetComponent<TitleIDManager>().ChoiceID].transform.position);
            //選択肢えらんだら
            switch (titleIDManager.GetComponent<TitleIDManager>().ChoiceID)
            {
                case 0:
                    //stateを操作受け付けないやつにする
                    TitleState.m_state = TitleState.State.Decided;
                    //Fadeout関数でフェードアウト
                    //STARTのシーン遷移KoitanLibの方つかう(その方が演出がシームレス)
                    FadeManager.FadeIn(1f, "Prologue");
                    break;
                case 1:
                    TitleState.m_state = TitleState.State.Story;
                    break;
                case 2:
                    //stateを操作受け付けないやつにする
                    TitleState.m_state = TitleState.State.Decided;
                    //Fadeout関数でフェードアウト
                    mySceneManager.GetComponent<MySceneManager>().Fadeout(null);
                    break;
            }
        }
    }
}