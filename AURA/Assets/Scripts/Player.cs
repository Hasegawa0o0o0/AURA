/*
 20181002 作成
	20181006 基本動作完成
	20181007 火に当たった時の動作作成
	20181008 サーキュレーターに当たった時の動作作成
	20181018 アニメーション追加(待機、歩行)
	20181020 上昇、下降のスプライトの追加
	20181023 仕様変更と変更(風力、そよかぜでの移動、ジャンプのキー)
	20181025 風になった時は本体もTagを変更
	20181029 風のアニメーション追加
	20181214 判定方法変更
	20181215 デバッグ追加
	20181217 ジャンプ追加
	20181223 3D化
	20181224 突風追加
	20181226 ゲージの位置調整、表示・非表示追加
			 エフェクト追加
	20190102 StageManager.csに処理を任せるための変更
	20190103 突風の時のタグや衝突判定を変える
	20190106 blowWind削除
			 areaWind追加
	20190108 地面判定追加
 */
/* プレイヤーの基本動作 */
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
	// プレイヤーの衝突判定情報
	public struct PlayerColList
	{
		public PartiallyCollider footCol;	// 下
		public PartiallyCollider rightCol;	// 右
		public PartiallyCollider leftCol;	// 左
		public PartiallyCollider roofCol;	// 上
	}
	// プレイヤーの状態
	public enum PhysicalState
	{
		SUBSTANCE,	// 通常
		SOFTWIND,	// そよ風
		GUST,		// 突風
		NONE		// なし
	}
	// プレイヤーの追加効果
	public enum AdditionalState
	{
		NONE,			// なし
		ACCELERATION,	// 加速
	}
	public enum Direction
	{
		UP,
		DOWN,
		RIGHT,
		LEFT
	}
	// デバッグ用--------------------------------------------------------------
	bool debugMode = false;
	// 変数宣言----------------------------------------------------------------
	Rigidbody rb;						// Rigidbody2D
	Animator animator;					// アニメーション管理
	PhysicalState physicalState;		// 状態管理
	AdditionalState additionalState;	// プレイヤーへの追加効果
	BoxCollider boxCollider;			// コライダー情報
	GameObject halo;					// Haloコンポーネントの情報オブジェクト
	CapsuleCollider capsuleCollider;	// 突風用コライダー情報
	public LayerMask hitLayer;

	public Vector3 speed = Vector3.zero;								// x、yのスピード
	Vector3 previousSpeed = Vector3.zero;								// 1フレーム前のスピード
	int gustFrameCnt = 0;												// 突風になるまでのフレーム数
	public float walkSpeedMax_x;										// x方向の通常時の最大速度
	public float flySpeedMax;											// x方向の風状態の最大速度
	public float addSpeed_x;											// x方向の加速度
	Vector3 addFlySpeed = Vector3.zero;									// 飛ぶ時の加速度
	float defaultAddSpeed_x;											// x方向の従来の加速度
	float frictionScale_x = 0.5f;										// 摩擦係数
	public float gravityScale_y;										// 重力
	float gravityScaleMax_y = -5.5f;									// 落ちるときの最大速度
	public float jumpPower_y;											// ジャンプの初速度
	int jumpFrame = 0;													// ジャンプしたときのフレーム
	bool isGround = true;												// ジャンプをするための判定
	Transform groundObject;												// 
	public List<Transform> groundColliderPoint = new List<Transform>();	// 地面にヒットさせるためのポイント

	public PlayerColList col;				// 上、下、右、左の衝突判定情報
	bool isHitAreaWind = false;				// ステージ上に吹いている風に当たったか
	float areaWindAngleZ_rad = 0;			// ステージ上に吹いている風の角度
	bool isHitFire = false;					// 火の範囲に当たったか

	public const int kWindPowerMax = 120;
	public int windPower = kWindPowerMax;
	public int windPowerMaxStateFrame = 0;								// ゲージが最大の時のフレーム数
	public List<TrailRenderer> windEffects = new List<TrailRenderer>();	// 風のエフェクト
	float effectsSpeed = 0f;											// エフェクトを動かすスピード
	public List<Vector3> windTrajectoryAxis = new List<Vector3>();		// 風のエフェクトを動かす軌道
	//----------------------------------------------------------------------------------------
	void Start()
	{
		// アタッチされた情報を取得
		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		boxCollider = GetComponent<BoxCollider>();
		halo = transform.Find("Halo").gameObject;
		capsuleCollider = GetComponent<CapsuleCollider>();
		col.footCol = transform.Find("FootCol").GetComponent<PartiallyCollider>();
		col.rightCol = transform.Find("RightCol").GetComponent<PartiallyCollider>();
		col.leftCol = transform.Find("LeftCol").GetComponent<PartiallyCollider>();
		col.roofCol = transform.Find("RoofCol").GetComponent<PartiallyCollider>();
		// 情報の設定
		physicalState = PhysicalState.SUBSTANCE;
		additionalState = AdditionalState.NONE;
		defaultAddSpeed_x = addSpeed_x;
		addFlySpeed.x = addSpeed_x / 4f;
		addFlySpeed.y = addSpeed_x / 4.5f;
		InitColState();
		halo.SetActive(false);
		SetActiveEffects(false);
		windPowerMaxStateFrame = 30;
		effectsSpeed = 360f;
	}
	public void Init()
	{
		// アタッチされた情報を取得
		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		boxCollider = GetComponent<BoxCollider>();
		halo = transform.Find("Halo").gameObject;
		capsuleCollider = GetComponent<CapsuleCollider>();
		col.footCol = transform.Find("FootCol").GetComponent<PartiallyCollider>();
		col.rightCol = transform.Find("RightCol").GetComponent<PartiallyCollider>();
		col.leftCol = transform.Find("LeftCol").GetComponent<PartiallyCollider>();
		col.roofCol = transform.Find("RoofCol").GetComponent<PartiallyCollider>();
		// 情報の設定
		physicalState = PhysicalState.SUBSTANCE;
		additionalState = AdditionalState.NONE;
		defaultAddSpeed_x = addSpeed_x;
		addFlySpeed.x = addSpeed_x / 4f;
		addFlySpeed.y = addSpeed_x / 4.5f;
		InitColState();
		halo.SetActive(false);
		SetActiveEffects(false);
		windPowerMaxStateFrame = 30;
		effectsSpeed = 360f;
	}
	//--------------------------------------------------------------------------------------------

	void LateUpdate()
	{
		// 位置補正
		CorrectHit(col.footCol, Direction.UP);
		CorrectHit(col.rightCol, Direction.LEFT);
		CorrectHit(col.leftCol, Direction.RIGHT);
	}

	// プレイヤーのデバッグ用のアクション-------------------------------------------------------------
	public void ActionDebug()
	{
		// デバッグ用-------------------------------------------------------------
		if (debugMode)
		{
			if (Input.GetKeyDown(KeyCode.Backspace))
			{
				debugMode = false;
			}
			windPower = kWindPowerMax;
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.Backspace))
			{
				debugMode = true;
			}
		}
		if (Input.GetKey(KeyCode.Joystick1Button4) || Input.GetKey(KeyCode.V))
		{
			flySpeedMax = 21f;
		}
		else
		{
			flySpeedMax = 7f;
		}
		//------------------------------------------------------------------
	}

	// プレイヤーのステージでのアクション-------------------------------------------------------------
	public void Action()
	{
		// 落ちているかのアニメーション設定
		animator.SetBool("Falling", !isGround);
		// 左右反転
		if (speed.x != 0f)
		{
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90f * Mathf.Sign(speed.x), transform.eulerAngles.z);
			for (int i = 0; i < windEffects.Count; ++i)
			{
				windEffects[i].transform.eulerAngles = new Vector3(Mathf.Abs(windEffects[i].transform.eulerAngles.x) * Mathf.Sign(speed.x),
					Mathf.Abs(windEffects[i].transform.eulerAngles.y) * Mathf.Sign(speed.x),
					Mathf.Abs(windEffects[i].transform.eulerAngles.z) * Mathf.Sign(speed.x));
			}
			col.rightCol.transform.localPosition = new Vector3(col.rightCol.transform.localPosition.x,
				col.rightCol.transform.localPosition.y,
				Mathf.Abs(col.rightCol.transform.localPosition.z) * Mathf.Sign(speed.x));
			col.rightCol.transform.localScale = new Vector3(1f, 1f * -Mathf.Sign(speed.x), 1f);
			col.leftCol.transform.localPosition = new Vector3(col.leftCol.transform.localPosition.x,
				col.leftCol.transform.localPosition.y,
				Mathf.Abs(col.leftCol.transform.localPosition.z) * Mathf.Sign(speed.x) * -1);
			col.leftCol.transform.localScale = new Vector3(1f, 1f * -Mathf.Sign(speed.x), 1f);
		}
		// 状態変化
		if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.JoystickButton2)) && windPower != 0)
		{
			physicalState = PhysicalState.SOFTWIND;
			SetAllTag("SoftWind");
			SetActiveEffects(true);
			animator.SetBool("Run", false);
			animator.SetBool("Wait", false);
			animator.SetBool("Falling", false);
			animator.SetBool("Flying", GetSpeed() > 1f);
			animator.SetBool("Hovering", GetSpeed() <= 1f);
		}
		// ジャンプができるか判定
		CheckIsGround();
		// 入力を含めたスピード更新
		UpdateSpeed();
		// 足場を自分の親にする
		if (isGround && groundObject)
		{
			transform.parent = groundObject;
		}
		else
		{
			transform.parent = null;
		}
		// そよ風状態の処理
		if (physicalState == PhysicalState.SOFTWIND || physicalState == PhysicalState.GUST)
		{
			// 音のフェードイン
			if (physicalState == PhysicalState.SOFTWIND)
			{
				SEManager.instance.FadeIn(SEManager.AudioLoopArray.SOFTWIND, 0.2f);
			}
			else if (physicalState == PhysicalState.GUST)
			{
				SEManager.instance.FadeIn(SEManager.AudioLoopArray.GUST, 0.2f);
			}
			--windPower;
			if (additionalState == AdditionalState.ACCELERATION) { --windPower; }
			animator.SetBool("Falling", false);
			animator.SetBool("Flying", GetSpeed() > 1f);
			animator.SetBool("Hovering", GetSpeed() <= 1f);
			// 風力が無くなったる、またはキーが離されたらもとに戻す
			if (Input.GetKeyUp(KeyCode.X) || Input.GetKeyUp(KeyCode.Joystick1Button1) || Input.GetKeyUp(KeyCode.JoystickButton2) || windPower <= 0)
			{
				physicalState = PhysicalState.SUBSTANCE;
				gravityScale_y = 0.1f;
				addSpeed_x = defaultAddSpeed_x;
				SetActiveEffects(false);
				InitColState();
				animator.SetBool("Flying", false);
				animator.SetBool("Hovering", false);
				animator.SetBool("Run", speed.x != 0f && isGround);
			}
		}
		// 通常の処理
		else
		{
			// 音をフェードアウト
			SEManager.instance.FadeOut(SEManager.AudioLoopArray.SOFTWIND, 4f / 60f);
			SEManager.instance.FadeOut(SEManager.AudioLoopArray.GUST, 4f / 60f);
			// スピードで走るアニメーションを管理
			if (speed.x == 0f || !isGround)
			{
				animator.SetBool("Run", false);
			}
			else
			{
				animator.SetBool("Run", true);
			}
			animator.SetBool("Wait", speed == Vector3.zero);
			animator.SetBool("Jump", speed.y > 0f);
			// Haloをなくす
			halo.SetActive(false);
		}
		// 風力管理
		ManagementWindPower();
		// 火にあたった時の処理
		FireAction();
		// ステージに吹いている風に当たった時の処理
		AreaWindAction();
		rb.velocity = speed;
		--jumpFrame;
		// ゲージの表示・非表示
		if (windPower == kWindPowerMax) { ++windPowerMaxStateFrame; }
		else { windPowerMaxStateFrame = 0; }
		// エフェクトを動かす
		MoveEffect(effectsSpeed);
	}

	// 1フレーム前のスピードをチェックして近かったら突風の状態にする-------------------------------------------------------
	void CheckPreviousSpeed()
	{
		if (speed == Vector3.zero) { return; }
		// スピードが最高値の95%以上であればカウント
		if (GetSpeed() >= flySpeedMax * 0.95f)
		{
			if (Mathf.Abs(speed.y - previousSpeed.y) < addFlySpeed.y / 2f && Mathf.Abs(speed.x - previousSpeed.x) < addFlySpeed.x / 2f)
			{
				++gustFrameCnt;
			}
		}
		else
		{
			gustFrameCnt = 0;
			physicalState = PhysicalState.SOFTWIND;
			halo.SetActive(false);
			capsuleCollider.enabled = false;
			SetAllTag("SoftWind");
		}
		// 15フレームを超えたら突風にする
		if (gustFrameCnt > 15)
		{
			physicalState = PhysicalState.GUST;
			gustFrameCnt = 0;
			halo.SetActive(true);
			capsuleCollider.enabled = true;
			SetAllTag("Gust");
		}
		previousSpeed = speed;
	}

	// ジャンプができる位置か判定してその結果をisGroundに代入-----------------------------------------------------------------------
	void CheckIsGround()
	{
		isGround = false;
		RaycastHit hit = new RaycastHit();
		for (int i = 0; i + 1 < groundColliderPoint.Count && !isGround; ++i)
		{
			isGround = Physics.Linecast(groundColliderPoint[i].position, groundColliderPoint[i + 1].position, out hit, hitLayer);
			if (isGround && (hit.transform.tag == "IntertiaBlock" || hit.transform.tag == "MoveBlock"))
			{
				groundObject = hit.transform;
				break;
			}
			else
			{
				groundObject = null;
			}
		}
	}

	// スピード更新--------------------------------------------------------------------------------------------------------
	void UpdateSpeed()
	{
		// 通常のスピード更新
		if (physicalState == PhysicalState.SUBSTANCE)
		{
			SubstanceUpdateSpeed(walkSpeedMax_x, addSpeed_x, frictionScale_x, gravityScale_y);
		}
		// そよかぜのスピード更新
		else if (physicalState == PhysicalState.SOFTWIND || physicalState == PhysicalState.GUST)
		{
			float acceleration = 1f;
			if (additionalState == AdditionalState.ACCELERATION) { acceleration = 2f; }
			SubstanceUpdateSpeed(flySpeedMax, addFlySpeed.x * acceleration, 0.0f, gravityScale_y / 5.0f);
			WindUpdateSpeed_y(flySpeedMax, addFlySpeed.y * acceleration, 0.0f);
			// チェックして突風にする
			CheckPreviousSpeed();
		}
	}

	// 風状態の時のプレイヤーの縦の速度更新----------------------------------------------------------------
	void WindUpdateSpeed_y(float flySpeedMax_y_, float addSpeed_y_, float frictionScale_y_)
	{
		float input_y = Input.GetAxisRaw("Vertical");
		// 止まるやつに当たっていたら止めておく
		if (col.footCol.isHitStopObj || col.roofCol.isHitStopObj)
		{
			speed.y = 0.0f;
		}
		speed.y += addSpeed_y_ * input_y;
		float sign = Mathf.Sign(speed.y);
		float value = Mathf.Abs(speed.y);
		// 入力あり
		if (input_y != 0f)
		{
			// 上
			if (input_y > 0.0f)
			{
				if (sign < 0.0f)
				{
					value -= addSpeed_y_ * 1.5f;
				}
				// 止まるやつに当たると跳ね返す
				if (col.roofCol.isHitStopObj)
				{
					value = -0.01f;
				}
			}
			// 下
			else if (input_y < 0.0f)
			{
				// 止まるやつに当たると止める
				if (col.footCol.isHitStopObj)
				{
					value = 0.0f;
				}
			}
			// 最大速度を制御する
			if (value > flySpeedMax_y_ * Mathf.Abs(input_y * 1.005f))
			{
				value -= addSpeed_y_ * Mathf.Abs(input_y);
				if (value < flySpeedMax_y_ * Mathf.Abs(input_y * 1.005f))
				{
					value = flySpeedMax_y_ * Mathf.Abs(input_y);
				}
			}
		}
		// 入力なし
		else
		{
			value -= frictionScale_y_;
			if (value < 0f) { value = 0; }
			// ブロックに当たると止める
			if (col.footCol.isHitStopObj)
			{
				value = 0.0f;
			}
			// 跳ね返す
			if (col.roofCol.isHitStopObj)
			{
				value = -0.01f;
			}
		}
		speed.y = value * sign;
	}

	// 通常状態のプレイヤーの速度更新----------------------------------------------------------------------
	void SubstanceUpdateSpeed(float walkSpeedMax_x_, float addSpeed_x_, float frictionScale_x_, float gravityScale_y_)
	{
		// 左右操作
		float input_x = Input.GetAxisRaw("Horizontal");
		// 止まるやつに立っていないと慣性
		if (physicalState == PhysicalState.SUBSTANCE && !col.footCol.isHitStopObj)
		{
			addSpeed_x_ /= 5.0f;
			frictionScale_x_ = 0.0f;
		}
		// 止まるやつに当たっていたら止めておく
		if (col.leftCol.isHitStopObj || col.rightCol.isHitStopObj)
		{
			speed.x = 0.0f;
		}
		speed.x += addSpeed_x_ * input_x;
		float sign = Mathf.Sign(speed.x);
		float value = Mathf.Abs(speed.x);
		// 入力あり
		if (input_x != 0f)
		{
			// 右
			if (input_x > 0.0f)
			{
				// 止まるやつに当たると止める
				if (col.rightCol.isHitStopObj)
				{
					value = 0.0f;
				}
			}
			// 左
			else if (input_x < 0.0f)
			{
				// 止まるやつに当たると止める
				if (col.leftCol.isHitStopObj)
				{
					value = 0.0f;
				}
			}
			// 最大速度を制御する
			if (value > walkSpeedMax_x_ * Mathf.Abs(input_x * 1.005f))
			{
				value -= physicalState == PhysicalState.SUBSTANCE ? addSpeed_x_ * 2f : addSpeed_x_ * Mathf.Abs(input_x);
				if (value < walkSpeedMax_x_ * Mathf.Abs(input_x * 1.005f))
				{
					value = walkSpeedMax_x_ * Mathf.Abs(input_x);
				}
			}
		}
		// 入力なし
		else
		{
			value -= frictionScale_x_;
			if (value < 0f) { value = 0f; }
			if (value > walkSpeedMax_x_) { value = walkSpeedMax_x_; }
			// ブロックに当たると止める
			if (col.rightCol.isHitStopObj || col.leftCol.isHitStopObj)
			{
				value = 0.0f;
			}
		}
		speed.x = value * sign;
		// 自由落下
		speed.y -= gravityScale_y_;
		if (speed.y < gravityScaleMax_y)
		{
			speed.y = gravityScaleMax_y;
		}
		// 止まるやつに当たると止める
		if (col.footCol.isHitStopObj && jumpFrame <= 0)
		{
			speed.y = 0.0f;
		}
		// 天井に当たると少し跳ね返す
		if (col.roofCol.isHitBlock && speed.y > 0f)
		{
			speed.y = -0.1f;
		}
		// ジャンプ
		if (physicalState == PhysicalState.SUBSTANCE && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.JoystickButton3))
			&& isGround)
		{
			speed.y = jumpPower_y;
			jumpFrame = 3;
			col.footCol.SetStopObj(false);
			animator.SetBool("Jump", true);
		}
	}

	// めり込んだときの位置補正----------------------------------------------------------
	void CorrectHit(PartiallyCollider hitCol, Direction dir)
	{
		if (!hitCol.isHitStopObj) { return; }
		Vector3 correctionValue = Vector3.zero;
		if (dir == Direction.UP)
		{
			// 当たらなくなる位置まで上にずらす
			int j = 1;
			for (int i = 0; i + 1 < hitCol.point.Count; ++i)
			{
				for (; Physics.Linecast(hitCol.point[i].position + new Vector3(0f, 0.01f, 0f) * j,
					hitCol.point[i + 1].position + new Vector3(0f, 0.01f, 0f) * j, hitLayer) && j < 15;
						correctionValue += new Vector3(0f, 0.01f, 0f), ++j) ;
			}
		}
		else if (dir == Direction.DOWN)
		{
			// 当たらなくなる位置まで下にずらす
			int j = 1;
			for (int i = 0; i + 1 < hitCol.point.Count; ++i)
			{
				for (; Physics.Linecast(hitCol.point[i].position - new Vector3(0f, 0.01f, 0f) * j,
					hitCol.point[i + 1].position - new Vector3(0f, 0.01f, 0f) * j, hitLayer) && j < 15;
						correctionValue -= new Vector3(0f, 0.01f, 0f), ++j) ;
			}
		}
		else if (dir == Direction.RIGHT)
		{
			// 当たらなくなる位置まで右にずらす
			int j = 1;
			for (int i = 0; i + 1 < hitCol.point.Count; ++i)
			{
				for (; Physics.Linecast(hitCol.point[i].position + new Vector3(0.01f, 0f, 0f) * j,
					hitCol.point[i + 1].position + new Vector3(0.01f, 0f, 0f) * j, hitLayer) && j < 15;
						correctionValue += new Vector3(0.01f, 0f, 0f), ++j) ;
			}
		}
		else if (dir == Direction.LEFT)
		{
			// 当たらなくなる位置まで左にずらす
			int j = 1;
			for (int i = 0; i + 1 < hitCol.point.Count; ++i)
			{
				for (; Physics.Linecast(hitCol.point[i].position - new Vector3(0.01f, 0f, 0f) * j,
					hitCol.point[i + 1].position - new Vector3(0.01f, 0f, 0f) * j, hitLayer) && j < 15;
						correctionValue -= new Vector3(0.01f, 0f, 0f), ++j) ;
			}
		}
		rb.position += correctionValue;
	}

	// プレイヤーの衝突判定を初期状態に戻す-----------------------------------------------------------------
	void InitColState()
	{
		boxCollider.size = new Vector3(0.01f, 1.85f, 0.4f);
		capsuleCollider.enabled = false;
		col.footCol.transform.localScale = Vector3.one;
		col.footCol.transform.localPosition = Vector3.zero;
		col.rightCol.transform.localScale = new Vector3(1f, -1f, 1f);
		col.rightCol.transform.localPosition = new Vector3(0f, 1f, 0.4f);
		col.leftCol.transform.localScale = new Vector3(1f, -1f, 1f);
		col.leftCol.transform.localPosition = new Vector3(0f, 1f, -0.4f);
		col.roofCol.transform.localScale = Vector3.one;
		col.roofCol.transform.localPosition = new Vector3(0f, 2f, 0f);
		SetAllTag("Player");
	}

	// 衝突判定と本体のタグを設定する-------------------------------------------------------------------------
	void SetAllTag(string tag)
	{
		col.footCol.tag = col.rightCol.tag = col.leftCol.tag = col.roofCol.tag = gameObject.tag = tag;
	}

	// 風力管理--------------------------------------------------------------------------------------------
	void ManagementWindPower()
	{
		if (physicalState == PhysicalState.SUBSTANCE && windPower < kWindPowerMax && isGround)
		{
			windPower += 2;
			if (windPower > kWindPowerMax)
			{
				windPower = kWindPowerMax;
			}
		}
	}

	// エフェクトを動かす-----------------------------------------------------------------------------------
	public void MoveEffect(float speed)
	{
		if (windEffects.Count == 0 || windEffects.Count != windTrajectoryAxis.Count) { return; }
		// 自分を中心に回転する https://gist.github.com/hiroyukihonda/8552618
		Vector3 center = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
		for (int i = 0; i < windEffects.Count; ++i)
		{
			Vector3 trjAxis = windTrajectoryAxis[i];
			trjAxis.y *= Mathf.Sign(transform.eulerAngles.y);
			windEffects[i].transform.RotateAround(center, trjAxis, speed * Time.deltaTime);
		}
	}

	// エフェクトのオンオフを設定する------------------------------------------------------------------------
	public void SetActiveEffects(bool isActive)
	{
		if (windEffects.Count == 0) { return; }
		for (int i = 0; i < windEffects.Count; ++i)
		{
			windEffects[i].enabled = isActive;
		}
	}

	// スピードの大きさを求める
	float GetSpeed()
	{
		return Mathf.Sqrt(Mathf.Pow(speed.x, 2) + Mathf.Pow(speed.y, 2));
	}

	// 火に当たった時のアクション(判定も含む)-----------------------------------------------------------------
	void FireAction()
	{
		if (!isHitFire) { return; }
		if (physicalState == PhysicalState.SOFTWIND)
		{
			float inputY = Input.GetAxis("Vertical");
			gravityScale_y -= 0.8f;
			if (gravityScale_y < 0.08f)
			{
				gravityScale_y = 0.08f;
			}
			if (inputY > 0f)
			{
				speed.y += addSpeed_x / 3f;
				if (speed.y > flySpeedMax)
				{
					speed.y = flySpeedMax;
				}
			}
			if (inputY < 0f)
			{
				speed.y -= addSpeed_x / 4.5f;
				if (speed.y < -2.0f)
				{
					speed.y = -2.0f;
				}
			}
			else if (speed.y < -1.0f)
			{
				speed.y = -1.0f;
			}
		}
	}

	// ステージに吹いている風に当たった時の処理(判定を含む)----------------------------------------------------
	void AreaWindAction()
	{
		if (isHitAreaWind && (physicalState == PhysicalState.GUST || physicalState == PhysicalState.SOFTWIND))
		{
			speed.x += addFlySpeed.y * Mathf.Cos(areaWindAngleZ_rad) * 2f;
			speed.y += addFlySpeed.y * Mathf.Sin(areaWindAngleZ_rad) * 2f;
		}
	}

	// 外部からアニメーションの設定をできるようにする----------------------------------------------------------
	public void SetAnimatorBool(string animName, bool set)
	{
		animator.SetBool(animName, set);
	}

	// クリアしたときのアクション----------------------------------------------------------------------------
	public void ClearAction()
	{
		// アニメーションを飛んでいる状態にする
		animator.SetBool("Flying", true);
		// 向きを速さに合わせる
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90f * Mathf.Sign(speed.x), transform.eulerAngles.z);
		// エフェクトを動かす
		MoveEffect(effectsSpeed);
		effectsSpeed += 4f;
		// エフェクトの表示
		halo.SetActive(true);
		for (int i = 0; i < windEffects.Count; ++i)
		{
			windEffects[i].enabled = true;
			Gradient grad = windEffects[i].colorGradient;
			// エフェクトの色を変えていく
			GradientColorKey[] gradColor = new GradientColorKey[2];
			GradientAlphaKey[] gradAlpha = new GradientAlphaKey[2];
			gradColor = grad.colorKeys;
			gradAlpha = grad.alphaKeys;
			if (gradColor[0].color.g < 1f)
			{
				gradColor[0].color += Color.green / 60f;
			}
			if (gradColor[0].time < 0.2f)
			{
				gradColor[0].time += 0.2f / 60f;
			}
			if (gradColor[1].color.r < 1f)
			{
				gradColor[1].color += Color.white / 60f;
			}
			if (gradAlpha[0].alpha > 0.5f)
			{
				gradAlpha[0].alpha -= 0.1f;
			}
			grad.SetKeys(gradColor, gradAlpha);
			windEffects[i].colorGradient = grad;
			// エフェクトを太くしていく
			if (windEffects[i].widthMultiplier < 1f)
			{
				windEffects[i].widthMultiplier += 1f / 120f;
			}
			else
			{
				windEffects[i].widthMultiplier = 1f;
			}
		}
		// エフェクトが変わりきっていなかったらゆっくり落ちる状態にする
		if (windEffects[0].endColor.r < 1f)
		{
			if (Mathf.Abs(speed.x) > 1.5f)
			{
				float remed = Mathf.Abs(speed.x);
				float sign = Mathf.Sign(speed.x);
				remed -= addFlySpeed.x * 1.5f;
				if (remed < 1.5f) { remed = 1.5f; }
				speed.x = remed * sign;
			}
			if (speed.y > -gravityScaleMax_y / 7.5f)
			{
				speed.y -= speed.y < 0f ? addFlySpeed.y * 0.75f : addFlySpeed.y * 1.5f;
			}
			else
			{
				speed.y += addFlySpeed.y;
				if (speed.y > -gravityScaleMax_y / 7.5f) { speed.y = -gravityScaleMax_y / 7.5f; }
			}
		}
		// エフェクトが変わりきったら右に移動させる
		else
		{
			speed.x += speed.x < flySpeedMax * 0.75 ? addFlySpeed.x * 0.75f : -addFlySpeed.x * 0.2f;
			speed.y += speed.y > flySpeedMax * 0.2f ? addFlySpeed.y * 0.6f : addFlySpeed.y * 0.3f;
			if (speed.x > flySpeedMax * 1.25f)
			{
				speed.x = flySpeedMax * 1.25f;
			}
			if (speed.y > flySpeedMax * 0.85f)
			{
				speed.y = flySpeedMax * 0.85f;
			}
		}
		rb.velocity = speed;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "AreaWind")
		{
			isHitAreaWind = true;
			areaWindAngleZ_rad = col.transform.eulerAngles.z * Mathf.Deg2Rad;
		}
		if (col.tag == "Fire")
		{
			isHitFire = true;
		}
		if (col.tag == "Recover")
		{
			windPower = kWindPowerMax;
		}
	}

	void OnTriggerStay(Collider col)
	{
		if (col.tag == "Damage" && transform.tag != "Gust")
		{
			windPower = 0;
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.tag == "AreaWind")
		{
			isHitAreaWind = false;
			areaWindAngleZ_rad = 0f;
		}
		if (col.tag == "Fire")
		{
			isHitFire = false;
		}
	}
}
