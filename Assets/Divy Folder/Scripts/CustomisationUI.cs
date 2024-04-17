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
    public TextMeshProUGUI errorText;

    public void GenerateButtonClicked()
    {
        errorText.text = "";
        try
        {
            int length = int.Parse(lengthInput.text.Substring(0, lengthInput.text.Length - 1));
            print(length);
            mazeGen.numNodesX = length;
        }
        catch (Exception e)
        {
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

    private void ParseTextToInt(string text)
    {
        string resString = "";
        int resInt;
        foreach (char s in text)
        {
            print(s);
            resString.Append(s);
        }
        print("before" + resString);
        resInt = int.Parse(resString);
        print(resInt);
    }
}
