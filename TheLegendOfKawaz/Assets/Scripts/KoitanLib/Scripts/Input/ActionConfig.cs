using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ActionConfig : ScriptableObject
{

    //設定したいデータの変数
    public string spriteName;
    public int dicedeIndex, backIndex, jumpIndex, shotIndex, dashIndex;

}