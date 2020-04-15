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

public class SettingManager : MonoBehaviour
{
    public TextMeshProUGUI titleUi;
    public TextMeshProUGUI headUi;
    public Image cursor;
    public Image window;
    public GameObject item;
    private OpenState eState = OpenState.Closed;
    public float width;
    private int nowIndex = 0;
    private int maxIndex;
    private int addIndex = 0;
    private Vector3 cursorDefaultPos;
    public Vector2 targetDeltaSize;
    public Ease ease;

    delegate void OnPush();
    delegate void OnSelected();
    OnSelected[] onSelecteds;
    OnPush onCancel;

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
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        cursorDefaultPos = cursor.transform.localPosition;
        StartPlacement();
        nowIndex = 0;
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(window);
        //MusicManager.Play(MusicManager.Instance.bgm4);
    }

    // Update is called once per frame
    void Update()
    {
        switch (eState)
        {
            case OpenState.Closed:
                if (ActionInput.GetButtonDown(ActionCode.Pause))
                {
                    OpenWindow();
                }
                break;
            case OpenState.Opened:
                if (ActionInput.GetButtonDown(ButtonCode.Up))
                {
                    nowIndex--;
                    nowIndex = (nowIndex + maxIndex) % maxIndex;
                    audioSource.PlayOneShot(drumSe);
                }

                if (ActionInput.GetButtonDown(ButtonCode.Down))
                {
                    nowIndex++;
                    nowIndex %= maxIndex;
                    audioSource.PlayOneShot(drumSe);
                }

                if (onSelecteds[nowIndex] != null)
                {
                    onSelecteds[nowIndex]();
                }


                if (onCancel != null && ActionInput.GetButtonDown(ActionCode.Back))
                {
                    onCancel();
                    audioSource.PlayOneShot(cancelSe);
                }
                cursor.transform.localPosition = cursorDefaultPos + Vector3.down * width * (nowIndex + addIndex);

                if (ActionInput.GetButtonDown(ActionCode.Pause))
                {
                    CloseWindow();
                }
                break;
            default:
                break;
        }
    }

    void OpenWindow()
    {
        Sequence seq = DOTween.Sequence()
            .OnStart(() =>
            {
                eState = OpenState.Opening;
                Time.timeScale = 0.0f;
            })
            .Append(window.rectTransform.DOSizeDelta(targetDeltaSize, 0.5f)).SetEase(Ease.OutBounce).SetUpdate(true)
            .AppendCallback(() =>
            {
                StartPlacement();
                nowIndex = 0;
                item.SetActive(true);
                eState = OpenState.Opened;
            });        
    }

    void CloseWindow()
    {
        Sequence seq = DOTween.Sequence()
            .OnStart(()=>
            {
                nowIndex = 0;
                item.SetActive(false);
                eState = OpenState.Closing;
            })
            .Append(window.rectTransform.DOSizeDelta(Vector2.zero, 0.25f)).SetEase(Ease.InOutSine).SetUpdate(true)
            .AppendCallback(() =>
            {
                eState = OpenState.Closed;
                Time.timeScale = 1.0f;
            });
    }



    OnSelected SetButtonPush(Action onPush)
    {
        return () =>
        {
            if (ActionInput.GetButtonDown(ActionCode.Decide))
            {
                onPush();
                audioSource.PlayOneShot(decisionSe);
            }
        };
    }

    void StartPlacement()
    {
        cursor.gameObject.SetActive(true);
        maxIndex = 3;
        addIndex = 0;
        headUi.text = "メニュー";
        titleUi.text = "そうさほうほう\nオプション\nメニューをとじる";
        onSelecteds = new OnSelected[maxIndex];
        onSelecteds[0] = SetButtonPush(Manual);
        onSelecteds[1] = SetButtonPush(Option);
        onSelecteds[2] = SetButtonPush(CloseWindow);
        onCancel = CloseWindow;        
    }

    void StartRocketScene()
    {
        /*
        if (!FadeManager.Instance.isFading)
        {
            FadeManager.Instance.LoadScene("RocketStage", 1f);
        }
        */
    }

    void Manual()
    {
        maxIndex = 1;
        nowIndex = 0;
        headUi.text = "そうさほうほう";
        titleUi.text = ActionInput.GetSpriteCode(ActionCode.Jump) + "ジャンプ\n" + ActionInput.GetSpriteCode(ActionCode.Shot) + "ショット\n" + ActionInput.GetSpriteCode(ActionCode.Dash) + "ダッシュ";
        onSelecteds[0] = SetButtonPush(StartPlacement);
        onSelecteds[0] += SetButtonPush(() => nowIndex = 0);
        onCancel = StartPlacement;
        onCancel += () => nowIndex = 0;
        cursor.gameObject.SetActive(false);
    }

    void Option()
    {
        cursor.gameObject.SetActive(true);
        headUi.text = "オプション";
        titleUi.text = "がめんせってい\nおんりょうせってい\nタイトルにもどる";
        maxIndex = 3;
        nowIndex = 0;
        addIndex = 0;
        onSelecteds[0] = SetButtonPush(VideoOption);
        onSelecteds[1] = SetButtonPush(BgmOption);
        onSelecteds[2] = SetButtonPush(ReturnTitle);
        onCancel = StartPlacement;
        onCancel += () => nowIndex = 1;
    }

    void VideoOption()
    {
        cursor.gameObject.SetActive(true);
        maxIndex = 2;
        nowIndex = 0;
        addIndex = 0;
        headUi.text = "がめんせってい";
        titleUi.text = "フルスクリーン\u3000< " + ScreenIsFull() + " >\nかいぞうど < " + ScreenSizeString() + " >";
        onSelecteds[0] = SetFullScreen();
        onSelecteds[1] = SetScreenSize();
        onCancel = Option;
        onCancel += () => nowIndex = 0;
    }

    void BgmOption()
    {
        cursor.gameObject.SetActive(true);
        maxIndex = 4;
        nowIndex = 0;
        addIndex = 0;
        audioMixer.GetFloat("MasterVol", out masterVol);
        audioMixer.GetFloat("BGMVol", out bgmVol);
        audioMixer.GetFloat("SEVol", out seVol);
        headUi.text = "おんりょうせってい";
        titleUi.text = "<sprite=7>でへんこう\n全体 < " + (masterVol + 80) + " >\nBGM < " + (bgmVol + 80) + " >\nこうかおん < " + (seVol + 80) + " >\n元にもどす";
        onSelecteds[0] = SetMaster();
        onSelecteds[1] = SetBGM();
        onSelecteds[2] = SetSE();
        onSelecteds[3] = SetDefault();
        onCancel = Option;
        onCancel += () => nowIndex = 1;
    }

    OnSelected SetMaster()
    {
        return () =>
        {
            if (ActionInput.GetButton(ButtonCode.Right))
            {
                masterVol++;
                masterVol = Mathf.Min(masterVol, 20);
                audioMixer.SetFloat("MasterVol", masterVol);
            }
            if (ActionInput.GetButton(ButtonCode.Left))
            {
                masterVol--;
                masterVol = Math.Max(masterVol, -80);
                audioMixer.SetFloat("MasterVol", masterVol);
            }
            titleUi.text = "おんりょうせってい … <sprite=7>でへんこう\n全体 < " + (masterVol + 80) + " >\nBGM < " + (bgmVol + 80) + " >\nこうかおん < " + (seVol + 80) + " >\n元にもどす";
        };
    }

    OnSelected SetBGM()
    {
        return () =>
        {
            if (ActionInput.GetButton(ButtonCode.Right))
            {
                bgmVol++;
                bgmVol = Mathf.Min(bgmVol, 20);
                audioMixer.SetFloat("BGMVol", bgmVol);
            }
            if (ActionInput.GetButton(ButtonCode.Left))
            {
                bgmVol--;
                bgmVol = Math.Max(bgmVol, -80);
                audioMixer.SetFloat("BGMVol", bgmVol);
            }
            titleUi.text = "おんりょうせってい … <sprite=7>でへんこう\n全体 < " + (masterVol + 80) + " >\nBGM < " + (bgmVol + 80) + " >\nこうかおん < " + (seVol + 80) + " >\n元にもどす";
        };
    }

    OnSelected SetSE()
    {
        return () =>
        {
            if (ActionInput.GetButton(ButtonCode.Right))
            {
                seVol++;
                seVol = Mathf.Min(seVol, 20);
                audioMixer.SetFloat("SEVol", seVol);
            }
            if (ActionInput.GetButton(ButtonCode.Left))
            {
                seVol--;
                seVol = Math.Max(seVol, -80);
                audioMixer.SetFloat("SEVol", seVol);
            }
            titleUi.text = "おんりょうせってい … <sprite=7>でへんこう\n全体 < " + (masterVol + 80) + " >\nBGM < " + (bgmVol + 80) + " >\nこうかおん < " + (seVol + 80) + " >\n元にもどす";
        };
    }

    OnSelected SetFullScreen()
    {
        return () =>
        {
            if (ActionInput.GetButtonDown(ButtonCode.Right) || ActionInput.GetButtonDown(ButtonCode.Left))
            {
                Screen.fullScreen = !Screen.fullScreen;
                audioSource.PlayOneShot(drumSe);
            }
            titleUi.text = "フルスクリーン\u3000< " + ScreenIsFull() + " >\nかいぞうど < " + ScreenSizeString() + " >";
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
                audioSource.PlayOneShot(drumSe);
                ScreenSizeChange();
            }
            if (ActionInput.GetButtonDown(ButtonCode.Left))
            {
                ScreenSizeNum--;
                ScreenSizeNum = (ScreenSizeNum + 4) % 4;
                audioSource.PlayOneShot(drumSe);
                ScreenSizeChange();
            }
            titleUi.text = "フルスクリーン\u3000< " + ScreenIsFull() + " >\nかいぞうど < " + ScreenSizeString() + " >";
        };
    }

    string ScreenSizeString()
    {
        switch (ScreenSizeNum)
        {
            case 0:
                return "640 × 360";
            case 1:
                return "1280 × 720";
            case 2:
                return "1440 × 810";
            case 3:
                return "1920 × 1080";
        }
        return "";
    }

    void ScreenSizeChange()
    {
        switch (ScreenSizeNum)
        {
            case 0:
                Screen.SetResolution(640, 360, Screen.fullScreen);
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

    /*
    OnSelected SetVol(string n)
    {
        return () =>
        {
            out float vol = 0;
            switch (n)
            {
                case "MasterVol":
                    vol = masterVol;
                    break;
                case "BGMVol":
                    vol = bgmVol;
                    break;
                case "SEVol":
                    vol = seVol;
                    break;
            }
            if (ActionInput.GetButton(ButtonCode.RightArrow))
            {
                vol++;
                vol = Mathf.Min(vol, 20);
                audioMixer.SetFloat(n, vol);
            }
            if (ActionInput.GetButton(ButtonCode.LeftArrow))
            {
                vol--;
                vol = Math.Max(vol, -80);
                audioMixer.SetFloat(n, vol);
            }
            titleUi.text = "おんりょうせってい … <sprite=7>でへんこう\n全体 < " + (masterVol + 80) + " >\nBGM < " + (bgmVol + 80) + " >\nこうかおん < " + (seVol + 80) + " >\n元にもどす";
        };
    }
    */

    OnSelected SetDefault()
    {
        return SetButtonPush(() =>
        {
            masterVol = bgmVol = seVol = 0;
            audioMixer.SetFloat("MasterVol", masterVol);
            audioMixer.SetFloat("BGMVol", bgmVol);
            audioMixer.SetFloat("SEVol", seVol);
            titleUi.text = "おんりょうせってい … <sprite=7>でへんこう\n全体 < " + (masterVol + 80) + " >\nBGM < " + (bgmVol + 80) + " >\nこうかおん < " + (seVol + 80) + " >\n元にもどす";
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
