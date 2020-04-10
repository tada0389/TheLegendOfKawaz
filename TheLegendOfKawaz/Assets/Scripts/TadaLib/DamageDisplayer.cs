using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace TadaLib
{
    public class DamageDisplayer : SingletonMonoBehaviour<DamageDisplayer>
    {
        public enum eDamageType
        {
            Mini,
            Normal,
            Big,
        }

        [SerializeField]
        private Canvas canvas_;

        // ダメージのテキスト
        [SerializeField]
        private DamageText text_prefab_;

        private List<DamageText> texts_;

        protected override void Awake()
        {
            base.Awake();

            texts_ = new List<DamageText>();

            KoitanLib.ObjectPoolManager.Init(text_prefab_, this,  10);
        }

        public void ShowDamage(int damage, Vector3 pos, eDamageType type)
        {
            DamageText text = KoitanLib.ObjectPoolManager.GetInstance<DamageText>(text_prefab_);

            if (text == null) return;

            switch (type)
            {
                case eDamageType.Mini:
                    text.Display(damage, 24, 0.5f, pos, canvas_);
                    break;
                case eDamageType.Normal:
                    text.Display(damage, 36, 0.75f, pos, canvas_);
                    break;
                case eDamageType.Big:
                    text.Display(damage, 48, 1.0f, pos, canvas_);
                    break;
            }
        }
    }
}