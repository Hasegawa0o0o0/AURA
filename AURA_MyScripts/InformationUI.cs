/*
 * 20190205 作成
 */
/* 説明用UI処理 */
using UnityEngine;
using System.Collections.Generic;

public class InformationUI : MonoBehaviour
{
	[HideInInspector]
	public bool isHitPlayer = false;										// プレイヤーに当たっているか
	public List<SpriteRenderer> mistImageList = new List<SpriteRenderer>();	// もやもやのSpriteRendererコンポーネントリスト、小さい奴を上から順番に入れていくこと
	SpriteRenderer infoImage;												// 一枚絵のSpriteRendererコンポーネント(child)、親子関係の一番上に置くこと

	void Start ()
	{
		// 情報設定
		infoImage = transform.GetChild(0).GetComponent<SpriteRenderer>();
		infoImage.color = new Color(1f, 1f, 1f, 0f);
		for (int i = 0; i < mistImageList.Count; ++i)
		{
			mistImageList[i].color = new Color(1f, 1f, 1f, 0f);
		}
	}
	
	void Update ()
	{
		// 表示範囲にプレイヤーがいたらフェードインする
		if (isHitPlayer)
		{
			// 順番に表示していく
			for (int i = 0; i < mistImageList.Count; ++i)
			{
				CommonBehaviour.instance.FadeIn(mistImageList[i], 0.05f);
				// 最後の大きい靄を表示していたら絵も表示していく
				if (i == mistImageList.Count - 1)
				{
					CommonBehaviour.instance.FadeIn(infoImage, 0.05f);
				}
				// 完全に表示がされていなかったら処理を抜ける
				if (mistImageList[i].color.a < 1f)
				{
					break;
				}
			}
		}
		// 表示範囲からプレイヤーが離れたらフェードアウトしていく
		else
		{
			for (int i = 0; i < mistImageList.Count; ++i)
			{
				CommonBehaviour.instance.FadeOut(mistImageList[i]);
			}
			CommonBehaviour.instance.FadeOut(infoImage);
		}
	}

	// 引数で渡された位置を追いかける
	public void FollowPosition(Vector3 pos)
	{
		transform.position = Vector3.Lerp(transform.position, pos, 5f * Time.deltaTime);
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player" || col.tag == "SoftWind" || col.tag == "Gust")
		{
			isHitPlayer = true;
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player" || col.tag == "SoftWind" || col.tag == "Gust")
		{
			isHitPlayer = false;
		}
	}
}
