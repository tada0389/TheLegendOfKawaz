using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementUi : MonoBehaviour
{
    public AchievementContent content;
    public Image icon;
    public TextMeshProUGUI headMesh;
    public TextMeshProUGUI detailMesh;

    public void UpdateUi()
    {
        icon.sprite = content.icon;
        headMesh.text = content.isUnlocked ? content.body : "???";
        detailMesh.text = "条件：" + content.detail;
    }
}
