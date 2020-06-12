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
using Actor.Player;

// 設定のセーブデータ by tada
[Serializable]
public class SettingData : TadaLib.Save.BaseSaver<SettingData>
{
    public int vSyncCount = 1;
    public int bgmVol = 8;
    public int seVol = 8;
    public int masterVol = 8;
    public int screenSizeNum = 1;
    public bool isFullScreen = false;
    public bool postEffectEnable = true;

    private const string kFileName = "Setting";

    // ロードする ロードが正常に完了したら true それ以外は false
    public bool Load()
    {
        SettingData data = Load(kFileName);
        if (data == null) return false;
        vSyncCount = data.vSyncCount;
        bgmVol = data.bgmVol;
        seVol = data.seVol;
        masterVol = data.masterVol;
        screenSizeNum = data.screenSizeNum;
        isFullScreen = data.isFullScreen;
        postEffectEnable = data.postEffectEnable;
        return true;
    }

    // セーブ申請を送る
    public void RequestSave()
    {
        if (save_completed_)
        {
            save_completed_ = false;
            TadaLib.Save.SaveManager.Instance.RequestSave(() => { Save(kFileName); save_completed_ = true; });
        }
    }

    // セーブデータを削除する
    public void DeleteSaveData()
    {
        TadaLib.Save.SaveManager.Instance.DeleteData(kFileName);
        // 初期値に戻す 戻す必要ない？
        vSyncCount = 1;
        bgmVol = 8;
        seVol = 8;
        masterVol = 8;
        screenSizeNum = 1;
        isFullScreen = false;
        postEffectEnable = true;
    }
}

public class SettingManager : MonoBehaviour
{
    public TextMeshProUGUI titleUi;
    public TextMeshProUGUI headUi;
    public Image cursor;
    public Image window;
    public Image curtain;
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
    public float width;
    private int nowIndex = 0;
    private int maxIndex;
    private Vector3 cursorDefaultPos;
    public Vector2 defaultDeltaSize;
    public Vector2 targetDeltaSize;
    public Ease ease;
    public Color defaultColor;
    public Color targetColor;
    public Material blurMat;
    public float defaultBlur;
    public float targetBlur;

    delegate void OnPush();
    delegate void OnSelected();
    OnSelected[] onSelecteds;
    Func<string> textStr;
    string exitSceneStr;
    Action onCancel;

    //前のページを覚えておく
    Stack<Action> pageActStack = new Stack<Action>();
    Stack<int> pageIndexStack = new Stack<int>();

    //現在のメニューを覚えておく
    Action nowMenu;

    //チュートリアル
    [SerializeField]
    TutorialManager tutorial;
    int pageIndex;

    [SerializeField]
    private AudioClip decisionSe;
    [SerializeField]
    private AudioClip cancelSe;
    [SerializeField]
    private AudioClip drumSe;
    [SerializeField]
    private AudioMixer audioMixer;

    private AudioSource audioSource;

    static SettingManager Instance;

    // 設定データ by tada
    private SettingData data;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            data = new SettingData();
            data.Load();
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
        onSelecteds = new OnSelected[6];
        StartPlacement();
        nowIndex = 0;
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(window);
        window.rectTransform.sizeDelta = defaultDeltaSize;
        curtain.color = defaultColor;
        blurMat.SetFloat("_Blur", defaultBlur);
        window.gameObject.SetActive(false);
        item.SetActive(false);
        QualitySettings.vSyncCount = data.vSyncCount;

        SetScreenSize();
        Screen.fullScreen = data.isFullScreen;

        //音量
        audioMixer.SetFloat("MasterVol", VolToDb(data.masterVol / 10f));
        audioMixer.SetFloat("BGMVol", VolToDb(data.bgmVol / 10f));
        audioMixer.SetFloat("SEVol", VolToDb(data.seVol / 10f));

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

