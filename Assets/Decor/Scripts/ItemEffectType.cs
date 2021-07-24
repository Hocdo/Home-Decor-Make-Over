using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemEffectType", menuName = "ScriptableObjects/ItemEffectType", order = 1)]
public class ItemEffectType : ScriptableObject
{
    public enum InternalType
    {
        HorizontalBlendWithParticle,
        BounceScaleWithParticle,
        ParticleOnly
    }

    public InternalType type;
}

public enum DesignItemEffectType
{
    HorizontalBlendWithParticle = 122,
    BounceScaleWithParticle = 483,
    OnlyParticle = 859
}
