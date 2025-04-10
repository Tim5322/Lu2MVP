using System;
using System.Collections.Generic;
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

    // Enter buttons for environments
    public Button enterButton1;
    public Button enterButton2;
    public Button enterButton3;

    // Exit button
    public Button exitButton;

    // Warning text
    [SerializeField] private TMP_Text warningText;

    // All Scenes
    public GameObject Scene2;
    public GameObject Scene3;
    public GameObject Scene4; // Referentie naar Scene4

    // All important elements of scene 2
    public TMP_Text Env1Text;
    public TMP_Text Env2Text;
    public TMP_Text Env3Text;

    // Environment names
    private string environmentName1;
    private string environmentName2;
    private string environmentName3;
    private int currentEnvironmentIndex;

    // Environment IDs
    private string environmentId1;
    private string environmentId2;
    private string environmentId3;

    // API Client
    public Environment2DApiClient environment2DApiClient;
    public ObjectManager objectManager; // Reference to ObjectManager

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

        // Voeg listeners toe aan de enter-knoppen
        enterButton1.onClick.AddListener(() => SetActiveEnvironment(1));
        enterButton2.onClick.AddListener(() => SetActiveEnvironment(2));
        enterButton3.onClick.AddListener(() => SetActiveEnvironment(3));

        // Voeg listener toe aan de exit-knop
        exitButton.onClick.AddListener(ExitToScene2);

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
            enterButton1.interactable = true;
        }
        else
        {
            Env1Text.text = "No Environment";
            deleteButton1.interactable = false;
            createButton1.gameObject.SetActive(true);
            enterButton1.interactable = false;
        }

        if (!string.IsNullOrEmpty(environmentName2))
        {
            Env2Text.text = "Environment 2: " + environmentName2;
            deleteButton2.interactable = true;
            createButton2.gameObject.SetActive(false);
            enterButton2.interactable = true;
        }
        else
        {
            Env2Text.text = "No Environment";
            deleteButton2.interactable = false;
            createButton2.gameObject.SetActive(true);
            enterButton2.interactable = false;
        }

        if (!string.IsNullOrEmpty(environmentName3))
        {
            Env3Text.text = "Environment 3: " + environmentName3;
            deleteButton3.interactable = true;
            createButton3.gameObject.SetActive(false);
            enterButton3.interactable = true;
        }
        else
        {
            Env3Text.text = "No Environment";
            deleteButton3.interactable = false;
            createButton3.gameObject.SetActive(true);
            enterButton3.interactable = false;
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
                    Environment2D createdEnvironment = dataResponse.Data;
                    switch (currentEnvironmentIndex)
                    {
                        case 1:
                            environmentName1 = createdEnvironment.name;
                            environmentId1 = createdEnvironment.id;
                            break;
                        case 2:
                            environmentName2 = createdEnvironment.name;
                            environmentId2 = createdEnvironment.id;
                            break;
                        case 3:
                            environmentName3 = createdEnvironment.name;
                            environmentId3 = createdEnvironment.id;
                            break;
                    }

                    PlayerPrefs.SetString("EnvironmentName" + currentEnvironmentIndex, createdEnvironment.name);
                    PlayerPrefs.SetString("EnvironmentId" + currentEnvironmentIndex, createdEnvironment.id);
                    UpdateUI();
                    BackToScene2();

                    // Update the active environment ID in ObjectManager
                    objectManager.SetActiveEnvironmentId(createdEnvironment.id);
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

    public async void FetchEnvironments()
    {
        Debug.Log("Fetching environments...");

        IWebRequestReponse webRequestResponse = await environment2DApiClient.ReadEnvironment2Ds();

        switch (webRequestResponse)
        {
            case WebRequestData<List<Environment2D>> dataResponse:
                List<Environment2D> environments = dataResponse.Data;
                DisplayEnvironments(environments);
                break;
            case WebRequestError errorResponse:
                Debug.LogError("Read environments error: " + errorResponse.ErrorMessage);
                break;
            default:
                throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
        }
    }

    private void DisplayEnvironments(List<Environment2D> environments)
    {
        // Clear existing environment names and IDs
        environmentName1 = null;
        environmentName2 = null;
        environmentName3 = null;
        environmentId1 = null;
        environmentId2 = null;
        environmentId3 = null;

        // Display environment names and IDs
        for (int i = 0; i < environments.Count; i++)
        {
            switch (i)
            {
                case 0:
                    environmentName1 = environments[i].name;
                    environmentId1 = environments[i].id;
                    break;
                case 1:
                    environmentName2 = environments[i].name;
                    environmentId2 = environments[i].id;
                    break;
                case 2:
                    environmentName3 = environments[i].name;
                    environmentId3 = environments[i].id;
                    break;
            }
        }

        UpdateUI();
    }

    private async void DeleteEnvironment(int index)
    {
        string environmentId = null;
        switch (index)
        {
            case 1:
                environmentId = environmentId1;
                environmentName1 = null;
                environmentId1 = null;
                break;
            case 2:
                environmentId = environmentId2;
                environmentName2 = null;
                environmentId2 = null;
                break;
            case 3:
                environmentId = environmentId3;
                environmentName3 = null;
                environmentId3 = null;
                break;
        }

        if (environmentId != null)
        {
            try
            {
                IWebRequestReponse webRequestResponse = await environment2DApiClient.DeleteEnvironment(environmentId);

                switch (webRequestResponse)
                {
                    case WebRequestData<string> dataResponse:
                        Debug.Log("Environment deletion success!");
                        PlayerPrefs.DeleteKey("EnvironmentName" + index);
                        PlayerPrefs.DeleteKey("EnvironmentId" + index);
                        UpdateUI();
                        break;
                    case WebRequestError errorResponse:
                        string errorMessage = errorResponse.ErrorMessage;
                        Debug.Log("Environment deletion error: " + errorMessage);
                        break;
                    default:
                        throw new NotImplementedException("No implementation for webRequestResponse of class: " + webRequestResponse.GetType());
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Environment deletion exception: " + ex.Message);
            }
        }
    }

    public void SetActiveEnvironment(int index)
    {
        // Hier kun je de logica toevoegen om de objecten te laden die bij de geselecteerde environment horen
        Debug.Log($"Environment {index} is now active.");
        // Laad de objecten die bij de geselecteerde environment horen
        LoadObjectsForEnvironment(index);

        // Zet Scene4 op active en Scene2 op inactive
        Scene4.SetActive(true);
        Scene2.SetActive(false);

        // Update the active environment ID in ObjectManager
        switch (index)
        {
            case 1:
                objectManager.SetActiveEnvironmentId(environmentId1);
                break;
            case 2:
                objectManager.SetActiveEnvironmentId(environmentId2);
                break;
            case 3:
                objectManager.SetActiveEnvironmentId(environmentId3);
                break;
        }
    }

    private void LoadObjectsForEnvironment(int index)
    {
        // Laad de objecten die bij de geselecteerde environment horen
        // Dit is een voorbeeld en moet worden aangepast aan jouw specifieke logica
        Debug.Log($"Loading objects for environment {index}.");
    }

    private void BackToScene2()
    {
        Scene3.SetActive(false);
        Scene2.SetActive(true);
        environmentNameInputField.text = "";
        environmentNameInputField.onValueChanged.RemoveListener(OnEnvironmentNameChanged);
        warningText.gameObject.SetActive(false); // Hide warning text when going back
    }

    private void ExitToScene2()
    {
        Scene4.SetActive(false);
        Scene2.SetActive(true);
    }
}

















