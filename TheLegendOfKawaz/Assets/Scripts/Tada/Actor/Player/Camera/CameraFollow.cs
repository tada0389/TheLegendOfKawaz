using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ごみすくりぷと

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    private bool isFollow = true;//カメラを追従するかどうか

    private void LateUpdate()
    {
        if(isFollow)
        {
            transform.position = new Vector3(target.transform.position.x, target.transform.position.y, -10f);
        }

        //勝手にキー割り当て変更しました、ごめん by koitan 2/17
        if (transform.position.y < -20f || Input.GetKeyDown(KeyCode.Alpha1)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            isFollow = !isFollow;
        }
    }
}
