using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor;
using DG.Tweening;

namespace Target
{
    public class TargetController : BaseActorController
    {
        [SerializeField]
        private TargetManager manager_;

        // 移動情報
        [SerializeField]
        private float duration_ = 3.0f;

        [SerializeField]
        private Vector3 move_distance_;

        [SerializeField]
        private Ease ease_ = Ease.Linear;

        private Vector3 to_ = Vector3.zero;

        // Start is called before the first frame update
        void Start()
        {
            manager_.RegisterTarget();

            to_ = transform.position + move_distance_;
            transform.DOMove(to_, duration_).SetLoops(-1, LoopType.Yoyo).SetEase(ease_);
        }

        public override void Damage(int damage)
        {
            manager_.BreakTarget(transform.position);
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            if (to_ == Vector3.zero)
                Gizmos.DrawLine(transform.position, transform.position + move_distance_);
            else Gizmos.DrawLine(transform.position, to_);
        }
    }
}