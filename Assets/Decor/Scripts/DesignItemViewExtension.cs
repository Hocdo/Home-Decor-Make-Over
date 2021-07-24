using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Decor
{
    public static class DesignItemViewExtension
    {
        public static string GetId(this DesignItemView item)
        {
            return item.primaryData.id;
        }

        public static bool HasDefaultVariant(this DesignItemView item)
        {
            return item.subVisuals[0].defaultVariant.sprite != null;
        }

        public static GameObject GetIcon(this DesignItemView item)
        {
            return item.transform.Find("icon").gameObject;
        }

        public static bool IsUnlocked(this DesignItemView item)
        {
            return item.primaryData.IsUnlocked();
        }

        public static int GetUnlockedCountToUnlock(this DesignItemView item)
        {
            return item.primaryData.unlockedCountToUnlock;
        }

        public static bool IsDependenciesUnlocked(this DesignItemView item)
        {
            if (item.unlockDependencies == null) return true;

            for (int i = 0; i < item.unlockDependencies.Length; i++)
            {
                if (item.unlockDependencies[i].IsUnlocked() == false)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsDependenciesUnlocked(this DesignItemView item, DesignItemView current)
        {
            if (item.unlockDependencies == null) return true;

            for (int i = 0; i < item.unlockDependencies.Length; i++)
            {
                if (item.unlockDependencies[i].IsUnlocked() == false && item.unlockDependencies[i] != current)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
