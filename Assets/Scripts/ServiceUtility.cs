using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceUtility 
{
    public static bool InternetAvailable
    {
        get 
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
        
    }
}
