using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System;
using DG.Tweening;
using KoitanLib;
using UnityEngine.Rendering.PostProcessing;


public class SettingManager : MonoBehaviour
{
    public TextMeshProUGUI titleUi;
    public TextMeshProUGUI headUi;
    public Image cursor;
    public Image window;
    public GameObject item;
    public GameObject skillItem;
    public RectTransform achievementItem;
    [SerializeField]
    private float scrollMinY;
    private float scrollMaxY;
    private float nowScrollY;
    [SerializeField]
    private float scrollSpeed = 100;
    private OpenState openState = OpenState.Closed;
    private SceneState sceneState = SceneState.Game;
    public float width;
    private int nowIndex = 0;
    private int maxIndex;
    private Vector3 cursorDefaultPos;
    public Vector2 defaultDeltaSize;
    public Vector2 targetDeltaSize;
    public Ease ease;
    public Color defaultColor;
    public Color targetColor;

    delegate void OnPush();
    delegate void OnSelected();
    OnSelected[] onSelecteds;
    OnPush onCancel;
    Func<string> textStr;

    [SerializeField]
    private AudioClip decisionSe;
    [SerializeField]
    private AudioClip cancelSe;
    [SerializeField]
    private AudioClip drumSe;
    [SerializeField]
    private AudioMixer audioMixer;

    private float bgmVol;
    private float seVol;
    private float masterVol;

    private AudioSource audioSource;

    private int ScreenSizeNum = 1;
    private bool isPostEffect = true;

    static SettingManager Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        cursorDefaultPos = cursor.transform.localPosition;
        onSelecteds = new OnSelected[4];
        StartPlacement();
        nowIndex = 0;
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(window);
        window.rectTransform.sizeDelta = defaultDeltaSize;
        window.color = defaultColor;
        window.gameObject.SetActive(false);
        item.SetActive(false);

