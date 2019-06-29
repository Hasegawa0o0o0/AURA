/*
 * 20190210 作成
 */
 /* 崩れる足場の実装 */
using System.Collections.Generic;
using UnityEngine;

public class CollapseBlock : MonoBehaviour
{
	public enum CollapseBlockState
	{
		eNormal,
		eCollapsed,
		eNone
	}
	//----------------------------
	Vector3 playerSpeed = Vector3.zero;	// プレイヤーのスピード
	CollapseBlockState state;			// ギミックの状態
	Collider colliderComponent;			// 衝突判定

	MeshRenderer meshRenderer;			// レンダラー

	void Start ()
	{
		state = CollapseBlockState.eNormal;
		colliderComponent = GetComponent<Collider>();
		meshRenderer = GetComponent<MeshRenderer>();
	}
	
	void Update ()
	{
	}

	// アクション
	public void Action()
	{
		colliderComponent.enabled = state == CollapseBlockState.eNormal;
		meshRenderer.enabled = state == CollapseBlockState.eNormal;
	}

	// プレイヤーの速度をセットする
	public void SetPlayerSpeed(Vector3 speed)
	{
		playerSpeed = speed;
	}

	// 状態のリセット
	public void ResetState()
	{
		state = CollapseBlockState.eNormal;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player" || col.tag == "SoftWind" || col.tag == "Gust")
		{
			if (Mathf.Abs(playerSpeed.y) > 2.75f)
			{
				state = CollapseBlockState.eCollapsed;
			}
		}
	}
}
