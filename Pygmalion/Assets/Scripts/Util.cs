using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util {
  public static Vector3 Clamp(Vector3 vec, Vector3 min, Vector3 max){
    return new Vector3(
      Mathf.Clamp(vec.x, min.x, max.x),
      Mathf.Clamp(vec.y, min.y, max.y),
      Mathf.Clamp(vec.z, min.z, max.z)
    );
  }

  public static float getDistanceFromRange(float val, float min, float max){
    if (val > max)
			return val-max;
		else if (val < min)
			return val-min;
		else
			return 0.0f;
  }

  public static Vector3 getDistanceFromRange(Vector3 vec, Vector3 min, Vector3 max){
    return new Vector3(
      getDistanceFromRange(vec.x, min.x, max.x),
      getDistanceFromRange(vec.y, min.y, max.y),
      getDistanceFromRange(vec.z, min.z, max.z)
    );
  }
}
