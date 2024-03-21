using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour        //Doesn't Work :/
{
    public static ScoreManager Instance;

    private NetworkVariable<int> score = new NetworkVariable<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddScoreServerRpc(int scoreToAdd)
    {
        score.Value += scoreToAdd;
    }

    [ClientRpc]
    private void UpdateScoreClientRpc(int newScore)
    {
        
    }

    public int GetScore()
    {
        return score.Value;
    }
}