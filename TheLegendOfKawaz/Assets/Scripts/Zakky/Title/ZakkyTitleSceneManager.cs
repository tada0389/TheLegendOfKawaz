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

    // セーブデータを削除するクラス
    [SerializeField]
    private Save.SaveDeleter save_deleter_; // by tada

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
                    // スキル情報や部屋情報のセーブデータを削除
                    save_deleter_.DeleteTempData();
                    //Fadeout関数でフェードアウト
                    //STARTのシーン遷移KoitanLibの方つかう(その方が演出がシームレス)
                    FadeManager.FadeIn(1f, "Prologue");
                    break;
                case 1:
                    //TitleState.m_state = TitleState.State.Story;
                    //stateを操作受け付けないやつにする
                    TitleState.m_state = TitleState.State.Decided;
                    // セーブデータを削除せずに始める
                    //save_deleter_.DeleteTempData();
                    //Fadeout関数でフェードアウト
                    //STARTのシーン遷移KoitanLibの方つかう(その方が演出がシームレス)
                    FadeManager.FadeIn(1f, "ZakkyScene");
                    break;
                case 2:
                    //stateを操作受け付けないやつにする
                    TitleState.m_state = TitleState.State.Decided;
                    //Fadeout関数でフェードアウト
                    mySceneManager.GetComponent<MySceneManager>().Fadeout(null);
                    break;
                case 3: // セーブデータをすべて削除 by tada
                    save_deleter_.DeleteAllData();
                    // TODO ここにContinueの選択肢をなくす処理を実装する
                    break;
            }
        }
    }
}