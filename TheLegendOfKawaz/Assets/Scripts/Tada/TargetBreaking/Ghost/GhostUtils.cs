using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゴーストデータの圧縮，解凍などに用いる関数など
/// </summary>

namespace TargetBreaking
{
    public class GhostUtils : MonoBehaviour
    {
        // アニメーション名を数字に置き換える

        /*
        ターゲットを壊せで使うもの

        isWalk
        isJump
        isFall
        isGround
        isDash
        isWall
        Idle
        Walk
        Dash
        Fall
        Jump
        Wall
        Shot
        ChargeShot

        あとは
        Play
        SetBoolTrue
        SetBoolFalse
        Restart

        14 * 4 で 56通り

        */

        private static Dictionary<int, string> index2Name;
        private static Dictionary<string, int> name2Index;

        private static bool inited_ = false;

        // 変換表
        static readonly string map = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        public static void Init()
        {
            inited_ = true;

            index2Name = new Dictionary<int, string>();
            name2Index = new Dictionary<string, int>();

            index2Name.Add(0, "isWalk");
            index2Name.Add(1, "isJump");
            index2Name.Add(2, "isFall");
            index2Name.Add(3, "isGround");
            index2Name.Add(4, "isDash");
            index2Name.Add(5, "isWall");
            index2Name.Add(6, "Idle");
            index2Name.Add(7, "Walk");
            index2Name.Add(8, "Dash");
            index2Name.Add(9, "Fall");
            index2Name.Add(10, "Jump");
            index2Name.Add(11, "Wall");
            index2Name.Add(12, "Shot");
            index2Name.Add(13, "ChargeShot");

            name2Index.Add("isWalk", 0);
            name2Index.Add("isJump", 1);
            name2Index.Add("isFall", 2);
            name2Index.Add("isGround", 3);
            name2Index.Add("isDash", 4);
            name2Index.Add("isWall", 5);
            name2Index.Add("Idle", 6);
            name2Index.Add("Walk", 7);
            name2Index.Add("Dash", 8);
            name2Index.Add("Fall", 9);
            name2Index.Add("Jump", 10);
            name2Index.Add("Wall", 11);
            name2Index.Add("Shot", 12);
            name2Index.Add("ChargeShot", 13);
        }

        public static string Index2Name(int index)
        {
            if (!inited_) Init();

            int id = index / 4;
            return index2Name[id];
        }

        public static int Name2Index(string name)
        {
            if (!inited_) Init();

            return name2Index[name] * 4;
        }

        // 数字を64進数に変換する
        public static string NumTo64Ary(long num)
        {
            string res = "";

            while (num > 0)
            {
                long mod = num % 64;
                res += map[(int)mod];
                num /= 64;
            }

            return res;
        }

        // 64進数を数字に変換する
        public static long Ary64ToNum(string ary)
        {
            long res = 0;

            long b = 1;
            for(int i = 0, n = ary.Length; i < n; ++i)
            {
                if (ary[i] == '/') res += b * 63;
                else if (ary[i] == '+') res += b * 62;
                else if (ary[i] <= '9') res += (ary[i] - '0' + 52) * b;
                else if (ary[i] <= 'Z') res += (ary[i] - 'A') * b;
                else res += (ary[i] - 'a' + 26) * b;
                b *= 64;
            }

            return res;
        }
    }
}