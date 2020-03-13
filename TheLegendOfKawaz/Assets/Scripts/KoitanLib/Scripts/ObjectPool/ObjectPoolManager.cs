using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;

namespace KoitanLib
{
    public class ObjectPoolManager : SingletonMonoBehaviour<ObjectPoolManager>
    {
        private static Dictionary<string, PoolMonoElement> monoPoolList = new Dictionary<string, PoolMonoElement>();
        private static Dictionary<string, PoolObjElement> objectPoolList = new Dictionary<string, PoolObjElement>();


        public static void Init(string key, MonoBehaviour o, int MaxNum = 1)
        {
            if (!monoPoolList.ContainsKey(key))
            {
                PoolMonoElement elem = new PoolMonoElement(o, MaxNum);
                monoPoolList.Add(key, elem);
            }
        }

        /*
        public static void Init(string key, GameObject o, int MaxNum = 1)
        {
            if (!objectPoolList.ContainsKey(key))
            {
                PoolObjElement elem = new PoolObjElement(o, MaxNum);
                objectPoolList.Add(key, elem);
            }
        }
        */

        public static T GetInstance<T>(string key) where T : MonoBehaviour
        {
            if (monoPoolList.ContainsKey(key))
            {
                return monoPoolList[key].GetInstance<T>();
            }
            return null;
        }

        public static GameObject GetInstanceObj(string key)
        {
            if (monoPoolList.ContainsKey(key))
            {
                return monoPoolList[key].GetInstanceObj();
            }
            return null;
        }

        public static void Release(string key)
        {
            if (monoPoolList.ContainsKey(key))
            {
                monoPoolList[key].Clear();
                monoPoolList.Remove(key);
            }
        }

        public class PoolMonoElement
        {
            public int maxNum;
            public MonoBehaviour original;
            List<MonoBehaviour> poolList;

            public PoolMonoElement(MonoBehaviour o, int max)
            {
                maxNum = max;
                original = o;
                poolList = new List<MonoBehaviour>(max);
                for (int i = 0; i < maxNum; i++)
                {
                    MonoBehaviour m = Instantiate(original);
                    m.gameObject.SetActive(false);
                    poolList.Add(m);
                }
            }

            //足りないときはnullを返す
            public T GetInstance<T>() where T : MonoBehaviour
            {
                foreach (T elem in poolList)
                {
                    if (!elem.gameObject.activeSelf)
                    {
                        elem.gameObject.SetActive(true);
                        return elem;
                    }
                }
                return null;
            }

            //足りないときはnullを返す
            public GameObject GetInstanceObj()
            {
                foreach (MonoBehaviour elem in poolList)
                {
                    if (!elem.gameObject.activeSelf)
                    {
                        elem.gameObject.SetActive(true);
                        return elem.gameObject;
                    }
                }
                return null;
            }

            public void Clear()
            {
                for (int i = maxNum - 1; i >= 0; i--)
                {
                    Destroy(poolList[i].gameObject);
                }
            }
        }

        public class PoolObjElement
        {
            public int maxNum;
            public GameObject original;
            List<GameObject> poolList;

            public PoolObjElement(GameObject o, int max)
            {
                maxNum = max;
                original = o;
                poolList = new List<GameObject>(max);
                for (int i = 0; i < maxNum; i++)
                {
                    GameObject m = Instantiate(original);
                    m.SetActive(false);
                    poolList.Add(m);
                }
            }

            //足りないときはnullを返す
            public GameObject GetInstanceObj()
            {
                foreach (GameObject elem in poolList)
                {
                    if (!elem.activeSelf)
                    {
                        elem.SetActive(true);
                        return elem;
                    }
                }
                return null;
            }

            public void Clear()
            {
                for (int i = maxNum - 1; i >= 0; i--)
                {
                    Destroy(poolList[i].gameObject);
                }
            }
        }
    }
}
