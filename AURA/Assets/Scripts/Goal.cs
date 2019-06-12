/* 
 * 20181025 作成
 *		20190109 3Dに修正
 */
 /* プレイヤーに当たったら次のシーンに行く */
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
	public bool isGoal = false;

	Material mat;															// マテリアル
	List<TrailRenderer> goalWindEffectsList = new List<TrailRenderer>();	// ゴール用エフェクトリスト
	public List<Vector3> goalWindTrajectoryAxis = new List<Vector3>();		// 軌道

	void Start ()
	{
		isGoal = false;
		mat = GetComponent<Renderer>().material;
		foreach (Transform o in transform)
		{
			goalWindEffectsList.Add(o.GetComponent<TrailRenderer>());
		}
	}
	
	void Update ()
	{
		// エフェクトの回転
		if (goalWindEffectsList.Count != 0 && goalWindEffectsList.Count == goalWindTrajectoryAxis.Count)
		{
			// ゴールを中心に回転する
			for (int i = 0; i < goalWindEffectsList.Count; ++i)
			{
				goalWindEffectsList[i].transform.RotateAround(transform.position,
					goalWindTrajectoryAxis[i], 540f * Time.deltaTime);
			}
		}
	}

	// クリアしたときのアクション
	public void ClearAction()
	{
		// エフェクトを細くしていく
		for (int i = 0; i < goalWindEffectsList.Count; ++i)
		{
			goalWindEffectsList[i].widthMultiplier /= 1.5f;
			if (goalWindEffectsList[i].widthMultiplier < 0.05f)
			{
				goalWindEffectsList[i].widthMultiplier = 0f;
			}
		}
		// マテリアルを薄くしていく
		mat.SetColor("_TintColor", new Color(mat.GetColor("_TintColor").r, mat.GetColor("_TintColor").g,
			mat.GetColor("_TintColor").b, mat.GetColor("_TintColor").a - 1f / 120f));
		if (mat.GetColor("_TintColor").a < 0f) { mat.SetColor("_TintColor", new Color(1f, 1f, 1f, 0f)); }
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player" || col.tag == "SoftWind" || col.tag == "Gust")
		{
			isGoal = true;
		}
	}
}
