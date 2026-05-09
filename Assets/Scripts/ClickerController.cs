using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using YG;
using TMPro;

[System.Serializable]
public class Skin
{
    public string skinName;
    public int cost;
    public Sprite sprite;
    public Button buyButton;
    public bool isPurchased = false;
}
public class ClickerController : MonoBehaviour
{
    public GameObject clickImage;
    public float imageShowTime = 0.5f;
    public Transform ScaleImage;
    public int coins = 0;
    public int coinsPerClick = 1;
    public TextMeshProUGUI coinsText;
    public Button upgradeButtonX2;
    public Button upgradeButtonX4;
    public Button upgradeButtonX8;
    public Button clickButton;
    public List<Skin> skins = new List<Skin>();

    private float timer = 0f;
    private bool imageActive = false;
    private float clickDelay;
    private bool isCanAnimationButtom;
    private float animationDelay = 0.3f;
    private bool upgradePurchasedX2 = false;
    private bool upgradePurchasedX4 = false;
    private bool upgradePurchasedX8 = false;
    private int upgradeCostX2 = 1500;
    private int upgradeCostX4 = 8000;
    private int upgradeCostX8 = 30000;

    private void Start()
    {
        UpdateCoinsText();
        Application.targetFrameRate = 60;

        clickButton.onClick.AddListener(OnClick);
        upgradeButtonX2.onClick.AddListener(BuyUpgrade);
        upgradeButtonX2.interactable = false;
        upgradeButtonX4.onClick.AddListener(BuyUpgrade);
        upgradeButtonX4.interactable = false;
        upgradeButtonX8.onClick.AddListener(BuyUpgrade);
        upgradeButtonX8.interactable = false;

        foreach (Skin skin in skins)
        {
            skin.buyButton.onClick.AddListener(() => BuySkin(skin));
            //skin.buyButton.interactable = false;
        }
        LoadGame();
        UpdateCoinsText();
        LoadAllSkins(skins);
    }
    private void Update()
    {
        if (imageActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                clickImage.SetActive(false);
                imageActive = false;
            }
        }
        Tick();

        foreach (Skin skin in skins)
        {
            skin.buyButton.interactable = skin.isPurchased || coins >= skin.cost;
        }
        if (!upgradePurchasedX2)
        {
            upgradeButtonX2.interactable = coins >= upgradeCostX2;
        }
        if (!upgradePurchasedX4)
        {
            upgradeButtonX4.interactable = coins >= upgradeCostX4;
        }
        if (!upgradePurchasedX8)
        {
            upgradeButtonX8.interactable = coins >= upgradeCostX8;
        }
    }

    private void OnClick()
    {
        coins += coinsPerClick;
        SaveGame();
        UpdateCoinsText();

        clickImage.SetActive(true);
        timer = imageShowTime;
        imageActive = true;
        if (isCanAnimationButtom)
        {
            ScaleImage.DOPunchScale(Vector3.one * 0.5f, animationDelay, 1, 1);
            isCanAnimationButtom = false;
        }
    }
    private void Tick()
    {
        if (isCanAnimationButtom == true)
        {
            return;
        }

        clickDelay += Time.deltaTime;
        if (clickDelay > animationDelay)
        {
            clickDelay = 0;
            isCanAnimationButtom = true;

        }
    }

    private void BuySkin(Skin skin)
    {
        if (skin.isPurchased)
        {
            clickButton.image.sprite = skin.sprite;
            return;
        }

        if (coins >= skin.cost)
        {
            coins -= skin.cost;
            clickButton.image.sprite = skin.sprite;
            skin.isPurchased = true;
            SaveGame();
            SaveAllSkins(skins);
            UpdateCoinsText();
            YG2.InterstitialAdvShow();
        }
    }
    void BuyUpgrade()
    {
        if (coins >= upgradeCostX2 && !upgradePurchasedX2)
        {
            coins -= upgradeCostX2;
            coinsPerClick = 2;
            upgradePurchasedX2 = true;
            upgradeButtonX2.interactable = false;
            SaveGame();
            UpdateCoinsText();
            YG2.InterstitialAdvShow();
        }
        if (coins >= upgradeCostX4 && !upgradePurchasedX4)
        {
            coins -= upgradeCostX4;
            coinsPerClick = 4;
            upgradePurchasedX4 = true;
            upgradeButtonX4.interactable = false;
            SaveGame();
            UpdateCoinsText();
            YG2.InterstitialAdvShow();
        }
        if (coins >= upgradeCostX8 && !upgradePurchasedX8)
        {
            coins -= upgradeCostX8;
            coinsPerClick = 8;
            upgradePurchasedX8 = true;
            upgradeButtonX8.interactable = false;
            SaveGame();
            UpdateCoinsText();
            YG2.InterstitialAdvShow();
        }
    }
    private void UpdateCoinsText()
    {
        coinsText.text = "Монетки: " + coins;
    }
    public void SaveGame()
    {
        PlayerPrefs.SetInt("Score", coins);
        PlayerPrefs.SetInt("ClickPower", coinsPerClick);
        PlayerPrefs.Save();
    }
    public void LoadGame()
    {
        coins = PlayerPrefs.GetInt("Score", 0);
        coinsPerClick = PlayerPrefs.GetInt("ClickPower", 1);
    }
    [System.Serializable]
    public class SkinData
    {
        public string skinName;
        public int cost;
        public string spriteName;
        public bool isPurchased;
    }

    [System.Serializable]
    public class SkinDataList
    {
        public List<SkinData> skins = new List<SkinData>();
    }

    public void SaveAllSkins(List<Skin> skins)
    {
        SkinDataList dataList = new SkinDataList();

        foreach (var skin in skins)
        {
            dataList.skins.Add(new SkinData
            {
                skinName = skin.skinName,
                cost = skin.cost,
                spriteName = skin.sprite.name,
                isPurchased = skin.isPurchased
            });
        }

        string json = JsonUtility.ToJson(dataList);
        PlayerPrefs.SetString("AllSkins", json);
        PlayerPrefs.Save();
    }
    public void LoadAllSkins(List<Skin> skins)
    {
        if (!PlayerPrefs.HasKey("AllSkins")) return;

        string json = PlayerPrefs.GetString("AllSkins");
        SkinDataList dataList = JsonUtility.FromJson<SkinDataList>(json);

        foreach (var data in dataList.skins)
        {
            Skin skin = skins.Find(s => s.skinName == data.skinName);
            if (skin != null)
            {
                skin.cost = data.cost;
                skin.isPurchased = data.isPurchased;
                skin.sprite = Resources.Load<Sprite>(data.spriteName);
            }
        }
    }
    

}