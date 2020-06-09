using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Test
{
    public class GlobalTimeDisplayer : MonoBehaviour
    {
        private TextMeshProUGUI text_;

        private void Start()
        {
            text_ = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            double e_time = Global.GlobalDataManager.EternalTimer;
            double s_time = Global.GlobalDataManager.StoryTimer;
            text_.text = e_time.ToString("F3") + " (s)\n";
            text_.text += s_time.ToString("F3") + " (s)\n";
        }
    }
}