using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class PlayerSettings : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;                    //Text Mesh Variable set in the inspector
    private NetworkVariable<FixedString128Bytes> networkPlayerName =        //Initialize the name and put it on the network
        new NetworkVariable<FixedString128Bytes>("Player #0", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()                                   //Overide the netcodes on spawn function
    {
        networkPlayerName.Value = "Player #" + (OwnerClientId + 1);         //Grab the player name from the player's client id
        playerName.text = networkPlayerName.Value.ToString();               //Set the text mesh to the updated playername
    }
}
