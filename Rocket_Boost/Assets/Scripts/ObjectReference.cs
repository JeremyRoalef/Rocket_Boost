using UnityEngine;

public static class ObjectReference
{
    /// <summary>
    /// Method to determine whether the given object reference is null
    /// </summary>
    /// <param name="reference">the object reference</param>
    /// <param name="warningIfNull">the message displayed to console if null</param>
    /// <returns>true if null. false if not null</returns>
    public static bool IsNull(object reference, string warningIfNull)
    {
        if (reference == null)
        {
            Debug.LogWarning(warningIfNull);
            return true;
        }
        else
        {
            return false;
        }
    }
}
