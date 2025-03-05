using UnityEngine;
using UnityEngine.UI;

//It resets scrollbar attched to Response Panel to it's default value
public class ResetSlider : MonoBehaviour
{
    [SerializeField]
    private Scrollbar scrollBar;

    private void OnEnable()
    {
        scrollBar.value = 1;
    }
}
