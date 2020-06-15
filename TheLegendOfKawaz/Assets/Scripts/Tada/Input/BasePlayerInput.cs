using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public abstract class BasePlayerInput : MonoBehaviour
    {
        public bool ActionEnabled = true;

        // 入力状態をリセットする
        public abstract void Reset();

        /// <summary>
        /// 指定したボタンが入力されたかを取得する
        /// </summary>
        /// <param name="code">ボタン</param>
        /// <returns></returns>
        public abstract bool GetButtonDown(ButtonCode code, bool use = true);
        /// <summary>
        /// 指定したボタンが入力されているかを取得する
        /// </summary>
        /// <param name="code">ボタン</param>
        /// <returns></returns>
        public abstract bool GetButton(ButtonCode code);

        /// <summary>
        /// 指定したボタンの入力が離されたかを取得する
        /// </summary>
        /// <param name="code">ボタン</param>
        /// <returns></returns>
        public abstract bool GetButtonUp(ButtonCode code);



        /// <summary>
        /// 指定したボタンが入力されたかを取得する
        /// </summary>
        /// <param name="code">ボタン</param>
        /// <returns></returns>
        public abstract bool GetButtonDown(ActionCode code, bool use = true);
        /// <summary>
        /// 指定したボタンが入力されているかを取得する
        /// </summary>
        /// <param name="code">ボタン</param>
        /// <returns></returns>
        public abstract bool GetButton(ActionCode code);
        /// <summary>
        /// 指定したボタンの入力が離されたかを取得する
        /// </summary>
        /// <param name="code">ボタン</param>
        /// <returns></returns>
        public abstract bool GetButtonUp(ActionCode code);



        /// <summary>
        /// 指定したボタンが入力されているかを取得する
        /// </summary>
        /// <param name="code">ボタン</param>
        /// <returns></returns>
        public abstract float GetAxis(AxisCode code);
    }
}