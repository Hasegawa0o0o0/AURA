/*
 * 20190103 作成
 *		20190205 倒れる角度の対応
 */
 /* 突風で壊せるもの */
using System.Collections.Generic;
using UnityEngine;

public class BreakedObject : MonoBehaviour
{
	bool isHitGust = false;												// 突風に当たったか
	List<GameObject> dividedObjectList = new List<GameObject>();		// 突風によって分けられたオブジェクト
	CapsuleCollider capsuleCollider;

	float toppleSpeed = 0f;		// 倒木を操作するときの倒れるスピード

	void Start ()
	{
		// 情報取得
		capsuleCollider = GetComponent<CapsuleCollider>();
		// 子どもを取得する
		foreach(Transform o in transform)
		{
			dividedObjectList.Add(o.gameObject);
			if (name == "StonePillar")
			{
				o.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
			}
		}
	}
	
	// デバッグアクション---------------------------------------------------
	public void ActionDebug()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			isHitGust = true;
			capsuleCollider.enabled = false;
		}
	}

	// ステージでのアクション------------------------------------------------
	public void Action()
	{
		// アタッチされているオブジェクトの名前によって処理を変える
		if (isHitGust && name == "StonePillar")
		{
			WoodBarkAction();
		}
	}

	// 倒木のアクション-----------------------------------------------------
	void WoodBarkAction()
	{
		// 完全に倒す
		if (dividedObjectList[0].transform.localEulerAngles.y >= 270f)
		{
			toppleSpeed += 0.125f;
			dividedObjectList[0].transform.localEulerAngles += new Vector3(0f, toppleSpeed, 0f);
		}
		// 倒されすぎたら戻す
		else if (dividedObjectList[0].transform.localEulerAngles.y > 0f)
		{
			dividedObjectList[0].transform.localEulerAngles = Vector3.zero;
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Gust")
		{
			isHitGust = true;
			capsuleCollider.enabled = false;
		}
	}
}
