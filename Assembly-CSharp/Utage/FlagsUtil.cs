using System;

namespace Utage
{
	public static class FlagsUtil
	{
		public static bool Has<T>(T value, T flags) where T : struct
		{
			try
			{
				return ((int)(object)value & (int)(object)flags) == (int)(object)flags;
			}
			catch
			{
				return false;
			}
		}

		public static bool HasAny<T>(T value, T flags) where T : struct
		{
			try
			{
				return ((int)(object)value & (int)(object)flags) != 0;
			}
			catch
			{
				return false;
			}
		}

		public static bool Is<T>(T value, T flags) where T : struct
		{
			try
			{
				return (int)(object)value == (int)(object)flags;
			}
			catch
			{
				return false;
			}
		}

		public static T Add<T>(T value, T flags) where T : struct
		{
			try
			{
				return (T)(object)((int)(object)value | (int)(object)flags);
			}
			catch (Exception innerException)
			{
				throw new ArgumentException(string.Format("Could not add flags type '{0}'.", typeof(T).Name), innerException);
			}
		}

		public static T Remove<T>(T value, T flags) where T : struct
		{
			try
			{
				return (T)(object)((int)(object)value & ~(int)(object)flags);
			}
			catch (Exception innerException)
			{
				throw new ArgumentException(string.Format("Could not remove flags type '{0}'.", typeof(T).Name), innerException);
			}
		}

		public static T SetEnable<T>(T value, T flags, bool isEnable) where T : struct
		{
			try
			{
				if (isEnable)
				{
					return Add(value, flags);
				}
				return Remove(value, flags);
			}
			catch (Exception innerException)
			{
				throw new ArgumentException(string.Format("Could not SetEnable flags type '{0}'.", typeof(T).Name), innerException);
			}
		}
	}
}
