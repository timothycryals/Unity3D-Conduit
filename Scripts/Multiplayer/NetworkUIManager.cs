using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class NetworkUIManager : MonoBehaviour
{
    public static bool isMenuOpen;

    public OnlinePlayer stats;

    [SerializeField]
    private Text hp;
    [SerializeField]
    private Text shield;
    [SerializeField]
    private Text ammoInMag;
    [SerializeField]
    private Text ammoTotal;
    [SerializeField]
    private Image HitMarker;
    [SerializeField]
    private AudioSource HitMarkerSound;

    [SerializeField]
    private Image hpBar;
    [SerializeField]
    private Image shieldBar;

    [SerializeField]
    private GameObject InGameMenu;
    [SerializeField]
    private GameObject SettingsMenu;
    [SerializeField]
    private Slider MouseSlider;
    [SerializeField]
    private Slider GamepadSlider;
    [SerializeField]
    private Slider AimSlider;
    [SerializeField]
    private Button ApplyButton;
    [SerializeField]
    private Button SettingsButton;

    private VignetteModel.Settings vignetteSettings;

    //bool isMenuOpen = false;

    private void Start()
    {
        InitializeButtons();
        InitializeSliders();
        //postProcessingProfile.vignette.enabled = true;
        vignetteSettings = MultiplayerGameManager.instance.playerPostProcessing.profile.vignette.settings;
    }

    void Update()
    {
        hp.text = stats.CurrentHealth.ToString();
        shield.text = stats.CurrentShield.ToString();
        ammoInMag.text = stats.AmmoInMag.ToString();
        ammoTotal.text = stats.AmmoTotal.ToString();

        hpBar.fillAmount = (float)stats.CurrentHealth / 100f;
        vignetteSettings.smoothness =  1f - hpBar.fillAmount;
        MultiplayerGameManager.instance.playerPostProcessing.profile.vignette.settings = vignetteSettings;

        shieldBar.fillAmount = (float)stats.CurrentShield / 100f;

        OpenInGameMenu();
    }

    private void OnEnable()
    {
        SciFiWeaponBehaviour.PlayerHit += PlayHitMarker;
    }

    private void InitializeButtons()
    {
        ApplyButton.onClick.AddListener(delegate { ApplyChanges(); });
        SettingsButton.onClick.AddListener(delegate { SettingsButtonClick(); });
    }

    private void InitializeSliders()
    {
        MouseSlider.value = PlayerPreferences.MouseSensitivity;
        GamepadSlider.value = PlayerPreferences.GamepadSensitivity;
        AimSlider.value = PlayerPreferences.AimedSensitivityMultiplier;

        MouseSlider.onValueChanged.AddListener(delegate { EditMouseSensitivity(); });
        GamepadSlider.onValueChanged.AddListener(delegate { EditGamepadSensitivity(); });
        AimSlider.onValueChanged.AddListener(delegate { EditAimSensitivity(); });
    }

    private void OpenInGameMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(ControllerInputs.XBOX_MENU))
        {
            if (!isMenuOpen)
            {
                isMenuOpen = true;
                InGameMenu.SetActive(true);
                SettingsMenu.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                isMenuOpen = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                InGameMenu.SetActive(false);
                SettingsMenu.SetActive(false);
            }

        }
    }

    private void EditMouseSensitivity()
    {
        PlayerPreferences.MouseSensitivity = MouseSlider.value;
    }

    private void EditGamepadSensitivity()
    {
        PlayerPreferences.GamepadSensitivity = GamepadSlider.value;
    }

    private void EditAimSensitivity()
    {
        PlayerPreferences.AimedSensitivityMultiplier = AimSlider.value;
    }

    private void SettingsButtonClick()
    {
        SettingsMenu.SetActive(true);
        InGameMenu.SetActive(false);
    }

    private void ApplyChanges()
    {
        SettingsMenu.SetActive(false);
        InGameMenu.SetActive(true);
    }

    private void PlayHitMarker()
    {
        StartCoroutine(StartHitMarker());
    }

     private IEnumerator StartHitMarker()
    {
        HitMarker.enabled = true;
        HitMarkerSound.PlayOneShot(HitMarkerSound.clip);
        yield return new WaitForSeconds(0.1f);
        HitMarker.enabled = false;
    }
}
