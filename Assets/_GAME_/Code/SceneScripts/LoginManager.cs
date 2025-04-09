using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public Button registerButton;
    public Button loginButton;
    public Toggle showPasswordToggle; // Toggle om het wachtwoord te verbergen of weer te geven
    public UserApiClient userApiClient;
    public GameObject SceneLoginRegister; // Referentie naar het login/register paneel
    public GameObject ChooseEnvironment;  // Referentie naar het choose environment paneel
    public EnvironmentManager environmentManager; // Referentie naar de EnvironmentManager

    private void Start()
    {
        registerButton.onClick.AddListener(Register);
        loginButton.onClick.AddListener(Login);
        showPasswordToggle.onValueChanged.AddListener(TogglePasswordVisibility); // Voeg de listener toe voor de toggle
        passwordInputField.contentType = TMP_InputField.ContentType.Password; // Standaard wachtwoord verbergen
    }

    public async void Register()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        User user = new User
        {
            email = email,
            password = password
        };

        try
        {
            IWebRequestReponse webRequestResponse = await userApiClient.Register(user);

            switch (webRequestResponse)
            {
                case WebRequestData<string> dataResponse:
                    Debug.Log("Register success! Response: " + dataResponse.Data);
                    string token = dataResponse.Data;
                    PlayerPrefs.SetString("authToken", token); // Save the token
                    PlayerPrefs.Save();
                    break;
                case WebRequestError errorResponse:
                    string errorMessage = errorResponse.ErrorMessage;
                    Debug.Log("Register error: " + errorMessage);
                    break;
                default:
                    throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Register exception: " + ex.Message);
        }
    }

    public async void Login()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        User user = new User
        {
            email = email,
            password = password
        };

        try
        {
            IWebRequestReponse webRequestResponse = await userApiClient.Login(user);

            switch (webRequestResponse)
            {
                case WebRequestData<string> dataResponse:
                    Debug.Log("Login success! Response: " + dataResponse.Data);
                    string token = dataResponse.Data;
                    PlayerPrefs.SetString("authToken", token); // Save the token
                    PlayerPrefs.Save();
                    SceneLoginRegister.SetActive(false); // Verberg het login/register paneel
                    ChooseEnvironment.SetActive(true);  // Toon het choose environment paneel
                    environmentManager.FetchEnvironments(); // Fetch environments after login
                    break;
                case WebRequestError errorResponse:
                    string errorMessage = errorResponse.ErrorMessage;
                    Debug.Log("Login error: " + errorMessage);
                    break;
                default:
                    throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Login exception: " + ex.Message);
        }
    }

    private void TogglePasswordVisibility(bool isVisible)
    {
        passwordInputField.contentType = isVisible ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        passwordInputField.ForceLabelUpdate(); // Forceer een update om de wijziging door te voeren
    }
}

