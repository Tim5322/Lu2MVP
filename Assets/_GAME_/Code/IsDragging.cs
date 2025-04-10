using System;
using UnityEngine;
using UnityEngine.Events;

public class IsDragging : MonoBehaviour
{
    public ObjectManager objectManager;
    public string environmentId; // Voeg een veld toe voor de environmentId

    public bool isDragging = false;

    private void Start()
    {
        // Ensure environmentId is initialized
        if (string.IsNullOrEmpty(environmentId) && objectManager != null)
        {
            environmentId = objectManager.activeEnvironmentId;
        }
    }

    public void Update()
    {
        if (isDragging)
            this.transform.position = GetMousePosition();
    }

    private void OnMouseUpAsButton()
    {
        isDragging = !isDragging;

        if (!isDragging)
        {
            objectManager.ShowMenu();

            // Maak een nieuw Object2D aan zonder id
            Object2D newObject2D = new Object2D
            {
                prefabId = gameObject.name,
                positionX = (int)transform.position.x,
                positionY = (int)transform.position.y,
                environment2DId = environmentId // Gebruik het environmentId
            };

            // Roep de CreateObject2DInApi-methode aan om het object naar de API te sturen
            objectManager.CreateObject2DInApi(newObject2D);
        }
    }

    private Vector3 GetMousePosition()
    {
        Vector3 positionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        positionInWorld.z = 0;
        return positionInWorld;
    }
}



