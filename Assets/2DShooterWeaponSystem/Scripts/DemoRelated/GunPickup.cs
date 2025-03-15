using UnityEngine;

/// <summary>
/// A sample demo script that shows a simple way of how to pick up guns/weapons using ScriptableObjects and add them to the main WeaponSystem weapon configurations.
/// </summary>
public class GunPickup : MonoBehaviour
{
    public AudioClip pickupSfx;
    public WeaponSystemConfig weaponConfig;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var weaponSystem = collision.gameObject.GetComponent<WeaponSystem>();
            if (weaponSystem != null)
            {
                weaponSystem.weaponConfigs.Add(weaponConfig);
                weaponSystem.MaxWeaponIndex++;
                weaponSystem.EquipWeaponConfiguration(weaponSystem.weaponConfigs.IndexOf(weaponConfig));

                if (pickupSfx != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSfx, Vector2.zero, 1f);
                }

                if (WeaponIconsUnityGui.instance != null)
                {
                    WeaponIconsUnityGui.instance.UpdateWeaponsUI();
                }

                Destroy(gameObject);
            }
        }
    }
}
