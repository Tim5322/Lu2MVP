using System;
using UnityEngine;

[Serializable]
public class Object2D
{
    public Guid? id;
    public string prefabId;
    public int positionX;
    public int positionY;
    public string environment2DId; // Adjusted to match the JSON field name
}
