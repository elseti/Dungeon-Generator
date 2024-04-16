using UnityEngine;
using UnityEngine.UIElements;

public class GoBack : MonoBehaviour
{
    private UIDocument document;
    private GameObject playerObject;
    private GameObject cameraDolly;
    private GameObject generationUI;
    private Button goBackButton;
    private void Awake()
    {
        document = GetComponent<UIDocument>();
        
        playerObject = GameObject.Find("PlayerObject");
        cameraDolly = GameObject.Find("CameraDolly");
        generationUI = GameObject.Find("GenerationUI");
        
        goBackButton = document.rootVisualElement.Q("GoBackButton") as Button;
        goBackButton.RegisterCallback<ClickEvent>(goBackHandler);
    }

    private void goBackHandler(ClickEvent evt)
    {
        Debug.Log("Returning to Generation Menu...");
        playerObject.SetActive(false);
        cameraDolly.SetActive(true);
        generationUI.SetActive(true);
    }
}
