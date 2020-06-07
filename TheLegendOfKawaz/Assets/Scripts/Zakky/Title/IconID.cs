using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconID : MonoBehaviour
{

    public GameObject titleIDManager;
    public GameObject titleSceneManager;
    public StarEffectSpawner starEffectSpawner;
    //public GameObject titleState;
    public GameObject m_particle;

    //MyIDで番号を決める
    public int MyID;

    [SerializeField]
    private float defaultScale;
    private float θ;
    private float ω;

    // Use this for initialization
    void Start()
    {
        //titleIDManager = GameObject.Find("TitleScript");   //スクリプト管理するEmptyObjectを取得

        θ = 0f;
        ω = 1.5f;
    }
    
    // Update is called once per frame
    void Update()
    {


        if (titleIDManager.GetComponent<TitleIDManager>().ChoiceID == MyID)
        {    //今選ばれてるButtonIDが自分のと一致するなら
            θ += ω * Time.deltaTime;

            this.transform.localScale = Vector2.one * defaultScale * 1.2f;
        }
        else
        {    //IDが一致しないなら
            θ = 0f;
            this.transform.localScale = Vector2.one * defaultScale;

        }
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 10f * Mathf.Sin(θ)));
    }

    void OnMouseEnter()
    {
        //のったときMyIDからChoiceIDを変える
        titleIDManager.GetComponent<TitleIDManager>().ChoiceID = MyID;
    }

    void OnMouseDown()
    {
        //starEffectSpawner.StarEffect(transform.position);
        //ロードする関数を呼び出す
        titleSceneManager.GetComponent<ZakkyTitleSceneManager>().IsLoadScene();
    }

}
