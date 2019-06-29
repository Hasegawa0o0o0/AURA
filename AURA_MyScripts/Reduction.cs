/*
 * 20190209 作成
 */
 /* データ量削減 */
using System.Collections.Generic;
using UnityEngine;

public class Reduction : MonoBehaviour
{
	List<GameObject> blockList = new List<GameObject>();
	List<Collider> blockColliderList = new List<Collider>();

	void Start ()
	{
		foreach(Transform o in transform)
		{
			blockList.Add(o.gameObject);
			blockColliderList.Add(o.GetComponent<Collider>());
		}
	}
	
	void Update ()
	{
	}

	// ブロックの表示非表示を設定する----------------------------------------------------------
	public void SetBlockDisplay(GameObject camera)
	{
		if (camera.transform.position.z < -20f) { return; }
		for (int i = 0; i < blockList.Count; ++i)
		{
			bool isNear = CommonBehaviour.instance.IsNear(blockList[i].transform.position, camera.transform.position, 35f);
			blockList[i].SetActive(isNear);
			blockColliderList[i].enabled = isNear;
		}
	}
}
