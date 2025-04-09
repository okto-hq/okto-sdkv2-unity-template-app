using TMPro;
using UnityEngine;

namespace OktoSDK
{
    public class ResetField : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField[] inputFields;

        private void OnEnable()
        {
            for (int i = 0; i < inputFields.Length; i++)
            {
                inputFields[i].text = string.Empty;
            }
        }
    }
}