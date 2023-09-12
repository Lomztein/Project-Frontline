using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New CascadeDamageModifier", menuName = "Damage Modifiers/Cascade")]
public class CascadeDamageModifier : DamageModifier
{
    public Option[] Options;
    public float Fallback = 1f;

    protected override float GetValue_Internal(DamageModifier target)
    {
        foreach (Option option in Options)
        {
            if (option.AnyOf.Any(x => target.Is(x)))
            {
                return option.Value;
            }
        }
        return Fallback;
    }

    [System.Serializable]
    public class Option
    {
        public string Name;
        public DamageModifier[] AnyOf;
        public float Value;
    }
}
