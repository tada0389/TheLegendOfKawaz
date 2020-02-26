using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BossDoor : MonoBehaviour
{
    public int doorNum;

    BossFlagManager bossFlag;

    // Start is called before the first frame update
    void Start()
    {
        bossFlag = BossFlagManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay2D(Collider2D col)
    {
        
        //Debug.Log("Called");
        if (col.tag == "Player" && ActionInput.GetButtonDown(ActionCode.Shot))
        {
            bossFlag.bossRoomNum = doorNum;
            Debug.Log("tag:" + col.tag + "/nGetButtonDown(ActionCode.Shot):" + ActionInput.GetButtonDown(ActionCode.Shot));
            Debug.Log(bossFlag.bossRoomNum);
            //Scene読み込み
            SceneManager.LoadScene("ZakkyScene");
            
        }
    }
}