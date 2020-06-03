using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Configuration;
using UnityEngine;

/// <summary>
/// ステージ内にあるオブジェクトを参照し
/// ミニマップを生成するクラス
/// </summary>

[System.Serializable]
public class StageObject
{
    [field:SerializeField]
    public string Tag { private set; get; }

    [field:SerializeField]
    public SpriteRenderer Maker { private set; get; }

    [field:SerializeField]
    public List<GameObject> Objects { private set; get; }
}

public class MiniMapController : MonoBehaviour
{
    [SerializeField]
    private Rect WorldMap;

    [SerializeField]
    private Rect MiniMap;

    [SerializeField]
    private List<StageObject> object_types_;

    private List<GameObject> objects_;

    // タグ名と下のリストのインデックスを紐づける
    private Dictionary<string, int> dict_;
    // マーカーの種類を保持
    private List<SpriteRenderer> marker_types_;

    // マーカーのオブジェクト本体を保持
    private List<List<SpriteRenderer>> markers_;

    private void Awake()
    {
        dict_ = new Dictionary<string, int>();
        marker_types_ = new List<SpriteRenderer>();
        markers_ = new List<List<SpriteRenderer>>();
        objects_ = new List<GameObject>();

        InitObject();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        RenderObject();
    }

    // オブジェクトを登録する
    public void RegisterObject(GameObject obj)
    {
        objects_.Add(obj);

        int id = dict_[obj.tag];
        var marker = marker_types_[id];
        var pool = Instantiate(marker, transform);
        markers_[id].Add(pool);
    }

    // 初期化する
    private void InitObject()
    {
        int cnt = 0;
        // 必要な分プーリングする しないことにした やっぱする
        foreach(var type in object_types_)
        {
            dict_.Add(type.Tag, cnt++);
            markers_.Add(new List<SpriteRenderer>());
            marker_types_.Add(type.Maker);

            foreach(var obj in type.Objects)
            {
                RegisterObject(obj);
            }
        }
    }

    private void RenderObject()
    {
        List<int> cnt = new List<int>(marker_types_.Count);
        cnt.AddRange(System.Linq.Enumerable.Repeat(0, marker_types_.Count));

        foreach (var obj in objects_)
        {
            int id = dict_[obj.tag];
            // 対応するマーカー
            var marker = markers_[id][cnt[id]].gameObject;
            ++cnt[id];

            if (!obj.activeSelf)
            {
                marker.SetActive(false);
            }
            else
            {
                // ミニマップ上の座標を取得
                Vector3 new_pos = GetMiniMapPosition(obj.transform.position);
                marker.transform.localPosition = new_pos;
                marker.transform.localRotation = obj.transform.localRotation;
                // スケールはどうしようか とりあえずは関係なしで
                marker.SetActive(true);
            }
        }
    }

    // ワールド座標からミニマップ上の座標に変換する
    private Vector3 GetMiniMapPosition(Vector3 world_pos)
    {
        float x = (world_pos.x - WorldMap.x) / WorldMap.width;
        x = MiniMap.x + x * MiniMap.width;
        float y = (world_pos.y - WorldMap.y) / WorldMap.height;
        y = MiniMap.y + y * MiniMap.height;

        return new Vector3(x, y, 10f);
    }
}
