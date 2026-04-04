using UnityEngine;

[AddComponentMenu("NGUI/UI/NGUI Texture")]
[ExecuteInEditMode]
public class UITexture : UIWidget
{
	public enum Flip
	{
		Nothing = 0,
		Horizontally = 1,
		Vertically = 2,
		Both = 3
	}

	[HideInInspector]
	[SerializeField]
	private Rect mRect = new Rect(0f, 0f, 1f, 1f);

	[HideInInspector]
	[SerializeField]
	private Texture mTexture;

	[SerializeField]
	[HideInInspector]
	private Material mMat;

	[SerializeField]
	[HideInInspector]
	private Shader mShader;

	[HideInInspector]
	[SerializeField]
	private Flip mFlip;

	private int mPMA = -1;

	public override Texture mainTexture
	{
		get
		{
			return mTexture;
		}
		set
		{
			if (mTexture != value)
			{
				RemoveFromPanel();
				mTexture = value;
				MarkAsChanged();
			}
		}
	}

	public override Material material
	{
		get
		{
			return mMat;
		}
		set
		{
			if (mMat != value)
			{
				RemoveFromPanel();
				mShader = null;
				mMat = value;
				mPMA = -1;
				MarkAsChanged();
			}
		}
	}

	public override Shader shader
	{
		get
		{
			if (mMat != null)
			{
				return mMat.shader;
			}
			if (mShader == null)
			{
				mShader = Shader.Find("Unlit/Transparent Colored");
			}
			return mShader;
		}
		set
		{
			if (mShader != value)
			{
				RemoveFromPanel();
				mShader = value;
				mPMA = -1;
				mMat = null;
				MarkAsChanged();
			}
		}
	}

	public Flip flip
	{
		get
		{
			return mFlip;
		}
		set
		{
			if (mFlip != value)
			{
				mFlip = value;
				MarkAsChanged();
			}
		}
	}

	public bool premultipliedAlpha
	{
		get
		{
			if (mPMA == -1)
			{
				Material material = this.material;
				mPMA = ((material != null && material.shader != null && material.shader.name.Contains("Premultiplied")) ? 1 : 0);
			}
			return mPMA == 1;
		}
	}

	public Rect uvRect
	{
		get
		{
			return mRect;
		}
		set
		{
			if (mRect != value)
			{
				mRect = value;
				MarkAsChanged();
			}
		}
	}

	public override Vector4 drawingDimensions
	{
		get
		{
			Vector2 vector = base.pivotOffset;
			float num = (0f - vector.x) * (float)mWidth;
			float num2 = (0f - vector.y) * (float)mHeight;
			float num3 = num + (float)mWidth;
			float num4 = num2 + (float)mHeight;
			Texture texture = mainTexture;
			int num5 = ((!(texture != null)) ? mWidth : texture.width);
			int num6 = ((!(texture != null)) ? mHeight : texture.height);
			if ((num5 & 1) != 0)
			{
				num3 -= 1f / (float)num5 * (float)mWidth;
			}
			if ((num6 & 1) != 0)
			{
				num4 -= 1f / (float)num6 * (float)mHeight;
			}
			return new Vector4((mDrawRegion.x != 0f) ? Mathf.Lerp(num, num3, mDrawRegion.x) : num, (mDrawRegion.y != 0f) ? Mathf.Lerp(num2, num4, mDrawRegion.y) : num2, (mDrawRegion.z != 1f) ? Mathf.Lerp(num, num3, mDrawRegion.z) : num3, (mDrawRegion.w != 1f) ? Mathf.Lerp(num2, num4, mDrawRegion.w) : num4);
		}
	}

	public override void MakePixelPerfect()
	{
		Texture texture = mainTexture;
		if (texture != null)
		{
			int num = texture.width;
			if ((num & 1) == 1)
			{
				num++;
			}
			int num2 = texture.height;
			if ((num2 & 1) == 1)
			{
				num2++;
			}
			base.width = num;
			base.height = num2;
		}
		base.MakePixelPerfect();
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Color color = base.color;
		color.a = finalAlpha;
		Color32 item = ((!premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		Vector4 vector = drawingDimensions;
		verts.Add(new Vector3(vector.x, vector.y));
		verts.Add(new Vector3(vector.x, vector.w));
		verts.Add(new Vector3(vector.z, vector.w));
		verts.Add(new Vector3(vector.z, vector.y));
		if (mFlip == Flip.Horizontally)
		{
			uvs.Add(new Vector2(mRect.xMax, mRect.yMin));
			uvs.Add(new Vector2(mRect.xMax, mRect.yMax));
			uvs.Add(new Vector2(mRect.xMin, mRect.yMax));
			uvs.Add(new Vector2(mRect.xMin, mRect.yMin));
		}
		else if (mFlip == Flip.Vertically)
		{
			uvs.Add(new Vector2(mRect.xMin, mRect.yMax));
			uvs.Add(new Vector2(mRect.xMin, mRect.yMin));
			uvs.Add(new Vector2(mRect.xMax, mRect.yMin));
			uvs.Add(new Vector2(mRect.xMax, mRect.yMax));
		}
		else if (mFlip == Flip.Both)
		{
			uvs.Add(new Vector2(mRect.xMax, mRect.yMin));
			uvs.Add(new Vector2(mRect.xMax, mRect.yMax));
			uvs.Add(new Vector2(mRect.xMin, mRect.yMax));
			uvs.Add(new Vector2(mRect.xMin, mRect.yMin));
		}
		else
		{
			uvs.Add(new Vector2(mRect.xMin, mRect.yMin));
			uvs.Add(new Vector2(mRect.xMin, mRect.yMax));
			uvs.Add(new Vector2(mRect.xMax, mRect.yMax));
			uvs.Add(new Vector2(mRect.xMax, mRect.yMin));
		}
		cols.Add(item);
		cols.Add(item);
		cols.Add(item);
		cols.Add(item);
	}
}
