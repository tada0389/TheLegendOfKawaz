using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 入力情報を管理するクラス
/// プレイヤーのデモをお遊びで作成
/// </summary>

namespace TadaInput
{
    public class DemoPlayerInput : BasePlayerInput
    {
        bool flag = false;

        float hoge = 0;
        float foo = 0f;

        public override float GetAxis(AxisCode code)
        {
            flag = !flag;

            return (flag)? 1.0f : -1.0f;
        }

        public override bool GetButton(ButtonCode code)
        {
            return false;
        }

        public override bool GetButton(ActionCode code)
        {
            if(code == ActionCode.Jump)
            {
                foo += Time.deltaTime;
                if (foo >= 4f) foo = 0.0f;
                return foo > 2f;
            }
            return false;
        }

        public override bool GetButtonDown(ButtonCode code, bool use = true)
        {
            return false;
        }

        public override bool GetButtonDown(ActionCode code, bool use = true)
        {
            if (code == ActionCode.Shot)
            {
                hoge += Time.deltaTime;
                if (hoge >= 1f) hoge = -0.001f;
                return (hoge < 0f);
            }
            if (code == ActionCode.Jump) return true;
            if (code == ActionCode.Dash) return foo > 3f;
            return false;
        }

        public override bool GetButtonUp(ButtonCode code)
        {
            return false;
        }

        public override bool GetButtonUp(ActionCode code)
        {
            return false;
        }
    }
}