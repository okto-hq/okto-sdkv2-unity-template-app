using System.Collections;
using UnityEngine;

namespace OktoSDK
{
    public class SetUpDefaultOrientation : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Screen.orientation = Environment.GetDefaulOrientation();
        }

    }
}