using System;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class StaticAsset
	{
		[SerializeField]
		private UnityEngine.Object asset;

		public UnityEngine.Object Asset
        {
            get
            {
                return asset;
            }
            set // iTuskezigen++ ���������set
            {
                asset = value;
            }
        }
    }
}
