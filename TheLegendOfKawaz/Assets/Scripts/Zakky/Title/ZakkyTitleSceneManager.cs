using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || ActionInput.GetButtonDown(ActionCode.Jump))
        {
            iconID[titleIDManager.GetComponent<TitleIDManager>().ChoiceID].GetComponent<IconID>().StarEffect();
            IsLoadScene();
        }
    }

    public void IsLoadScene()
    {
        if (TitleState.m_state == TitleState.State.Select)
        {
            //選択肢えらんだら
            switch (titleIDManager.GetComponent<TitleIDManager>().ChoiceID)
            {
                case 0:
                    //SceneManager.LoadScene("Cooking");
                    //stateを操作受け付けないやつにする
                    TitleState.m_state = TitleState.State.Decided;
                    //Fadeout関数でフェードアウト
                    mySceneManager.GetComponent<MySceneManager>().Fadeout("ZakkyScene");
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