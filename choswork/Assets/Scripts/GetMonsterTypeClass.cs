using System;
using System.Linq;
using UnityEngine;

public static class GetMonsterTypeClass
{
    public static Type GetRagdollAction(Transform transform)
    {
        // Get all types derived from RagdollAction
        var types = typeof(RagDollAction).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(RagDollAction)));

        // Check if transform's type is derived from RagdollAction and return the type if it is
        foreach (var type in types)
        {
            if (transform.GetComponent(type) != null)
            {
                return type;
            }
        }

        // Return null if no derived type is found
        return null;
    }
}
