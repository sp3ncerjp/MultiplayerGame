using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class ScoreSettings : NetworkBehaviour           //Doesn't Work :/
{
    [SerializeField] private TextMeshProUGUI scoreTotal;
    private NetworkVariable<FixedString128Bytes> networkscoreTotal =
        new NetworkVariable<FixedString128Bytes>("Score: 0", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public void Update()
    {
        networkscoreTotal =
        new NetworkVariable<FixedString128Bytes>("Score: " + ScoreManager.Instance.GetScore(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        scoreTotal.text = networkscoreTotal.Value.ToString();
    }

}