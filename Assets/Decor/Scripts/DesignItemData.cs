using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Decor
{
    [Serializable]
    public class DesignItemData
    {
        #region Public Data
        [Range(-1, 2)]
        public int variantIndex = 0;

        public string id;

        public string displayName;

        public int variantCount = 3;

        public int unlockedCountToUnlock = 0;

        public Currency costToUnlock;

        public Currency[] variantCosts;
        #endregion

        #region Private Data

        [NonSerialized] public bool unlocked;
        [NonSerialized] public int variantUnlockedBits = 0;

        #endregion

        #region Public Method
        public void Unlock()
        {
            unlocked = true;
        }

        public bool IsUnlocked()
        {
            return unlocked;
        }

        public void UnlockVariant(int idx)
        {
            int bit = 1 << idx;
            variantUnlockedBits |= bit;
        }

        public bool IsVariantUnlocked(int idx)
        {
            int bit = 1 << idx;
            return (variantUnlockedBits & bit) != 0;
        }

        public bool IsAllVariantUnlocked()
        {
            for (int i = 0; i < variantCount; i++)
            {
                if (!IsVariantUnlocked(i)) 
                    return false;
            }

            return true;
        }

        #endregion
    }
}

