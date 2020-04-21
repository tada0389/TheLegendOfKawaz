using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndContext : MonoBehaviour
{
    private Sequence seq;
    [SerializeField]
    private float waitTime;
    [SerializeField]
    private float duration = 10f;
    [SerializeField]
    private float targetY = 8f;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        seq = DOTween.Sequence()
            .AppendInterval(waitTime)
            .AppendCallback(() => gameObject.SetActive(true))
            .Append(transform.DOMoveY(targetY, duration).SetEase(Ease.Linear))
            .AppendCallback(() => gameObject.SetActive(false));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
