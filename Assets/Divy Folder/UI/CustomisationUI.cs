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
    public GameObject playerObject;
    public GameObject cameraDolly;
    public GameObject customisationMenu;
    public GameObject escapeMenu;

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

    private bool _inPlayMode = false;
    private bool _hasError = false;

    public void GenerateButtonClicked()
    {
        _hasError = false;
        errorText.text = "";
        try
        {
            int length = int.Parse(lengthInput.text.Substring(0, lengthInput.text.Length - 1));
            int width = int.Parse(widthInput.text.Substring(0, widthInput.text.Length - 1));
            int height = int.Parse(heightInput.text.Substring(0, heightInput.text.Length - 1));

            int deadEndChance = (int)deadEndChanceSlider.value;
            int squareRoomChance = (int)squareRoomChanceSlider.value;
            int cornerRoomChance = (int)cornerRoomChanceSlider.value;
            int teeRoomChance = (int)teeRoomChanceSlider.value;
            int throughRoomChance = (int)throughRoomChanceSlider.value;

            int chanceSum = squareRoomChance + cornerRoomChance + teeRoomChance + throughRoomChance;

            // Check sum of chances
            if (chanceSum <= 100)
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

                try
                {
                    // Generate dungeon
                    mazeGen.StartGen();
                }
                catch (Exception e)
                {
                    print(e);
                    errorText.text = "No feasible dungeon generated.";
                }
                
                
            }
            else
            {
                _hasError = true;
                errorText.text = "Sum of room chances must be below 100%.";
            }

        }
        catch (Exception e)
        {
            print(e);
            errorText.text = "Wrong format.";
        }
    }

    public void PlayButtonClicked()
    {
        if (!_hasError)
        {
            playerObject.SetActive(true);
            cameraDolly.SetActive(false);
            customisationMenu.SetActive(false);
            escapeMenu.SetActive(true);
            _inPlayMode = true;
        }
        
    }

    private void Start()
    {
        escapeMenu.SetActive(false);
    }
    
    private void Update()
    {
        if (_inPlayMode)
        {
            // If in Play Mode, pressing Escape will lead you back to the main menu.
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                playerObject.SetActive(false);
                cameraDolly.SetActive(true);
                customisationMenu.SetActive(true);
                escapeMenu.SetActive(false);
                _inPlayMode = false;
            }
        }
    }
}
