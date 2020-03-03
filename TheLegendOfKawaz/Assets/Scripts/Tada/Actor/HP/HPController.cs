using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor;

namespace Actor
{
    public class HPController : MonoBehaviour
    {
        [SerializeField]
        private Canvas canvas_;

        [SerializeField]
        private HPBar hp_bar_prefab_;

        [SerializeField]
        private List<BaseActorController> actors_;

        [SerializeField]
        private RectTransform left_bar_pos_;

        [SerializeField]
        private RectTransform right_bar_pos_;

        private List<HPBar> hp_bars_;

        // Start is called before the first frame update
        void Start()
        {
            // プレハブの作成
            hp_bars_ = new List<HPBar>();

            if(actors_.Count == 1)
            {
                HPBar bar = Instantiate(hp_bar_prefab_, canvas_.transform);
                bar.transform.position = left_bar_pos_.position;
                hp_bars_.Add(bar);
            }
            else if(actors_.Count >= 2)
            {
                HPBar bar_l = Instantiate(hp_bar_prefab_, canvas_.transform);
                HPBar bar_r = Instantiate(hp_bar_prefab_, canvas_.transform);
                bar_l.transform.position = left_bar_pos_.position;
                bar_r.transform.position = right_bar_pos_.position;
                hp_bars_.Add(bar_l);
                hp_bars_.Add(bar_r);
            }
        }

        // Update is called once per frame
        void Update()
        {
            for(int i = 0; i < actors_.Count; ++i)
            {
                if(actors_[i] != null && hp_bars_[i] != null)
                {
                    hp_bars_[i].ChangeValue(actors_[i].HP);
                }
            }
        }
    }
}