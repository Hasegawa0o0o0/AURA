/*
 * 20181006 作成
 *		20181007 火の判定を追加
 *		20181008 サーキュレーターの判定を追加
 *		20181009 サーキュレーターから吹いている風の判定の追加
 *		20181014 木判定の追加
 * 20181017 新規作成、PlayerColliderから処理の受け継ぎ
 *		20181017 葉っぱの判定を追加
 *				 風の判定を追加
 *				 止まるものを追加
 *				 水面の判定を追加
 *		20181020 リフトの判定を追加
 *				 親の情報を追加
 *		20181024 水面にういている葉っぱの判定を追加
 *				 エレベーターの判定を追加
 *		20181025 バグ防止
 *		20181213 判定方法変更
 *		20190106 blowWind削除
 *				 areaWind追加(Playerでは未使用)
 */
/* 部分的な判定をさせるときのクラス */
using UnityEngine;
using System.Collections.Generic;

public class PartiallyCollider : MonoBehaviour
{
	public LayerMask hitLayer;
	public List<Transform> point = new List<Transform>();

	public bool isHitStopObj = false;		// 止まるもの

	public bool isHitSoftWind = false;		// 風判定
	GameObject softWindObj = null;			// 風状態のプレイヤー

	public bool isHitBlock = false;			// ブロック判定

	public bool isHitFire = false;			// 火判定
	public Vector3 firePos = Vector3.zero;	// 火の位置

	public bool isHitAreaWind = false;		// ステージに吹いている風の判定
	public float areaWindAngleZ_rad = 0f;	// ステージに吹いている風の角度

	Transform parentTransform;				// 親のトランスフォーム

	void Start()
	{
		parentTransform = transform.root;
	}

	void Update()
	{
		// ポイントが2つ以下だと処理しない
		if (point.Count < 2)
		{
			return;
		}
		ResetAll();
		for (int i = 0; i + 1 < point.Count; ++i)
		{
			RaycastHit hit;
			if (Physics.Linecast(point[i].position, point[i + 1].position, out hit, Physics.AllLayers))
			{
				switch (hit.transform.tag)
				{
					case "SoftWind":
						isHitSoftWind = true;
						if (!softWindObj)
						{
							softWindObj = hit.transform.gameObject;
						}
						break;
					case "Block":
						isHitBlock = true;
						break;
					case "IntertiaBlock":
						isHitBlock = true;
						break;
					case "MoveBlock":
						isHitBlock = true;
						break;
					case "Fire":
						isHitFire = true;
						firePos = hit.transform.position;
						break;
					case "AreaWind":
						isHitAreaWind = true;
						areaWindAngleZ_rad = hit.transform.eulerAngles.z * Mathf.Deg2Rad;
						break;
				}
			}
		}
		isHitStopObj = isHitBlock;
		if (softWindObj && softWindObj.tag != "SoftWind")
		{
			isHitSoftWind = false;
			softWindObj = null;
		}
	}

	public void SetStopObj(bool isHit)
	{
		isHitBlock = false;
	}

	void ResetAll()
	{
		isHitSoftWind = isHitBlock = isHitFire = isHitAreaWind = false;
	}
}
