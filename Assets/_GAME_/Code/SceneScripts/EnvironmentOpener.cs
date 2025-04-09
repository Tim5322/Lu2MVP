using UnityEngine;
using UnityEngine.UI;

public class EnvironmentOpener : MonoBehaviour
{
    public EnvironmentManager environmentManager;
    public int environmentIndex; // Index van de omgeving (1, 2 of 3)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Voeg een listener toe aan de knop om de juiste omgeving te openen
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OpenEnvironment);
        }
        else
        {
            Debug.LogError("Button component not found on this GameObject.");
        }

        // Controleer of environmentManager is ingesteld
        if (environmentManager == null)
        {
            Debug.LogError("EnvironmentManager is not assigned in the Inspector.");
        }
    }

    // Methode om de juiste omgeving te openen
    void OpenEnvironment()
    {
        if (environmentManager != null)
        {
            environmentManager.SetActiveEnvironment(environmentIndex);
        }
        else
        {
            Debug.LogError("EnvironmentManager is not assigned.");
        }
    }
}



