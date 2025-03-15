using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WeaponIconsUnityGui : MonoBehaviour
{
    public const string WeaponIconGameObjectNameTemplate = "WeaponConfiguration_";
    public WeaponSystem playerWeaponSystemRef;
    public RectTransform weaponListPanel;
    public float pixelPerUnit = 100f;
    public GameObject uiSelectionReticlePrefab;

    public Text weaponNameText;
    public Text ammoRemainingText;
    public Text magazinesRemainingText;
    public Text currentMagazineText;

    public Image selectedWeaponIcon;
    private Image selectionHighlightReticle;

    public RectTransform selectionWeaponPanel;

    public GameObject reticleGo;

    private string loadedLevelName;
    private List<Texture2D> weaponIconTextures = new List<Texture2D>();
    private int currentWeaponIconIndex;

    public static WeaponIconsUnityGui instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Use this for initialization
    void Start()
    {
        if (selectionWeaponPanel == null)
        {
            selectionWeaponPanel = GameObject.Find("SelectedWeaponPanel").GetComponent<RectTransform>();
        }
        loadedLevelName = SceneManager.GetActiveScene().name;
        UpdateWeaponsUI();
        playerWeaponSystemRef.WeaponConfigurationChanged += PlayerWeaponSystemRefWeaponConfigurationChanged;
    }

    private void ClearWeaponsUI()
    {
        foreach (Transform t in weaponListPanel)
        {
            Destroy(t.gameObject);
        }

        weaponIconTextures.Clear();
    }

    private void BuildWeaponsUI()
    {
        if (playerWeaponSystemRef.weaponConfigs.Count > 0)
        {
            reticleGo = Instantiate(uiSelectionReticlePrefab, Vector2.zero, Quaternion.identity);
            selectionHighlightReticle = reticleGo.GetComponent<Image>();

            for (var i = 0; i < playerWeaponSystemRef.weaponConfigs.Count; i++)
            {
                var weaponConfig = playerWeaponSystemRef.weaponConfigs[i];
                weaponIconTextures.Add(weaponConfig.WeaponIcon);
                var iconGameObject = new GameObject(WeaponIconGameObjectNameTemplate + i);
                var imageIcon = iconGameObject.AddComponent<Image>();
                imageIcon.sprite = Sprite.Create(weaponConfig.WeaponIcon,
                    new Rect(0, 0, weaponConfig.WeaponIcon.width, weaponConfig.WeaponIcon.height), new Vector2(0.5f, 0.5f), pixelPerUnit);
                var rectTransform = iconGameObject.GetComponent<RectTransform>();
                rectTransform.SetParent(weaponListPanel.transform, false);
            }
        }

        weaponNameText.text = playerWeaponSystemRef.weaponName;
        var weaponIconGo = GameObject.Find(WeaponIconGameObjectNameTemplate + 0);
        if (weaponIconGo == null) return;
        selectionHighlightReticle.rectTransform.SetParent(weaponIconGo.transform, false);

        PlayerWeaponSystemRefWeaponConfigurationChanged(playerWeaponSystemRef.gunPoint, currentWeaponIconIndex);
    }

    public void UpdateWeaponsUI()
    {
        if (playerWeaponSystemRef != null)
        {
            ClearWeaponsUI();
            BuildWeaponsUI();
        }
    }

    void PlayerWeaponSystemRefWeaponConfigurationChanged(Transform gunPointTransform, int weaponConfigIndex)
    {
        currentWeaponIconIndex = weaponConfigIndex;

        // Highlight current weapon icon and update selected icon.
        if (currentWeaponIconIndex > weaponIconTextures.Count - 1) return;
        if (selectionHighlightReticle == null)
        {
            // Rebuild selection reticle if it was lost
            reticleGo = (GameObject)Instantiate(uiSelectionReticlePrefab, Vector2.zero, Quaternion.identity);
            Debug.Log("Reticle GO: " + reticleGo.name);
            selectionHighlightReticle = reticleGo.GetComponent<Image>();
        }
        var weaponIconGo = GameObject.Find(WeaponIconGameObjectNameTemplate + currentWeaponIconIndex);
        if (weaponIconGo == null) return;
        selectionHighlightReticle.rectTransform.SetParent(weaponIconGo.transform, false);
        selectionHighlightReticle.rectTransform.anchoredPosition = Vector2.zero;
        weaponNameText.text = playerWeaponSystemRef.weaponName;

        if (selectedWeaponIcon != null)
        {
            selectedWeaponIcon.sprite = Sprite.Create(weaponIconTextures[weaponConfigIndex], new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f), pixelPerUnit);
        }

        currentMagazineText.enabled = playerWeaponSystemRef.usesMagazines;
        ammoRemainingText.enabled = playerWeaponSystemRef.usesMagazines;
        magazinesRemainingText.enabled = playerWeaponSystemRef.usesMagazines;
    }

    // Update is called once per frame
    void Update () {

        // Just for demo purposes - displaying ammo, magazines, selected weapon in the UI.

        var ammoRemaining = playerWeaponSystemRef.startingAmmo - playerWeaponSystemRef.ammoUsed;

        if (!playerWeaponSystemRef.limitedAmmo)
        {
            ammoRemainingText.text = "Ammo remaining: infinite";
        }
        else
        {
            ammoRemainingText.text = "Ammo remaining: " + ammoRemaining;
        }

        if (playerWeaponSystemRef.usesMagazines)
        {
            var remainingMags = Mathf.Abs(ammoRemaining / playerWeaponSystemRef.magazineSize);

            magazinesRemainingText.text = "Magazines remaining: " + remainingMags;

            currentMagazineText.text = "Current magazine: " + playerWeaponSystemRef.magazineRemainingBullets + "/" +
                playerWeaponSystemRef.magazineSize;
        }
    }
}
