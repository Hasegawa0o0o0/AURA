/*
 * 20190205 作成
*/
/* よく使う関数群の実装 */
using UnityEngine;
using UnityEngine.UI;

public class CommonBehaviour : MonoBehaviour
{
	float addSwing;	// 揺れを増やす値
	float swing;	// 揺らす値

	public static CommonBehaviour instance;

	void Start ()
	{
		instance = FindObjectOfType<CommonBehaviour>();
		addSwing = -0.0005f;
		swing = 0f;
	}
	
	void Update ()
	{
		swing += addSwing;
		if (Mathf.Abs(swing) > 0.02f) { addSwing *= -1; }
	}

	// 渡されたオブジェクトを呼ばれている間揺らす------------------------------------------------
	public void Swing(GameObject o)
	{
		o.transform.position += new Vector3(0f, swing, 0f);
	}

	// 二つの位置がどれくらい近いか比べる-------------------------------------------------------
	public bool IsNear(Vector3 a, Vector3 b, float value)
	{
		return Mathf.Abs(a.x - b.x) < value && Mathf.Abs(a.y - b.y) < value && Mathf.Abs(a.z - b.z) < value;
	}

	// スプライトのフェードイン----------------------------------------------------------------
	public void FadeIn(SpriteRenderer sr, float value = 0.01f)
	{
		if (value < 0) { return; }
		sr.color += new Color(0, 0, 0, value);
		if (sr.color.a > 1f)
		{
			sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
		}
	}
	public void FadeIn(Image img, float value = 0.01f)
	{
		if (value < 0) { return; }
		img.color += new Color(0, 0, 0, value);
		if (img.color.a > 1f)
		{
			img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
		}
	}
	public void FadeIn(Text txt, float value = 0.01f)
	{
		if (value < 0) { return; }
		txt.color += new Color(0, 0, 0, value);
		if (txt.color.a > 1f)
		{
			txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 1f);
		}
	}

	// スプライトのフェードアウト--------------------------------------------------------------
	public void FadeOut(SpriteRenderer sr, float value = 0.01f)
	{
		if (value < 0) { return; }
		sr.color -= new Color(0, 0, 0, value);
		if (sr.color.a < 0f)
		{
			sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
		}
	}
	public void FadeOut(Image img, float value = 0.01f)
	{
		if (value < 0) { return; }
		img.color -= new Color(0, 0, 0, value);
		if (img.color.a < 0f)
		{
			img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
		}
	}
	public void FadeOut(Text txt, float value = 0.01f)
	{
		if (value < 0) { return; }
		txt.color -= new Color(0, 0, 0, value);
		if (txt.color.a < 0f)
		{
			txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 0f);
		}
	}
}
