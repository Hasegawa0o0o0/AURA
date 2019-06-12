/*
 20190201 作成
*/
/* ゲージ回復アイテムの動作 */
using System.Collections.Generic;
using UnityEngine;

public class RecoverItem : MonoBehaviour
{
	List<Transform> windEffectList = new List<Transform>();
	float speed = 0f;
	int disabledFrame = 0;

	void Start()
	{
		foreach (Transform t in transform)
		{
			windEffectList.Add(t);
		}
		speed = 360f;
	}

	void Update()
	{
		// エフェクトをまわす
		for (int i = 0; i < windEffectList.Count; ++i)
		{
			windEffectList[i].transform.RotateAround(transform.position, Vector3.up, speed * Time.deltaTime);
		}
	}

	public void UpdateStatus()
	{
		// 自信をアクティブにするためにカウントダウンする
		if (--disabledFrame <= 0)
		{
			disabledFrame = 0;
			gameObject.SetActive(true);
		}
	}

	void OnTriggerEnter(Collider col)
	{
		// プレイヤーに当たったら非表示にする
		if (col.tag == "Player" || col.tag == "SoftWind" || col.tag == "Gust")
		{
			gameObject.SetActive(false);
			disabledFrame = 600;
		}
	}
}