                /*
                if (Input.GetKeyDown(KeyCode.R))
                {
                    RequestOpenTutorial(0);
                }
                if (Input.GetKeyDown(KeyCode.T))
                {
                    RequestOpenTutorial(1);
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

                //キャンセル
                if (ActionInput.GetButtonDown(ActionCode.Back))
                {
                    if (pageActStack.Count > 0)
                    {
                        nowMenu = pageActStack.Pop();
                        nowMenu();
                        nowIndex = pageIndexStack.Pop();
                    }
                    else
                    {
                        CloseWindow();
                    }
                    if (onCancel != null)
                    {
                        onCancel();
                    }
                    onCancel = null;
                    PlaySe(cancelSe);
                }

                /*
                if (onCancel != null && ActionInput.GetButtonDown(ActionCode.Back))
                {
                    onCancel();
                    PlaySe(cancelSe);
                }
                */

                //カーソルの更新
                cursor.transform.localPosition = cursorDefaultPos + Vector3.down * width * nowIndex;

                if (ActionInput.GetButtonDown(ActionCode.Pause))
                {
                    CloseWindow();
                    if (onCancel != null)
                    {
                        onCancel();
                    }
                    onCancel = null;
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
            ppl.enabled = data.postEffectEnable;
        }
    }

    void SetPost()
    {
        PostProcessLayer ppl = Camera.main.GetComponent<PostProcessLayer>();
        //Debug.Log(ppl);
        if (ppl != null)
        {
            ppl.enabled = data.postEffectEnable;
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
                //初期化
                Instance.pageActStack.Clear();
                Instance.pageIndexStack.Clear();

                //現在のシーン名で戻るシーンを変える
                if (isTargetScene("Target"))
                {
                    exitSceneStr = "TargetMediator";
                    RetryMenu();
                    nowMenu = RetryMenu;
                }
                else if (isTargetScene("WellDefence"))
                {
                    exitSceneStr = "WellDefenceMediator";
                    RetryMenu();
                    nowMenu = RetryMenu;
                }
                else if (isTargetScene("Boss"))
                {
                    exitSceneStr = "ZakkyScene";
                    RetryMenu();
                    nowMenu = RetryMenu;
                }
                else
                {
                    StartPlacement();
                    nowMenu = StartPlacement;
                }

                nowIndex = 0;
                //テキストの更新
                titleUi.text = textStr();
                //カーソルの更新
                cursor.transform.localPosition = cursorDefaultPos + Vector3.down * width * nowIndex;

            })
            .Append(window.rectTransform.DOSizeDelta(targetDeltaSize, 0.25f)).SetEase(ease).SetUpdate(true)
            .Join(curtain.DOColor(targetColor, 0.25f)).SetEase(ease).SetUpdate(true)
            .Join(blurMat.DOFloat(targetBlur,"_Blur",0.25f)).SetEase(ease).SetUpdate(true)
            .AppendCallback(() =>
            {
                item.SetActive(true);
                openState = OpenState.Opened;
            });
    }

    void OpenTutorial(int index)
    {
        //tutorial.pages[index].isOpen = true;
        Sequence seq = DOTween.Sequence()
            .OnStart(() =>
            {
                openState = OpenState.Opening;
                window.gameObject.SetActive(true);
                TadaLib.TimeScaler.Instance.RequestChange(0.0f);
                //初期化
                Instance.pageActStack.Clear();
                Instance.pageIndexStack.Clear();

                pageIndex = index;
                Instance.TutorialPageOpenFirst();

                //テキストの更新
                titleUi.text = textStr();

            })
            .Append(window.rectTransform.DOSizeDelta(targetDeltaSize, 0.25f)).SetEase(ease).SetUpdate(true)
            .Join(curtain.DOColor(targetColor, 0.25f)).SetEase(ease).SetUpdate(true)
            .Join(blurMat.DOFloat(targetBlur, "_Blur", 0.25f)).SetEase(ease).SetUpdate(true)
            .AppendCallback(() =>
            {
                item.SetActive(true);
                openState = OpenState.Opened;
            });
    }

    public static void RequestOpenWindow()
    {
        Instance.OpenWindow();
    }

    public static void RequestOpenTutorial(int index)
    {
        if (!Instance.tutorial.pages[index].isOpen)
        {
            Instance.OpenTutorial(index);
        }
    }

    public static bool WindowOpened()
    {
        return Instance.openState != OpenState.Closed;
    }

    void CloseWindow()
    {
        Sequence seq = DOTween.Sequence()
            .OnStart(() =>
            {
                // セーブ申請を送る by tada
                data.RequestSave();
                nowIndex = 0;
                item.SetActive(false);
                openState = OpenState.Closing;
            })
            .Append(window.rectTransform.DOSizeDelta(defaultDeltaSize, 0.25f)).SetEase(Ease.InOutSine).SetUpdate(true)
            .Join(curtain.DOColor(defaultColor, 0.25f)).SetEase(Ease.InOutSine).SetUpdate(true)
            .Join(blurMat.DOFloat(defaultBlur, "_Blur", 0.25f)).SetEase(ease).SetUpdate(true)
            .AppendCallback(() =>
            {
                openState = OpenState.Closed;
                window.gameObject.SetActive(false);
                TadaLib.TimeScaler.Instance.DismissRequest(0.0f);
                // セーブする
                TadaLib.Save.SaveManager.Instance.Save();
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
                pageActStack.Push(new Action(nowMenu));
                pageIndexStack.Push(nowIndex);
                nowMenu = onPush;
                onPush();
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
        textStr = () => "チュートリアル\nオプション\nじっせき\nメニューをとじる";
        onSelecteds[0] = SetButtonPush(TutorialTop);
        onSelecteds[1] = SetButtonPush(Option);
        onSelecteds[2] = SetButtonPush(AchievementScreen);
        onSelecteds[3] = SetButtonPush(CloseWindow);
    }

    void TutorialTop()
    {
        tutorial.OpenedPageUpdate();
        maxIndex = tutorial.openedList.Count;
        nowIndex = 0;
        headUi.text = "チュートリアル";
        string str = "";
        if (tutorial.openedList.Count > 0)
        {
            for (int i = 0; i < tutorial.openedList.Count; i++)
            {
                str += tutorial.openedList[i].pageTitle + "\n";
                onSelecteds[i] = SetButtonPush(TutorialPageOpen);
            }
            cursor.gameObject.SetActive(true);
        }
        else
        {
            str = "まだありません…";
            cursor.gameObject.SetActive(false);
        }
        textStr = () => str;
    }

    void TutorialPageOpen()
    {
        pageIndex = nowIndex;
        maxIndex = 1;
        nowIndex = 0;
        headUi.text = tutorial.openedList[pageIndex].pageTitle;
        textStr = () => "";
        tutorial.PageOpen(pageIndex);
        onSelecteds[0] = TutorialPage();
        onCancel = tutorial.PageClose;
        cursor.gameObject.SetActive(false);
    }

    void TutorialPageOpenFirst()
    {
        //フラグを建てる
        tutorial.pages[pageIndex].isOpen = true;
        maxIndex = 0;
        nowIndex = 0;
        headUi.text = tutorial.pages[pageIndex].pageTitle;
        textStr = () => "";
        tutorial.PageOpenFirst(pageIndex);
        onCancel = tutorial.PageClose;
        cursor.gameObject.SetActive(false);
    }

    OnSelected TutorialPage()
    {
        return () =>
        {
            if (ActionInput.GetButtonDown(ButtonCode.Right))
            {
                if (pageIndex < tutorial.openedList.Count - 1)
                {
                    pageIndex++;
                    tutorial.PageNext();
                    PlaySe(drumSe);
                    headUi.text = tutorial.openedList[pageIndex].pageTitle;
                    pageIndexStack.Pop();
                    pageIndexStack.Push(pageIndex);
                }
                else
                {
                    PlaySe(cancelSe);
                }
            }
            if (ActionInput.GetButtonDown(ButtonCode.Left))
            {
                if (pageIndex > 0)
                {
                    pageIndex--;
                    tutorial.PagePrev();
                    PlaySe(drumSe);
                    headUi.text = tutorial.openedList[pageIndex].pageTitle;
                    pageIndexStack.Pop();
                    pageIndexStack.Push(pageIndex);
                }
                else
                {
                    PlaySe(cancelSe);
                }
            }
        };
    }

    void Manual()
    {
        maxIndex = 0;
        nowIndex = 0;
        headUi.text = "そうさほうほう";
        textStr = () => ActionInput.GetSpriteCode(ActionCode.Jump) + "ジャンプ\n" + ActionInput.GetSpriteCode(ActionCode.Shot) + "ショット\n" + ActionInput.GetSpriteCode(ActionCode.Dash) + "ダッシュ";
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
    }

    void VideoOption()
    {
        cursor.gameObject.SetActive(true);
        maxIndex = 4;
        nowIndex = 0;
        headUi.text = "がめんせってい";
        textStr = () => "フルスクリーン\u3000< " + ScreenIsFull() + " >\nかいぞうど < " + ScreenSizeString() + " >\nポストエフェクト < " + PostEffectString() + " >\n垂直同期 < " + VsyncString() + " >";
        onSelecteds[0] = SetFullScreen();
        onSelecteds[1] = SetScreenSize();
        onSelecteds[2] = SetPostEffect();
        onSelecteds[3] = SetVsync();
    }

    void BgmOption()
    {
        cursor.gameObject.SetActive(true);
        maxIndex = 4;
        nowIndex = 0;
        /*
        audioMixer.GetFloat("MasterVol", out masterVol);
        audioMixer.GetFloat("BGMVol", out bgmVol);
        audioMixer.GetFloat("SEVol", out seVol);
        */
        headUi.text = "おんりょうせってい";
        textStr = () => "全体 < " + (data.masterVol) + " >\nBGM < " + (data.bgmVol) + " >\nこうかおん < " + (data.seVol) + " >\n元にもどす";
        onSelecteds[0] = () => SetVol("MasterVol", ref data.masterVol);
        onSelecteds[1] = () => SetVol("BGMVol", ref data.bgmVol);
        onSelecteds[2] = () => SetVol("SEVol", ref data.seVol);
        onSelecteds[3] = SetDefaultVol();
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


    void SetVol(string mixerName, ref int vol)
    {
        if (ActionInput.GetButtonDown(ButtonCode.Right))
        {
            vol += 1;
            vol = Mathf.Min(vol, 10);
            audioMixer.SetFloat(mixerName, VolToDb(vol / 10f));
            PlaySe(drumSe);
        }
        if (ActionInput.GetButtonDown(ButtonCode.Left))
        {
            vol -= 1;
            vol = Math.Max(vol, 0);
            float db = (vol + 80f) / 80f;
            audioMixer.SetFloat(mixerName, VolToDb(vol / 10f));
            PlaySe(drumSe);
        }
    }

    void RetryMenu()
    {
        cursor.gameObject.SetActive(true);
        //skillItem.SetActive(true);
        achievementItem.gameObject.SetActive(false);
        maxIndex = 6;
        headUi.text = "メニュー";
        textStr = () => "リトライ\nあきらめる\nそうさほうほう\nオプション\nじっせき\nメニューをとじる";
        onSelecteds[0] = SetButtonPush(Retry);
        onSelecteds[1] = SetButtonPush(ExitScene);
        onSelecteds[2] = SetButtonPush(Manual);
        onSelecteds[3] = SetButtonPush(Option);
        onSelecteds[4] = SetButtonPush(AchievementScreen);
        onSelecteds[5] = SetButtonPush(CloseWindow);
    }


    void ExitScene()
    {
        FadeManager.FadeIn(0.5f, exitSceneStr, 1);
        CloseWindow();
    }

    void Retry()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        FadeManager.FadeIn(0.3f, SceneManager.GetActiveScene().name, 2);
        CloseWindow();

    }

    bool isTargetScene(string str)
    {
        return SceneManager.GetActiveScene().name.IndexOf(str) >= 0;
    }

    OnSelected SetFullScreen()
    {
        return () =>
        {
            if (ActionInput.GetButtonDown(ButtonCode.Right) || ActionInput.GetButtonDown(ButtonCode.Left))
            {
                data.isFullScreen = !data.isFullScreen;
                Screen.fullScreen = data.isFullScreen;
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
                data.screenSizeNum++;
                data.screenSizeNum = (data.screenSizeNum + 4) % 4;
                PlaySe(drumSe);
                ScreenSizeChange();
            }
            if (ActionInput.GetButtonDown(ButtonCode.Left))
            {
                data.screenSizeNum--;
                data.screenSizeNum = (data.screenSizeNum + 4) % 4;
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
                data.postEffectEnable = !data.postEffectEnable;
                PlaySe(drumSe);
                SetPost();
            }
        };
    }

    string ScreenSizeString()
    {
        switch (data.screenSizeNum)
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
        if (data.postEffectEnable) return "ON";
        else return "OFF";
    }

    void ScreenSizeChange()
    {
        switch (data.screenSizeNum)
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

    OnSelected SetVsync()
    {
        return () =>
        {
            if (ActionInput.GetButtonDown(ButtonCode.Right))
            {
                data.vSyncCount++;
                data.vSyncCount = (data.vSyncCount + 4) % 4;
                QualitySettings.vSyncCount = data.vSyncCount;
                PlaySe(drumSe);
                ScreenSizeChange();
            }
            if (ActionInput.GetButtonDown(ButtonCode.Left))
            {
                data.vSyncCount--;
                data.vSyncCount = (data.vSyncCount + 4) % 4;
                QualitySettings.vSyncCount = data.vSyncCount;
                PlaySe(drumSe);
                ScreenSizeChange();
            }
        };
    }

    string VsyncString()
    {
        switch (data.vSyncCount)
        {
            case 0:
                return "OFF";
            case 1:
                return "1回ごと";
            case 2:
                return "2回ごと";
            case 3:
                return "3回ごと";
        }
        return "";
    }

    OnSelected SetDefaultVol()
    {
        return SetButtonPush(() =>
        {
            data.masterVol = data.bgmVol = data.seVol = 8;
            audioMixer.SetFloat("MasterVol", VolToDb(data.masterVol / 10f));
            audioMixer.SetFloat("BGMVol", VolToDb(data.bgmVol / 10f));
            audioMixer.SetFloat("SEVol", VolToDb(data.seVol / 10f));
        });
    }

    //デシベルへの変換は平方根でごまかす
    //[0,1]→[-80,0]
    float VolToDb(float vol)
    {
        return Mathf.Sqrt(vol) * 80f - 80f;
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

    // セーブデータを削除する
    public static void DeleteSaveData()
    {
        Instance.data.DeleteSaveData();
        // 設定データをもとに戻す
        Instance.InitSetting();
    }

    public void InitSetting()
    {
        QualitySettings.vSyncCount = data.vSyncCount;

        SetScreenSize();
        Screen.fullScreen = data.isFullScreen;

        //音量
        audioMixer.SetFloat("MasterVol", VolToDb(data.masterVol / 10));
        audioMixer.SetFloat("BGMVol", VolToDb(data.bgmVol / 10));
        audioMixer.SetFloat("SEVol", VolToDb(data.seVol / 10));
    }

    enum OpenState
    {
        Closed,
        Opening,
        Opened,
        Closing
    }
}