        SceneManager.sceneLoaded += SetPost;
    }

    // Update is called once per frame
    void Update()
    {
        switch (openState)
        {
            case OpenState.Closed:
                //TimeScaleが小さいときメニューを開けないようにする(仮)
                /*
                if (ActionInput.GetButtonDown(ActionCode.Pause) && Time.timeScale > 0.5f && !FadeManager.is_fading)
                {
                    OpenWindow();
                }
                */
                break;
            case OpenState.Opened:
                //項目が2個以上ないと動かせない
                if (maxIndex >= 2)
                {
                    if (ActionInput.GetButtonDown(ButtonCode.Up))
                    {
                        nowIndex--;
                        nowIndex = (nowIndex + maxIndex) % maxIndex;
                        PlaySe(drumSe);
                    }

                    if (ActionInput.GetButtonDown(ButtonCode.Down))
                    {
                        nowIndex++;
                        nowIndex %= maxIndex;
                        PlaySe(drumSe);
                    }
                }

                if (maxIndex >= 1)
                {
                    if (onSelecteds[nowIndex] != null)
                    {
                        onSelecteds[nowIndex]();
                    }
                }
                //テキストの更新
                titleUi.text = textStr();

                if (onCancel != null && ActionInput.GetButtonDown(ActionCode.Back))
                {
                    onCancel();
                    PlaySe(cancelSe);
                }
                cursor.transform.localPosition = cursorDefaultPos + Vector3.down * width * nowIndex;

                if (ActionInput.GetButtonDown(ActionCode.Pause))
                {
                    CloseWindow();
                }
                break;
            default:
                break;
        }
    }

    //シーンが切り替わったときに行う
    void SetPost(Scene nextScene, LoadSceneMode mode)
    {
        PostProcessLayer ppl = Camera.main.GetComponent<PostProcessLayer>();
        //Debug.Log(ppl);
        if (ppl != null)
        {
            ppl.enabled = isPostEffect;
        }
    }

    void SetPost()
    {
        PostProcessLayer ppl = Camera.main.GetComponent<PostProcessLayer>();
        //Debug.Log(ppl);
        if (ppl != null)
        {
            ppl.enabled = isPostEffect;
        }
    }

    void OpenWindow()
    {
        Sequence seq = DOTween.Sequence()
            .OnStart(() =>
            {
                openState = OpenState.Opening;
                window.gameObject.SetActive(true);
                TadaLib.TimeScaler.Instance.RequestChange(0.0f);
            })
            .Append(window.rectTransform.DOSizeDelta(targetDeltaSize, 0.25f)).SetEase(ease).SetUpdate(true)
            .Join(window.DOColor(targetColor, 0.25f)).SetEase(ease).SetUpdate(true)
            .AppendCallback(() =>
            {
                if (isTargetScene())
                {
                    RetryMenu();
                }
                else
                {
                    StartPlacement();
                }
                nowIndex = 0;
                item.SetActive(true);
                openState = OpenState.Opened;
            });
    }

    public static void RequestOpenWindow()
    {
        Instance.OpenWindow();
    }

    void CloseWindow()
    {
        Sequence seq = DOTween.Sequence()
            .OnStart(() =>
            {
                nowIndex = 0;
                item.SetActive(false);
                openState = OpenState.Closing;
            })
            .Append(window.rectTransform.DOSizeDelta(defaultDeltaSize, 0.25f)).SetEase(Ease.InOutSine).SetUpdate(true)
            .Join(window.DOColor(defaultColor, 0.25f)).SetEase(Ease.InOutSine).SetUpdate(true)
            .AppendCallback(() =>
            {
                openState = OpenState.Closed;
                window.gameObject.SetActive(false);
                TadaLib.TimeScaler.Instance.DismissRequest(0.0f);
            });
    }

    //audioSource.PlayOneShotが不便すぎる
    void PlaySe(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.time = 0;
        audioSource.Play();
    }

    OnSelected SetButtonPush(Action onPush)
    {
        return () =>
        {
            if (ActionInput.GetButtonDown(ActionCode.Decide))
            {
                onPush();
                //audioSource.PlayOneShot(decisionSe);
                //AudioSource.PlayClipAtPoint(decisionSe, transform.position);
                PlaySe(decisionSe);
            }
        };
    }

    void StartPlacement()
    {
        cursor.gameObject.SetActive(true);
        //skillItem.SetActive(true);
        achievementItem.gameObject.SetActive(false);
        maxIndex = 4;
        headUi.text = "メニュー";
        textStr = () => "そうさほうほう\nオプション\nじっせき\nメニューをとじる";
        onSelecteds[0] = SetButtonPush(Manual);
        onSelecteds[1] = SetButtonPush(Option);
        onSelecteds[2] = SetButtonPush(AchievementScreen);
        onSelecteds[3] = SetButtonPush(CloseWindow);
        onCancel = CloseWindow;
    }


    void Manual()
    {
        maxIndex = 0;
        nowIndex = 0;
        headUi.text = "そうさほうほう";
        textStr = () => ActionInput.GetSpriteCode(ActionCode.Jump) + "ジャンプ\n" + ActionInput.GetSpriteCode(ActionCode.Shot) + "ショット\n" + ActionInput.GetSpriteCode(ActionCode.Dash) + "ダッシュ";
        onCancel = StartPlacement;
        onCancel += () => nowIndex = 0;
        cursor.gameObject.SetActive(false);
    }

    void Option()
    {
        cursor.gameObject.SetActive(true);
        headUi.text = "オプション";
        textStr = () => "がめんせってい\nおんりょうせってい\nタイトルにもどる";
        maxIndex = 3;
        nowIndex = 0;
        onSelecteds[0] = SetButtonPush(VideoOption);
        onSelecteds[1] = SetButtonPush(BgmOption);
        onSelecteds[2] = SetButtonPush(ReturnTitle);
        onCancel = StartPlacement;
        onCancel += () => nowIndex = 1;
    }

    void VideoOption()
    {
        cursor.gameObject.SetActive(true);
        maxIndex = 3;
        nowIndex = 0;
        headUi.text = "がめんせってい";
        textStr = () => "フルスクリーン\u3000< " + ScreenIsFull() + " >\nかいぞうど < " + ScreenSizeString() + " >\nポストエフェクト < " + PostEffectString() + " >";
        onSelecteds[0] = SetFullScreen();
        onSelecteds[1] = SetScreenSize();
        onSelecteds[2] = SetPostEffect();
        onCancel = Option;
        onCancel += () => nowIndex = 0;
    }

    void BgmOption()
    {
        cursor.gameObject.SetActive(true);
        maxIndex = 4;
        nowIndex = 0;
        audioMixer.GetFloat("MasterVol", out masterVol);
        audioMixer.GetFloat("BGMVol", out bgmVol);
        audioMixer.GetFloat("SEVol", out seVol);
        headUi.text = "おんりょうせってい";
        textStr = () => "全体 < " + (masterVol / 10 + 8) + " >\nBGM < " + (bgmVol / 10 + 8) + " >\nこうかおん < " + (seVol / 10 + 8) + " >\n元にもどす";
        onSelecteds[0] = () => SetVol("MasterVol", ref masterVol);
        onSelecteds[1] = () => SetVol("BGMVol", ref bgmVol);
        onSelecteds[2] = () => SetVol("SEVol", ref seVol);
        onSelecteds[3] = SetDefault();
        onCancel = Option;
        onCancel += () => nowIndex = 1;
    }

    void AchievementScreen()
    {
        cursor.gameObject.SetActive(false);
        skillItem.SetActive(false);
        achievementItem.localPosition = new Vector3(0, scrollMinY, 0);
        achievementItem.gameObject.SetActive(true);
        maxIndex = 1;
        nowIndex = 0;
        nowScrollY = scrollMinY;
        AchievementManager.UpdateUis();
        scrollMaxY = AchievementManager.GetScrollMaxY();
        onSelecteds[0] = ScrollView;
        headUi.text = "じっせき";
        textStr = () => "";
        onCancel = StartPlacement;
        onCancel += () => nowIndex = 2;
    }

    void ScrollView()
    {
        if (ActionInput.GetButton(ButtonCode.Up))
        {
            nowScrollY -= scrollSpeed * Time.unscaledDeltaTime;
            if (nowScrollY < scrollMinY)
            {
                nowScrollY = scrollMinY;
            }
        }
        if (ActionInput.GetButton(ButtonCode.Down))
        {
            nowScrollY += scrollSpeed * Time.unscaledDeltaTime;
            if (nowScrollY > scrollMaxY)
            {
                nowScrollY = scrollMaxY;
            }
        }
        achievementItem.localPosition = new Vector3(0, nowScrollY, 0);

    }

    void SetVol(string mixerName, ref float vol)
    {
        if (ActionInput.GetButtonDown(ButtonCode.Right))
        {
            vol += 10;
            vol = Mathf.Min(vol, 20);
            audioMixer.SetFloat(mixerName, vol);
            PlaySe(drumSe);
        }
        if (ActionInput.GetButtonDown(ButtonCode.Left))
        {
            vol -= 10;
            vol = Math.Max(vol, -80);
            audioMixer.SetFloat(mixerName, vol);
            PlaySe(drumSe);
        }
    }

    void RetryMenu()
    {
        cursor.gameObject.SetActive(true);
        skillItem.SetActive(true);
        achievementItem.gameObject.SetActive(false);
        maxIndex = 3;
        headUi.text = "メニュー";
        textStr = () => "リトライ\nターゲットを壊せをやめる\nメニューをとじる";
        onSelecteds[0] = SetButtonPush(Retry);
        onSelecteds[1] = SetButtonPush(ExitTargetMode);
        onSelecteds[2] = SetButtonPush(CloseWindow);
        onCancel = CloseWindow;
    }

    void ExitTargetMode()
    {
        FadeManager.FadeIn(1.0f, "ZakkyScene");
        CloseWindow();
    }

    void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        CloseWindow();

    }

    bool isTargetScene()
    {
        return SceneManager.GetActiveScene().name.IndexOf("Target") >= 0;
    }

    OnSelected SetFullScreen()
    {
        return () =>
        {
            if (ActionInput.GetButtonDown(ButtonCode.Right) || ActionInput.GetButtonDown(ButtonCode.Left))
            {
                Screen.fullScreen = !Screen.fullScreen;
                PlaySe(drumSe);
            }
        };
    }

    string ScreenIsFull()
    {
        if (Screen.fullScreen)
        {
            return "ON";
        }
        else return "OFF";
    }

    OnSelected SetScreenSize()
    {
        return () =>
        {
            if (ActionInput.GetButtonDown(ButtonCode.Right))
            {
                ScreenSizeNum++;
                ScreenSizeNum = (ScreenSizeNum + 4) % 4;
                PlaySe(drumSe);
                ScreenSizeChange();
            }
            if (ActionInput.GetButtonDown(ButtonCode.Left))
            {
                ScreenSizeNum--;
                ScreenSizeNum = (ScreenSizeNum + 4) % 4;
                PlaySe(drumSe);
                ScreenSizeChange();
            }
        };
    }

    OnSelected SetPostEffect()
    {
        return () =>
        {
            if (ActionInput.GetButtonDown(ButtonCode.Right) || ActionInput.GetButtonDown(ButtonCode.Left))
            {
                isPostEffect = !isPostEffect;
                PlaySe(drumSe);
                SetPost();
            }
        };
    }

    string ScreenSizeString()
    {
        switch (ScreenSizeNum)
        {
            case 0:
                return "720 × 405";
            case 1:
                return "1280 × 720";
            case 2:
                return "1440 × 810";
            case 3:
                return "1920 × 1080";
        }
        return "";
    }

    string PostEffectString()
    {
        if (isPostEffect) return "ON";
        else return "OFF";
    }

    void ScreenSizeChange()
    {
        switch (ScreenSizeNum)
        {
            case 0:
                Screen.SetResolution(720, 405, Screen.fullScreen);
                break;
            case 1:
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                break;
            case 2:
                Screen.SetResolution(1440, 810, Screen.fullScreen);
                break;
            case 3:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
        }
    }

    OnSelected SetDefault()
    {
        return SetButtonPush(() =>
        {
            masterVol = bgmVol = seVol = -10;
            audioMixer.SetFloat("MasterVol", masterVol);
            audioMixer.SetFloat("BGMVol", bgmVol);
            audioMixer.SetFloat("SEVol", seVol);
        });
    }

    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
    UnityEngine.Application.Quit();
#endif
    }

    void ReturnTitle()
    {
        FadeManager.FadeIn(0.5f, "ZakkyTitle", 1);
        CloseWindow();
    }

    enum OpenState
    {
        Closed,
        Opening,
        Opened,
        Closing
    }
}

public enum SceneState
{
    Title,
    Game,
    Target,
    None
}
