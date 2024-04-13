using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectExtention
{
    public static bool IsNull(this UnityEngine.Object obj)
    {
        return obj == null;
    }
    public static bool IsNotNull(this UnityEngine.Object obj)
    {
        return obj != null;
    }


    public static bool IsNull(this System.Object obj)
    {
        return obj == null;
    }

    public static bool IsNotNull(this System.Object obj)
    {
        return obj != null;
    }
}
