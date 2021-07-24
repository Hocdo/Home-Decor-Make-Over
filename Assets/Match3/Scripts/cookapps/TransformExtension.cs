using UnityEngine;

namespace cookapps
{
	public static class TransformExtension
	{
		private static Vector3 pos;

		public static void SetPosition(this Transform transform, float x, float y, float z)
		{
			pos.Set(x, y, z);
			transform.position = pos;
		}

		public static void SetPositionX(this Transform transform, float x)
		{
			Vector3 position = transform.position;
			float y = position.y;
			Vector3 position2 = transform.position;
			pos.Set(x, y, position2.z);
			transform.position = pos;
		}

		public static void SetPositionY(this Transform transform, float y)
		{
			Vector3 position = transform.position;
			float x = position.x;
			Vector3 position2 = transform.position;
			pos.Set(x, y, position2.z);
			transform.position = pos;
		}

		public static void SetPositionZ(this Transform transform, float z)
		{
			Vector3 position = transform.position;
			float x = position.x;
			Vector3 position2 = transform.position;
			pos.Set(x, position2.y, z);
			transform.position = pos;
		}

		public static void AddPosition(this Transform transform, float x, float y, float z)
		{
			Vector3 position = transform.position;
			float newX = position.x + x;
			Vector3 position2 = transform.position;
			float newY = position2.y + y;
			Vector3 position3 = transform.position;
			pos.Set(newX, newY, position3.z + z);
			transform.position = pos;
		}

		public static void AddPositionX(this Transform transform, float x)
		{
			Vector3 position = transform.position;
			float newX = position.x + x;
			Vector3 position2 = transform.position;
			float y = position2.y;
			Vector3 position3 = transform.position;
			pos.Set(newX, y, position3.z);
			transform.position = pos;
		}

		public static void AddPositionY(this Transform transform, float y)
		{
			Vector3 position = transform.position;
			float x = position.x;
			Vector3 position2 = transform.position;
			float newY = position2.y + y;
			Vector3 position3 = transform.position;
			pos.Set(x, newY, position3.z);
			transform.position = pos;
		}

		public static void AddPositionZ(this Transform transform, float z)
		{
			Vector3 position = transform.position;
			float x = position.x;
			Vector3 position2 = transform.position;
			float y = position2.y;
			Vector3 position3 = transform.position;
			pos.Set(x, y, position3.z + z);
			transform.position = pos;
		}

		public static void SetLocalPosition(this Transform transform, float x, float y, float z)
		{
			pos.Set(x, y, z);
			transform.localPosition = pos;
		}

		public static void SetLocalPositionX(this Transform transform, float x)
		{
			Vector3 localPosition = transform.localPosition;
			float y = localPosition.y;
			Vector3 localPosition2 = transform.localPosition;
			pos.Set(x, y, localPosition2.z);
			transform.localPosition = pos;
		}

		public static void SetLocalPositionY(this Transform transform, float y)
		{
			Vector3 localPosition = transform.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = transform.localPosition;
			pos.Set(x, y, localPosition2.z);
			transform.localPosition = pos;
		}

		public static void SetLocalPositionZ(this Transform transform, float z)
		{
			Vector3 localPosition = transform.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = transform.localPosition;
			pos.Set(x, localPosition2.y, z);
			transform.localPosition = pos;
		}

		public static void AddLocalPosition(this Transform transform, float x, float y, float z)
		{
			Vector3 localPosition = transform.localPosition;
			float newX = localPosition.x + x;
			Vector3 localPosition2 = transform.localPosition;
			float newY = localPosition2.y + y;
			Vector3 localPosition3 = transform.localPosition;
			pos.Set(newX, newY, localPosition3.z + z);
			transform.localPosition = pos;
		}

		public static void AddLocalPositionX(this Transform transform, float x)
		{
			Vector3 localPosition = transform.localPosition;
			float newX = localPosition.x + x;
			Vector3 localPosition2 = transform.localPosition;
			float y = localPosition2.y;
			Vector3 localPosition3 = transform.localPosition;
			pos.Set(newX, y, localPosition3.z);
			transform.localPosition = pos;
		}

