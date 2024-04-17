using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Slider : MonoBehaviour
{
    public TextMeshProUGUI valueText;
    private UnityEngine.UI.Slider _slider;
    
    void Start()
    {
        _slider = GetComponent<UnityEngine.UI.Slider>();
        _slider.onValueChanged.AddListener(ChangeValueText);
    }

    private void ChangeValueText(float value)
    {
        valueText.text = value + " %";
    }
}
