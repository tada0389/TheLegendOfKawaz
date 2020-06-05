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

    private AchievementData achieve_data_; // by tada
    //private Dictionary<string, AchievementContent> achieveDict = new Dictionary<string, AchievementContent>();

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            seq.SetUpdate(true);
            // 実績情報をロード by tada
            achieve_data_ = new AchievementData();
            //Dictionaryに追加
            for (int i = 0; i < contents.Length; i++)
            {
                AchievementContent c = contents[i];
                achieve_data_.Add(c.key, c);
                AchievementUi au = Instantiate(achievementPrefab, achievementUiParent);
                au.transform.localPosition = new Vector3(0, 350 - 200 * i, 0);
                au.content = c;
                achievementUis.Add(au);
            }
            achieve_data_.Load();
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
                            //AchievementContent c = achieveDict[key];
                            AchievementContent c = achieve_data_.dict_[key];
                            iconIm.sprite = c.icon;
                            textMesh.text = c.body;
                            // セーブ申請を送る by tada
                            achieve_data_.Save();
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
        //AchievementContent c = Instance.achieveDict[key];
        AchievementContent c = Instance.achieve_data_.dict_[key];
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

    // セーブデータを削除する by tada
    public static void DeleteSaveData()
    {
        Instance.achieve_data_.DeleteSaveData();

        // さらにアンロック状態をリセットする
        foreach(var c in Instance.achieve_data_.dict_)
        {
            c.Value.isUnlocked = false;
        }
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

// セーブするデータ本体 キーとそれに対応する解放フラグだけを保存 前のは無駄が多すぎる
[System.Serializable] // by tada
public class AchievementData : TadaLib.Save.BaseSaver<AchievementData>
{
    // ディクショナリーはJsonUtilityに対応してないってまじ？？
    public Dictionary<string, AchievementContent> dict_;

    public List<string> keys_;
    public List<bool> values_;

    private const string kFileName = "Achievement";

    public AchievementData()
    {
        dict_ = new Dictionary<string, AchievementContent>();
        keys_ = new List<string>();
        values_ = new List<bool>();
    }

    // 辞書に新しい実績情報を登録する
    public void Add(string key, AchievementContent value)
    {
        if (dict_.ContainsKey(key)) return;
        keys_.Add(key);
        values_.Add(value.isUnlocked);
        dict_.Add(key, value);
    }

    // 初期化する ロードできないならfalseを返す
    public bool Load()
    {
        AchievementData data = Load(kFileName);

        if (data == null || data.keys_ == null || data.values_ == null || data.keys_.Count != data.values_.Count)
        {
            return false;
        }
        else
        {
            // 辞書型を形成
            //UnityEngine.Assertions.Assert.IsTrue(data.keys_.Count == data.values_.Count);

            for(int i = 0, n = data.keys_.Count; i < n; ++i)
            {
                if (!dict_.ContainsKey(data.keys_[i])) continue;
                // アンロック情報を更新
                dict_[data.keys_[i]].isUnlocked = data.values_[i];
                values_[i] = data.values_[i];
            }

            return true;
        }
    }

    // セーブする
    public void Save()
    {
        // アンロック情報を共有する
        for (int i = 0, n = values_.Count; i < n; ++i)
        {
            values_[i] = dict_[keys_[i]].isUnlocked;
        }

        if (save_completed_)
        {
            save_completed_ = false;
            TadaLib.Save.SaveManager.Instance.RequestSave(() => { Save(kFileName); save_completed_ = true; });
        }
    }

    // データを削除する
    public void DeleteSaveData()
    {
        TadaLib.Save.SaveManager.Instance.DeleteData(kFileName);
    }
}
