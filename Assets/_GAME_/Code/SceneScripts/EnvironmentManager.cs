using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentManager : MonoBehaviour
{
    // All buttons and other UI elements
    [SerializeField] private TMP_InputField environmentNameInputField;
    [SerializeField] private Button createButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button createButton1;
    [SerializeField] private Button createButton2;
    [SerializeField] private Button createButton3;
    [SerializeField] private Button deleteButton1;
    [SerializeField] private Button deleteButton2;
    [SerializeField] private Button deleteButton3;

    // Warning text
    [SerializeField] private TMP_Text warningText;

    // All Scenes
    public GameObject Scene2;
    public GameObject Scene3;

    // All important elements of scene 2
    public TMP_Text Env1Text;
    public TMP_Text Env2Text;
    public TMP_Text Env3Text;

    // Environment names
    private string environmentName1;
    private string environmentName2;
    private string environmentName3;
    private int currentEnvironmentIndex;

    // API Client
    public Environment2DApiClient environment2DApiClient;

    private void Start()
    {
        createButton.onClick.AddListener(CreateEnvironment);
        backButton.onClick.AddListener(BackToScene2);

        createButton1.onClick.AddListener(() => OpenCreateEnvironment(1));
        createButton2.onClick.AddListener(() => OpenCreateEnvironment(2));
        createButton3.onClick.AddListener(() => OpenCreateEnvironment(3));

        deleteButton1.onClick.AddListener(() => DeleteEnvironment(1));
        deleteButton2.onClick.AddListener(() => DeleteEnvironment(2));
        deleteButton3.onClick.AddListener(() => DeleteEnvironment(3));

        UpdateUI();
        warningText.gameObject.SetActive(false); // Hide warning text initially
    }

    private void UpdateUI()
    {
        // Update the UI based on the current state of the environments
        if (!string.IsNullOrEmpty(environmentName1))
        {
            Env1Text.text = "Environment 1: " + environmentName1;
            deleteButton1.interactable = true;
            createButton1.gameObject.SetActive(false);
        }
        else
        {
            Env1Text.text = "No Environment";
            deleteButton1.interactable = false;
            createButton1.gameObject.SetActive(true);
        }

        if (!string.IsNullOrEmpty(environmentName2))
        {
            Env2Text.text = "Environment 2: " + environmentName2;
            deleteButton2.interactable = true;
            createButton2.gameObject.SetActive(false);
        }
        else
        {
            Env2Text.text = "No Environment";
            deleteButton2.interactable = false;
            createButton2.gameObject.SetActive(true);
        }

        if (!string.IsNullOrEmpty(environmentName3))
        {
            Env3Text.text = "Environment 3: " + environmentName3;
            deleteButton3.interactable = true;
            createButton3.gameObject.SetActive(false);
        }
        else
        {
            Env3Text.text = "No Environment";
            deleteButton3.interactable = false;
            createButton3.gameObject.SetActive(true);
        }
    }

    private void OpenCreateEnvironment(int index)
    {
        currentEnvironmentIndex = index;
        Scene2.SetActive(false);
        Scene3.SetActive(true);
        createButton.interactable = false;
        environmentNameInputField.onValueChanged.AddListener(OnEnvironmentNameChanged);
    }

    private void OnEnvironmentNameChanged(string environmentName)
    {
        createButton.interactable = !string.IsNullOrEmpty(environmentName) && environmentName.Length <= 15;
        warningText.gameObject.SetActive(false); // Hide warning text when typing
    }

    private async void CreateEnvironment()
    {
        string environmentName = environmentNameInputField.text;

        if (environmentName.Length > 25)
        {
            Debug.LogWarning("Environment name must be 25 characters or less.");
            return;
        }
        if (environmentName.Length == 0)
        {
            Debug.LogWarning("Environment name must be between 1 and 25 characters long.");
            return;
        }
        if (environmentName == environmentName1 || environmentName == environmentName2 || environmentName == environmentName3)
        {
            Debug.LogWarning("Environment name must be unique.");
            warningText.text = "Environment name must be unique.";
            warningText.gameObject.SetActive(true); // Show warning text
            return;
        }

        Environment2D environment2D = new Environment2D
        {
            name = environmentName
        };

        try
        {
            IWebRequestReponse webRequestResponse = await environment2DApiClient.CreateEnvironment(environment2D);

            switch (webRequestResponse)
            {
                case WebRequestData<Environment2D> dataResponse:
                    Debug.Log("Environment creation success!");
                    switch (currentEnvironmentIndex)
                    {
                        case 1:
                            environmentName1 = environmentName;
                            break;
                        case 2:
                            environmentName2 = environmentName;
                            break;
                        case 3:
                            environmentName3 = environmentName;
                            break;
                    }

                    PlayerPrefs.SetString("EnvironmentName" + currentEnvironmentIndex, environmentName);
                    UpdateUI();
                    BackToScene2();
                    break;
                case WebRequestError errorResponse:
                    string errorMessage = errorResponse.ErrorMessage;
                    Debug.Log("Environment creation error: " + errorMessage);
                    break;
                default:
                    throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Environment creation exception: " + ex.Message);
        }
    }


    private void DeleteEnvironment(int index)
    {
        switch (index)
        {
            case 1:
                environmentName1 = null;
                break;
            case 2:
                environmentName2 = null;
                break;
            case 3:
                environmentName3 = null;
                break;
        }

        PlayerPrefs.DeleteKey("EnvironmentName" + index);
        Debug.Log("Environment " + index + " deleted.");
        UpdateUI();
    }

    private void BackToScene2()
    {
        Scene3.SetActive(false);
        Scene2.SetActive(true);
        environmentNameInputField.text = "";
        environmentNameInputField.onValueChanged.RemoveListener(OnEnvironmentNameChanged);
        warningText.gameObject.SetActive(false); // Hide warning text when going back
    }
}





