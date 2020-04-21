using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class SimpleFeeder : MonoBehaviour
{
    [SerializeField]
    private Ease ease_ = Ease.Linear;

    [SerializeField]
    private float feed_duration_ = 1.0f;

    private void Start()
    {
        TextMeshPro ren = GetComponent<TextMeshPro>();
        ren.DOFade(0f, feed_duration_).SetLoops(-1, LoopType.Yoyo);
    }
}
