using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UtageExtensions
{
	public static class UtageExtensions
	{
		public static void Separate(this string str, char separator, bool isFirst, out string str1, out string str2)
		{
			int num = (isFirst ? str.IndexOf(separator) : str.LastIndexOf(separator));
			str1 = str.Substring(0, num);
			str2 = str.Substring(num + 1);
		}

		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		public static void SafeSendMessage(this GameObject go, string functionName, object obj = null, bool isForceActive = false)
		{
			if (!string.IsNullOrEmpty(functionName))
			{
				if (isForceActive)
				{
					go.SetActive(true);
				}
				go.SendMessage(functionName, obj, SendMessageOptions.DontRequireReceiver);
			}
		}

		public static T GetComponentCache<T>(this GameObject go, ref T component) where T : class
		{
			return component ?? (component = go.GetComponent<T>());
		}

		public static T GetComponentCacheCreateIfMissing<T>(this GameObject go, ref T component) where T : Component
		{
			return component ?? (component = go.GetComponentCreateIfMissing<T>());
		}

		public static T GetComponentCreateIfMissing<T>(this GameObject go) where T : Component
		{
			T val = go.GetComponent<T>();
			if ((UnityEngine.Object)val == (UnityEngine.Object)null)
			{
				val = go.AddComponent<T>();
			}
			return val;
		}

		public static T GetComponentCacheInChildren<T>(this GameObject go, ref T component) where T : class
		{
			return component ?? (component = go.GetComponentInChildren<T>(true));
		}

		public static T[] GetComponentsCacheInChildren<T>(this GameObject go, ref T[] components) where T : class
		{
			return components ?? (components = go.GetComponentsInChildren<T>(true));
		}

		public static List<T> GetComponentListCacheInChildren<T>(this GameObject go, ref List<T> components) where T : class
		{
			return components ?? (components = new List<T>(go.GetComponentsInChildren<T>(true)));
		}

		public static T GetComponentCacheFindIfMissing<T>(this GameObject go, ref T component) where T : Component
		{
			if ((UnityEngine.Object)component == (UnityEngine.Object)null)
			{
				component = UnityEngine.Object.FindObjectOfType<T>();
			}
			return component;
		}

		public static T GetComponentCache<T>(this Component target, ref T component) where T : class
		{
			try
			{
				return target.gameObject.GetComponentCache(ref component);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				return null;
			}
		}

		public static T[] GetComponentsCacheInChildren<T>(this Component target, ref T[] components) where T : class
		{
			try
			{
				return target.gameObject.GetComponentsCacheInChildren(ref components);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				return null;
			}
		}

		public static List<T> GetComponentListCacheInChildren<T>(this Component target, ref List<T> components) where T : class
		{
			try
			{
				return target.gameObject.GetComponentListCacheInChildren(ref components);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				return null;
			}
		}

		public static T GetComponentCacheCreateIfMissing<T>(this Component target, ref T component) where T : Component
		{
			try
			{
				return target.gameObject.GetComponentCacheCreateIfMissing(ref component);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				return null;
			}
		}

		public static T GetComponentCreateIfMissing<T>(this Component target) where T : Component
		{
			return target.gameObject.GetComponentCreateIfMissing<T>();
		}

		public static void RemoveComponent<T>(this GameObject target, bool immediate = true) where T : Component
		{
			target.GetComponent<T>().RemoveComponentMySelf(immediate);
		}

		public static void RemoveComponents<T>(this GameObject target, bool immediate = true) where T : Component
		{
			T[] components = target.GetComponents<T>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].RemoveComponentMySelf(immediate);
			}
		}

		public static void RemoveComponentMySelf(this Component target, bool immediate = true)
		{
			if (target != null)
			{
				if (immediate)
				{
					UnityEngine.Object.DestroyImmediate(target);
				}
				else
				{
					UnityEngine.Object.Destroy(target);
				}
			}
		}

		public static T GetComponentCacheInChildren<T>(this Component target, ref T component) where T : Component
		{
			return component ?? (component = target.GetComponentInChildren<T>(true));
		}

		public static void ChangeLayerDeep(this GameObject go, int layer)
		{
			go.layer = layer;
			foreach (Transform item in go.transform)
			{
				item.gameObject.ChangeLayerDeep(layer);
			}
		}

		public static T GetComponentCacheFindIfMissing<T>(this Component target, ref T component) where T : Component
		{
			return target.gameObject.GetComponentCacheFindIfMissing(ref component);
		}

		public static T GetSingletonFindIfMissing<T>(this T target, ref T instance) where T : Component
		{
			if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
			{
				instance = UnityEngine.Object.FindObjectOfType<T>();
			}
			return instance;
		}

		public static void InitSingletonComponent<T>(this T target, ref T instance) where T : Component
		{
			if ((UnityEngine.Object)instance != (UnityEngine.Object)null && (UnityEngine.Object)instance != (UnityEngine.Object)target)
			{
				Debug.LogErrorFormat("{0} is multiple created", typeof(T).ToString());
				UnityEngine.Object.Destroy(target.gameObject);
			}
			else
			{
				instance = target;
			}
		}

		public static T Find<T>(this Transform t, string name) where T : Component
		{
			Transform transform = t.Find(name);
			if (transform == null)
			{
				return null;
			}
			return transform.GetComponent<T>();
		}

		public static Transform FindDeep(this Transform t, string name, bool includeInactive = false)
		{
			Transform[] componentsInChildren = t.GetComponentsInChildren<Transform>(includeInactive);
			foreach (Transform transform in componentsInChildren)
			{
				if (transform.gameObject.name == name)
				{
					return transform;
				}
			}
			return null;
		}

		public static T FindDeepAsComponent<T>(this Transform t, string name, bool includeInactive = false) where T : Component
		{
			T[] componentsInChildren = t.GetComponentsInChildren<T>(includeInactive);
			foreach (T val in componentsInChildren)
			{
				if (val.gameObject.name == name)
				{
					return val;
				}
			}
			return null;
		}

		public static GameObject AddChild(this Transform t, GameObject child)
		{
			return t.AddChild(child, Vector3.zero, Vector3.one);
		}

		public static GameObject AddChild(this Transform t, GameObject child, Vector3 localPosition)
		{
			return t.AddChild(child, localPosition, Vector3.one);
		}

		public static GameObject AddChild(this Transform t, GameObject child, Vector3 localPosition, Vector3 localScale)
		{
			child.transform.SetParent(t);
			child.transform.localScale = localScale;
			child.transform.localPosition = localPosition;
			if (child.transform is RectTransform)
			{
				(child.transform as RectTransform).anchoredPosition = localPosition;
			}
			child.transform.localRotation = Quaternion.identity;
			child.ChangeLayerDeep(t.gameObject.layer);
			return child;
		}

		public static GameObject AddChildPrefab(this Transform t, GameObject prefab)
		{
			return t.AddChildPrefab(prefab, Vector3.zero, Vector3.one);
		}

		public static GameObject AddChildPrefab(this Transform t, GameObject prefab, Vector3 localPosition)
		{
			return t.AddChildPrefab(prefab, localPosition, Vector3.one);
		}

		public static GameObject AddChildPrefab(this Transform t, GameObject prefab, Vector3 localPosition, Vector3 localScale)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, t);
			gameObject.transform.localScale = localScale;
			gameObject.transform.localPosition = localPosition;
			gameObject.ChangeLayerDeep(t.gameObject.layer);
			return gameObject;
		}

		public static GameObject AddChildUnityObject(this Transform t, UnityEngine.Object obj)
		{
			return UnityEngine.Object.Instantiate(obj, t) as GameObject;
		}

		public static GameObject AddChildGameObject(this Transform t, string name)
		{
			return t.AddChildGameObject(name, Vector3.zero, Vector3.one);
		}

		public static GameObject AddChildGameObject(this Transform t, string name, Vector3 localPosition)
		{
			return t.AddChildGameObject(name, localPosition, Vector3.one);
		}

		public static GameObject AddChildGameObject(this Transform t, string name, Vector3 localPosition, Vector3 localScale)
		{
			GameObject gameObject = ((t is RectTransform) ? new GameObject(name, typeof(RectTransform)) : new GameObject(name));
			t.AddChild(gameObject, localPosition, localScale);
			return gameObject;
		}

		public static T AddChildGameObjectComponent<T>(this Transform t, string name) where T : Component
		{
			return t.AddChildGameObjectComponent<T>(name, Vector3.zero, Vector3.one);
		}

		public static T AddChildGameObjectComponent<T>(this Transform t, string name, Vector3 localPosition) where T : Component
		{
			return t.AddChildGameObjectComponent<T>(name, localPosition, Vector3.one);
		}

		public static T AddChildGameObjectComponent<T>(this Transform t, string name, Vector3 localPosition, Vector3 localScale) where T : Component
		{
			GameObject gameObject = t.AddChildGameObject(name, localPosition, localScale);
			T component = gameObject.GetComponent<T>();
			if ((UnityEngine.Object)component == (UnityEngine.Object)null)
			{
				return gameObject.AddComponent<T>();
			}
			return component;
		}

		public static void InitCloneChildren<T>(this Transform t, int count) where T : Component
		{
			T[] componentsInChildren = t.GetComponentsInChildren<T>(true);
			if (componentsInChildren.Length == 0)
			{
				Debug.LogError(typeof(T).Name + " is not under " + t.gameObject.name);
				return;
			}
			int num = Mathf.Max(0, count - componentsInChildren.Length);
			for (int i = 0; i < num; i++)
			{
				t.AddChildPrefab(componentsInChildren[0].gameObject, componentsInChildren[0].gameObject.transform.localPosition, componentsInChildren[0].gameObject.transform.localScale);
			}
		}

		public static void InitCloneChildren<T>(this Transform t, int count, Action<T, int> callback) where T : Component
		{
			t.InitCloneChildren<T>(count);
			T[] componentsInChildren = t.GetComponentsInChildren<T>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (i < count)
				{
					componentsInChildren[i].gameObject.SetActive(true);
					callback(componentsInChildren[i], i);
				}
				else
				{
					componentsInChildren[i].gameObject.SetActive(false);
				}
			}
		}

		public static void InitCloneChildren<TComponent, TList>(this Transform t, List<TList> list, Action<TComponent, TList> callback) where TComponent : Component
		{
			t.InitCloneChildren(Mathf.Max(list.Count, 1), delegate(TComponent item, int index)
			{
				if (index < list.Count)
				{
					item.gameObject.SetActive(true);
					callback(item, list[index]);
				}
				else
				{
					item.gameObject.SetActive(false);
				}
			});
		}

		public static T GetCompoentInChildrenCreateIfMissing<T>(this Transform t) where T : Component
		{
			return t.GetCompoentInChildrenCreateIfMissing<T>(typeof(T).Name);
		}

		public static T GetCompoentInChildrenCreateIfMissing<T>(this Transform t, string name) where T : Component
		{
			T val = t.GetComponentInChildren<T>();
			if ((UnityEngine.Object)val == (UnityEngine.Object)null)
			{
				val = t.AddChildGameObjectComponent<T>(name);
			}
			return val;
		}

		public static void DestroyChildren(this Transform t)
		{
			List<Transform> list = new List<Transform>();
			foreach (Transform item in t)
			{
				item.gameObject.SetActive(false);
				list.Add(item);
			}
			t.DetachChildren();
			foreach (Transform item2 in list)
			{
				UnityEngine.Object.Destroy(item2.gameObject);
			}
		}

		public static void DestroyChildrenInEditorOrPlayer(this Transform t)
		{
			List<Transform> list = new List<Transform>();
			foreach (Transform item in t)
			{
				item.gameObject.SetActive(false);
				list.Add(item);
			}
			t.DetachChildren();
			foreach (Transform item2 in list)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(item2.gameObject);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(item2.gameObject);
				}
			}
		}

		public static Vector2 GetSize(this RectTransform t)
		{
			Rect rect = t.rect;
			return new Vector2(rect.width, rect.height);
		}

		public static Vector2 GetSizeScaled(this RectTransform t)
		{
			Rect rect = t.rect;
			return new Vector2(rect.width * t.localScale.x, rect.height * t.localScale.y);
		}

		public static void SetSize(this RectTransform t, Vector2 size)
		{
			t.SetWidth(size.x);
			t.SetHeight(size.y);
		}

		public static void SetSize(this RectTransform t, float width, float height)
		{
			t.SetWidth(width);
			t.SetHeight(height);
		}

		public static float GetWith(this RectTransform t)
		{
			return t.rect.width;
		}

		public static void SetWidth(this RectTransform t, float width)
		{
			t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		}

		public static void SetWidthWidthParentRatio(this RectTransform t, float ratio)
		{
			float width = (t.parent as RectTransform).GetWith() * ratio;
			t.SetWidth(width);
		}

		public static float GetHeight(this RectTransform t)
		{
			return t.rect.height;
		}

		public static void SetHeight(this RectTransform t, float height)
		{
			t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}

		public static void SetStretch(this RectTransform t)
		{
			t.anchorMin = Vector2.zero;
			t.anchorMax = Vector2.one;
			t.sizeDelta = Vector2.zero;
		}

		public static Rect RectInCanvas(this RectTransform t, Canvas canvas)
		{
			Vector3 position = t.TransformPoint(t.rect.center);
			position = canvas.transform.InverseTransformPoint(position);
			Vector3 vector = t.GetSizeScaled();
			t.TransformVector(vector);
			canvas.transform.InverseTransformVector(vector);
			Rect result = default(Rect);
			result.size = vector;
			result.center = position;
			return result;
		}

		public static void SetAlpha(this Graphic graphic, float alpha)
		{
			Color color = graphic.color;
			color.a = alpha;
			graphic.color = color;
		}

		public static float GetAlpha(this Graphic graphic)
		{
			return graphic.color.a;
		}

		internal static Rect ToUvRect(this Rect rect, float w, float h)
		{
			return new Rect(rect.x / w, 1f - rect.yMax / h, rect.width / w, rect.height / h);
		}

		public static RenderTexture CreateCopyTemporary(this RenderTexture renderTexture)
		{
			return renderTexture.CreateCopyTemporary(renderTexture.depth);
		}

		public static RenderTexture CreateCopyTemporary(this RenderTexture renderTexture, int depth)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, depth, renderTexture.format);
			Graphics.Blit(renderTexture, temporary);
			return temporary;
		}

		public static TValue GetValueOrSetDefaultIfMissing<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
		{
			if (!dictionary.TryGetValue(key, out var value))
			{
				dictionary.Add(key, defaultValue);
				return defaultValue;
			}
			return value;
		}

		public static TValue GetValueOrGetNullIfMissing<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : class
		{
			if (!dictionary.TryGetValue(key, out var value))
			{
				return null;
			}
			return value;
		}

		public static bool Approximately(this Vector2 a, Vector2 b)
		{
			if (Mathf.Approximately(a.x, b.x))
			{
				return Mathf.Approximately(a.y, b.y);
			}
			return false;
		}
	}
}
