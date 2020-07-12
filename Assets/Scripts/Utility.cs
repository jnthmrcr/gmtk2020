using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static int i;
    public static float f;
    public static Vector2 v2;
    public static Vector3 v3;

    public static Vector2 toV2(this Vector3 v3)
	{
        return new Vector2(v3.x, v3.z);
	}

    public static Vector3 toV3(this Vector2 v2, float y = 0)
	{
        return new Vector3(v2.x, y, v2.y);
	}
}
