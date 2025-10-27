using UnityEngine;

[CreateAssetMenu()]
public class PlayerDataSO : ScriptableObject
{
    public UserData userData;

    public void InitializeUserData(UserData data)
    {
        userData = data;
    }

    public void UpdateGameStats(int gamesPlayed, int wins, int platesDelivered)
    {
        userData.GamesPlayed += gamesPlayed;
        userData.Wins += wins;
        userData.PlatesDelivered += platesDelivered;
    }
}
