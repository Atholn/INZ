using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;


public class CampaignSettingsFileSystem
{
    private readonly string _path = Path.GameAppPath + $"/Game/Campaign/";
    private readonly string _settingsFile = "CampaignSettings";
    private readonly int _firstMission = 0;
    

    public CampaignSettings LoadSettings()
    {
        string tmpPath = _path + $"{_settingsFile}";
        SettingsIfNotExist(tmpPath);

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream stream = new FileStream(tmpPath, FileMode.Open);
        stream.Position = 0;
        CampaignSettings campaignSettings = binaryFormatter.Deserialize(stream) as CampaignSettings;
        stream.Close();

        return campaignSettings;
    }

    public void SaveSettings()
    {

    }

    public void SettingsIfNotExist(string tmpPath)
    {
        if (File.Exists(tmpPath))
        {
            return;
        }

        CampaignSettings settings = new CampaignSettings { AvailableTarget = _firstMission };
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream stream = new FileStream(tmpPath, FileMode.Create);
        binaryFormatter.Serialize(stream, settings);
        stream.Close();
    }
}
