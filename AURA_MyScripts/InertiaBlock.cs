/*
 * 20190201 作成
*/
/* 慣性のある足場の動作 */
using System.Collections.Generic;
using UnityEngine;

public class InertiaBlock : MonoBehaviour
{
	Vector3 startPos = Vector3.zero;		// オブジェクトの開始位置
	Vector3 centerPosition = Vector3.zero;	// 揺れるときの中心の位置
	Vector3 playerSpeed = Vector3.zero;		// 随時更新されるプレイヤーのスピード
	Vector3 influenceSpeed = Vector3.zero;	// 位置に反映させるスピード
	// 以下マイナス値にはならない
	public float returnSpeed = 2.5f;		// 戻るときのスピード
	public float decelerationSpeed = 0.1f;	// 減速度
	public float weight = 1f;				// プレイヤーの初速をどれくらい減速させるか

	void Start()
	{
		startPos = transform.position;
		centerPosition = startPos;
		returnSpeed = returnSpeed < 0f ? 0f : returnSpeed;
		decelerationSpeed = decelerationSpeed < 0f ? 0f : decelerationSpeed;
		weight = weight < 0f ? 0f : weight;
	}

	public void Action()
	{
		if (influenceSpeed == Vector3.zero && startPos == centerPosition)
		{
			// 揺らす
			CommonBehaviour.instance.Swing(gameObject);
		}
		else
		{
			// 元の位置に戻す
			transform.position = Vector3.MoveTowards(transform.position, startPos, returnSpeed * Time.deltaTime);
			// スピードを位置に反映させる
			transform.position += influenceSpeed;
			// 揺れるときの中心の位置を更新していく
			centerPosition = transform.position;
			// スピードを0にしていく
			influenceSpeed = Vector3.MoveTowards(influenceSpeed, Vector3.zero, decelerationSpeed * Time.deltaTime);
		}
	}

	public void SetSpeed(Vector3 speed)
	{
		playerSpeed = speed;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player" || col.tag == "SoftWind" || col.tag == "Gust")
		{
			influenceSpeed = playerSpeed / weight * Time.deltaTime;
		}
	}
}
