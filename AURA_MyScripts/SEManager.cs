/*
 * 20181028 作成
 *		20190210 管理方法を変更
 */
 /* SEを管理する */
using UnityEngine;
using System.Collections.Generic;

public class SEManager : MonoBehaviour
{
	public enum AudioLoopArray
	{
		BGM = 0,
		SOFTWIND = 1,
		GUST = 2,
		NONE = 3
	}

	public enum AudioOnceArray
	{
		DICISION = 0,
		NONE = 1
	}

	static public SEManager instance;
	List<AudioSource> audioSourceLoopList = new List<AudioSource>();	// 常に流れている音
	AudioSource thisAudioSource;										// SEManagerについているコンポーネント、SEに使用
	public List<AudioClip> audioClipList = new List<AudioClip>();		// 一度だけ流す音

	void Awake()
	{
		instance = FindObjectOfType<SEManager>();
		thisAudioSource = GetComponent<AudioSource>();
		foreach (Transform o in transform)
		{
			AudioSource audio = o.GetComponent<AudioSource>();
			audioSourceLoopList.Add(audio);
			audio.volume = 0f;
		}
	}

	// BGMを流し始める
	public void PlayBGM()
	{
		audioSourceLoopList[(int)AudioLoopArray.BGM].Play();
	}

	// 音量をセットする
	public void SetVolume(AudioLoopArray ala, float value)
	{
		audioSourceLoopList[(int)ala].volume = value;
	}

	// 音をフェードインする
	public void FadeIn(AudioLoopArray ala, float value)
	{
		audioSourceLoopList[(int)ala].volume += value;
		if (audioSourceLoopList[(int)ala].volume > 1f)
		{
			audioSourceLoopList[(int)ala].volume = 1f;
		}
	}

	// 音をフェードアウトする
	public void FadeOut(AudioLoopArray ala, float value)
	{
		audioSourceLoopList[(int)ala].volume -= value;
		if (audioSourceLoopList[(int)ala].volume < 0f)
		{
			audioSourceLoopList[(int)ala].volume = 0f;
		}
	}
	
	// SEを一度再生する
	public void PlayOnce(AudioOnceArray aoa)
	{
		thisAudioSource.PlayOneShot(audioClipList[(int)aoa]);
	}
}
