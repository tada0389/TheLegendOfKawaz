using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (col.tag == "Player" && ActionInput.GetButtonDown(ActionCode.Shot))
        {
            //Scene読み込み
            bossFlag.bossRoomNum = doorNum;
        }
    }
}