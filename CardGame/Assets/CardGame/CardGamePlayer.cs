using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using UnityEngine;
using UnityEngine.UI;
using Globals;
using System;

public class CardGamePlayer : CardGamePlayerBehavior
{
    public bool next;
    public bool keepSkipping;

    public List<CardReference> deck, hand, field;
    public RectTransform baseField;
    public Text deckText, handText, fieldText;
    public System.Action nextChanged;

    public Toggle nextButton;

    void Start () {
        //setup network variables
        if (networkObject.IsOwner)
        {
            nextButton.transform.SetParent(Component.FindObjectOfType<Canvas>().transform);
            if (networkObject.IsServer)
                name = "Host " + networkObject.MyPlayerId;
            else
                name = "Client " + networkObject.MyPlayerId;
            networkObject.SendRpc(RPC_SET_NAME, Receivers.All, name);
        }
        else
        {
            Destroy(nextButton.gameObject);
            nextButton = null;
        }

        //load deck
        if (networkObject.IsServer)
        {
            for(int i = 0; i<50; i++)
            {
                CardReference cr = new CardReference();
                deck.Add(cr);
            }
        }
    }
	void Update () {
        bool changed = false;

        if (!networkObject.IsOwner)
        {
            changed = next != networkObject.Skipping;
            next = networkObject.Skipping;
        }

        if (networkObject.IsServer && changed) 
        {
            if (nextChanged != null)
                nextChanged.Invoke();
        }
    }

    public void SwitchSkipping()
    {
        next = !next;
        networkObject.Skipping = next;

        nextButton.isOn = next;

        if (nextChanged != null)
            nextChanged.Invoke();
    }

    private void RefreshDeck()
    {
        deckText.text = deck.Count.ToString();
    }
    private void RefreshHand()
    {
        handText.text = hand.Count.ToString();
    }
    private void RefreshField()
    {
        fieldText.text = field.Count.ToString();
    }

    public override void SetName(RpcArgs args)
    {
        name = args.GetNext<string>();
        Component.FindObjectOfType<CardGameManager>().AddPlayer(this);
    }
    public override void SyncDeck(RpcArgs args)
    {
        deck = Utils.DeserializeObject<List<CardReference>>(args.GetNext<string>());
        RefreshDeck();
    }
    public override void SyncHand(RpcArgs args)
    {
        hand = Utils.DeserializeObject<List<CardReference>>(args.GetNext<string>());
        RefreshHand();
    }
    public override void SynField(RpcArgs args)
    {
        field = Utils.DeserializeObject<List<CardReference>>(args.GetNext<string>());
        RefreshField();
    }

    internal void DrawFromDeck(int qty = 1)
    {
        for (int i = 0; i < qty; i++)
        {
            hand.Add(deck[deck.Count - 1]);
            deck.RemoveAt(deck.Count - 1);
        }
    }

    internal void SyncAll()
    {
        networkObject.SendRpc(RPC_SYNC_DECK, Receivers.All, Utils.SerializeObject(deck));
        networkObject.SendRpc(RPC_SYNC_HAND, Receivers.All, Utils.SerializeObject(hand));
    }
}
