using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Actor.Player;

namespace test
{
    public class ParamTest : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text_;

        int index_ = 0;

        private ParamManager param_;

        // Start is called before the first frame update
        void Start()
        {
            param_ = ParamManager.Instance;
            Display();
        }

        void Update()
        {
            if (ActionInput.GetButtonDown(ButtonCode.Down)) index_ = (index_ + 1) % param_.Params.Count;
            if (ActionInput.GetButtonDown(ButtonCode.Up)) index_ = (index_ - 1 + param_.Params.Count) % param_.Params.Count;
            if (ActionInput.GetButtonDown(ActionCode.Jump)) param_.Params[index_].LevelUp();
            Display();
            if (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.B].wasPressedThisFrame){
                UnityEngine.SceneManagement.SceneManager.LoadScene("TestScene");
            }
        }

        private void Display()
        {
            int cnt = 0;
            text_.text = "内容  :能力値  :レベル  :必要なポイント  \n";
            foreach (var param in param_.Params)
            {
                if (index_ == cnt) text_.text += "<color=red>";
                text_.text += param.Name.PadRight(10) + ":";
                text_.text += param.Value.ToString().PadLeft(5) + ":";
                text_.text += param.Level.ToString().PadLeft(5) + ":";
                if (param.NeedPoint() != -1) text_.text += param.NeedPoint().ToString().PadLeft(10);
                else text_.text += "最大レベル".PadLeft(5);
                if (index_ == cnt) text_.text += "</color>";
                text_.text += "\n";
                ++cnt;
            }
        }
    }
}