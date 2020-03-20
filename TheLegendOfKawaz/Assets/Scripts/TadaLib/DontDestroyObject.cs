using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttachSurudake
{
    public class DontDestroyObject : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}