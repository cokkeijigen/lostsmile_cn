using UnityEngine;

namespace Utage
{
	[RequireComponent(typeof(Camera))]
	public abstract class ImageEffectSingelShaderBase : ImageEffectBase
	{
		[SerializeField]
		private Shader shader;

		private Shader tmpShader;

		public Shader Shader
		{
			get
			{
				return shader;
			}
			set
			{
				shader = value;
			}
		}

		protected Material Material { get; set; }

		public override void SetShaders(params Shader[] shadres)
		{
			if (shadres.Length != 0)
			{
				Shader = shadres[0];
			}
		}

		protected override bool CheckShaderAndCreateMaterial()
		{
			Material mat;
			bool result = TryCheckShaderAndCreateMaterialSub(Shader, Material, out mat);
			Material = mat;
			return result;
		}

		protected override void StoreObjectsOnJsonRead()
		{
			tmpShader = shader;
		}

		protected override void RestoreObjectsOnJsonRead()
		{
			shader = tmpShader;
		}
	}
}
