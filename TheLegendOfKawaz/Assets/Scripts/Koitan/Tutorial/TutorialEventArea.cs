using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEventArea : MonoBehaviour
{
    [SerializeField]
    private int tutorialBookIndex;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            SettingManager.RequestOpenTutorial(tutorialBookIndex);
            gameObject.SetActive(false);
        }
    }
}
