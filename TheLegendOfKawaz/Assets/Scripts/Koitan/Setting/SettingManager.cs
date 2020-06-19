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

    // チュートリアル
    public List<int> unlockedIndices = new List<int>();

    private const string kFileName = "Setting";

    // セーブ前のデータ
    private int prev_vSyncCount = 1;
    private int prev_bgmVol = 8;
    private int prev_seVol = 8;
    private int prev_masterVol = 8;
    private int prev_screenSizeNum = 1;
    private bool prev_isFullScreen = false;
    private bool prev_postEffectEnable = true;
    private int prev_unlocked_cnt = 0;

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
        unlockedIndices = data.unlockedIndices;
        prev_unlocked_cnt = unlockedIndices.Count;
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

        // めんどくさい
        prev_vSyncCount = vSyncCount;
        prev_bgmVol = bgmVol;
        prev_seVol = seVol;
        prev_masterVol = masterVol;
        prev_screenSizeNum = screenSizeNum;
        prev_isFullScreen = isFullScreen;
        prev_postEffectEnable = postEffectEnable;
        prev_unlocked_cnt = unlockedIndices.Count;
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
        unlockedIndices.Clear();
        prev_unlocked_cnt = 0;
    }

    // データが変更されたか取得する
    public bool DataChanged()
    {
        if (prev_vSyncCount != vSyncCount) return true;
        if (prev_bgmVol != bgmVol) return true;
        if (prev_seVol != seVol) return true;
        if (prev_masterVol != masterVol) return true;
        if (prev_screenSizeNum != screenSizeNum) return true;
        if (prev_isFullScreen != isFullScreen) return true;
        if (prev_postEffectEnable != postEffectEnable) return true;
        if (prev_unlocked_cnt != unlockedIndices.Count) return true;
        return false;
    }
}

public class SettingManager : MonoBehaviour
{
    public TextMeshProUGUI[] bodyTextMesh;
    public TextMeshProUGUI titleTextMesh;
    public TMProAnimator[] bodyAnimator;
    [SerializeField]
    private TextMeshProUGUI detailTextMesh;
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
    private int maxIndex = 0;
    private Vector3 cursorDefaultPos;
    public Vector2 defaultDeltaSize;
    public Vector2 targetDeltaSize;
    public Ease ease;
    public Color defaultColor;
    public Color targetColor;
    public Material blurMat;
    public float defaultBlur;
    public float targetBlur;
    public float bodyTextShiftLocalX = -10f;

    delegate void OnPush();
    delegate void OnSelected();
    OnSelected[] onSelecteds;
    Func<string>[] textStr = new Func<string>[6];
    string exitSceneStr;
    Action onCancel;

    //前のページを覚えておく
    Stack<Action> pageActStack = new Stack<Action>();
    Stack<int> pageIndexStack = new Stack<int>();

    //現在のメニューを覚えておく
    Action nowMenu;

    //チュートリアル
    int pageIndex;
    [SerializeField]
    private TutorialBook[] books;
    private List<TutorialBook> unlockedBooks = new List<TutorialBook>();
    private GameObject opendPageObj;
    private TutorialBook nowBook;
    [SerializeField]
    private Image rightIm;
    [SerializeField]
    private Image leftIm;

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
        bodyAnimator = new TMProAnimator[bodyTextMesh.Length];
        for (int i = 0; i < bodyAnimator.Length; i++)
        {
            bodyAnimator[i] = bodyTextMesh[i].GetComponent<TMProAnimator>();
        }
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

        // 既に登録されたチュートリアルを入れる by tada
        foreach (var index in data.unlockedIndices)
        {
            unlockedBooks.Add(books[index]);
            books[index].isUnlocked = true;
        }
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


                if (Input.GetKeyDown(KeyCode.R))
                {
                    //RequestOpenTutorial(0);
                    //RequestOpenTutorial(0);
                }
                if (Input.GetKeyDown(KeyCode.T))
                {
                    //RequestOpenTutorial(1);
                    //RequestOpenTutorial(1);
                }


