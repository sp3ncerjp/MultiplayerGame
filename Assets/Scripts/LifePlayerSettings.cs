using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class LifePlayerSettings : NetworkBehaviour          //Doesn't work :/
{
    [SerializeField] private TextMeshProUGUI LifeTotal;
    private NetworkVariable<FixedString128Bytes> networkLifeTotal =
        new NetworkVariable<FixedString128Bytes>("Life: 3", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public AnimationAndMovementController playerController;

    public void Update()
    {
        networkLifeTotal =
        new NetworkVariable<FixedString128Bytes>("Life: " + playerController.getLifeTotal(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        LifeTotal.text = networkLifeTotal.Value.ToString();
    }

}
