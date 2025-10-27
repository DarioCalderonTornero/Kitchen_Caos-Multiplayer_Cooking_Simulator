using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class PlayerLogin : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;

    public UserData userData; // Objeto para almacenar datos del usuario

    public void LoginPlayer()
    {
        string username = usernameField.text.Trim();
        string password = passwordField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("El nombre de usuario o la contraseña están vacíos.");
            return;
        }

        StartCoroutine(LoginCoroutine(username, password));
    }

    private IEnumerator LoginCoroutine(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/chefrenzy/login_player.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Debug.Log("Respuesta del servidor: " + json); // Imprime el JSON para ver qué devuelve PHP

                // Manejo de posibles errores en la respuesta
                if (json.Contains("error"))
                {
                    // Si la respuesta contiene "error", extrae y muestra el mensaje de error
                    try
                    {
                        var errorResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                        if (errorResponse.ContainsKey("error"))
                        {
                            Debug.LogError("Error del servidor: " + errorResponse["error"]);
                        }
                    }
                    catch (JsonException ex)
                    {
                        Debug.LogError("Error al procesar la respuesta JSON de error: " + ex.Message);
                    }
                }
                else
                {
                    // Procesa los datos del jugador si no hay error
                    try
                    {
                        userData = JsonConvert.DeserializeObject<UserData>(json);

                        if (userData != null)
                        {
                            Debug.Log($"Inicio de sesión exitoso. Username: {userData.Username}, GameTag: {userData.GameTag}");

                            // Inicializa los datos de usuario
                            PlayerSessionManager.Instance.InitializeUserData(userData);

                            // Cargar la escena principal
                            SceneManager.LoadScene("MainMenu");
                        }
                        else
                        {
                            Debug.LogError("Error: El servidor devolvió datos incompletos o mal formateados.");
                        }
                    }
                    catch (JsonException ex)
                    {
                        Debug.LogError("Error al procesar la respuesta JSON: " + ex.Message);
                    }
                }
            }
            else
            {
                Debug.LogError("Error en la conexión con el servidor: " + www.error);
            }
        }
    }
}