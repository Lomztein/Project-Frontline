using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

public class TurnamentSetup : MonoBehaviour
{
    public PlayerInfo[] PlayerInfos;
    public bool Shuffle;
    public bool All;

    private void Start()
    {
        IEnumerable<PlayerInfo> playerList;
        if (!All)
        {
            playerList = PlayerInfos;
        }
        else
        {
            List<PlayerInfo> list = new List<PlayerInfo>();
            foreach (var faction in Faction.LoadFactions())
            {
                foreach (var ai in AIPlayerProfile.LoadAll())
                {
                    PlayerInfo newInfo = new PlayerInfo();
                    newInfo.Name = faction.Name + " - " + ai.Name;
                    newInfo.StartingCredits = 500;
                    newInfo.PlayerInputDeviceId = -1;
                    newInfo.Faction = faction;
                    newInfo.AIProfile = ai;

                    list.Add(newInfo);
                }
            }

            playerList = list;
        }


        if (Shuffle)
        {
            TurnamentRunner.GetInstance().StartTurnament(playerList.Shuffle());
        }
        else
        {
            TurnamentRunner.GetInstance().StartTurnament(playerList);
        }
    }
}
