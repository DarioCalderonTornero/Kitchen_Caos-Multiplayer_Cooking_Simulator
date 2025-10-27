using UnityEngine;

public class PlayerSessionManager : MonoBehaviour
{
    public static PlayerSessionManager Instance { get; private set; }

    public UserData userData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Mantener la instancia entre escenas
    }

    // Método para inicializar los datos del usuario desde el servidor
    public void InitializeUserData(UserData data)
    {
        userData = data;
    }

    // Método para actualizar datos al final de una partida
    public void UpdateGameStats(int gamesPlayed, int wins, int platesDelivered)
    {
        userData.GamesPlayed += gamesPlayed;
        userData.Wins += wins;
        userData.PlatesDelivered += platesDelivered;
    }

    // Método para sincronizar los datos con el servidor
    public void SaveDataToServer()
    {
        // Aquí puedes implementar la lógica para enviar los datos actualizados al servidor
        Debug.Log("Sincronizando datos con el servidor...");
    }
}
