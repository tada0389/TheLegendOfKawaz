using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndingManager : MonoBehaviour
{
    [SerializeField]
    EndObj[] objects;
    Sequence seq;
    // Start is called before the first frame update
    void Start()
    {
        seq = DOTween.Sequence();
        for (int i = 0; i < objects.Length; i++)
        {
            seq.AppendCallback(() =>
            {
                objects[i].obj.transform.DOMoveY(8, 10);
            }).AppendInterval(objects[i].waitTime);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    [System.Serializable]
    public class EndObj
    {
        public GameObject obj;
        public float waitTime;
    }
}
