using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullCheckScript : MonoBehaviour
{
    public static NullCheckScript g_cInstance;
    //Name: NullCheck
    //Ins: T inComponent
    //Outs:
    //Function: Checks if inComponent is null. returns a bool. If false, throws a debug error.
    //-Petra
    public static bool NullCheck<T>(T inComponent)
    {
        if (inComponent != null)
        {
            return true;
        }
        else
        {
            //Debug.Log("inComponent was null.");
            return false;
        }
    }

    //Name: NullCheckElseError
    //Ins: T inComponent, string errorMessage
    //Outs:
    //Function: Checks if inComponent is null. If true, returns true If false, throws a debug error with the string errorMessage and returns false.
    //-Petra
    public static bool NullCheckElseError<T>(T inComponent, string errorMessage = "A component was passed to NullCheckManager but no error message was set")
    {
        if (NullCheck(inComponent))
        {
            return true;
        }
        else
        {
            Debug.LogError(errorMessage);
            return false;
        }
    }

    public static bool NullCheckElseWarning<T>(T inComponent, string errorMessage = "A component was passed to NullCheckManager but no error message was set")
    {
        if (NullCheck(inComponent))
        {
            return true;
        }
        else
        {
            Debug.LogWarning(errorMessage);
            return false;
        }
    }

}
