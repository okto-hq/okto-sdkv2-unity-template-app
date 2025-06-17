using UnityEngine;

namespace OktoSDK
{
    public class PanelSwitch : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] panels;

        public void ActivatePanel(int index)
        {
            for (int i = 0; i < panels.Length; i++)
            {
                if (i == index)
                {
                    panels[i].SetActive(true);
                }
                else
                {
                    panels[i].SetActive(false);
                }
            }
        }
    }
}