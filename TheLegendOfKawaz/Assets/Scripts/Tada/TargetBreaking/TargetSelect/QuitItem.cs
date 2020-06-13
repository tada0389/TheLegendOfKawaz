using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TargetBreaking
{
    [System.Serializable]
    public class QuitItem : BaseItem
    {
        [SerializeField]
        private string next_scene_ = "ZakkyScene";

        TargetSelectManager parent_;

        public override void Init(TargetSelectManager parent)
        {
            parent_ = parent;
            rectTransform = GetComponent<RectTransform>();
        }

        public override void OnStart()
        {
            // 即戻る
            KoitanLib.FadeManager.FadeIn(0.5f, next_scene_);
            parent_.box_.DOKill();
        }

        public override void Proc()
        {

        }

        public override void OnEnd()
        {

        }
    }
}