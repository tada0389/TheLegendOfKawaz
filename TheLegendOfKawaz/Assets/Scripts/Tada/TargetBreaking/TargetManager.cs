using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Target
{
    public class TargetManager : MonoBehaviour
    {
        private int target_num_ = 0;

        private float timer_;

        [SerializeField]
        private TextMeshProUGUI timer_text_;
        [SerializeField]
        private TextMeshProUGUI clear_text_;

        private bool started_ = false;
        private bool finished_ = false;

        private void Start()
        {
            timer_ = 0.0f;
            started_ = true;
        }

        private void Update()
        {
            if (!started_ || finished_) return;
            timer_ += Time.deltaTime;
            timer_text_.text = timer_.ToString("f1");
        }

        public void RegisterTarget()
        {
            ++target_num_;
        }

        public void BreakTarget()
        {
            --target_num_;
            if (target_num_ == 0) Finish();
        }

        private void Finish()
        {
            finished_ = true;
            StartCoroutine(FinishFlow());
        }

        private IEnumerator FinishFlow()
        {
            clear_text_.gameObject.SetActive(true);

            clear_text_.rectTransform.DOPunchScale(Vector3.one, 3.0f);

            yield return new WaitForSeconds(3.0f);
            // もどったゆ「
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}