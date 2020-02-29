using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Test
{
    public class TestMover : TadaLib.Mover
    {
        [SerializeField]
        private float duration_ = 3.0f;

        [SerializeField]
        private Vector3 from_;
        [SerializeField]
        private Vector3 to_;

        [SerializeField]
        private Ease ease_;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            transform.DOMove(to_, duration_).SetLoops(-1, LoopType.Yoyo).SetEase(ease_);
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}