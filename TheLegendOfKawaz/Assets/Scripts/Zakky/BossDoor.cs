using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using KoitanLib;

public class BossDoor : MonoBehaviour
{
    public int doorNum;

    BossFlagManager bossFlag;

    [SerializeField]
    private string next_scene_ = "TadaBossScene";

    [SerializeField,Multiline(5)]
    private string message;

    // Start is called before the first frame update
    void Start()
    {
        bossFlag = BossFlagManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {

        //Debug.Log("Called");
        if (col.tag == "Player")
        {
            MessageManager.OpenKanbanWindow(message);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {

        //Debug.Log("Called");
        if (col.tag == "Player")
        {
            //DOTweenでちょっと大きくする
            transform.DOScale(new Vector2(1.5f, 1.5f), 0.3f);
            //決定ボタン押したらシーン遷移
            if (ActionInput.GetButtonDown(ButtonCode.Up) && !FadeManager.is_fading)
            {
                bossFlag.bossRoomNum = doorNum;
                Debug.Log("tag:" + col.tag + "/nGetButtonDown(ActionCode.Shot):" + ActionInput.GetButtonDown(ActionCode.Shot));
                Debug.Log(bossFlag.bossRoomNum);
                //Scene読み込み
                //SceneManager.LoadScene(next_scene_);
                //koitan追加
                FadeManager.FadeIn(0.5f, next_scene_, 0);

            }
        }
    }

    //ドアから離れたらドアを縮める
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.DOScale(new Vector2(1f, 1f), 0.3f);
            MessageManager.CloseKanbanWindow();
        }
    }
}