		public static void AddLocalPositionY(this Transform transform, float y)
		{
			Vector3 localPosition = transform.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = transform.localPosition;
			float newY = localPosition2.y + y;
			Vector3 localPosition3 = transform.localPosition;
			pos.Set(x, newY, localPosition3.z);
			transform.localPosition = pos;
		}

		public static void AddLocalPositionZ(this Transform transform, float z)
		{
			Vector3 localPosition = transform.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = transform.localPosition;
			float y = localPosition2.y;
			Vector3 localPosition3 = transform.localPosition;
			pos.Set(x, y, localPosition3.z + z);
			transform.localPosition = pos;
		}

		public static void SetLocalScale(this Transform transform, float x, float y, float z)
		{
			pos.Set(x, y, z);
			transform.localScale = pos;
		}

		public static void SetLocalScaleX(this Transform transform, float x)
		{
			Vector3 localScale = transform.localScale;
			float y = localScale.y;
			Vector3 localScale2 = transform.localScale;
			pos.Set(x, y, localScale2.z);
			transform.localScale = pos;
		}

		public static void SetLocalScaleY(this Transform transform, float y)
		{
			Vector3 localScale = transform.localScale;
			float x = localScale.x;
			Vector3 localScale2 = transform.localScale;
			pos.Set(x, y, localScale2.z);
			transform.localScale = pos;
		}

		public static void SetLocalScaleZ(this Transform transform, float z)
		{
			Vector3 localScale = transform.localScale;
			float x = localScale.x;
			Vector3 localScale2 = transform.localScale;
			pos.Set(x, localScale2.y, z);
			transform.localScale = pos;
		}

		public static void AddLocalScale(this Transform transform, float x, float y, float z)
		{
			Vector3 localScale = transform.localScale;
			float newX = localScale.x + x;
			Vector3 localScale2 = transform.localScale;
			float newY = localScale2.y + y;
			Vector3 localScale3 = transform.localScale;
			pos.Set(newX, newY, localScale3.z + z);
			transform.localScale = pos;
		}

		public static void AddLocalScaleX(this Transform transform, float x)
		{
			Vector3 localScale = transform.localScale;
			float newX = localScale.x + x;
			Vector3 localScale2 = transform.localScale;
			float y = localScale2.y;
			Vector3 localScale3 = transform.localScale;
			pos.Set(newX, y, localScale3.z);
			transform.localScale = pos;
		}

		public static void AddLocalScaleY(this Transform transform, float y)
		{
			Vector3 localScale = transform.localScale;
			float x = localScale.x;
			Vector3 localScale2 = transform.localScale;
			float newY = localScale2.y + y;
			Vector3 localScale3 = transform.localScale;
			pos.Set(x, newY, localScale3.z);
			transform.localScale = pos;
		}

		public static void AddLocalScaleZ(this Transform transform, float z)
		{
			Vector3 localScale = transform.localScale;
			float x = localScale.x;
			Vector3 localScale2 = transform.localScale;
			float y = localScale2.y;
			Vector3 localScale3 = transform.localScale;
			pos.Set(x, y, localScale3.z + z);
			transform.localScale = pos;
		}

		public static void SetEulerAngles(this Transform transform, float x, float y, float z)
		{
			pos.Set(x, y, z);
			transform.eulerAngles = pos;
		}

		public static void SetEulerAnglesX(this Transform transform, float x)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			float y = localEulerAngles.y;
			Vector3 localEulerAngles2 = transform.localEulerAngles;
			pos.Set(x, y, localEulerAngles2.z);
			transform.eulerAngles = pos;
		}

