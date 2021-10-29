﻿using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    public Button ResourceButton;
    public Image ResourceImage;
    public Text ResourceDescription;
    public Text ResourceUpgradeCost;
    public Text ResourceUnlockCost;

    private ResourceConfig _config;

    //private int _level = 1;
    private int _index;

    public bool IsUnlocked { get; private set; }

    private void Start()
    {
        ResourceButton.onClick.AddListener(() =>
        {
            if (IsUnlocked)
            {
                UpgradeLevel();
            }
            else
            {
                UnlockResource();
            }
        });
    }

    private int _level
    {
        set
        {
            // menyimpan value yang di set ke _level pada Progress data
            UserDataManager.Progress.ResourcesLevels[_index] = value;
            UserDataManager.Save();
        }

        get
        {
            // mengecek apakah index sudah terdapat pada progress data
            if (!UserDataManager.HasResources(_index))
            {
                // jika tidak maaka tampilkan lecel 1
                return 1;
            }

            // jika iya maka tampilkan berdasarkan Progress data
            return UserDataManager.Progress.ResourcesLevels[_index];
        }
    }

    public void SetConfig(int index, ResourceConfig config)
    {
        _index = index;
        _config = config;

        // ToString("0") berfungsi untuk membuang angka di belakang koma
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput().ToString("0") }";
        ResourceUnlockCost.text = $"Unlock Cost\n{ _config.UnlockCost }";
        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost() }";

        SetUnlocked(_config.UnlockCost == 0 || UserDataManager.HasResources(_index));
    }

    public double GetOutput()
    {
        return _config.Output * _level;
    }

    public double GetUpgradeCost()
    {
        return _config.UpgradeCost * _level;
    }

    public double GetUnlockCost()
    {
        return _config.UnlockCost;
    }

    public void UpgradeLevel()
    {
        double upgradeCost = GetUpgradeCost();
        if (UserDataManager.Progress.Gold < upgradeCost)
        {
            return;
        }

        GameManager.Instance.AddGold(-upgradeCost);
        _level++;

        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost() }";
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput().ToString("0") }";
    }

    public void UnlockResource()
    {
        double unlockCost = GetUnlockCost();
        if (UserDataManager.Progress.Gold < unlockCost)
        {
            return;
        }

        SetUnlocked(true);
        GameManager.Instance.ShowNextResource();
        AchievementController.Instance.UnlockAchievement(AchievementType.UnlockResource, _config.Name);
    }

    public void SetUnlocked(bool unlocked)
    {
        IsUnlocked = unlocked;

        if (unlocked)
        {
            // jika resources baru di unlock dan belum ada di progress data, maka tambahkan data
            if (!UserDataManager.HasResources(_index))
            {
                UserDataManager.Progress.ResourcesLevels.Add(_level);
                UserDataManager.Save();
            }
        }
        ResourceImage.color = IsUnlocked ? Color.white : Color.grey;
        ResourceUnlockCost.gameObject.SetActive(!unlocked);
        ResourceUpgradeCost.gameObject.SetActive(unlocked);
    }
}