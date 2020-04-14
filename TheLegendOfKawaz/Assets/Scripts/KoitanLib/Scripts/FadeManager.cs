using System.Collections;
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
        public static bool is_fading { private set; get; }

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
            var async = SceneManager.LoadSceneAsync(next_scene_name);
            Sequence seq = DOTween.Sequence();
            seq.SetUpdate(true);
            seq.OnStart(() =>
            {
                is_fading = true;
                Instance.fadeImage.UpdateMaskTexture(Instance.masks[maskNum]);
            })
            .AppendCallback(() =>
            {
                async.allowSceneActivation = false;
                DOTween.To(
                () => Instance.fadeImage.Range = 0,          // 何を対象にするのか
                num => Instance.fadeImage.Range = num,   // 値の更新
                1,                  // 最終的な値
                duration                  // アニメーション時間
                ).SetUpdate(true);
            }).AppendInterval(duration).SetUpdate(true)
            .AppendCallback(() =>
            {
                async.allowSceneActivation = true;
                DOTween.To(
                () => Instance.fadeImage.Range = 1,          // 何を対象にするのか
                num => Instance.fadeImage.Range = num,   // 値の更新
                0,                  // 最終的な値
                duration                  // アニメーション時間
                ).SetUpdate(true);
            }
            ).AppendInterval(duration).SetUpdate(true)
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
            image.DOFade(1, duration).OnComplete(() => SceneManager.LoadScene(next_scene_name)).SetUpdate(true);
        }
    }

}

