using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Instructions : MonoBehaviour
{
    public CanvasGroup instructionsCanvasGroup;
    public Button okayButton;
    public GameObject emailPanel; // Assign in Inspector

    void Start()
    {
        okayButton.onClick.AddListener(OnOkayButtonClicked);
        ShowInstructions();
    }

    void ShowInstructions()
    {
       

        // Disable interactions with the email panel
        ToggleEmailPanelInteractable(false);
    }

    void OnOkayButtonClicked()
    {
        

        // Re-enable interactions with the email panel
        ToggleEmailPanelInteractable(true);
    }

    void ToggleEmailPanelInteractable(bool enable)
    {
        var emailPanelCanvasGroup = emailPanel.GetComponent<CanvasGroup>();
        if (emailPanelCanvasGroup != null)
        {
            emailPanelCanvasGroup.interactable = enable;
            emailPanelCanvasGroup.blocksRaycasts = enable;
        }
    }
}
