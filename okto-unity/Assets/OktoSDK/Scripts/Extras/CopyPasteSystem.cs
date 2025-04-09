using TMPro;
using UnityEngine;

namespace OktoSDK
{
    //This is a utility class to copy responses from response panel
    public class CopyPasteSystem : MonoBehaviour
    {
        public TextMeshProUGUI inputField;

        public void CopyToClipboard()
        {
            CustomLogger.Log("inputField.text " + inputField.text);
            TextEditor textEditor = new TextEditor();
            textEditor.text = inputField.text;
            textEditor.SelectAll();
            textEditor.Copy();  //Copy string from textEditor.text to Clipboard
        }

        public void PasteFromClipboard()
        {
            TextEditor textEditor = new TextEditor();
            textEditor.multiline = true;
            textEditor.Paste();  //Copy string from Clipboard to textEditor.text
            inputField.text = textEditor.text;
        }
    }
}