using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 何かに当たったら看板を揺らす
/// </summary>

public class BilliboardController : MonoBehaviour
{
    [SerializeField]
    private float init_power_ = 1.0f;

    [SerializeField]
    private float flip_interval_ = 0.5f;

    [SerializeField]
    private float power_down_rate_ = 0.5f;

    [SerializeField]
    private float stop_threshold_ = 0.1f;

    [SerializeField]
    private float max_rotate_y_ = 80f;

    [SerializeField]
    private float max_power_ = 1.0f;

    private bool is_front_ = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            TadaLib.TadaRigidbody actor = collision.GetComponent<TadaLib.TadaRigidbody>();
            if(actor != null)
            {
                float power = Mathf.Abs(actor.Velocity.y) + Mathf.Abs(actor.Velocity.x) * 0.2f;

                StopAllCoroutines();
                StartCoroutine(Flow(power * init_power_));
            }
        }   
    }

    private IEnumerator Flow(float power)
    {
        power = Mathf.Min(max_power_, power);

        while (power >= stop_threshold_)
        {
            transform.DOKill();
            float target_rotate_y = (is_front_) ? -max_rotate_y_ : max_rotate_y_;
            target_rotate_y *= (power / max_power_);
            float diff = Mathf.Abs(target_rotate_y - transform.rotation.eulerAngles.y);
            float duration = (diff / Mathf.Abs(target_rotate_y * 2.0f)) * flip_interval_;

            transform.DORotate(new Vector3(target_rotate_y, 0f, 0f), duration).SetEase(Ease.InOutCubic);
            power *= power_down_rate_;

            is_front_ = !is_front_;
            yield return new WaitForSeconds(duration);
        }

        // 最後に元に戻す
        {
            transform.DOKill();
            float target_rotate_y = 0f;
            float duration = flip_interval_ / 2f;

            transform.DORotate(new Vector3(target_rotate_y, 0f, 0f), duration).SetEase(Ease.InOutCubic);

            is_front_ = false;
        }
    }
}
