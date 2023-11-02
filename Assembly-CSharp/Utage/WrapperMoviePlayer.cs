using System.Collections;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Wrapper/MoviePlayer")]
	public class WrapperMoviePlayer : MonoBehaviour
	{
		private static WrapperMoviePlayer instance;

		private bool isPlaying;

		private bool cancel;

		public Color bgColor = Color.black;

		public bool ignoreCancel;

		public float cancelFadeTime = 0.5f;

		[SerializeField]
		private GameObject renderTarget;

		[SerializeField]
		private bool overrideRootDirectory;

		[SerializeField]
		[Hide("NotOverrideRootDirectory")]
		private string rootDirectory;

		public GameObject Target
		{
			get
			{
				if (renderTarget == null)
				{
					return base.gameObject;
				}
				return renderTarget;
			}
			set
			{
				if (renderTarget != value)
				{
					renderTarget = value;
				}
			}
		}

		public bool OverrideRootDirectory
		{
			get
			{
				return overrideRootDirectory;
			}
			set
			{
				overrideRootDirectory = value;
			}
		}

		private bool NotOverrideRootDirectory => !OverrideRootDirectory;

		public string RootDirectory
		{
			get
			{
				return rootDirectory;
			}
			set
			{
				rootDirectory = value;
			}
		}

		public static WrapperMoviePlayer GetInstance()
		{
			return instance;
		}

		public static void SetRenderTarget(GameObject target)
		{
			GetInstance().Target = target;
		}

		public static void Play(string path, bool isLoop = false)
		{
			GetInstance().PlayMovie(path, isLoop);
		}

		public static void Play(string path, bool isLoop, bool cancel)
		{
			GetInstance().PlayMovie(path, isLoop, cancel);
		}

		public static void Cancel()
		{
			GetInstance().CancelMovie();
		}

		public static bool IsPlaying()
		{
			return GetInstance().isPlaying;
		}

		private void Awake()
		{
			if (null != instance)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				instance = this;
			}
		}

		public void PlayMovie(string path, bool isLoop, bool cancel)
		{
			this.cancel = cancel && !ignoreCancel;
			StartCoroutine(CoPlayMobileMovie(path));
		}

		public void PlayMovie(string path, bool isLoop)
		{
			PlayMovie(path, isLoop, true);
		}

		public void CancelMovie()
		{
			_ = cancel;
		}

		private IEnumerator CoPlayMobileMovie(string path)
		{
			isPlaying = false;
			yield break;
		}

		private string ToStreamingPath(string path)
		{
			return FilePathUtil.Combine((Application.platform == RuntimePlatform.Android) ? "" : "file://", Application.streamingAssetsPath, path);
		}
	}
}
