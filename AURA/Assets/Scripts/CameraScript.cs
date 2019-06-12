/*
 * 20181021 作成
 */
/* プレイヤーを追いかける */
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CameraScript : MonoBehaviour
{
	public float end_x;			// 画面右端
	public float highend_y;		// 画面上端
	public float lowend_y;		// 画面下端

	// 引数で渡された位置を追尾する-------------------------------------------------------------
	public void TrackingMainCamera(Vector3 target)
	{
		transform.position = Vector3.Lerp(transform.position, target, 3.0f * Time.deltaTime);
		//左端
		if (transform.position.x < 0.0f)
		{
			transform.position = new Vector3(0.0f, transform.position.y, transform.position.z);
		}
		//右端
		else if (transform.position.x > end_x)
		{
			transform.position = new Vector3(end_x, transform.position.y, transform.position.z);
		}
		//上端
		if (transform.position.y > highend_y)
		{
			transform.position = new Vector3(transform.position.x, highend_y, transform.position.z);
		}
		//下端
		else if (transform.position.y < lowend_y)
		{
			transform.position = new Vector3(transform.position.x, lowend_y, transform.position.z);
		}
	}
}
