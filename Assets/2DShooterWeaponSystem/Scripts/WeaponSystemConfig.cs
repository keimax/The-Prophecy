using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponSystemConfig", menuName = "WeaponSystem Config", order = 51)]
public class WeaponSystemConfig : ScriptableObject
{
    [SerializeField]
    private string weaponName;

    public string WeaponName
    {
        get
        {
            return weaponName;
        }
        set
        {
            weaponName = value;
        }
    }

    [SerializeField]
    private float bulletSpread;

    public float BulletSpread
    {
        get
        {
            return bulletSpread;
        }
        set
        {
            bulletSpread = value;
        }
    }

    [SerializeField]
    private float bulletSpreadPingPongMax, bulletSpreadPingPongMin;

    public float BulletSpreadPingPongMax
    {
        get
        {
            return bulletSpreadPingPongMax;
        }
        set
        {
            bulletSpreadPingPongMax = value;
        }
    }

    public float BulletSpreadPingPongMin
    {
        get
        {
            return bulletSpreadPingPongMin;
        }
        set
        {
            bulletSpreadPingPongMin = value;
        }
    }

    [SerializeField]
    private float spreadPingPongSpeed = 1f;

    public float SpreadPingPongSpeed
    {
        get
        {
            return spreadPingPongSpeed;
        }
        set
        {
            spreadPingPongSpeed = value;
        }
    }

    [SerializeField]
    private float bulletSpacing;

    public float BulletSpacing
    {
        get
        {
            return bulletSpacing;
        }
        set
        {
            bulletSpacing = value;
        }
    }

    [SerializeField]
    private int bulletCount;

    public int BulletCount
    {
        get
        {
            return bulletCount;
        }
        set
        {
            bulletCount = value;
        }
    }

    [SerializeField]
    private float bulletRandomness;

    public float BulletRandomness
    {
        get
        {
            return bulletRandomness;
        }
        set
        {
            bulletRandomness = value;
        }
    }

    [SerializeField]
    private float bulletSpeed;

    public float BulletSpeed
    {
        get
        {
            return bulletSpeed;
        }
        set
        {
            bulletSpeed = value;
        }
    }

    [SerializeField]
    private float bulletAmplitude;

    public float BulletAmplitude
    {
        get
        {
            return bulletAmplitude;
        }
        set
        {
            bulletAmplitude = value;
        }
    }

    [SerializeField]
    private float bulletFrequencyIncrement;

    public float BulletFrequencyIncrement
    {
        get
        {
            return bulletFrequencyIncrement;
        }
        set
        {
            bulletFrequencyIncrement = value;
        }
    }

    [SerializeField]
    private float weaponFireRate;

    public float WeaponFireRate
    {
        get
        {
            return weaponFireRate;
        }
        set
        {
            weaponFireRate = value;
        }
    }

    [SerializeField]
    private float weaponXOffset;

    public float WeaponXOffset
    {
        get
        {
            return weaponXOffset;
        }
        set
        {
            weaponXOffset = value;
        }
    }

    [SerializeField]
    private float weaponYOffset;

    public float WeaponYOffset
    {
        get
        {
            return weaponYOffset;
        }
        set
        {
            weaponYOffset = value;
        }
    }

    [SerializeField]
    private float ricochetChancePercent;

    public float RicochetChancePercent
    {
        get
        {
            return ricochetChancePercent;
        }
        set
        {
            ricochetChancePercent = value;
        }
    }

    [SerializeField]
    private float magazineChangeDelay;

    public float MagazineChangeDelay
    {
        get
        {
            return magazineChangeDelay;
        }
        set
        {
            magazineChangeDelay = value;
        }
    }

    [SerializeField]
    private Color bulletColour = Color.white;

    public Color BulletColour
    {
        get
        {
            return bulletColour;
        }
        set
        {
            bulletColour = value;
        }
    }

    [SerializeField]
    private Texture2D weaponIcon;

    public Texture2D WeaponIcon
    {
        get
        {
            return weaponIcon;
        }
        set
        {
            weaponIcon = value;
        }
    }

    [SerializeField]
    private int magazineSize;

    public int MagazineSize
    {
        get
        {
            return magazineSize;
        }
        set
        {
            magazineSize = value;
        }
    }

    [SerializeField]
    private int magazineRemainingBullets;

    public int MagazineRemainingBullets
    {
        get
        {
            return magazineRemainingBullets;
        }
        set
        {
            magazineRemainingBullets = value;
        }
    }

    [SerializeField]
    private int ammoAvailable;

    public int AmmoAvailable
    {
        get
        {
            return ammoAvailable;
        }
        set
        {
            ammoAvailable = value;
        }
    }

