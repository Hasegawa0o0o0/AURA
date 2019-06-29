/* 画面表示、アニメーション関係 */
using UnityEngine;

public class MyAnimation : MonoBehaviour
{
	SpriteRenderer spriteRenderer;
	int drawingSpriteIndex = 0;
	int drawingSpriteFrame = 0;
	int drawingSpriteFrameMax;

	void Start ()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void Update ()
	{
	}

	// 取得したスプライトでアニメーションさせる関数
	// spriteList	アニメーションさせるスプライト
	// indexMax		配列の大きさ
	public void AnimSprite(Sprite[] spriteList, int indexMax, bool isIncrement = true)
	{
		spriteRenderer.enabled = true;
		// 表示しているフレームを更新
		++drawingSpriteFrame;
		// 表示するフレームを超えたらスプライトを変更
		if (drawingSpriteFrame > drawingSpriteFrameMax)
		{
			// 表示しているフレームの初期化
			drawingSpriteFrame = 0;
			// 表示するスプライトを変える
			if (isIncrement)
			{
				++drawingSpriteIndex;
			}
			else
			{
				--drawingSpriteIndex;
			}
			// スプライトの最大値を超えたら0に戻す
			if (drawingSpriteIndex >= indexMax)
			{
				drawingSpriteIndex = 0;
			}
			else if (drawingSpriteIndex < 0)
			{
				drawingSpriteIndex = indexMax - 1;
			}
			spriteRenderer.sprite = spriteList[drawingSpriteIndex];
		}
	}

	// スプライトを非表示にする
	public void OffSprite()
	{
		spriteRenderer.enabled = false;
	}

	// スプライトを表示する
	public void DisplaySprite()
	{
		spriteRenderer.enabled = true;
	}

	// 色を変える
	public void SetColor(Color c)
	{
		spriteRenderer.color = c;
	}

	// 左右反転のセット
	public void SetFlipX(bool flip)
	{
		spriteRenderer.flipX = flip;
	}

	// flipを取得する
	public bool GetFlipX()
	{
		return spriteRenderer.flipX;
	}

	// インデックスを0にする
	public void ResetIndex()
	{
		drawingSpriteIndex = 0;
	}

	// 表示時間を設定
	public void SetFrameMax(int frame)
	{
		drawingSpriteFrameMax = frame;
	}

	// スプライトを設定する
	public void SetSprite(Sprite sprite)
	{
		spriteRenderer.sprite = sprite;
	}
}
