using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PlayerRegistration : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_InputField gameTagField;

    public void RegisterPlayer()
    {
        string username = usernameField.text.Trim();
        string password = passwordField.text;
        string gameTag = gameTagField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.Log($"Registrando: {username}, {password}, {gameTag}");
            Debug.LogError("El nombre de usuario o la contraseña están vacíos");
            return;
        }
        StartCoroutine(RegisterCoroutine(username, password, gameTag));
    }

    private IEnumerator RegisterCoroutine(string username, string password, string gameTag)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        form.AddField("gameTag", gameTag);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/chefrenzy/register_player.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error en el registro: " + www.error);
            }
        }
    }
}