using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PiggyBankData 
{
    public bool removed = false;

    public bool enterPeriod = false;

    public bool smashLvl5Flag;

    public int level = 1;
    public int gemCount = 0;
    public int missingCount = 0;

    public string nextLevelAvailaibleTime;
    public string periodExpireTime;
    public string periodAvailableTime;
}
