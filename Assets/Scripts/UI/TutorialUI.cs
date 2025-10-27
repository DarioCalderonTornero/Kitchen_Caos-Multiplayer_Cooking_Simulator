using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keyMoveUpText;
    [SerializeField] private TextMeshProUGUI keyMoveDownText;
    [SerializeField] private TextMeshProUGUI keyMoveLeftText;
    [SerializeField] private TextMeshProUGUI keyMoveRightText;
    [SerializeField] private TextMeshProUGUI keyInteractText;
    [SerializeField] private TextMeshProUGUI keyInteractAltText;
    [SerializeField] private TextMeshProUGUI keyPauseText;
    [SerializeField] private TextMeshProUGUI keyGamepadInteracText;
    [SerializeField] private TextMeshProUGUI keyGamePadInteractAltText;
    [SerializeField] private TextMeshProUGUI keyGamepadPauseText;

    private void Start()
    {
        PlayerInputs.Instance.OnRebinBinding += PlayerInputs_OnRebinBinding;
        Show();
        KitchenGameManager.Instance.OnLocalPlayerReadyChanged += KitchenGameManager_OnLocalPlayerReadyChanged;
        UpdateVisuals();
    }

    private void KitchenGameManager_OnLocalPlayerReadyChanged(object sender, System.EventArgs e)
    {
        if (KitchenGameManager.Instance.PlayerLocalReady())
        {
            Hide();
        }
    }

    

    private void PlayerInputs_OnRebinBinding(object sender, System.EventArgs e)
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        keyMoveUpText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.moveUp);
        keyMoveDownText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.moveDown);
        keyMoveLeftText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.moveLeft);
        keyMoveRightText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.moveRight);
        keyInteractText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.interact);
        keyInteractAltText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.interactAlt);
        keyPauseText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.pause);
        keyGamepadInteracText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.gamepadInteract);
        keyInteractAltText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.gamepadInteractAlt);
        keyGamepadPauseText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.gamepadPause);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