                break;
            case OpenState.Opened:
                //項目が2個以上ないと動かせない
                if (maxIndex >= 2)
                {
                    if (ActionInput.GetButtonDown(ButtonCode.Up))
                    {
                        bodyAnimator[nowIndex].StopAnimation();
                        nowIndex--;
                        nowIndex = (nowIndex + maxIndex) % maxIndex;
                        PlaySe(drumSe);
                        bodyAnimator[nowIndex].StartAnimation();
                    }

                    if (ActionInput.GetButtonDown(ButtonCode.Down))
                    {
                        bodyAnimator[nowIndex].StopAnimation();
                        nowIndex++;
                        nowIndex %= maxIndex;
                        PlaySe(drumSe);
                        bodyAnimator[nowIndex].StartAnimation();
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
                UpdateBodyText();

                //キャンセル
                if (ActionInput.GetButtonDown(ActionCode.Back))
                {
                    if (pageActStack.Count > 0)
                    {
                        bodyAnimator[nowIndex].StopAnimation();
                        nowMenu = pageActStack.Pop();
                        nowMenu();                        
                        nowIndex = pageIndexStack.Pop();                        
                    }
                    else
                    {
                        bodyAnimator[nowIndex].StopAnimation();
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
                        bodyAnimator[nowIndex].StopAnimation();
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
                UpdateBodyText();
                //カーソルの更新
                cursor.transform.localPosition = cursorDefaultPos + Vector3.down * width * nowIndex;

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

                OpenTutorialBookFirst(index);


                //テキストの更新
                UpdateBodyText();

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
        if (!Instance.books[index].isUnlocked)
        {
            Instance.books[index].isUnlocked = true;
            Instance.OpenTutorial(index);
        }
    }

    public static bool WindowOpened()
    {
        return Instance.openState != OpenState.Closed;
    }

    void CloseWindow()
    {
        bool changed = data.DataChanged();

        Sequence seq = DOTween.Sequence()
            .OnStart(() =>
            {
                // セーブ申請を送る by tada
                if (changed) data.RequestSave();
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
                if (changed) TadaLib.Save.SaveManager.Instance.Save();
            });
    }

    //audioSource.PlayOneShotが不便すぎる
    void PlaySe(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.time = 0;
        audioSource.Play();
    }

    void AppearanceBodyText()
    {
        //タイトル
        titleTextMesh.DOKill();
        titleTextMesh.rectTransform.DOKill();
        var pos = titleTextMesh.rectTransform.localPosition;
        pos.x = -bodyTextShiftLocalX;
        titleTextMesh.rectTransform.localPosition = pos;
        titleTextMesh.color = new Color(1, 1, 1, 0);
        titleTextMesh.DOFade(1, 0.1f).SetUpdate(true);
        titleTextMesh.rectTransform.DOLocalMoveX(bodyTextShiftLocalX, 0.1f).SetEase(Ease.OutCubic).SetRelative().SetUpdate(true);


        //maxIndexが0の時だけ挙動が特殊
        for (int i = 0; i < bodyTextMesh.Length; i++)
        {
            if (i < maxIndex)
            {
                bodyTextMesh[i].gameObject.SetActive(true);
                bodyTextMesh[i].DOKill();
                bodyTextMesh[i].rectTransform.DOKill();
                pos = bodyTextMesh[i].rectTransform.localPosition;
                pos.x = -bodyTextShiftLocalX;
                bodyTextMesh[i].rectTransform.localPosition = pos;
                bodyTextMesh[i].color = new Color(1, 1, 1, 0);
                //bodyTextMesh[i].rectTransform.localScale = new Vector3(1, 1, 1);                
                bodyTextMesh[i].DOFade(1, 0.1f).SetDelay(0.05f * i).SetUpdate(true);
                bodyTextMesh[i].rectTransform.DOLocalMoveX(bodyTextShiftLocalX, 0.1f).SetEase(Ease.OutCubic).SetRelative().SetDelay(0.05f * i).SetUpdate(true);
            }
            else
            {
                bodyTextMesh[i].text = string.Empty;
                bodyTextMesh[i].gameObject.SetActive(false);
            }
        }
    }

    void UpdateBodyText()
    {
        //maxIndexが0の時だけ挙動が特殊
        if (maxIndex == 0)
        {
            bodyTextMesh[0].text = textStr[0]();
            return;
        }
        for (int i = 0; i < bodyTextMesh.Length; i++)
        {
            if (i < maxIndex)
            {
                bodyTextMesh[i].text = textStr[i]();
            }
            else
            {
                bodyTextMesh[i].text = string.Empty;
            }
        }
    }

    OnSelected SetButtonPush(Action onPush)
    {
        return () =>
        {
            if (ActionInput.GetButtonDown(ActionCode.Decide))
            {
                pageActStack.Push(new Action(nowMenu));
                pageIndexStack.Push(nowIndex);
                bodyAnimator[nowIndex].StopAnimation();
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
        titleTextMesh.text = "メニュー";
        textStr[0] = () => "チュートリアル";
        textStr[1] = () => "オプション";
        textStr[2] = () => "じっせき";
        textStr[3] = () => "メニューをとじる";
        onSelecteds[0] = SetButtonPush(TutorialTop);
        onSelecteds[1] = SetButtonPush(Option);
        onSelecteds[2] = SetButtonPush(AchievementScreen);
        onSelecteds[3] = SetButtonPush(CloseWindow);
        AppearanceBodyText();
    }

    void TutorialTop()
    {
        maxIndex = unlockedBooks.Count;
        nowIndex = 0;
        titleTextMesh.text = "チュートリアル";
        if (maxIndex > 0)
        {
            cursor.gameObject.SetActive(true);
            for (int i = 0; i < maxIndex; i++)
            {
                //デリゲートの仕様？
                int tmp = i;
                textStr[tmp] = () => unlockedBooks[tmp].bookTitle;
                onSelecteds[tmp] = SetButtonPush(() => OpenTutorialBook(tmp));
            }
        }
        else
        {
            cursor.gameObject.SetActive(false);
            textStr[0] = () => "まだありません…";
        }
        AppearanceBodyText();
    }

    void OpenTutorialBook(int index)
    {
        cursor.gameObject.SetActive(false);
        maxIndex = 1;
        nowIndex = 0;
        onSelecteds[0] = SlideTutorialPage();
        nowBook = unlockedBooks[index];
        pageIndex = 0;
        opendPageObj = nowBook.pages[pageIndex];
        opendPageObj.SetActive(true);
        onCancel += () => opendPageObj.SetActive(false);
        onCancel += () => rightIm.gameObject.SetActive(false);
        onCancel += () => leftIm.gameObject.SetActive(false);
        titleTextMesh.text = nowBook.bookTitle + "(" + (pageIndex + 1) + "/" + nowBook.pages.Length + ")";
        textStr[0] = () => "";
    }

    void OpenTutorialBookFirst(int index)
    {
        nowBook = books[index];
        /*
        if (!nowBook.isUnlocked)
        {
            nowBook.isUnlocked = true;
            unlockedBooks.Add(nowBook);
        }
        */
        //RequestOpenTutorialで保証されている
        unlockedBooks.Add(nowBook);
        data.unlockedIndices.Add(index);
        cursor.gameObject.SetActive(false);
        maxIndex = 1;
        nowIndex = 0;
        onSelecteds[0] = SlideTutorialPage();
        pageIndex = 0;
        opendPageObj = nowBook.pages[pageIndex];
        opendPageObj.SetActive(true);
        onCancel += () => opendPageObj.SetActive(false);
        onCancel += () => rightIm.gameObject.SetActive(false);
        onCancel += () => leftIm.gameObject.SetActive(false);
        titleTextMesh.text = nowBook.bookTitle + "(" + (pageIndex + 1) + "/" + nowBook.pages.Length + ")";
        textStr[0] = () => "";
    }

    OnSelected SlideTutorialPage()
    {
        return () =>
        {
            if (ActionInput.GetButtonDown(ButtonCode.Right) || ActionInput.GetButtonDown(ActionCode.Decide))
            {
                if (pageIndex < nowBook.pages.Length - 1)
                {
                    opendPageObj.SetActive(false);
                    pageIndex++;
                    opendPageObj = nowBook.pages[pageIndex];
                    opendPageObj.SetActive(true);
                    titleTextMesh.text = nowBook.bookTitle + "(" + (pageIndex + 1) + "/" + nowBook.pages.Length + ")";
                    PlaySe(drumSe);
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
                    opendPageObj.SetActive(false);
                    pageIndex--;
                    opendPageObj = nowBook.pages[pageIndex];
                    opendPageObj.SetActive(true);
                    titleTextMesh.text = nowBook.bookTitle + "(" + (pageIndex + 1) + "/" + nowBook.pages.Length + ")";
                    PlaySe(drumSe);
                }
                else
                {
                    PlaySe(cancelSe);
                }
            }
            if (pageIndex > 0)
            {
                leftIm.gameObject.SetActive(true);
            }
            else
            {
                leftIm.gameObject.SetActive(false);
            }
            if (pageIndex < nowBook.pages.Length - 1)
            {
                rightIm.gameObject.SetActive(true);
            }
            else
            {
                rightIm.gameObject.SetActive(false);
            }
        };
    }

    /*
    void Manual()
    {
        maxIndex = 0;
        nowIndex = 0;
        titleTextMesh.text = "そうさほうほう";
        textStr = () => ActionInput.GetSpriteCode(ActionCode.Jump) + "ジャンプ\n" + ActionInput.GetSpriteCode(ActionCode.Shot) + "ショット\n" + ActionInput.GetSpriteCode(ActionCode.Dash) + "ダッシュ";
        cursor.gameObject.SetActive(false);
    }
    */

    void Option()
    {
        cursor.gameObject.SetActive(true);
        titleTextMesh.text = "オプション";
        maxIndex = 3;
        nowIndex = 0;
        textStr[0] = () => "がめんせってい";
        textStr[1] = () => "おんりょうせってい";
        textStr[2] = () => "タイトルにもどる";
        onSelecteds[0] = SetButtonPush(VideoOption);
        onSelecteds[1] = SetButtonPush(BgmOption);
        onSelecteds[2] = SetButtonPush(ReturnTitle);
        AppearanceBodyText();
    }

    void VideoOption()
    {
        cursor.gameObject.SetActive(true);
        maxIndex = 4;
        nowIndex = 0;
        titleTextMesh.text = "がめんせってい";
        textStr[0] = () => "フルスクリーン\u3000< " + ScreenIsFull() + " >";
        textStr[1] = () => "かいぞうど < " + ScreenSizeString() + " >";
        textStr[2] = () => "ポストエフェクト < " + PostEffectString() + " >";
        textStr[3] = () => "垂直同期 < " + VsyncString() + " >";
        onSelecteds[0] = SetFullScreen();
        onSelecteds[1] = SetScreenSize();
        onSelecteds[2] = SetPostEffect();
        onSelecteds[3] = SetVsync();
        AppearanceBodyText();
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
        titleTextMesh.text = "おんりょうせってい";
        textStr[0] = () => "全体 < " + (data.masterVol) + " >";
        textStr[1] = () => "BGM < " + (data.bgmVol) + " >";
        textStr[2] = () => "こうかおん < " + (data.seVol) + " >";
        textStr[3] = () => "元にもどす";
        onSelecteds[0] = () => SetVol("MasterVol", ref data.masterVol);
        onSelecteds[1] = () => SetVol("BGMVol", ref data.bgmVol);
        onSelecteds[2] = () => SetVol("SEVol", ref data.seVol);
        onSelecteds[3] = SetDefaultVol();
        AppearanceBodyText();
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
        titleTextMesh.text = "じっせき";
        textStr[0] = () => "";
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
        maxIndex = 4;
        titleTextMesh.text = "メニュー";
        textStr[0] = () => "リトライ";
        textStr[1] = () => "あきらめる";
        //textStr[2] = () => "チュートリアル";
        textStr[2] = () => "オプション";
        //textStr[4] = () => "じっせき";
        textStr[3] = () => "メニューをとじる";
        onSelecteds[0] = SetButtonPush(Retry);
        onSelecteds[1] = SetButtonPush(ExitScene);
        //onSelecteds[2] = SetButtonPush(TutorialTop);
        onSelecteds[2] = SetButtonPush(Option);
        //onSelecteds[4] = SetButtonPush(AchievementScreen);
        onSelecteds[3] = SetButtonPush(CloseWindow);
        AppearanceBodyText();
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
        return (() =>
        {
            if (ActionInput.GetButtonDown(ActionCode.Decide))
            {
                data.masterVol = data.bgmVol = data.seVol = 8;
                audioMixer.SetFloat("MasterVol", VolToDb(data.masterVol / 10f));
                audioMixer.SetFloat("BGMVol", VolToDb(data.bgmVol / 10f));
                audioMixer.SetFloat("SEVol", VolToDb(data.seVol / 10f));
                PlaySe(decisionSe);
            }
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

    //チュートリアル要素用クラス
    [Serializable]
    public class TutorialBook
    {
        public string bookTitle;
        public GameObject[] pages;
        public bool isUnlocked = false;
    }
}
