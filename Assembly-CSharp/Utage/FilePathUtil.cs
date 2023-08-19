using System;
using System.IO;
using UnityEngine;

namespace Utage
{
	public class FilePathUtil
	{
		public static string GetDirectoryNameOnly(string path)
		{
			return Path.GetFileName(Path.GetDirectoryName(path));
		}

		public static string GetDirectoryPath(string path)
		{
			int num = Mathf.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
			if (num >= 0)
			{
				return path.Substring(0, num);
			}
			if (path.IndexOf('.') >= 0)
			{
				return "";
			}
			return path;
		}

		public static string Format(string path)
		{
			path = path.Replace("\\", "/");
			if (!path.Contains("://"))
			{
				path = path.Replace(":/", "://");
			}
			return path;
		}

		public static string GetFileName(string path)
		{
			return Path.GetFileName(path);
		}

		public static string GetFileNameWithoutExtension(string path)
		{
			try
			{
				return Path.GetFileNameWithoutExtension(path);
			}
			catch (Exception ex)
			{
				Debug.LogError(path + "  " + ex.Message);
				return "";
			}
		}

		public static string GetPathWithoutExtension(string path)
		{
			int num = path.LastIndexOf('.');
			if (num > 0)
			{
				path = path.Substring(0, num);
			}
			return path;
		}

		public static string GetExtension(string path)
		{
			return Path.GetExtension(path);
		}

		public static string ChangeExtension(string path, string extenstion)
		{
			return Path.ChangeExtension(path, extenstion);
		}

		public static bool CheckExtension(string path, string ext)
		{
			return string.Compare(GetExtension(path), ext, true) == 0;
		}

		public static bool CheckExtensionWithOutDouble(string path, string ext, string doubleExtension)
		{
			return CheckExtension(GetExtensionWithOutDouble(path, doubleExtension), ext);
		}

		public static string GetExtensionWithOutDouble(string path, string doubleExtension)
		{
			string extension = Path.GetExtension(path);
			if (string.Compare(extension, doubleExtension, true) != 0)
			{
				return extension;
			}
			path = path.Substring(0, path.Length - doubleExtension.Length);
			return Path.GetExtension(path);
		}

		public static string AddDoubleExtension(string path, string doubleExtension)
		{
			if (!CheckExtension(path, doubleExtension))
			{
				path += doubleExtension;
			}
			return path;
		}

		public static string GetFileNameWithoutDoubleExtension(string path)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			if (fileNameWithoutExtension.Contains("."))
			{
				fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNameWithoutExtension);
			}
			return fileNameWithoutExtension;
		}

		public static bool IsAbsoluteUri(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}
			if (path.Length <= 1)
			{
				return false;
			}
			try
			{
				return new Uri(path, UriKind.RelativeOrAbsolute).IsAbsoluteUri;
			}
			catch (Exception ex)
			{
				Debug.LogError(path + ":" + ex.Message);
				return false;
			}
		}

		public static string EncodeUrl(string url)
		{
			try
			{
				return new Uri(url.Replace('\\', '/')).AbsoluteUri;
			}
			catch (Exception ex)
			{
				Debug.LogError(url + ":" + ex.Message);
				return url;
			}
		}

		public static string ToCacheClearUrl(string url)
		{
			if (url.Contains(Application.streamingAssetsPath) && Application.platform != RuntimePlatform.WebGLPlayer)
			{
				return url;
			}
			return string.Format("{0}?datetime={1}", url, DateTime.Now.ToFileTime());
		}

		public static string ToStreamingAssetsPath(string path)
		{
			return AddFileProtocol(Combine(Application.streamingAssetsPath, path));
		}

		public static string AddFileProtocol(string path)
		{
			if (path.Contains("://"))
			{
				return path;
			}
			if (path[0] != '/')
			{
				path = "/" + path;
			}
			return "file://" + path;
		}

		public static string Combine(params string[] args)
		{
			string text = "";
			foreach (string text2 in args)
			{
				if (!string.IsNullOrEmpty(text2))
				{
					text = Path.Combine(text, text2);
				}
			}
			return text.Replace("\\", "/");
		}

		public static string RemoveDirectory(string path, string directoryPath)
		{
			path = Format(path);
			directoryPath = Format(directoryPath);
			string newPath;
			if (!TryRemoveDirectory(path, directoryPath, out newPath))
			{
				Debug.LogError("RemoveDirectoryPath Error [" + path + "]  [" + directoryPath + "] ");
			}
			return newPath;
		}

		public static bool TryRemoveDirectory(string path, string directoryPath, out string newPath)
		{
			newPath = path;
			if (!path.StartsWith(directoryPath))
			{
				return false;
			}
			int num = directoryPath.Length;
			if (path.Length > num)
			{
				char c = path[num];
				if (c == '/' || c == '\\')
				{
					num++;
				}
			}
			newPath = path.Remove(0, num);
			return true;
		}

		internal static bool IsUnderDirectory(string path, string directoryPath)
		{
			path = Format(path);
			directoryPath = Format(directoryPath);
			return path.StartsWith(directoryPath);
		}

		public static string ToRelativePath(string root, string path)
		{
			Uri uri = new Uri(root);
			Uri uri2 = new Uri(path);
			return uri.MakeRelativeUri(uri2).ToString();
		}
	}
}