    [SerializeField]
    private int ammoUsed;

    public int AmmoUsed
    {
        get
        {
            return ammoUsed;
        }
        set
        {
            ammoUsed = value;
        }
    }

    [SerializeField]
    private bool autoFire, pingPongSpread, richochetsEnabled, hitEffectEnabled, limitedAmmo, usesMagazines, isFirstEquip, mirrorX;

    public bool AutoFire
    {
        get
        {
            return autoFire;
        }
        set
        {
            autoFire = value;
        }
    }

    public bool PingPongSpread
    {
        get
        {
            return pingPongSpread;
        }
        set
        {
            pingPongSpread = value;
        }
    }

    public bool RichochetsEnabled
    {
        get
        {
            return richochetsEnabled;
        }
        set
        {
            richochetsEnabled = value;
        }
    }

    public bool HitEffectEnabled
    {
        get
        {
            return hitEffectEnabled;
        }
        set
        {
            hitEffectEnabled = value;
        }
    }

    public bool LimitedAmmo
    {
        get
        {
            return limitedAmmo;
        }
        set
        {
            limitedAmmo = value;
        }
    }

    public bool UsesMagazines
    {
        get
        {
            return usesMagazines;
        }
        set
        {
            usesMagazines = value;
        }
    }

    public bool IsFirstEquip
    {
        get
        {
            return isFirstEquip;
        }
        set
        {
            isFirstEquip = value;
        }
    }

    public bool MirrorX
    {
        get
        {
            return mirrorX;
        }
        set
        {
            mirrorX = value;
        }
    }

    [SerializeField]
    private bool bulletCountsAsAmmo;

    public bool BulletCountsAsAmmo
    {
        get
        {
            return bulletCountsAsAmmo;
        }
        set
        {
            bulletCountsAsAmmo = value;
        }
    }

    [SerializeField]
    private bool playReloadingSfx, playEmptySfx;

    public bool PlayReloadingSfx
    {
        get
        {
            return playReloadingSfx;
        }
        set
        {
            playReloadingSfx = value;
        }
    }

    public bool PlayEmptySfx
    {
        get
        {
            return playEmptySfx;
        }
        set
        {
            playEmptySfx = value;
        }
    }

    [SerializeField]
    private bool weaponRelativeToComponent;

    public bool WeaponRelativeToComponent
    {
        get
        {
            return weaponRelativeToComponent;
        }
        set
        {
            weaponRelativeToComponent = value;
        }
    }

    [SerializeField]
    private float trackingTurnRate;

    public float TrackingTurnRate
    {
        get
        {
            return trackingTurnRate;
        }
        set
        {
            trackingTurnRate = value;
        }
    }

    [SerializeField]
    private bool lerpTurnRate;

    public bool LerpTurnRate
    {
        get
        {
            return lerpTurnRate;
        }
        set
        {
            lerpTurnRate = value;
        }
    }

    [SerializeField]
    private float circularFireRotationRate;

    public float CircularFireRotationRate
    {
        get
        {
            return circularFireRotationRate;
        }
        set
        {
            circularFireRotationRate = value;
        }
    }

    [SerializeField]
    private bool circularFireMode;

    public bool CircularFireMode
    {
        get
        {
            return circularFireMode;
        }
        set
        {
            circularFireMode = value;
        }
    }

    [SerializeField]
    private Transform targetToTrack;

    public Transform TargetToTrack
    {
        get
        {
            return targetToTrack;
        }
        set
        {
            targetToTrack = value;
        }
    }

    [SerializeField]
    private AudioClip reloadSfxClip, emptySfxClip, shotFiredClip;

    public AudioClip ReloadSfxClip
    {
        get
        {
            return reloadSfxClip;
        }
        set
        {
            reloadSfxClip = value;
        }
    }

    public AudioClip EmptySfxClip
    {
        get
        {
            return emptySfxClip;
        }
        set
        {
            emptySfxClip = value;
        }
    }

    public AudioClip ShotFiredClip
    {
        get
        {
            return shotFiredClip;
        }
        set
        {
            shotFiredClip = value;
        }
    }

    [SerializeField]
    private WeaponSystem.ShooterType shooterType;

    public WeaponSystem.ShooterType ShooterType
    {
        get
        {
            return shooterType;
        }
        set
        {
            shooterType = value;
        }
    }

    [SerializeField]
    private WeaponSystem.BulletOption bulletOptionType;

    public WeaponSystem.BulletOption BulletOptionType
    {
        get
        {
            return bulletOptionType;
        }
        set
        {
            bulletOptionType = value;
        }
    }
}
