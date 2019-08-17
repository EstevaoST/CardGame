using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine.UI;
using Globals;

public class CardGameManager : CardGameManagerBehavior {
    const char NAMES_SEPARATOR = '@';

    public List<CardGamePlayer> players = new List<CardGamePlayer>();
    public CardGamePlayer me;
    public Animator Phases;
    public GameState state;

    public Text turnHeader;
    public Text playerList;

    internal void AddPlayer(CardGamePlayer cardGamePlayer)
    {
        if (networkObject.IsServer)
        {
            players.Add(cardGamePlayer);

            string namesString = "";
            foreach (var player in players)         
                namesString += player.name + NAMES_SEPARATOR + player.networkObject.NetworkId + NAMES_SEPARATOR;
            
            networkObject.SendRpc(RPC_CONNECT_RESULT, Receivers.Others, namesString);
            for (int i = 0; i < players.Count; i++)         
            {
                var player = players[i];
                player.transform.Rotate(0, 0, -player.transform.rotation.z);
                player.transform.Rotate(0, 0, i * 360 / players.Count);
            }
        }

        cardGamePlayer.nextChanged += Next;

        RefreshPlayerList();
    }

    // Use this for initialization
    void Start ()
    {
        me = NetworkManager.Instance.InstantiateCardGamePlayer() as CardGamePlayer;
        Camera.main.transform.parent = me.transform;
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;
    }
	
    public void Next()
    {
        if (networkObject.IsServer)
        {
            bool next = true;
            foreach (var item in Component.FindObjectsOfType<CardGamePlayer>())
                next &= item.next;

            if (next)
            {
                if (state.phase != TurnPhase.End)
                    state.phase++;
                else
                {
                    state.phase = TurnPhase.Upkeep;
                    state.turnCount = (state.turnCount + 1) % players.Count;
                }

                networkObject.SendRpc(RPC_FULL_STATE_UPDATE, Receivers.Others, Utils.SerializeObject<GameState>(state));
                RefreshText();
            }
        }
    }

    private void RefreshText()
    {
        turnHeader.text = players[state.turnCount % players.Count].name + "'s " + state.phase.ToString();
    }
    private void RefreshPlayerList()
    {
        string list = "";
        foreach (var item in players)
        {
            if (list == "")
                list += item.name;
            else
                list += "\n" + item.name;

            list += " - " + item.networkObject.MyPlayerId + "//" + item.networkObject.NetworkId;
        }
        playerList.text = list;
    }

    #region RPCs
    public override void Connect(RpcArgs args)
    {
        //string name = args.GetNext<string>();
        //names.Add(name);
        //Debug.Log(name + " - Connected");

        //string allNames = "";
        //foreach (var item in names)
        //    if (allNames == "")
        //        allNames += item;
        //    else
        //        allNames += NAMES_SEPARATOR + item;
        //RefreshPlayerList();

        //networkObject.SendRpc(RPC_CONNECT_RESULT, Receivers.All, allNames);
    }
    public override void ConnectResult(RpcArgs args)
    {
        //if (!networkObject.IsServer)
        {
            var names = args.GetNext<string>().Split(NAMES_SEPARATOR);
            string name;
            uint id;
            CardGamePlayer player;
            var players = new List<CardGamePlayer>(FindObjectsOfType<CardGamePlayer>());
            this.players.Clear();

            for (int i = 0; i < names.Length - 1; i += 2)
            {
                name = names[i];
                id = uint.Parse(names[i + 1]);

                player = players.Find(x => x.networkObject.NetworkId == id);
                if (player != null)
                {
                    player.transform.Rotate(0, 0, -player.transform.rotation.z);
                    player.transform.Rotate(0,0,this.players.Count * 360 / players.Count);

                    player.name = name;
                    this.players.Add(player);
                }
            }
            
            RefreshPlayerList();
        }
    }
    public override void FullStateUpdate(RpcArgs args)
    {
        state = Utils.DeserializeObject<GameState>(args.GetNext<string>());
        RefreshText();
    }
    #endregion
}
