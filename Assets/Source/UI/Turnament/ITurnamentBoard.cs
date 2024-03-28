using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurnamentBoard
{
    public void Initialize(TurnamentRunner controller);

    public void HighlightMatch(TurnamentMatchNode matchToHighlight);
}
