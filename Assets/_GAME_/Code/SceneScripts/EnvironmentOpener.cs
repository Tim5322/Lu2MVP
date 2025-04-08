//using UnityEngine;
//using UnityEngine.UIElements;

//public class EnvironmentOpener : MonoBehaviour
//{
//    public EnvironmentManager environmentManager;
//    public int environmentIndex; // Index van de omgeving (1, 2 of 3)

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        // Voeg een listener toe aan de knop om de juiste omgeving te openen
//        GetComponent<Button>().clicked += OpenEnvironment;
//    }

//    // Methode om de juiste omgeving te openen
//    void OpenEnvironment()
//    {
//        switch (environmentIndex)
//        {
//            case 1:
//                environmentManager.OpenEnvironment1();
//                break;
//            case 2:
//                environmentManager.OpenEnvironment2();
//                break;
//            case 3:
//                environmentManager.OpenEnvironment3();
//                break;
//            default:
//                Debug.LogError("Invalid environment index.");
//                break;
//        }
//    }
//}


