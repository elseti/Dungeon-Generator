using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CustomisationUI : MonoBehaviour
{
    public MazeGen mazeGen;

    public TextMeshProUGUI lengthInput;
    public TextMeshProUGUI widthInput;
    public TextMeshProUGUI heightInput;

    public UnityEngine.UI.Slider deadEndChanceSlider;
    public UnityEngine.UI.Slider squareRoomChanceSlider;
    public UnityEngine.UI.Slider cornerRoomChanceSlider;
    public UnityEngine.UI.Slider teeRoomChanceSlider;
    public UnityEngine.UI.Slider throughRoomChanceSlider;

    public Toggle allowOverlappingRoomsToggle;
    public Toggle debugShowPathToggle;
    
    public TextMeshProUGUI errorText;

    public void GenerateButtonClicked()
    {
        errorText.text = "";
        try
        {
            int length = int.Parse(lengthInput.text.Substring(0, lengthInput.text.Length - 1));
            int width = int.Parse(widthInput.text.Substring(0, lengthInput.text.Length - 1));
            int height = int.Parse(heightInput.text.Substring(0, lengthInput.text.Length - 1));

            int deadEndChance = (int) deadEndChanceSlider.value;
            int squareRoomChance = (int) squareRoomChanceSlider.value;
            int cornerRoomChance = (int) cornerRoomChanceSlider.value;
            int teeRoomChance = (int) teeRoomChanceSlider.value;
            int throughRoomChance = (int) throughRoomChanceSlider.value;

            int chanceSum = squareRoomChance + cornerRoomChance + teeRoomChance + throughRoomChance;
            
            // Check sum of chances
            if (chanceSum < 101)
            {
                // Assign to MazeGen 
                mazeGen.numNodesX = length;
                mazeGen.numNodesY = height;
                mazeGen.numNodesZ = width;

                mazeGen.deadEndChance = deadEndChance;
                mazeGen.squareRoomChance = squareRoomChance;
                mazeGen.cornerRoomChance = cornerRoomChance;
                mazeGen.teeRoomChance = teeRoomChance;
                mazeGen.throughRoomChance = throughRoomChance;

                mazeGen.allowOverlappingRooms = allowOverlappingRoomsToggle.isOn;
                mazeGen.debugShowPath = debugShowPathToggle.isOn;
            }
            else
            {
                errorText.text = "Sum of room chances must be below 100%.";
            }
            
            
        }
        catch (Exception e)
        {
            print(e);
            errorText.text = "Wrong format.";
        }
        
        mazeGen.RunGen();
    }
    
    private void Start()
    {
        
    }
    
    private void Update()
    {
        
    }
}
