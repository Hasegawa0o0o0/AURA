/*
 * 20181025 作成
 *		20190116 新規追加
 */
 /* シーン切り替え、動き(予定)、タイトルのすべてを管理する */
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
	public SpriteRenderer whitePlane;
	public SpriteRenderer titleLogo;
	public Text press;

	int frameCnt = 0;				// フレーム

	bool isInput = false;			// キーが押されたか

	public bool isPlayModeMovie = false;	// ムービーを再生するかどうか
	public VideoPlayer videoPlayer;			// 動画を操作するためのコンポーネント
	bool isMovie = false;					// 動画が再生中かどうか
	bool isStopMovie = false;				// 動画を途中で止めるかどうか

	void Awake()
	{
		Application.targetFrameRate = 60;
		Screen.SetResolution(1920, 1080, true);
	}

	void Start ()
	{
		isInput = false;
		whitePlane.color = new Color(0f, 0f, 0f, 1f);
		Cursor.visible = false;
	}
	
	void Update ()
	{
		if (!isInput && Input.anyKeyDown && frameCnt > 12)
		{
			if (frameCnt > 280 && isPlayModeMovie)
			{
				frameCnt = 0;
			}
			else
			{
				SEManager.instance.PlayOnce(SEManager.AudioOnceArray.DICISION);
				isInput = true;
				whitePlane.color = new Color(1f, 1f, 1f, 0f);
			}
		}
		if (isMovie && isPlayModeMovie)
		{
			// 動画が終わりに近づくと画面を暗くする
			if ((ulong)videoPlayer.frame >= videoPlayer.frameCount - 10 || isStopMovie)
			{
				CommonBehaviour.instance.FadeIn(whitePlane, 0.1f);
			}
			else if (videoPlayer.frame > 15)
			{
				CommonBehaviour.instance.FadeOut(whitePlane, 0.1f);
			}
			if (Input.anyKeyDown)
			{
				isStopMovie = true;
			}
			if (((ulong)videoPlayer.frame == videoPlayer.frameCount && videoPlayer.frame != 0) || (isStopMovie && whitePlane.color.a >= 1f))
			{
				isMovie = false;
				isStopMovie = false;
				videoPlayer.Stop();
				frameCnt = 0;
			}
		}
		else if (!isInput && isPlayModeMovie)
		{
			if (frameCnt > 280)
			{
				CommonBehaviour.instance.FadeIn(whitePlane, 1f / 60f);
				if (whitePlane.color.a >= 1f)
				{
					isMovie = true;
					videoPlayer.Play();
				}
			}
			else if (frameCnt < 12)
			{
				CommonBehaviour.instance.FadeOut(whitePlane, 0.1f);
			}
		}
		// 入力されていたら画面を白くしてシーン移行
		if (isInput)
		{
			SEManager.instance.FadeOut(SEManager.AudioLoopArray.SOFTWIND, 0.01f);
			CommonBehaviour.instance.FadeOut(titleLogo, 0.5f / 60f);
			CommonBehaviour.instance.FadeOut(press, 1f / 60f);
			whitePlane.color += new Color(0f, 0f, 0f, 0.3f / 60f);
			if (whitePlane.color.a >= 1.25f)
			{
				SceneManager.LoadScene("Stage01");
			}
		}
		// 入力されていなかったら黒い画面を消す
		else if (frameCnt < 280)
		{
			CommonBehaviour.instance.FadeOut(whitePlane, 2f / 60f);
			SEManager.instance.FadeIn(SEManager.AudioLoopArray.SOFTWIND, 0.01f);
		}
		++frameCnt;
	}
}
