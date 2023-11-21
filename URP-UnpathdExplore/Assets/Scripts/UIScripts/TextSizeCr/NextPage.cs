using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//script from tutorial https://www.youtube.com/watch?v=PXbyZhR8fGc&ab_channel=ChristinaCreatesGamess

[RequireComponent(typeof(TMP_Text))]
public class NextPage : MonoBehaviour
{
    [SerializeField] private TMP_Text _textBox;

    [TextArea(5,10)][SerializeField] private string FirstChunkOfText;
    [TextArea(5,10)][SerializeField] private string SecondChunkOfText;
    
    private List<string> _textList => new List<string> {FirstChunkOfText, SecondChunkOfText};
    
    //assign intex to sring
    private int _textIndex = 0;

    //pagecount needed to display entire text
    private int _currentTextPages => _textBox.textInfo.pageCount;
    
    //page being currentely displayed
    private int _currentPage => _textBox.pageToDisplay;


    private void Awake()
    {
        _textBox = GetComponent<TMP_Text>();
    }

    //method to scroll through pages
    // to test in editor add this line
    [ContextMenu("Display Next Page")]
    public void DisplayNextPage()
    {
        if (_currentPage < _currentTextPages)
        {
            _textBox.pageToDisplay++;
        }
        else
        {
            _textIndex++;
            if (_textIndex >= _textList.Count)
            {
                _textIndex = 0;
            }
            
            _textBox.text = _textList[_textIndex];
            _textBox.pageToDisplay = 1;
        }
    }
}
