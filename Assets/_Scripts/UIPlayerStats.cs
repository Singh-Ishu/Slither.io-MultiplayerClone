using UnityEngine;
using TMPro;

public class UIPlayerStats : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lengthText;

    private void OnEnable()
    {
        PlayerLength.ChangedLengthEvent += ChangeLengthtext;
    }

    private void OnDisable()
    {
        PlayerLength.ChangedLengthEvent -= ChangeLengthtext;
    }

    private void ChangeLengthtext(ushort length)
    {
        lengthText.text = length.ToString();
    }
}
