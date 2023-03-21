using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Extension methods in C# are addition methods that can be applied to an existing type
// without the need to create a new derived type.
// They are special static methods that can be called as if they were
// instance methods of the existing type without having to modify that type.

public static class ExtensionMethods
{
    private const float DOT_THRESHOLD = 0.5f;

    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        return dot >= DOT_THRESHOLD;
    }
}
