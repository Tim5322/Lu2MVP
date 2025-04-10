using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YourNamespace.ApiClient;

public class ObjectManager : MonoBehaviour
{
    // Menu om objecten vanuit te plaatsen
    public GameObject UISideMenu;
    // Lijst met objecten die geplaatst kunnen worden die overeenkomen met de prefabs in de prefabs map
    public List<GameObject> prefabObjects;

    // Lijst met objecten die geplaatst zijn in de wereld
    private List<GameObject> placedObjects = new List<GameObject>();

    // Referentie naar Object2DApiClient
    public Object2DApiClient object2DApiClient;
    public Environment2DApiClient environment2DApiClient;

    public IsDragging isDragging;

    // Veld voor het actieve environmentId
    public string activeEnvironmentId { get; private set; }

    // Methode om een nieuw 2D object te plaatsen
    public void PlaceNewObject2D(int index)
    {
        // Verberg het zijmenu
        UISideMenu.SetActive(false);
        // Instantieer het prefab object op de positie (0,0,0) met geen rotatie
        GameObject instanceOfPrefab = Instantiate(prefabObjects[index], Vector3.zero, Quaternion.identity);
        // Haal het IsDragging component op van het nieuw geplaatste object
        IsDragging isDragging = instanceOfPrefab.GetComponent<IsDragging>();
        // Stel de objectManager van het object in op deze instantie van ObjectManager
        isDragging.objectManager = this;
        // Zet de isDragging eigenschap van het object op true zodat het gesleept kan worden
        isDragging.isDragging = true;
        // Stel het environmentId in op het IsDragging component
        isDragging.environmentId = activeEnvironmentId;
    }

    // Methode om het actieve environmentId in te stellen
    public void SetActiveEnvironmentId(string environmentId)
    {
        activeEnvironmentId = environmentId;
    }

    // Methode om het menu te tonen
    public void ShowMenu()
    {
        UISideMenu.SetActive(true);
    }

    // Methode om de huidige sc�ne te resetten
    public void Reset()
    {
        // Laad de huidige sc�ne opnieuw
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Methode om een nieuw Object2D aan te maken in de API
    public async void CreateObject2DInApi(Object2D object2D)
    {
        // Zorg ervoor dat het object het juiste environment2DId heeft
        object2D.environment2DId = activeEnvironmentId;

        Debug.Log($"Creating new Object2D with data: {JsonUtility.ToJson(object2D)}");
        IWebRequestReponse response = await object2DApiClient.CreateObject2D(object2D);
        Debug.Log($"Received response: {response}");
        if (response is WebRequestData<Object2D> createdObject2D)
        {
            Debug.Log($"Object2D created successfully with ID: {createdObject2D.Data.id}");
            // Update the local object with the ID from the backend
            object2D.id = createdObject2D.Data.id;
        }
        else
        {
            Debug.LogError("Failed to create Object2D.");
        }
    }

    // Methode om alle Object2D's voor het actieve environmentId op te halen
    public async void LoadObjectsForActiveEnvironment()
    {
        if (string.IsNullOrEmpty(activeEnvironmentId))
        {
            Debug.LogError("Active environment ID is not set.");
            return;
        }

        Debug.Log($"Loading Object2Ds for environment ID: {activeEnvironmentId}");
        IWebRequestReponse response = await object2DApiClient.ReadObject2Ds(activeEnvironmentId);
        if (response is WebRequestData<List<Object2D>> object2DListResponse)
        {
            List<Object2D> object2DList = object2DListResponse.Data;
            Debug.Log($"Loaded {object2DList.Count} Object2Ds.");
            // Process the loaded objects as needed
            foreach (var object2D in object2DList)
            {
                Debug.Log($"Object2D: {JsonUtility.ToJson(object2D)}");
                // Instantiate or update objects in the scene based on the loaded data
                InstantiateObject2D(object2D);
            }
        }
        else
        {
            Debug.LogError("Failed to load Object2Ds.");
        }
    }

    // Helper method to instantiate or update Object2D in the scene
    private void InstantiateObject2D(Object2D object2D)
    {
        // Find the prefab by name
        GameObject prefab = prefabObjects.Find(p => p.name == object2D.prefabId);
        if (prefab == null)
        {
            Debug.LogError($"Prefab with name {object2D.prefabId} not found.");
            return;
        }

        // Instantiate the prefab at the specified position
        Vector3 position = new Vector3(object2D.positionX, object2D.positionY, 0);
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);

        // Set the environment ID and other properties if needed
        IsDragging isDragging = instance.GetComponent<IsDragging>();
        if (isDragging != null)
        {
            isDragging.environmentId = object2D.environment2DId;
            isDragging.objectManager = this;
        }

        // Name the instance and parent it under a specific GameObject in the hierarchy
        instance.name = $"{prefab.name}_Clone_{object2D.id}";
        instance.transform.SetParent(this.transform);

        // Add the instance to the placedObjects list
        placedObjects.Add(instance);

        // Log the instantiation
        Debug.Log($"Instantiated {instance.name} at position {position}");
    }

    // Update method to check for Enter key press
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadObjectsForActiveEnvironment();
        }
    }
}








