using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;

public class AchievementManager : MonoBehaviour
{
    //シングルトン
    public static AchievementManager Instance;
    [SerializeField]
    private Image windowIm;
    [SerializeField]
    private Image iconIm;
    [SerializeField]
    private TextMeshProUGUI textMesh;
    public static Queue<string> que = new Queue<string>();
    private OpenState eState = OpenState.Closed;
    private Sequence seq;
    [SerializeField]
    private AchievementContent[] contents;
    [SerializeField]
    private AchievementUi achievementPrefab;
    [SerializeField]
    private Transform achievementUiParent;
    private List<AchievementUi> achievementUis = new List<AchievementUi>();
    private Dictionary<string, AchievementContent> achieveDict = new Dictionary<string, AchievementContent>();

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            seq.SetUpdate(true);
            //Dictionaryに追加
            for (int i = 0; i < contents.Length; i++)
            {
                AchievementContent c = contents[i];
                achieveDict.Add(c.key, c);
                AchievementUi au = Instantiate(achievementPrefab, achievementUiParent);
                au.transform.localPosition = new Vector3(0, 350 - 200 * i, 0);
                au.content = c;
                achievementUis.Add(au);                
            }
        }
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (eState)
        {
            case OpenState.Closed:
                if (que.Count > 0)
                {
                    seq = DOTween.Sequence()
                        .OnStart(() =>
                        {
                            eState = OpenState.Opening;
                            windowIm.gameObject.SetActive(true);
                            string key = que.Dequeue();
                            AchievementContent c = achieveDict[key];
                            iconIm.sprite = c.icon;
                            textMesh.text = c.body;
                        })
                        .Append(windowIm.transform.DOLocalMoveX(800, 0.3f).SetEase(Ease.OutCubic).SetRelative().SetUpdate(true))
                        .AppendCallback(() =>
                        {
                            eState = OpenState.Opened;
                        })
                        .AppendInterval(3f).SetUpdate(true)
                        .AppendCallback(() =>
                        {
                            eState = OpenState.Closing;
                        })
                        .Append(windowIm.transform.DOLocalMoveX(-800, 0.3f).SetEase(Ease.InCubic).SetRelative().SetUpdate(true))
                        .OnComplete(() =>
                        {
                            eState = OpenState.Closed;
                            windowIm.gameObject.SetActive(false);
                        });
                }
                break;
            default:
                break;
        }

        //デバッグ用
        /*
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AchievementManager.FireAchievement("GameClear");
            AchievementManager.FireAchievement("Kagawa");
        }
        */
    }

    public static void FireAchievement(string key)
    {
        //すでに解除済みの場合発火しない
        AchievementContent c = Instance.achieveDict[key];
        if (!c.isUnlocked)
        {
            c.isUnlocked = true;
            que.Enqueue(key);
        }
    }

    public static void UpdateUis()
    {
        foreach(AchievementUi ui in Instance.achievementUis)
        {
            ui.UpdateUi();
        }
    }

    public static float GetScrollMaxY()
    {
        return Mathf.Max(-450 - 850 + Instance.contents.Length * 200, -450);
    }

    public static int GetAchieveMax()
    {
        return Instance.contents.Length;
    }

    public static int GetAchieveNowUnlockedNum()
    {
        return Instance.contents.Count(c => c.isUnlocked);
    }

    enum OpenState
    {
        Closed,
        Opening,
        Opened,
        Closing
    }
}

[System.Serializable]
public class AchievementContent
{
    public string key;
    public Sprite icon;
    public string body;
    public string detail;
    public bool isUnlocked;
}
