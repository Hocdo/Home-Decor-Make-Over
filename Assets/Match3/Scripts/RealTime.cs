using UnityEngine;

public class RealTime : MonoBehaviour
{
	public static float time => Time.unscaledTime;

	public static float deltaTime => Mathf.Clamp(Time.unscaledDeltaTime, 1E-05f, Time.maximumDeltaTime);
}
