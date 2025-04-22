using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class ClientButtonHandler : MonoBehaviour
{
    public InputField joinCodeInputField;

    public void OnClientButtonClick()
    {
        if (joinCodeInputField != null && !string.IsNullOrEmpty(joinCodeInputField.text))
        {
            RelayManager.Instance.JoinWithRelay(joinCodeInputField.text);
        }
        else
        {
            Debug.LogError("Join code input field is null or empty!");
        }
    }
}