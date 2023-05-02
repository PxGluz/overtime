using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [Tooltip("USED IF YOU WANT TO CHANGE SOMETHING IN UNITY. PREVENTS INITIAL LOAD.")]
    public bool loadOnAwake = true;
    public string fileName;
    [HideInInspector]
    public SaveManager instance = null;
    public Contract contractScript;
    public SettingsManager settingsManager;
    public WeaponManager weaponManager;

    private SaveManager() { }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        if (loadOnAwake)
            Load();
    }

    public void Save()
    {
        Debug.Log("Save");
        // Create/Open save file.
        FileStream file = File.Create(Application.persistentDataPath + fileName);

        // Prepare data.
        SaveData saveData = new SaveData();
        foreach (Level.LevelInfo level in contractScript.levelList)
        {
            SaveData.LevelInfo currentLevel;
            currentLevel.highscore = level.highscore;
            currentLevel.levelLayout = level.levelLayout;
            currentLevel.levelName = level.levelName;
            currentLevel.levelScene = level.levelScene;
            saveData.levelInfos.Add(currentLevel);
        }

        foreach (WeaponManager.Weapon weapon in weaponManager.WeaponsList)
        {
            SaveData.WeaponInfo currentWeapon;
            currentWeapon.weaponName = weapon.name;
            currentWeapon.isUnlocked = weapon.isUnlocked;
            saveData.weaponInfos.Add(currentWeapon);
        }

        saveData.settingsInfo.masterVolume = settingsManager.masterVolume;
        saveData.settingsInfo.resolution = settingsManager.resolution;
        saveData.settingsInfo.sensitivity = settingsManager.sensitivity;
        saveData.settingsInfo.subtitles = settingsManager.subtitles;
        saveData.settingsInfo.fullScreenMode = settingsManager.fullScreenMode;

        // XML Serializer
        DataContractSerializer serializer = new DataContractSerializer(saveData.GetType());
        MemoryStream memoryStream = new MemoryStream();

        // Serialize the file.
        serializer.WriteObject(memoryStream, saveData);
        memoryStream.Seek(0, SeekOrigin.Begin);

        // Save to disk.
        file.Write(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length);
        file.Close();

        Debug.Log(XElement.Parse(Encoding.ASCII.GetString(memoryStream.GetBuffer()).Replace("\0", "")).ToString());
    }

    public void Load()
    {
        Debug.Log("Load");
        if (File.Exists(Application.persistentDataPath + fileName))
        {
            // Open the save file.
            FileStream file = File.OpenRead(Application.persistentDataPath + fileName);

            // XML Serializer
            DataContractSerializer serializer = new DataContractSerializer(typeof(SaveData));
            MemoryStream memoryStream = new MemoryStream();

            // Deserialize the file.
            file.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            SaveData saveData = (SaveData)serializer.ReadObject(memoryStream);
            Debug.Log(saveData.settingsInfo.masterVolume);

            // Update game.
            contractScript.levelList.Clear();
            foreach (SaveData.LevelInfo level in saveData.levelInfos)
            {
                Level.LevelInfo currentLevel = new Level.LevelInfo();
                currentLevel.highscore = level.highscore;
                currentLevel.levelLayout = level.levelLayout;
                currentLevel.levelName = level.levelName;
                currentLevel.levelScene = level.levelScene;
                contractScript.levelList.Add(currentLevel);
            }

            foreach (SaveData.WeaponInfo weapon in saveData.weaponInfos)
                foreach (WeaponManager.Weapon currentWeapon in weaponManager.WeaponsList)
                    if (currentWeapon.name.Equals(weapon.weaponName))
                        currentWeapon.isUnlocked = weapon.isUnlocked;

            settingsManager.SetMasterVolume(saveData.settingsInfo.masterVolume);
            settingsManager.SetResolution((int)saveData.settingsInfo.resolution.x, (int)saveData.settingsInfo.resolution.y);
            settingsManager.SetFullScreenMode(saveData.settingsInfo.fullScreenMode);
            settingsManager.SetSensitivity(saveData.settingsInfo.sensitivity);
            settingsManager.SetSubtitles(saveData.settingsInfo.subtitles);

            file.Close();
        }
    }

    // For testing  
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            Save();
        else if (Input.GetKeyDown(KeyCode.P))
            Load();
        else if (Input.GetKeyDown(KeyCode.I))
            File.Delete(Application.persistentDataPath + fileName);
    }
}
