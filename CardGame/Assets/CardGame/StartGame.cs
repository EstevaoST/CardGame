using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (NetworkManager.Instance.IsServer)
            NetworkManager.Instance.InstantiateCardGameManager();
    }

}
