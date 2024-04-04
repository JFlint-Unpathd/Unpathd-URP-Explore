using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Reflection;
using System.Text.RegularExpressions;

// script from tutorial https://www.youtube.com/watch?v=PXbyZhR8fGc&ab_channel=ChristinaCreatesGames
public class FontSizeManager : MonoBehaviour

{
    private TMP_StyleSheet _styleSheet => TMP_Settings.defaultStyleSheet;

    [SerializeField] private string styleName;

    public static Action<string> UpdatedTheTextStyle;
    
    // // Ensure that the FontSizeCustomizer object persists across scene changes
    // private void Awake()
    // {
    //     DontDestroyOnLoad(gameObject);
    // }
    public void ChangeFontSize(float fontSize)
    {
        //retrieves the stylename set
        TMP_Style style = _styleSheet.GetStyle(styleName);
        
        if (style == null)
        {
            Debug.LogError($"No style with name {styleName} found in the default style sheet. Check for spelling?");
            return;
        }

        //looks for the sizevar in the textstylesheet
        Regex regex = new Regex(@"<size=\d+>");

        string modifiedOpeningDefinition = regex.Replace(style.styleOpeningDefinition, $"<size={fontSize}>");  
        
        //allows to acess private fields from the tmp pro class
        FieldInfo openingDefinitionField = typeof(TMP_Style).GetField("m_OpeningDefinition", BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (openingDefinitionField != null) 
            openingDefinitionField.SetValue(style, modifiedOpeningDefinition);

        style.RefreshStyle();
        //Debug.Log("style refreshed");
        
        UpdatedTheTextStyle?.Invoke(styleName);
        //Debug.Log("update text style");
    }
}
