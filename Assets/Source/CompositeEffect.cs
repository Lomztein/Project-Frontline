using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompositeEffect : Effect
{
    public Effect[] Effects;

    public override bool IsPlaying => Effects.Any(x => x.IsPlaying);

    public override void Play()
    {
        base.Play();
        foreach (Effect effect in Effects)
        {
            effect.Play();
        }
    }

    public override void Stop()
    {
        foreach (Effect effect in Effects)
        {
            effect.Stop();
        }
    }
}
