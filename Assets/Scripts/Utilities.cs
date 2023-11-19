using UnityEngine;

public static class Utilities
{
    public static void DecreaseTimerIfPositive(this ref float f)
    {
        if (f > 0) f -= Time.deltaTime;
        if (f < 0) f = 0;
    }

    /// <summary>
    /// RETURN THE VECTOR 2 FROM THE DIRECTION OF A GIVEN ANGLE
    /// </summary>
    public static Vector2 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }


    /// <summary>
    /// RETURN THE ANGLE FROM A GIVEN DIRECTION
    /// </summary>
    public static float GetAngleFromVector(this Vector2 dir)
    {
        dir = dir.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        return angle;
    }


    /// <summary>
    /// ROTATES A VECTOR FROM A CERTAIN ANGLE
    /// </summary>
    public static Vector2 RotateDirection(this Vector2 originalDirection, float addedAngle)
    {
        float currentAngle = GetAngleFromVector(originalDirection);

        currentAngle += addedAngle;

        return GetVectorFromAngle(currentAngle);
    }

    public static Vector2 ConvertTo8Dir(Vector2 inputVector)
    {
        //checks if the x or y axis is dominant
        Vector2 newDir = new Vector2();

        int dirCount = 8;
        float angle = Mathf.Atan2(inputVector.y, inputVector.x);
        float normalized = Mathf.Repeat(angle / (Mathf.PI * 2f), 1f);
        angle = Mathf.Round(normalized * dirCount) * Mathf.PI * 2f / dirCount;
        newDir.x = Mathf.Cos(angle);
        newDir.y = Mathf.Sin(angle);
        return newDir;
    }

    public static int Convert8DirToOrientation(Vector2 dir)
    {
        if (CheckIfVector2Equals(dir, Vector2.up)) return 0;
        if (CheckIfVector2Equals(dir, new Vector2(0.71f, 0.71f))) return 315;
        if (CheckIfVector2Equals(dir, Vector2.right)) return 270;
        if (CheckIfVector2Equals(dir, new Vector2(0.71f, -0.71f))) return 225;
        if (CheckIfVector2Equals(dir, Vector2.down)) return 180;
        if (CheckIfVector2Equals(dir, new Vector2(-0.71f, -0.71f))) return 135;
        if (CheckIfVector2Equals(dir, Vector2.left)) return 90;
        if (CheckIfVector2Equals(dir, new Vector2(-0.71f, 0.71f))) return 45;
        Debug.LogError($"Could not convert dir to rotation : {dir}");
        return int.MinValue;
    }

    public static bool CheckIfVector2Equals(Vector2 a, Vector2 b)
    {
        float maxDiff = 0.05f;
        if ((a - b).magnitude <= maxDiff) return true;
        else return false;
    }
}