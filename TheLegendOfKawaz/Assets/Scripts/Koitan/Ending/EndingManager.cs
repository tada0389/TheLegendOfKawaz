using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using KoitanLib;

public class EndingManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro staffrollMesh;
    private Sequence seq;
    [SerializeField]
    private GameObject lastObj;
    private bool canSkip;
    // Start is called before the first frame update
    void Start()
    {
        lastObj.SetActive(false);
        staffrollMesh.gameObject.SetActive(true);
        staffrollMesh.color = new Color(1, 1, 1, 0);
        seq = DOTween.Sequence()
            .Append(staffrollMesh.DOFade(1f, 1f))
            .AppendInterval(4f)
            .Append(staffrollMesh.DOFade(0f, 1f))
            .AppendCallback(() => staffrollMesh.gameObject.SetActive(false))
            .AppendInterval(71f)
            .AppendCallback(()=> lastObj.SetActive(true))
            .Append(lastObj.transform.DOMoveY(2f, 6f).SetEase(Ease.Linear))
            .AppendCallback(() => canSkip = true);
    }

    // Update is called once per frame
    void Update()
    {
        if(canSkip & ActionInput.GetButtonDown(ActionCode.Decide))
        {
            FadeManager.FadeIn(2f, "ZakkyTitle");
        }

        if(ActionInput.GetButtonDown(ActionCode.Dash))
        {
            FadeManager.FadeIn(2f, "ZakkyTitle");
        }
    }
}
