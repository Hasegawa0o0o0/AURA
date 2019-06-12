/*
 * 20190209 作成
 */
 /* シーン移行する動作のインターフェース */
using System.Collections.Generic;
using UnityEngine;
using System;

public class SceneBehaviour : MonoBehaviour
{
	protected int mainFrame;
	protected Camera cameraObject;		// カメラ
	protected UIManager uiManager;		// UI管理
	protected Player playerObject;		// プレイヤーオブジェクト

	protected float startTrackSpeed = 0.05f;	// 導入するときのカメラが動く速さ

	public virtual void Init(CameraScript mainCamera, UIManager mainUI, Player mainPlayer, out string nextSceneName)
	{
		uiManager = mainUI;
		playerObject = mainPlayer;
		cameraObject = mainCamera.GetComponent<Camera>();
		nextSceneName = "";
	}

	// スタートしたときの演出
	public virtual void StartBehaviour(out StageManager.SceneState sceneState)
	{
		sceneState = StageManager.SceneState.ePlay;
	}

	public virtual void ClearBehaviour(out StageManager.SceneState sceneState, Goal goalObject)
	{
		sceneState = StageManager.SceneState.eEndClear;
	}
}
