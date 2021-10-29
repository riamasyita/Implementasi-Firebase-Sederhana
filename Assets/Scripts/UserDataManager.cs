using UnityEngine;

public class UserDataManager
{
    private const string PROGRESS_KEY = "Progress";

    public static UserProgressData Progress;

    public static void Load()
    {
        // cek apakah ada data yang tersimpan sebagai PROGESS_KEY
        if (!PlayerPrefs.HasKey(PROGRESS_KEY))
        {
            // jika tidak ada, maka buat data baru
            Progress = new UserProgressData();
            Save();
        }
        else
        {
            // Jika ada, maka timpa progess dengan yang sebelumnya
            string json = PlayerPrefs.GetString(PROGRESS_KEY);
            Progress = JsonUtility.FromJson<UserProgressData>(json);
        }
    }

    public static bool HasResources(int index)
    {
        return index + 1 <= Progress.ResourcesLevels.Count;
    }

    public static void Save()
    {
        string json = JsonUtility.ToJson(Progress);
        PlayerPrefs.SetString(PROGRESS_KEY, json);
    }
}
