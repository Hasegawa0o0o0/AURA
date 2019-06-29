/*
 * 20181007 作成
 *		20190111 場面転換で使うものを追加
 *		20190205 UIを追加
 */
 /* UI管理 */
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public Image windGage;														// 風力ゲージ
	public Image windEmptyGage;													// ゲージの空絵
	public Image windowPlane;													// 画面の白背景
	public List<InformationUI> informationUIList = new List<InformationUI>();	// ステージに出てくるUIのリスト

	void Start ()
	{
		windowPlane.color = Color.clear;
		windGage.enabled = false;
		windEmptyGage.enabled = false;
	}

	void Update ()
	{
	}

	// ステージ上に出てくるUIを引数の位置に追従させる
	public void FollowInformationPosition(Vector3 pos)
	{
		for (int i = 0; i < informationUIList.Count; ++i)
		{
			// プレイヤーが表示範囲内に入っていたら追いかけさせる
			if (informationUIList[i].isHitPlayer)
			{
				informationUIList[i].FollowPosition(pos);
			}
		}
	}

	// クリアしたときの演出
	public void ClearAction()
	{
		windEmptyGage.enabled = false;
		windGage.enabled = false;
	}

	// ミスしたときのアクション
	public void MissAction()
	{
		windowPlane.color += Color.black / 60f;
	}

	// 場面転換用のUIの色を設定する
	public void SetWindowPlaneColor(Color color)
	{
		windowPlane.color = color;
	}

	// 場面転換用のUIの色を加算する
	public void AddWindowPlaneColor(Color color)
	{
		windowPlane.color += color;
	}

	// 場面転換用のUIの色を取得する
	public Color GetWindowPlaneColor()
	{
		return windowPlane.color;
	}

	// 風力ゲージの長さを引数で渡された値にする
	public void AdjustmentWindGage(float amount)
	{
		windGage.fillAmount = amount;
	}

	// 風力ゲージの位置をワールド座標の引数の位置にする
	public void SetWindGagePos(Vector3 pos)
	{
		windEmptyGage.transform.position = windGage.transform.position = pos;
	}

	// 風力ゲージの表示・非表示
	public void SetActiveWindGage(bool isActive)
	{
		windEmptyGage.enabled = isActive;
		windGage.enabled = isActive;
	}
}