		public static void SetEulerAnglesY(this Transform transform, float y)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			float x = localEulerAngles.x;
			Vector3 localEulerAngles2 = transform.localEulerAngles;
			pos.Set(x, y, localEulerAngles2.z);
			transform.eulerAngles = pos;
		}

		public static void SetEulerAnglesZ(this Transform transform, float z)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			float x = localEulerAngles.x;
			Vector3 localEulerAngles2 = transform.localEulerAngles;
			pos.Set(x, localEulerAngles2.y, z);
			transform.eulerAngles = pos;
		}

		public static void AddEulerAngles(this Transform transform, float x, float y, float z)
		{
			Vector3 eulerAngles = transform.eulerAngles;
			float newX = eulerAngles.x + x;
			Vector3 eulerAngles2 = transform.eulerAngles;
			float newY = eulerAngles2.y + y;
			Vector3 eulerAngles3 = transform.eulerAngles;
			pos.Set(newX, newY, eulerAngles3.z + z);
			transform.eulerAngles = pos;
		}

		public static void AddEulerAnglesX(this Transform transform, float x)
		{
			Vector3 eulerAngles = transform.eulerAngles;
			float newX = eulerAngles.x + x;
			Vector3 eulerAngles2 = transform.eulerAngles;
			float y = eulerAngles2.y;
			Vector3 eulerAngles3 = transform.eulerAngles;
			pos.Set(newX, y, eulerAngles3.z);
			transform.eulerAngles = pos;
		}

		public static void AddEulerAnglesY(this Transform transform, float y)
		{
			Vector3 eulerAngles = transform.eulerAngles;
			float x = eulerAngles.x;
			Vector3 eulerAngles2 = transform.eulerAngles;
			float newY = eulerAngles2.y + y;
			Vector3 eulerAngles3 = transform.eulerAngles;
			pos.Set(x, newY, eulerAngles3.z);
			transform.eulerAngles = pos;
		}

		public static void AddEulerAnglesZ(this Transform transform, float z)
		{
			Vector3 eulerAngles = transform.eulerAngles;
			float x = eulerAngles.x;
			Vector3 eulerAngles2 = transform.eulerAngles;
			float y = eulerAngles2.y;
			Vector3 eulerAngles3 = transform.eulerAngles;
			pos.Set(x, y, eulerAngles3.z + z);
			transform.eulerAngles = pos;
		}

		public static void SetLocalEulerAngles(this Transform transform, float x, float y, float z)
		{
			pos.Set(x, y, z);
			transform.localEulerAngles = pos;
		}

		public static void SetLocalEulerAnglesX(this Transform transform, float x)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			float y = localEulerAngles.y;
			Vector3 localEulerAngles2 = transform.localEulerAngles;
			pos.Set(x, y, localEulerAngles2.z);
			transform.localEulerAngles = pos;
		}

		public static void SetLocalEulerAnglesY(this Transform transform, float y)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			float x = localEulerAngles.x;
			Vector3 localEulerAngles2 = transform.localEulerAngles;
			pos.Set(x, y, localEulerAngles2.z);
			transform.localEulerAngles = pos;
		}

		public static void SetLocalEulerAnglesZ(this Transform transform, float z)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			float x = localEulerAngles.x;
			Vector3 localEulerAngles2 = transform.localEulerAngles;
			pos.Set(x, localEulerAngles2.y, z);
			transform.localEulerAngles = pos;
		}

		public static void AddLocalEulerAngles(this Transform transform, float x, float y, float z)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			float newX = localEulerAngles.x + x;
			Vector3 localEulerAngles2 = transform.localEulerAngles;
			float newY = localEulerAngles2.y + y;
			Vector3 localEulerAngles3 = transform.localEulerAngles;
			pos.Set(newX, newY, localEulerAngles3.z + z);
			transform.localEulerAngles = pos;
		}

		public static void AddLocalEulerAnglesX(this Transform transform, float x)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			float newX = localEulerAngles.x + x;
			Vector3 localEulerAngles2 = transform.localEulerAngles;
			float y = localEulerAngles2.y;
			Vector3 localEulerAngles3 = transform.localEulerAngles;
			pos.Set(newX, y, localEulerAngles3.z);
			transform.localEulerAngles = pos;
		}

		public static void AddLocalEulerAnglesY(this Transform transform, float y)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			float x = localEulerAngles.x;
			Vector3 localEulerAngles2 = transform.localEulerAngles;
			float newY = localEulerAngles2.y + y;
			Vector3 localEulerAngles3 = transform.localEulerAngles;
			pos.Set(x, newY, localEulerAngles3.z);
			transform.localEulerAngles = pos;
		}

		public static void AddLocalEulerAnglesZ(this Transform transform, float z)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			float x = localEulerAngles.x;
			Vector3 localEulerAngles2 = transform.localEulerAngles;
			float y = localEulerAngles2.y;
			Vector3 localEulerAngles3 = transform.localEulerAngles;
			pos.Set(x, y, localEulerAngles3.z + z);
			transform.localEulerAngles = pos;
		}
	}
}
