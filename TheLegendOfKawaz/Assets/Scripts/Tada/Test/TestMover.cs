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
        private Vector3 move_distance_;

        [SerializeField]
        private Ease ease_;

        private Vector3 to_ = Vector3.zero;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            to_ = transform.position + move_distance_;
            transform.DOMove(to_, duration_).SetLoops(-1, LoopType.Yoyo).SetEase(ease_);
        }

        protected override void Update()
        {
            base.Update();
        }

        private void OnDrawGizmos()
        {
            if (to_ == Vector3.zero)
                Gizmos.DrawLine(transform.position, transform.position + move_distance_);
            else Gizmos.DrawLine(transform.position, to_);
        }
    }
}