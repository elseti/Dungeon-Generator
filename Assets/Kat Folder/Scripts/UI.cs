using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    private UIDocument document;

    private Button generateButton;
    private Button playButton;

    private IntegerField lengthInput;
    private IntegerField widthInput;
    private IntegerField heightInput;
    private FloatField deadEndChanceInput;

    private MazeGen mazeGenScript;
    private GameObject playerObject;
    private GameObject cameraDolly;
    private GameObject generationUI;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        generateButton = document.rootVisualElement.Q("GenerateButton") as Button;
        generateButton.RegisterCallback<ClickEvent>(generateHandler);
        
        playButton = document.rootVisualElement.Q("PlayButton") as Button;
        playButton.RegisterCallback<ClickEvent>(playHandler);

        lengthInput = document.rootVisualElement.Q("LengthInput") as IntegerField;
        widthInput = document.rootVisualElement.Q("WidthInput") as IntegerField;
        heightInput = document.rootVisualElement.Q("HeightInput") as IntegerField;
        deadEndChanceInput = document.rootVisualElement.Q("DeadEndChanceInput") as FloatField;
        
        playerObject = GameObject.Find("PlayerObject");
        cameraDolly = GameObject.Find("CameraDolly");
        generationUI = GameObject.Find("GenerationUI");
        
        playerObject.SetActive(false);
        
        mazeGenScript = FindObjectOfType<MazeGen>();
        // mazeGenScript.RunGen(lengthInput.value, widthInput.value, heightInput.value, deadEndChanceInput.value);
    }

    private void generateHandler(ClickEvent evt)
    {
        Debug.Log("Size: ["+lengthInput.value+", " + widthInput.value + "," + heightInput.value + "]");
        Debug.Log("Dead End Chance: "+ deadEndChanceInput.value );

        // mazeGenScript.RunGen(lengthInput.value, widthInput.value, heightInput.value, deadEndChanceInput.value);
    }
    
    private void playHandler(ClickEvent evt)
    {
        Debug.Log("Playing...");
        
        playerObject.SetActive(true);
        // Reset position
        playerObject.transform.position = new Vector3(1, 5, 0);
        cameraDolly.SetActive(false);
        generationUI.SetActive(false);
    }

    private void onDisable()
    {
        generateButton.UnregisterCallback<ClickEvent>(generateHandler);
        playButton.UnregisterCallback<ClickEvent>(playHandler);
    }
}
