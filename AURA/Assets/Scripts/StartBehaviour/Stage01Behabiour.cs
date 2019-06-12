/*
 * 20190209 作成
 */
 /* ステージ１の演出動作 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage01Behabiour : SceneBehaviour
{
	public Text opText;

	const int kCameraMoveFrame = 8 * 60;					// カメラワークのフレーム
	const int kTextShowFrame = kCameraMoveFrame + 3 * 60;	// テキストを表示するフレーム
	const int kCorrectCameraFoV = kTextShowFrame + 1 * 60;	// カメラをゲームプレイ状態に合わせるフレーム

	public override void Init(CameraScript mainCamera, UIManager mainUI, Player mainPlayer, out string nextSceneName)
	{
		base.Init(mainCamera, mainUI, mainPlayer, out nextSceneName);
		nextSceneName = "Stage02";
		mainFrame = 0;
		opText.color = new Color(opText.color.r, opText.color.g, opText.color.b, 0f);
		cameraObject.fieldOfView = 20f;
		cameraObject.transform.position = new Vector3(-138f, 76f, -176f);
		uiManager.windowPlane.color = Color.white;
		SEManager.instance.SetVolume(SEManager.AudioLoopArray.SOFTWIND, 1f);
	}

	// スタートしたときの演出
	public override void StartBehaviour(out StageManager.SceneState sceneState)
	{
		sceneState = StageManager.SceneState.eStart;
		// カメラを所定の位置に動かす
		if (mainFrame <= kCameraMoveFrame)
		{
			SEManager.instance.FadeOut(SEManager.AudioLoopArray.SOFTWIND, 0.2f / 60f);
			CommonBehaviour.instance.FadeOut(uiManager.windowPlane, 0.05f);
			// カメラを動かす
			// 視野角を広げる
			float difference = 50f - cameraObject.fieldOfView;
			cameraObject.fieldOfView += difference / 2.25f * Time.deltaTime;
			if (cameraObject.fieldOfView > 49.8f)
			{
				cameraObject.fieldOfView = 50f;
			}
			// カメラを移動させる
			cameraObject.transform.position = Vector3.Slerp(cameraObject.transform.position, new Vector3(0f, playerObject.transform.position.y + 2f, -12f), startTrackSpeed * Time.deltaTime);
			startTrackSpeed += 0.005f;
			if (startTrackSpeed > 1.5f)
			{
				startTrackSpeed = 1.5f;
			}
			// プレイヤーのほうにむける
			cameraObject.transform.LookAt(new Vector3(playerObject.transform.position.x, playerObject.transform.position.y + 2f, playerObject.transform.position.z));
		}
		// テキストを表示する
		else if (mainFrame <= kTextShowFrame)
		{
			uiManager.windowPlane.color = new Color(0f, 0f, 0f, uiManager.windowPlane.color.a);
			CommonBehaviour.instance.FadeIn(uiManager.windowPlane, 1f / 60f);
			CommonBehaviour.instance.FadeIn(opText, 3f / 60f);
			if (uiManager.windowPlane.color.a > 0.4f)
			{
				uiManager.windowPlane.color = new Color(0f, 0f, 0f, 0.4f);
			}
		}
		// 視野角を戻してゲームスタート
		else if (mainFrame <= kCorrectCameraFoV)
		{
			CommonBehaviour.instance.FadeOut(uiManager.windowPlane, 1f / 60f);
			CommonBehaviour.instance.FadeOut(opText, 5f / 60f);
			cameraObject.fieldOfView += 1f;
			if (cameraObject.fieldOfView > 60f)
			{
				cameraObject.fieldOfView = 60f;
			}
		}
		else
		{
			sceneState = StageManager.SceneState.ePlay;
		}
		++mainFrame;
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
