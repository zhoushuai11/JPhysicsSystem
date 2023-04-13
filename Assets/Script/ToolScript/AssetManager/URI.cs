using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using System.IO;

using Debug = UnityEngine.Debug;

public class URI {
    public static string streamingAssetsPath = Application.streamingAssetsPath;
    public static string persistentDataPath = Application.persistentDataPath;
    public static string configPath = Application.dataPath + "/ToBundle/Config/";
    public static string globalConfigPath = Application.dataPath + "/ToBundle/Global/Config/";
    public static string soundConfigPath = Application.dataPath + "/Audio/Config/";

    public static readonly string alienWarship = "ToBundle/GamePlayItem/ServerData/SAlienShip/";
    public static readonly string skinPickItem = "ToBundle/Skin/PickItems/";

    private static readonly string itPistol = "Weapons/Pistol/";
    private static readonly string itMachineGun = "Weapons/SubmachineGun/";
    private static readonly string itAssault = "Weapons/Assault/";
    private static readonly string itShotGun = "Weapons/ShotGun/";
    private static readonly string itSniper = "Weapons/Sniper/";
    private static readonly string itElasticWeapon = "Weapons/ElasticWeapon/";
    private static readonly string itDesignatedMarksmanRifle = "Weapons/DesignatedMarksmanRifle/";
    private static readonly string itLightMachineGun = "Weapons/LightMachineGun/";
    private static readonly string itMelee = "Weapons/Melee/";
    private static readonly string itRpg = "Weapons/SpecialWeapons/";
    private static readonly string itChargeWeapons = "Weapons/ChargeWeapons/";
    private static readonly string itActivityItems = "ActivityItems/";
    private static readonly string itAmmunition = "Ammunition/";
    private static readonly string itConsumables = "Stunt/Consumables/";
    private static readonly string itBomb = "Stunt/Bomb/";
    private static readonly string itCarrier = "Stunt/Carrier/";
    private static readonly string itBack = "Equipment/Back/";
    private static readonly string itArmoredVests = "Equipment/ArmoredVests/";
    private static readonly string itFunctionalGarment = "Equipment/FunctionalGarment/";
    private static readonly string itHeadgear = "Equipment/Headgear/";
    private static readonly string itSmallVest = "Equipment/SmallVest/";
    private static readonly string itUpperRail = "Attachments/UpperRail/";
    private static readonly string itMagazine = "Attachments/Magazine/";
    private static readonly string itStock = "Attachments/Stock/";
    private static readonly string itMuzzle = "Attachments/Muzzle/";
    private static readonly string itLowerRail = "Attachments/LowerRail/";
    private static readonly string itChip = "Attachments/Chip/";

    public static void Init() {
        if (Application.isEditor) {
            configPath = Application.dataPath + "/ToBundle/Config";
            globalConfigPath = Application.dataPath + "/ToBundle/Global/Config";
            if (GlobalDefine.IsUseBundle) {
                streamingAssetsPath = Path.GetFullPath(Application.dataPath + "/../Res");
            } else {
                streamingAssetsPath = Application.streamingAssetsPath;
            }

            persistentDataPath = Path.GetFullPath(Application.dataPath + "/../Local");
            if (!Directory.Exists(persistentDataPath)) {
                Directory.CreateDirectory(persistentDataPath);
            }
        } else {
            configPath = Application.streamingAssetsPath;
            streamingAssetsPath = Application.streamingAssetsPath;
            persistentDataPath = Application.persistentDataPath;
        }
        Debug.Log(persistentDataPath);
    }

    public static string GetItemUrlByItemType(int itemType) {
        // switch (itemType) {
        //     case ItemData.Pistol:
        //         return itPistol;
        //     case ItemData.SubmachineGun:
        //         return itMachineGun;
        //     case ItemData.Assault:
        //         return itAssault;
        //     case ItemData.ShotGun:
        //         return itShotGun;
        //     case ItemData.Sniper:
        //         return itSniper;
        //     case ItemData.ElasticWeapon:
        //         return itElasticWeapon;
        //     case ItemData.DesignatedMarksmanRifle:
        //         return itDesignatedMarksmanRifle;
        //     case ItemData.LightMachineGun:
        //         return itLightMachineGun;
        //     case ItemData.Ammunition:
        //         return itAmmunition;
        //     case ItemData.Melee:
        //         return itMelee;
        //     case ItemData.Bomb:
        //         return itBomb;
        //     case ItemData.Carrier:
        //         return itCarrier;
        //     case ItemData.Consumables:
        //         return itConsumables;
        //     case ItemData.Back:
        //         return itBack;
        //     case ItemData.ArmoredVests:
        //         return itArmoredVests;
        //     case ItemData.SmallVest:
        //         return itSmallVest;
        //     case ItemData.FunctionalGarment:
        //     case ItemData.IdCard:
        //         return itFunctionalGarment;
        //     case ItemData.Headgear:
        //         return itHeadgear;
        //     case ItemData.UpperRail:
        //         return itUpperRail;
        //     case ItemData.Magazine:
        //         return itMagazine;
        //     case ItemData.Stock:
        //         return itStock;
        //     case ItemData.Muzzle:
        //         return itMuzzle;
        //     case ItemData.LowerRail:
        //         return itLowerRail;
        //     case ItemData.SpecialWeapons:
        //         return itRpg;
        //     case ItemData.ChargeWeapons:
        //         return itChargeWeapons;
        //     case ItemData.ActivityItems:
        //         return itActivityItems;
        //     case ItemData.Chip:
        //         return itChip;
        // }

        return "";
    }
}
