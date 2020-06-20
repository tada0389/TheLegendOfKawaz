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

        public override void Init(TargetSelectManager parent, int index)
        {
            parent_ = parent;
            ItemIndex = index;
            rectTransform = GetComponent<RectTransform>();
        }

        public override void OnStart()
        {
            // ゴーストを消す
            TargetSelectManager.LoadGameGhost = null;
            TargetSelectManager.IsLoadGhost = false;
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