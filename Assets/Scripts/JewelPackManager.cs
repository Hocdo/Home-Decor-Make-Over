using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class JewelPackManager : MonoBehaviour
{
    public SpawnPool poolBlock;
    public SpawnPool poolEffect;

    public List<JewelPack> jewelPacks;

    public void Awake()
    {
        if (jewelPacks.Count > 0)
        {
            var match3Data = PlayerData.current.match3Data;

            var jewelPack = jewelPacks.Find((x) => x != null && x.id == match3Data.jewelPackId);
            if (jewelPack != null)
            {
                ChangePrefab(poolBlock, 0, jewelPack.effSimpleB);
                ChangePrefab(poolBlock, 1, jewelPack.effSimpleG);
                ChangePrefab(poolBlock, 2, jewelPack.effSimpleO);
                ChangePrefab(poolBlock, 3, jewelPack.effSimpleP);
                ChangePrefab(poolBlock, 4, jewelPack.effSimpleR);
                ChangePrefab(poolBlock, 5, jewelPack.effSimpleY);

                ChangePrefab(poolEffect, 0, jewelPack.effSimpleRemoveB);
                ChangePrefab(poolEffect, 1, jewelPack.effSimpleRemoveG);
                ChangePrefab(poolEffect, 2, jewelPack.effSimpleRemoveO);
                ChangePrefab(poolEffect, 3, jewelPack.effSimpleRemoveP);
                ChangePrefab(poolEffect, 4, jewelPack.effSimpleRemoveR);
                ChangePrefab(poolEffect, 5, jewelPack.effSimpleRemoveY);

                ChangePrefab(poolEffect, 45, jewelPack.effCollectBlockB);
                ChangePrefab(poolEffect, 46, jewelPack.effCollectBlockG);
                ChangePrefab(poolEffect, 47, jewelPack.effCollectBlockO);
                ChangePrefab(poolEffect, 48, jewelPack.effCollectBlockP);
                ChangePrefab(poolEffect, 49, jewelPack.effCollectBlockR);
                ChangePrefab(poolEffect, 50, jewelPack.effCollectBlockY);
            }
        }

        poolBlock.Initialize();
        poolEffect.Initialize();
    }

    private void ChangePrefab(SpawnPool spawnPool, int index, GameObject prefab)
    {
        var prefabPool = spawnPool._perPrefabPoolOptions[index];
        prefabPool.prefab = prefab.transform;
    }

}