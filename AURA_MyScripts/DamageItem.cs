/*
 * 20190201 作成
 *		20190218 プレイヤーを追いかけるように調整
*/
/* ゲージにダメージを与えるオブジェクトの動作 */
using System.Collections.Generic;
using UnityEngine;

public class DamageItem : MonoBehaviour
{
	float speed;
	const float kMaxSpeed = 1.5f;
	public float addSpeed = 0.05f;

	public void Action(Player playerObject)
	{
		// プレイヤーが近くにいたら加速していく
		if (CommonBehaviour.instance.IsNear(transform.position, playerObject.transform.position, 8f))
		{
			speed = speed + addSpeed > kMaxSpeed ? kMaxSpeed : speed + addSpeed;
		}
		// 近くにいなければ停止する
		else
		{
			speed = speed - kMaxSpeed / 45f < 0 ? 0 : speed - kMaxSpeed / 45f;
		}
		transform.position = Vector3.MoveTowards(transform.position, new Vector3(playerObject.transform.position.x, playerObject.transform.position.y + 1f, playerObject.transform.position.z),
			speed * Time.deltaTime);
		// 方向転換
		if (transform.position.x > playerObject.transform.position.x)
		{
			// 左を向く
			transform.localEulerAngles = new Vector3(-90f, 0f, -120f);
		}
		else if (transform.position.x < playerObject.transform.position.x)
		{
			// 右を向く
			transform.localEulerAngles = new Vector3(-90f, 0f, 120f);
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Gust")
		{
			gameObject.SetActive(false);
		}
	}
}
