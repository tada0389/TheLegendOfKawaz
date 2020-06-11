using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 入力情報を管理するクラス
/// 先行入力などに対応している
/// 先行入力は，ジャンプとダッシュのみ対応
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

        private void Awake()
        {
            jump_buff_ = new Queue<float>();
            dash_buff_ = new Queue<float>();
        }

        private void Update()
        {
            // 制限時間を超えているものはあるか
            while(jump_buff_.Count >= 1)
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


        public override float GetAxis(AxisCode code)
        {
            return ActionInput.GetAxis(code);
        }

        public override bool GetButton(ButtonCode code)
        {
            return ActionInput.GetButton(code);
        }

        public override bool GetButton(ActionCode code)
        {
            return ActionInput.GetButton(code);
        }

        public override bool GetButtonDown(ButtonCode code, bool use = true)
        {
            return ActionInput.GetButtonDown(code);
        }

        public override bool GetButtonDown(ActionCode code, bool use = true)
        {
            if(code == ActionCode.Jump)
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
            return ActionInput.GetButtonDown(code);
        }

        public override bool GetButtonUp(ButtonCode code)
        {
            return ActionInput.GetButtonUp(code);
        }

        public override bool GetButtonUp(ActionCode code)
        {
            return ActionInput.GetButtonUp(code);
        }
    }
}