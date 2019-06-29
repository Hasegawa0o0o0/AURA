/*
 * 20190102 作成
 *		20190103 ギミック追加
 *		20190108 ゴール用エフェクト追加
 *		20190209 プレイ状況のステータスを追加・スタートとゴールの演出を移植
 *		20190212 動くギミック追加
 *		20190221 デバッグ追加
 */
 /* ステージシーンを管理する */
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StageManager : MonoBehaviour
{
	// ゲームのプレイ状況-------------------------------------------
	public enum SceneState
	{
		eStart,		// ステージスタート時
		ePlay,		// プレイ中
		eMiss,		// ミス
		eClear,		// クリア状態
		eEndClear,	// クリア時の演出終了
		eNone
	}
	// 変数宣言-----------------------------------------------------
	Player playerObject;													// プレイヤー
	Goal goalObject;														// ゴール
	UIManager uiManager;													// UI管理
	CameraScript cameraObject;												// カメラ
	public SceneBehaviour sceneBehaviour;									// シーン移行の動作インターフェース
	Reduction reduction;													// データ量削減処理
	int mainFrame = 0;														// フレーム管理
	SceneState sceneState;													// ステージシーンのプレイ状況
	string nextSceneName = "";													// 次のシーンの名前

	bool isMiss = false;													// ミスをしたか
	Vector3 beginPlayerPos;													// プレイヤーの初期ワールド座標
	bool isInitPlayerState = false;											// プレイヤーがミスしたときに初期状態に戻ったか

	public List<BreakedObject> breakedGimmickList = new List<BreakedObject>();	// 突風で壊れるギミック
	public List<InertiaBlock> inertiaGimmickList = new List<InertiaBlock>();	// 慣性で動く足場ギミック
	public List<CollapseBlock> collapseGimmickList = new List<CollapseBlock>();	// 崩れる足場のギミック
	public List<MoveBlock> moveGimmickList = new List<MoveBlock>();				// 動くギミック
	public List<DamageItem> enemyObjectList = new List<DamageItem>();			// エネミー
	public List<RecoverItem> recoverItemList = new List<RecoverItem>();			// 回復アイテム

	void Start ()
	{
		// シーン上の情報を取得
		playerObject = FindObjectOfType<Player>();
		playerObject.Init();
		goalObject = FindObjectOfType<Goal>();
		uiManager = FindObjectOfType<UIManager>();
		cameraObject = FindObjectOfType<CameraScript>();
		reduction = FindObjectOfType<Reduction>();
		// 情報設定
		beginPlayerPos = playerObject.transform.position;
		mainFrame = -1;
		sceneBehaviour.Init(cameraObject, uiManager, playerObject, out nextSceneName);
		// オブジェクトが入っていなかったら要素を削除
		// 壊れるギミック
		for (int i = 0; i < breakedGimmickList.Count; ++i)
		{
			if (!breakedGimmickList[i])
			{
				breakedGimmickList.RemoveAt(i);
				--i;
			}
		}
		// 慣性で動くギミック
		for (int i = 0; i < inertiaGimmickList.Count; ++i)
		{
			if (!inertiaGimmickList[i])
			{
				inertiaGimmickList.RemoveAt(i);
				--i;
			}
		}
		// 崩れる足場のギミック
		for (int i = 0; i < collapseGimmickList.Count; ++i)
		{
			if (!collapseGimmickList[i])
			{
				collapseGimmickList.RemoveAt(i);
				--i;
			}
		}
		// 動く足場のギミック
		for (int i = 0; i < moveGimmickList.Count; ++i)
		{
			if (!moveGimmickList[i])
			{
				moveGimmickList.RemoveAt(i);
				--i;
			}
		}
		// エネミー
		for (int i = 0; i < enemyObjectList.Count; ++i)
		{
			if (!enemyObjectList[i])
			{
				enemyObjectList.RemoveAt(i);
				--i;
			}
		}
		// 回復アイテム
		for (int i = 0; i < recoverItemList.Count; ++i)
		{
			if (!recoverItemList[i])
			{
				recoverItemList.RemoveAt(i);
				--i;
			}
		}
	}

	void Update ()
	{
		// デバッグ------------------------
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SceneManager.LoadScene("Title");
		}
		if (Input.GetKeyDown(KeyCode.F1))
		{
			string nowSceneName = "Title";
			if (nextSceneName == "Stage02") { nowSceneName = "Stage01"; }
			else if (nextSceneName == "Stage03") { nowSceneName = "Stage02"; }
			else if (nextSceneName == "Title") { nowSceneName = "Stage03"; }
			SceneManager.LoadScene(nowSceneName);
		}
		// フレームカウント
		++mainFrame;
		// ブロックをカメラとの距離に応じて表示する
		if (reduction)
		{
			reduction.SetBlockDisplay(cameraObject.gameObject);
		}
		// スタート時のアクション
		if (sceneState == SceneState.eStart)
		{
			sceneBehaviour.StartBehaviour(out sceneState);
			if (sceneState == SceneState.ePlay)
			{
				SEManager.instance.PlayBGM();
			}
			return;
		}
		// カメラがプレイヤーを追いかける
		cameraObject.TrackingMainCamera(new Vector3(playerObject.transform.position.x, playerObject.transform.position.y + 2f, cameraObject.transform.position.z));
		// ゴールをしていたら操作を変え、Actionを処理しない
		if (goalObject.isGoal)
		{
			sceneBehaviour.ClearBehaviour(out sceneState, goalObject);
			if (sceneState == SceneState.eEndClear)
			{
				SceneManager.LoadScene(nextSceneName);
			}
			return;
		}
		// ミスをしていたらブラックアウトさせてから再スタート
		if (isMiss)
		{
			uiManager.windowPlane.color = new Color(0f, 0f, 0f, uiManager.windowPlane.color.a);
			Color addColor = isInitPlayerState ? Color.black / -30f : Color.black / 60f;
			uiManager.AddWindowPlaneColor(addColor);
			if (uiManager.GetWindowPlaneColor().a > 1.5f)
			{
				playerObject.windPower = Player.kWindPowerMax;
				playerObject.windPowerMaxStateFrame = 30;
				playerObject.transform.position = new Vector3(beginPlayerPos.x, beginPlayerPos.y + 8f, beginPlayerPos.z);
				isInitPlayerState = true;
				cameraObject.transform.position = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y + 2f, cameraObject.transform.position.z);
			}
			if (uiManager.GetWindowPlaneColor().a <= 0f && isInitPlayerState)
			{
				isMiss = false;
				isInitPlayerState = false;
			}
			return;
		}
		// BGMのフェードイン
		SEManager.instance.FadeIn(SEManager.AudioLoopArray.BGM, 1f / 60f);
		// プレイヤーのメインアクション
		playerObject.Action();
		// 壊れるギミックの処理
		for (int i = 0; i < breakedGimmickList.Count; ++i)
		{
			breakedGimmickList[i].Action();
		}
		// 慣性で動くギミックの処理
		for (int i = 0; i < inertiaGimmickList.Count; ++i)
		{
			inertiaGimmickList[i].SetSpeed(playerObject.speed);
			inertiaGimmickList[i].Action();
		}
		// 崩れる足場ギミックの処理
		for (int i = 0; i < collapseGimmickList.Count; ++i)
		{
			collapseGimmickList[i].SetPlayerSpeed(playerObject.speed);
			collapseGimmickList[i].Action();
		}
		// 動く足場ギミックの処理
		for (int i = 0; i < moveGimmickList.Count; ++i)
		{
			moveGimmickList[i].Action();
		}
		// エネミーの処理
		for (int i = 0; i < enemyObjectList.Count; ++i)
		{
			enemyObjectList[i].Action(playerObject);
		}
		for (int i = 0; i < recoverItemList.Count; ++i)
		{
			recoverItemList[i].UpdateStatus();
		}
		// 風力ゲージのUIを移動
		uiManager.SetWindGagePos(new Vector3(playerObject.transform.position.x, playerObject.transform.position.y + 0.9f, -0.5f));
		// 風力ゲージの長さを調整
		uiManager.AdjustmentWindGage(playerObject.windPower / (float)Player.kWindPowerMax);
		// 風力ゲージの表示・非表示
		uiManager.SetActiveWindGage(playerObject.windPowerMaxStateFrame < 30);
		// ゴール自体を回転させる
		goalObject.transform.eulerAngles += Vector3.back * 10f;
		// ミスに移行
		isMiss = playerObject.transform.position.y < -41f;
	}
}
