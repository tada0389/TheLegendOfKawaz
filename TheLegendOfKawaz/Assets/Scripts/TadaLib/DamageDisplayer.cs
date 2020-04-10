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

            texts_ = new List<DamageText>(10);
            //KoitanLib.ObjectPoolManager.Init(text_prefab_, this,  10);
            for (int i = 0; i < 10; ++i)
            {
                texts_.Add(Instantiate(text_prefab_));
                DontDestroyOnLoad(texts_[i]);
                texts_[i].gameObject.SetActive(false);
            }
        }

        public void ShowDamage(int damage, Vector3 pos, eDamageType type)
        {
            //DamageText text = KoitanLib.ObjectPoolManager.GetInstance<DamageText>(text_prefab_);
            DamageText text = null;
            for(int i = 0; i < 10; ++i)
            {
                if (!texts_[i].gameObject.activeSelf)
                {
                    text = texts_[i];
                    break;
                }
            }

            if (text == null) return;

            text.gameObject.SetActive(true);
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