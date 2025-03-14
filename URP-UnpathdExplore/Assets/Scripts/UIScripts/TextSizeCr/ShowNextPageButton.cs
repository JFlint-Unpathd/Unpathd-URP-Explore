using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//[[RequireComponent(typeof(Image))]]
public class ShowNextPageButton : MonoBehaviour
{

    [SerializeField] private Image _buttonImage;
    
    [SerializeField] private TMP_Text _textBox;
    
    private void Update()
    {
        if (_textBox.overflowMode == TextOverflowModes.Page && 
            _textBox.textInfo.pageCount > 1)
        {
            if (_buttonImage.enabled == false)
                _buttonImage.enabled = true;
        }
        else
        {
            if (_buttonImage.enabled)
                _buttonImage.enabled = false;
        }
    }

}
