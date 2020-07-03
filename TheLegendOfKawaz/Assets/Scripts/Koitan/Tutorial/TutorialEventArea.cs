using Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEventArea : MonoBehaviour
{
    [SerializeField]
    private int tutorialBookIndex;

    [SerializeField]
    private eBossType bossType;

    [SerializeField]
    private int needDeathCnt = 3;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            if (Global.GlobalDataManager.DeathCnts(bossType) >= needDeathCnt)
            {
                SettingManager.RequestOpenTutorial(tutorialBookIndex);
                gameObject.SetActive(false);
            }
        }
    }
}
