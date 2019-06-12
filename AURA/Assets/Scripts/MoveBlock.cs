//動く足場
//作成者鈴木航
//作成日２/１２
using System.Collections.Generic;
using UnityEngine;

public class MoveBlock : MonoBehaviour
{
	public List<Transform> targetPosList = new List<Transform>();	// ターゲットのリスト
	public float speed = 2f;										// スピード
	int targetingPosNum = 0;										// ターゲット要素のカウント

	void Start ()
	{
		targetingPosNum = 0;
		for (int i = 0; i < targetPosList.Count; ++i)
		{
			if (!targetPosList[i])
			{
				targetPosList.RemoveAt(i);
				--i;
			}
		}
	}
	
	void Update ()
	{
	}

	// メイン---------------------------
	public void Action()
	{
		// ターゲットが一つもなかったら処理しない
		if (targetPosList.Count <= 0)
		{
			Debug.Log("targetPosList is not enough");
			return;
		}
		//動く足場とターゲットの位置判定
		if (CommonBehaviour.instance.IsNear(transform.position, targetPosList[targetingPosNum].position, 0.1f))
		{
			targetingPosNum++;
			//カウントがTargetの要素以上になったらカウントを0にする
			if (targetingPosNum >= targetPosList.Count)
			{
				targetingPosNum = 0;
			}
		}
		//動く足場をターゲットに移動させる
		transform.position = Vector2.MoveTowards(transform.position, targetPosList[targetingPosNum].transform.position, speed * Time.deltaTime);
	}
}
