using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// script from tutorial https://www.youtube.com/watch?v=PXbyZhR8fGc&ab_channel=ChristinaCreatesGames
[RequireComponent(typeof(TMP_Text))]
public class UpdateTextBoxStyle : MonoBehaviour
{
    private TMP_Text _textBox;
    private void Awake()
    {
        _textBox = GetComponent<TMP_Text>();
    }
    private void OnEnable()
    {
        FontSizeCustomizer.UpdatedTheTextStyle += UpdateTextStyle;
        
    }
    
    private void OnDisable()
    {
        FontSizeCustomizer.UpdatedTheTextStyle -= UpdateTextStyle;
    }

    private void UpdateTextStyle(string styleName)
    {
        Debug.Log($"UpdateTextStyle called with styleName: {styleName}");

        if (_textBox.textStyle.name != styleName)
            return;
        Debug.Log("StyleNameError");
        
        _textBox.textStyle = TMP_Settings.defaultStyleSheet.GetStyle(styleName);
        Debug.Log("Updated the text style from FontSizeCustomizer script");
        
        int lastPage = _textBox.textInfo.pageCount - 1;
        
        if (_textBox.pageToDisplay > lastPage)
            _textBox.pageToDisplay = lastPage;
        
        Debug.Log($"Text needs {_textBox.textInfo.pageCount} pages to display. Current page is {_textBox.pageToDisplay}");
    }
}

