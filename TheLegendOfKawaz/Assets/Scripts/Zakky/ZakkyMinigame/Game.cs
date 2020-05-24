using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    //Gameクラスの唯一のインスタンス
    private static Game mInstance;
    //Gameのインスタンスを渡すパブリックな関数
    public static Game instance
    {
        get
        {
            //インスタンスが格納されているか
            if (mInstance == null)
            {
                //インスタンスを探し、参照
                mInstance = FindObjectOfType<Game>();
            }
            //インスタンスを返す
            return mInstance;
        }
    }

    //ゲームの状態
    public enum STATE
    {
        NONE,
        START,
        MOVE,
        FEVER,
        GAMEOVER
    };
    //ゲームの状態
    public STATE state
    {
        get;
        set;
    }
        

    // Start is called before the first frame update
    void Start()
    {
        //ゲームの状態をスタートにする
        state = STATE.MOVE;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
