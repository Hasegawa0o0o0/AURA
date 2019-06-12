/*
 * 20190209 作成
 */
 /* ステージ２の演出動作 */
using System.Collections.Generic;
using UnityEngine;

public class Stage02Behaviour : SceneBehaviour
{
	// 変数宣言------------------------------------------
	const int kPlayerFlyFrame = 230;
	const int kPlayerFallFrame = kPlayerFlyFrame + 108;
	const int kPlayerSetFrame = kPlayerFallFrame + 120;
	Vector3 playerSpeed = Vector3.zero;

	// 初期化
	public override void Init(CameraScript mainCamera, UIManager mainUI, Player mainPlayer, out string nextSceneName)
	{
		base.Init(mainCamera, mainUI, mainPlayer, out nextSceneName);
		nextSceneName = "Stage03";
		mainFrame = 0;
		playerObject.transform.position = new Vector3(-11.98f, 0f, playerObject.transform.position.z);
		playerObject.SetActiveEffects(true);
		playerSpeed = new Vector3(0f, 1.5f, 3f);
		playerObject.SetAnimatorBool("Flying", true);
		uiManager.windowPlane.color = Color.white;
		SEManager.instance.SetVolume(SEManager.AudioLoopArray.SOFTWIND, 0.5f);
		cameraObject.transform.position = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, -12f);
	}

	// スタート時の演出
	public override void StartBehaviour(out StageManager.SceneState sceneState)
	{
		cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position,
			new Vector3(playerObject.transform.position.x, playerObject.transform.position.y + 2f,
			cameraObject.transform.position.z), 4f * Time.deltaTime);
		sceneState = StageManager.SceneState.eStart;
		CommonBehaviour.instance.FadeOut(uiManager.windowPlane, 0.25f / 60f);
		// プレイヤーを飛行させる
		if (mainFrame < kPlayerFlyFrame)
		{
			playerObject.SetActiveEffects(true);
			SEManager.instance.FadeOut(SEManager.AudioLoopArray.SOFTWIND, 0.2f / 60f);
			playerSpeed -= new Vector3(0f, 0.4f / 60f, 0.3f / 60f);
			if (playerSpeed.z < 0.5f)
			{
				playerSpeed.z = 0.5f;
			}
			if (playerSpeed.y < -0.1f)
			{
				playerSpeed.y = -0.1f;
			}
			if (mainFrame == kPlayerFlyFrame - 1)
			{
				playerObject.SetAnimatorBool("Falling", true);
			}
		}
		// プレイヤーを落とす
		else if (mainFrame < kPlayerFallFrame)
		{
			playerObject.SetActiveEffects(false);
			playerSpeed.y -= 2f / 60f;
			if (playerSpeed.y < -5.5f)
			{
				playerSpeed.y = -5.5f;
			}
			playerObject.SetAnimatorBool("Flying", false);
			if (mainFrame == kPlayerFallFrame - 1)
			{
				playerObject.SetAnimatorBool("Run", true);
			}
		}
		// プレイヤーを止める
		else if (mainFrame < kPlayerSetFrame)
		{
			playerSpeed.y = 0f;
			playerSpeed.z -= 5f / 60f;
			if (playerSpeed.z < 0f)
			{
				playerSpeed.z = 0;
				playerObject.SetAnimatorBool("Run", false);
				playerObject.SetAnimatorBool("Wait", true);
			}
			playerObject.SetAnimatorBool("Falling", false);
		}
		else
		{
			sceneState = StageManager.SceneState.ePlay;
		}
		++mainFrame;
		playerObject.transform.Translate(playerSpeed * Time.deltaTime);
		playerObject.MoveEffect(60f);
	}

	// クリアしたときの演出
	public override void ClearBehaviour(out StageManager.SceneState sceneState, Goal goalObject)
	{
		sceneState = StageManager.SceneState.eClear;
		uiManager.windowPlane.color = new Color(1f, 1f, 1f, uiManager.windowPlane.color.a);
		uiManager.ClearAction();
		goalObject.ClearAction();
		playerObject.ClearAction();
		if (uiManager.windowPlane.color.a < 1.5f)
		{
			// 白画面のフェードインをα値によって変える
			uiManager.windowPlane.color += uiManager.windowPlane.color.a < 1f / 2f ? Color.white / 240f : Color.white / 180f;
		}
		else
		{
			sceneState = StageManager.SceneState.eEndClear;
		}
	}
}
