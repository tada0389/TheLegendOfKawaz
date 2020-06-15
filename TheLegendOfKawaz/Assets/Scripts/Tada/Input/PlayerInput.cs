using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 入力情報を管理するクラス
/// 先行入力などに対応している
/// 先行入力は，ジャンプとダッシュのみ対応
/// また，Updateで受け取った入力をFixedUpdateへの入力に変換している
/// 
/// 先行入力に関して
/// ・タイムスケールが通常とは異なった時はどうなるか
/// -> タイムスケールを考慮せず，常にタイムスケールが1のときと同じ挙動をする
/// 
/// </summary>

namespace TadaInput
{
    public class PlayerInput : BasePlayerInput
    {
        // 先行入力の受付時間
        [SerializeField]
        private float persude_time_ = 0.06f;

        private Queue<float> jump_buff_;
        private Queue<float> dash_buff_;

        private Dictionary<ButtonCode, bool> button_down_;
        private Dictionary<ButtonCode, bool> button_released_;
        private Dictionary<ActionCode, bool> action_down_;
        private Dictionary<ActionCode, bool> action_released_;

        private Dictionary<AxisCode, float> axis_;


        private void Awake()
        {
            jump_buff_ = new Queue<float>();
            dash_buff_ = new Queue<float>();
            button_down_ = new Dictionary<ButtonCode, bool>();
            button_released_ = new Dictionary<ButtonCode, bool>();
            action_down_ = new Dictionary<ActionCode, bool>();
            action_released_ = new Dictionary<ActionCode, bool>();

            axis_ = new Dictionary<AxisCode, float>();

            //初期化
            foreach (ButtonCode code in System.Enum.GetValues(typeof(ButtonCode)))
            {
                button_down_.Add(code, false);
                button_released_.Add(code, false);
            }

            foreach (ActionCode code in System.Enum.GetValues(typeof(ActionCode)))
            {
                action_down_.Add(code, false);
                action_released_.Add(code, false);
            }

            foreach (AxisCode code in System.Enum.GetValues(typeof(AxisCode)))
            {
                axis_.Add(code, 0.0f);
            }
        }

        private void Update()
        {
            if (!ActionEnabled)
            {
                jump_buff_.Clear();
                dash_buff_.Clear();
                return;
            }

            foreach (ButtonCode code in System.Enum.GetValues(typeof(ButtonCode)))
            {
                if (ActionInput.GetButtonDown(code)) button_down_[code] = true;
                if (ActionInput.GetButtonUp(code)) button_released_[code] = true;
            }

            foreach (ActionCode code in System.Enum.GetValues(typeof(ActionCode)))
            {
                if (ActionInput.GetButtonDown(code)) action_down_[code] = true;
                if (ActionInput.GetButtonUp(code)) action_released_[code] = true;
            }

            foreach (AxisCode code in System.Enum.GetValues(typeof(AxisCode)))
            {
                axis_[code] = ActionInput.GetAxis(code);
            }

            // 制限時間を超えているものはあるか
            while (jump_buff_.Count >= 1)
            {
                if (Time.unscaledTime - jump_buff_.Peek() > persude_time_) jump_buff_.Dequeue();
                else break;
            }
            while (dash_buff_.Count >= 1)
            {
                if (Time.unscaledTime - dash_buff_.Peek() > persude_time_) dash_buff_.Dequeue();
                else break;
            }

            if (ActionInput.GetButtonDown(ActionCode.Jump)) jump_buff_.Enqueue(Time.unscaledTime);
            if (ActionInput.GetButtonDown(ActionCode.Dash)) dash_buff_.Enqueue(Time.unscaledTime);
        }


        // 入力状態をリセットする
        public override void Reset()
        {
            foreach (ButtonCode code in System.Enum.GetValues(typeof(ButtonCode)))
            {
                button_down_[code] = false;
                button_released_[code] = false;
            }

            foreach (ActionCode code in System.Enum.GetValues(typeof(ActionCode)))
            {
                action_down_[code] = false;
                action_released_[code] = false;
            }
        }

        public override float GetAxis(AxisCode code)
        {
            if (!ActionEnabled) return 0.0f;
            return axis_[code];// ActionInput.GetAxis(code);
        }

        public override bool GetButton(ButtonCode code)
        {
            if (!ActionEnabled) return false;
            return ActionInput.GetButton(code);
        }

        public override bool GetButton(ActionCode code)
        {
            if (!ActionEnabled) return false;
            return ActionInput.GetButton(code);
        }

        public override bool GetButtonDown(ButtonCode code, bool use = true)
        {
            if (!ActionEnabled) return false;
            return button_down_[code];
        }

        public override bool GetButtonDown(ActionCode code, bool use = true)
        {
            if (!ActionEnabled) return false;
            if (code == ActionCode.Jump)
            {
                bool res = (jump_buff_.Count >= 1);
                if (use && res) jump_buff_.Dequeue();
                return res;
            }
            if(code == ActionCode.Dash)
            {
                bool res = (dash_buff_.Count >= 1);
                if (use && res) dash_buff_.Dequeue();
                return res;
            }
            return action_down_[code];
        }

        public override bool GetButtonUp(ButtonCode code)
        {
            if (!ActionEnabled) return false;
            return button_released_[code];
        }

        public override bool GetButtonUp(ActionCode code)
        {
            if (!ActionEnabled) return false;
            return action_released_[code];
        }
    }
}