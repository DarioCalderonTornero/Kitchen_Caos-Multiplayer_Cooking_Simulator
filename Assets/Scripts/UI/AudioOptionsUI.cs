using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioOptionsUI : MonoBehaviour
{
    [SerializeField] private Button musicButton;
    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAltButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button gamepadInteractButton;
    [SerializeField] private Button gamepadInteractAltButton;
    [SerializeField] private Button gamepadPauseButton;

    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI SoundEffectsText;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAltText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI gamepadInteractText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAltText;
    [SerializeField] private TextMeshProUGUI gamepadPauseText;

    [SerializeField] private Transform pressKeyToRebind;


    public static AudioOptionsUI Instance { get; private set; }

    private void Awake()
    {
        soundEffectsButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisuals();
        });

        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisuals();
        });

        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });

        moveUpButton.onClick.AddListener(() =>
        {
            rebindBinding(PlayerInputs.Binding.moveUp);
        });

        moveDownButton.onClick.AddListener(() =>
        {
            rebindBinding(PlayerInputs.Binding.moveDown);
        });

        moveLeftButton.onClick.AddListener(() =>
        {
            rebindBinding(PlayerInputs.Binding.moveLeft);
        });

        moveRightButton.onClick.AddListener(() =>
        {
            rebindBinding(PlayerInputs.Binding.moveRight);
        });

        interactButton.onClick.AddListener(() =>
        {
            rebindBinding(PlayerInputs.Binding.interact);
        });

        interactAltButton.onClick.AddListener(() =>
        {
            rebindBinding(PlayerInputs.Binding.interactAlt);
        });

        pauseButton.onClick.AddListener(() =>
        {
            rebindBinding(PlayerInputs.Binding.pause);
        });

        gamepadInteractButton.onClick.AddListener(() =>
        {
            rebindBinding(PlayerInputs.Binding.gamepadInteract);
        });

        gamepadInteractAltButton.onClick.AddListener(() =>
        {
            rebindBinding(PlayerInputs.Binding.gamepadInteractAlt);
        });

        gamepadPauseButton.onClick.AddListener(() =>
        {
            rebindBinding(PlayerInputs.Binding.gamepadPause);
        });


        Instance = this;
    }

    private void Start()
    {
        UpdateVisuals();
        Hide();
        HidePressAnyKeyToRebind();
        KitchenGameManager.Instance.OnGameUnPaused += KitchenGameManager_OnGameUnPaused;
    }

    private void KitchenGameManager_OnGameUnPaused(object sender, System.EventArgs e)
    {
        Hide();
    }



    private void UpdateVisuals()
    {
        SoundManager.Instance.GetVolume();
        SoundEffectsText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

        moveUpText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.moveUp);
        moveDownText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.moveDown);
        moveLeftText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.moveLeft);
        moveRightText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.moveRight);
        interactText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.interact);
        interactAltText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.interactAlt);
        pauseText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.pause);
        gamepadInteractText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.gamepadInteract);
        gamepadInteractAltText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.gamepadInteractAlt);
        gamepadPauseText.text = PlayerInputs.Instance.GetBinding(PlayerInputs.Binding.gamepadPause);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        GamePausedUI.Instance.Hide();
        soundEffectsButton.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        GamePausedUI.Instance.Show();
    }

    private void ShowPressAnyKeyToRebind()
    {
        pressKeyToRebind.gameObject.SetActive(true);
    }

    private void HidePressAnyKeyToRebind()
    {
        pressKeyToRebind.gameObject.SetActive(false);
    }

    private void rebindBinding(PlayerInputs.Binding binding)
    {
        ShowPressAnyKeyToRebind();
        PlayerInputs.Instance.RebindBinding(binding, () =>
        {
            HidePressAnyKeyToRebind(); UpdateVisuals();
        });


    }
}


