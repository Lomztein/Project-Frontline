using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnamentMatchNode
{
    public TurnamentPlayerNode[] PlayerNodes;
    public TurnamentPlayerNode WinnerNode;

    public bool IsReady => !PlayerNodes.Any(x => x.PlayerInfo == null) && WinnerNode.PlayerInfo == null;

    public IEnumerable<PlayerInfo> GetFightPlayers()
    {
        return PlayerNodes.Select(x => x.PlayerInfo);
    }

    public void SetWinnerNodePlayer(PlayerInfo playerInfo)
    {
        WinnerNode.PlayerInfo = playerInfo;
    }

    public override string ToString()
    {
        return string.Join(" vs ", PlayerNodes.Select(x => x.ToString()));
    }
}
