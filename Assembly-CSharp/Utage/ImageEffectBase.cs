using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[RequireComponent(typeof(Camera))]
	public abstract class ImageEffectBase : MonoBehaviour
	{
		private List<Material> createdMaterials = new List<Material>();

		private const int Version = 0;

		protected virtual bool NeedRenderTexture => false;

		protected virtual bool NeedDepth => false;

		protected virtual bool NeedHdr => false;

		private void Start()
		{
			CheckResources();
		}

		protected virtual void OnDestroy()
		{
			ClearCreatedMaterials();
		}

		protected virtual bool CheckResources()
		{
			if (!CheckSupport() || !CheckShaderAndCreateMaterial())
			{
				base.enabled = false;
				Debug.LogWarning("The image effect " + ToString() + " has been disabled as it's not supported on the current platform.");
				return false;
			}
			return true;
		}

		protected bool CheckSupport()
		{
			return CheckSupport(NeedRenderTexture, NeedDepth, NeedHdr);
		}

		public abstract void SetShaders(params Shader[] shadres);

		protected abstract bool CheckShaderAndCreateMaterial();

		protected bool CheckSupport(bool needRenderTexture, bool needDepth, bool needHdr)
		{
			if (!CheckSupportSub(needRenderTexture, needDepth, needHdr))
			{
				return false;
			}
			if (needDepth)
			{
				GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
			}
			return true;
		}

		protected bool CheckSupportSub(bool needRenderTexture, bool needDepth, bool needHdr)
		{
			if (!ImageEffectUtil.SupportsImageEffects)
			{
				return false;
			}
			if (needRenderTexture && !ImageEffectUtil.SupportsRenderTextures)
			{
				return false;
			}
			if (needDepth && !ImageEffectUtil.SupportsDepth)
			{
				return false;
			}
			if (needHdr && !ImageEffectUtil.SupportsHDRTextures)
			{
				return false;
			}
			return true;
		}

		protected bool TryCheckShaderAndCreateMaterialSub(Shader s, Material m2Create, out Material mat)
		{
			mat = null;
			if (!s)
			{
				Debug.Log("Missing shader in " + ToString());
				return false;
			}
			if (!s.isSupported)
			{
				Debug.Log("The shader " + s.ToString() + " on effect " + ToString() + " is not supported on this platform!");
				return false;
			}
			if ((bool)m2Create && m2Create.shader == s)
			{
				mat = m2Create;
				return true;
			}
			m2Create = new Material(s);
			createdMaterials.Add(m2Create);
			m2Create.hideFlags = HideFlags.DontSave;
			mat = m2Create;
			return true;
		}

		private void ClearCreatedMaterials()
		{
			foreach (Material createdMaterial in createdMaterials)
			{
				Object.Destroy(createdMaterial);
			}
			createdMaterials.Clear();
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!CheckResources())
			{
				Graphics.Blit(source, destination);
			}
			else
			{
				RenderImage(source, destination);
			}
		}

		protected abstract void RenderImage(RenderTexture source, RenderTexture destination);

		public void Write(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(base.enabled);
			writer.WriteJson(this);
		}

		public virtual void Read(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num < 0 || num > 0)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
				return;
			}
			StoreObjectsOnJsonRead();
			base.enabled = reader.ReadBoolean();
			reader.ReadJson(this);
			RestoreObjectsOnJsonRead();
		}

		protected abstract void StoreObjectsOnJsonRead();

		protected abstract void RestoreObjectsOnJsonRead();
	}
}
