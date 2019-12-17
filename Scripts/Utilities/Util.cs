using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static void SetLayerRecursively(GameObject obj, int layer)
    {
        if (!obj) return;

        obj.layer = layer;

        foreach(Transform child in obj.transform)
        {
            if (!child) continue;

            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
