using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class SaveData
{
    // Level info
    public struct LevelInfo
    {
        public string levelName;
        public float highscore;
        public string levelScene;
        public GameObject levelLayout;
    }

    [DataMember]
    public List<LevelInfo> levelInfos = new List<LevelInfo>();

    // Weapon info
    public struct WeaponInfo
    {
        public string weaponName;
        public bool isUnlocked;
    }

    [DataMember]
    public List<WeaponInfo> weaponInfos = new List<WeaponInfo>();

    // Settings info
    public struct SettingsInfo
    {
        public float masterVolume;
        public FullScreenMode fullScreenMode;
        public Vector2 resolution;
        public bool subtitles;
        public float sensitivity;
    }

    [DataMember]
    public SettingsInfo settingsInfo;
}
