﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace KoitanLib
{
    public class FadeManager : MonoBehaviour
    {
        static FadeManager instance;
        public static FadeManager Instance
        {
            get { return instance; }
        }

        [SerializeField]
        private Canvas fadeCanvas;
        [SerializeField]
        private FadeImage fadeImage;
        [SerializeField]
        private Texture[] masks;

        public static RawImage image;
        private static bool is_fading = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                //DontDestroyOnLoad(instance);
                DontDestroyOnLoad(gameObject);
                DontDestroyOnLoad(fadeCanvas);
            }
            else Destroy(gameObject);

            image = GetComponent<RawImage>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public static void FadeIn(float duration, string next_scene_name, int maskNum = 0)
        {
            Instance.fadeImage.UpdateMaskTexture(Instance.masks[maskNum]);
            var async = SceneManager.LoadSceneAsync(next_scene_name);
            Sequence seq = DOTween.Sequence();
            seq.AppendCallback(() =>
            {
                is_fading = true;
                async.allowSceneActivation = false;
                DOTween.To(
                () => Instance.fadeImage.Range = 0,          // 何を対象にするのか
                num => Instance.fadeImage.Range = num,   // 値の更新
                1,                  // 最終的な値
                duration                  // アニメーション時間
                );
            }).AppendInterval(duration)
            .AppendCallback(() =>
            {
                async.allowSceneActivation = true;
                DOTween.To(
                () => Instance.fadeImage.Range = 1,          // 何を対象にするのか
                num => Instance.fadeImage.Range = num,   // 値の更新
                0,                  // 最終的な値
                duration                  // アニメーション時間
                );
            }
            ).AppendInterval(duration)
            .AppendCallback(() =>
            {
                is_fading = false;
            });
            //image.DOFade(0, duration);            
        }

        public static void FadeOut(float duration, string next_scene_name)
        {
            if (is_fading) return;
            is_fading = true;
            image.DOFade(1, duration).OnComplete(() => SceneManager.LoadScene(next_scene_name));
        }
    }

}

