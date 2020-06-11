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

        [SerializeField]
        private TMPro.TextMeshProUGUI loadingTextMesh;
        public static bool is_fading { private set; get; }
        public static OpenState openState = OpenState.Opened;
        static AsyncOperation async;
        static float m_duration;


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
        }

        // Start is called before the first frame update
        void Start()
        {
            DebugTextManager.Display(() => "FadeState:" + openState.ToString() + "\n");
            //DebugTextManager.Display(() => "NowLoading..." + (async.priority * 100f).ToString() + "%\n");
            //ChangeFadeColor(Color.white);
        }

        // Update is called once per frame
        void Update()
        {
            if (openState == OpenState.Closed && async.progress >= 0.9f)
            {
                openState = OpenState.Opening;
                async.allowSceneActivation = true;
                DOTween.To(
                () => Instance.fadeImage.Range = 1,          // 何を対象にするのか
                num => Instance.fadeImage.Range = num,   // 値の更新
                0,                  // 最終的な値
                m_duration               // アニメーション時間
                ).SetUpdate(true)
                .OnComplete(() =>
                {
                    is_fading = false;
                    openState = OpenState.Opened;
                    loadingTextMesh.gameObject.SetActive(false);
                });
            }
            if (async != null)
            {
                loadingTextMesh.text = (async.progress * 100).ToString("0") + "%";
            }
        }

        public static void ChangeFadeColor(Color c)
        {
            Instance.fadeImage.color = c;
        }

        public static bool FadeIn(float duration, string next_scene_name, int maskNum = 0)
        {
            //if (openState != OpenState.Opened) return false;
            openState = OpenState.Closing;
            m_duration = duration;
            async = SceneManager.LoadSceneAsync(next_scene_name);
            async.allowSceneActivation = false;
            instance.loadingTextMesh.gameObject.SetActive(true);
            Sequence seq = DOTween.Sequence();
            seq.SetUpdate(true);
            seq.OnStart(() =>
            {
                is_fading = true;
                Instance.fadeImage.UpdateMaskTexture(Instance.masks[maskNum]);
            })
                .Append(
                DOTween.To(
                () => Instance.fadeImage.Range = 0,          // 何を対象にするのか
                num => Instance.fadeImage.Range = num,   // 値の更新
                1,                  // 最終的な値
                duration                  // アニメーション時間
                )
                .SetUpdate(true))
            .AppendCallback(() =>
            {
                openState = OpenState.Closed;
            });
            return true;
        }
    }

    public enum OpenState
    {
        Closed,
        Opening,
        Opened,
        Closing
    }
}

