/*
 * 20190106 作成
 */
 /* 風が吹いているところに風のエフェクトを生成する */
using System.Collections.Generic;
using UnityEngine;

public class BlowWind : MonoBehaviour
{
	public GameObject windEffectPrefab;							// エフェクトのプレハブ
	List<GameObject> windEffectsList = new List<GameObject>();	// エフェクトを管理するリスト
	Vector3 effectVelocity = Vector3.zero;						// エフェクトを進める方向
	public int deleteEffectFrame;								// エフェクトを削除するフレーム数
	List<int> deleteEffectFrameList = new List<int>();			// エフェクトを削除するまでのフレーム数
	int insFrame = 0;											// 生成するまでのフレーム数
	float angleZ_rad;											// z軸の角度(ラジアン)

	void Start ()
	{
		angleZ_rad = transform.eulerAngles.z * Mathf.Deg2Rad;
		float speedValue = 0.05f;
		effectVelocity.x = speedValue * Mathf.Cos(angleZ_rad);
		effectVelocity.y = speedValue * Mathf.Sin(angleZ_rad);
	}

	void Update ()
	{
		// エフェクトの削除
		for (int i = 0; i < windEffectsList.Count; ++i)
		{
			// 一定時間経過したエフェクトを削除する
			if (deleteEffectFrameList[i] > deleteEffectFrame)
			{
				GameObject deleteObj = windEffectsList[i];
				windEffectsList.RemoveAt(i);
				Destroy(deleteObj);
				deleteEffectFrameList.RemoveAt(i);
				continue;
			}
			windEffectsList[i].transform.position += effectVelocity;
			deleteEffectFrameList[i]++;
		}
		// エフェクトの生成
		if (insFrame >= 3)
		{
			GameObject obj = Instantiate(windEffectPrefab, transform.position, Quaternion.identity);
			obj.transform.parent = transform;
			obj.transform.localPosition = new Vector3(Random.Range(-0.5f, 0f), Random.Range(-0.5f, 0.5f), 0f);
			obj.transform.parent = null;
			windEffectsList.Add(obj);
			deleteEffectFrameList.Add(0);
			insFrame = 0;
		}
		++insFrame;
	}
}
