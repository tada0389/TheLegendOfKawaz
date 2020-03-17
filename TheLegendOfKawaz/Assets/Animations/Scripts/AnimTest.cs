using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoitanLib;

public class AnimTest : MonoBehaviour
{
    Animator animator;
    float vx = 0;
    float vy = 0;
    bool isGround;

    [SerializeField]
    ParticleSystem shotEff;
    [SerializeField]
    ParticleSystem chargeShotEff;
    [SerializeField]
    mameTest tama;
    [SerializeField]
    GameObject chargeTama;
    [SerializeField]
    GameObject chargeStartEff;
    [SerializeField]
    GameObject chargeEndEff;
    private float chargeTime;
    [SerializeField]
    private float chargeEndTime;

    [SerializeField]
    private float vxMax = 2;
    [SerializeField]
    private float dashMax = 10;
    [SerializeField]
    private float vyMax = 4;
    [SerializeField]
    private float gravity = -1;
    Vector3 oldPos, nowPos;
    [SerializeField]
    float stageWidth = 8.64f;
    [SerializeField]
    float stageHeight = 4.8f;
    [SerializeField]
    float charaWidth = 2;
    [SerializeField]
    float charaHeight = 4;
    [SerializeField]
    private float dashTimeMax = 1f;
    float dashTime = 0;
    private float shotTime = 0;
    float dir = 1;
    [SerializeField]
    int mameMaxCount = 3;
    List<mameTest> mameList = new List<mameTest>();
    [SerializeField]
    private mameTest chargeMame;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        oldPos = nowPos = transform.position;

        //オブジェクト登録
        ObjectPoolManager.Init(tama, 3);
    }

    // Update is called once per frame
    void Update()
    {
        oldPos = nowPos;
        if (ActionInput.GetButtonDown(ActionCode.Shot))
        {
            animator.SetLayerWeight(1, 1);
            shotTime = 1;
            animator.Play("Shot", 1, 0);
            shotEff.Play();



            mameTest m = ObjectPoolManager.GetInstance<mameTest>(tama);
            if (m != null)
            {
                m.gameObject.SetActive(true);
                m.Init(shotEff.transform.position, Vector3.right * dir);
            }

        }

        if (ActionInput.GetButton(ActionCode.Shot))
        {
            chargeTime += Time.deltaTime;
        }

        if (ActionInput.GetButtonUp(ActionCode.Shot))
        {
            if (chargeTime >= chargeEndTime)
            {
                animator.SetLayerWeight(1, 1);
                shotTime = 1;
                animator.Play("ChargeShot", 1, 0);
                chargeShotEff.Play();
                if (!chargeMame.gameObject.activeSelf)
                {
                    chargeMame.gameObject.SetActive(true);
                    chargeMame.Init(chargeShotEff.transform.position, Vector3.right * dir);
                }
            }
            chargeTime = 0;
        }
        if (chargeTime < chargeEndTime / 2)
        {
            chargeStartEff.SetActive(false);
            chargeEndEff.SetActive(false);
        }
        else if (chargeTime < chargeEndTime)
        {
            chargeStartEff.SetActive(true);
            chargeEndEff.SetActive(false);
        }
        else
        {
            chargeStartEff.SetActive(false);
            chargeEndEff.SetActive(true);
        }



        if (shotTime > 0)
        {
            animator.SetLayerWeight(1, 1);
            shotTime -= Time.deltaTime;
        }
        else
        {
            animator.SetLayerWeight(1, 0);
        }

        //ダッシュ
        if (isGround)
        {
            if (dashTime > 0)
            {
                dashTime -= Time.deltaTime;
            }
            if (ActionInput.GetButtonDown(ActionCode.Dash))
            {
                dashTime = dashTimeMax;
                animator.Play("Dash");
            }
        }
        else
        {
            animator.SetBool("isDash", false);
        }

        //水平方向
        if (dashTime > 0)
        {
            animator.SetBool("isDash", true);
            /*
            float inputX = ActionInput.GetAxis(AxisCode.Horizontal);
            if (inputX != 0)
            {
                vx = ActionInput.GetAxis(AxisCode.Horizontal) * dashMax;
            }
            */
            vx = ActionInput.GetAxis(AxisCode.Horizontal) * dashMax;
        }
        else
        {
            animator.SetBool("isDash", false);
            vx = ActionInput.GetAxis(AxisCode.Horizontal) * vxMax;
        }

        nowPos.x += vx * Time.deltaTime;
        if (vx > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            dir = 1;
            animator.SetBool("isWalk", true);
        }
        else if (vx < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            dir = -1;
            animator.SetBool("isWalk", true);
        }
        else
        {
            animator.SetBool("isWalk", false);
        }

        //垂直方向
        if (isGround)
        {
            if (ActionInput.GetButtonDown(ActionCode.Jump))
            {
                vy = vyMax;
                isGround = false;
                animator.Play("Jump");
            }
            else
            {
                vy = 0;
            }
        }
        else
        {
            vy += gravity * Time.deltaTime;
            if (vy > 0)
            {
                animator.SetBool("isFall", false);
            }
            else
            {
                animator.SetBool("isFall", true);
            }
        }

        //座標計算
        nowPos.y += vy * Time.deltaTime;

        //押し出し判定
        if (nowPos.y - charaHeight <= -stageHeight)
        {
            nowPos.y = -stageHeight + charaHeight;
            isGround = true;
        }

        if (nowPos.x + charaWidth >= stageWidth)
        {
            nowPos.x = stageWidth - charaWidth;
        }

        if (nowPos.x - charaWidth <= -stageWidth)
        {
            nowPos.x = -stageWidth + charaWidth;
        }

        //着地アニメーション
        animator.SetBool("isGround", isGround);

        //座標代入
        transform.position = nowPos;
    }



    private mameTest GenerateMame()
    {
        foreach (mameTest mame in mameList)
        {
            if (!mame.gameObject.activeSelf)
            {
                return mame;
            }
        }
        if (mameList.Count < mameMaxCount)
        {
            mameTest m = Instantiate(tama).GetComponent<mameTest>();
            mameList.Add(m);
            return m;
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        GizmosExtensions2D.DrawWireRect2D(transform.position, charaWidth * 2, charaHeight * 2);
        Gizmos.color = Color.white;
        GizmosExtensions2D.DrawWireRect2D(Vector3.zero, stageWidth * 2, stageHeight * 2);
    }
}
