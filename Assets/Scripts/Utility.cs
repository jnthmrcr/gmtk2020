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

    public static Vector2Int toV2i(this Vector2 v2)
    {
        return new Vector2Int(Mathf.RoundToInt(v2.x), Mathf.RoundToInt(v2.y));
    }

    public static Vector2Int toV2i(this Vector3 v3)
    {
        return new Vector2Int(Mathf.RoundToInt(v3.x), Mathf.RoundToInt(v3.z));
    }

    public static Vector3 toV3(this Vector2 v2, float y = 0)
	{
        return new Vector3(v2.x, y, v2.y);
	}

    public static Vector3Int toV3i(this Vector2Int v2, int z = 0)
    {
        return new Vector3Int(v2.x, v2.y, z);
    }
}
