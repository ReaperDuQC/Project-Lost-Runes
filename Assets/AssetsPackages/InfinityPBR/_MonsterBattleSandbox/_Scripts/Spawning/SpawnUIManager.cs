using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpawnUIManager : MonoBehaviour
{
    public List<CharacterStats_SO> characterSOList;
    public CharacterStats_SO currentCharacterSO;
    //public List<int> unitList;
    //public int currentUnit;
    int index;

    [SerializeField] private TextMeshProUGUI species;
    [SerializeField] private TextMeshProUGUI classType;
    [SerializeField] private TextMeshProUGUI maxHealth;
    [SerializeField] private TextMeshProUGUI weapon;
    [SerializeField] private TextMeshProUGUI currentDamage;
    [SerializeField] private TextMeshProUGUI attackRate;
    [SerializeField] private TextMeshProUGUI criticalChance;
    [SerializeField] private TextMeshProUGUI specialAttack;

    [SerializeField] private List<Image> uiColorImages;
    [SerializeField] private Button spawnButton;
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Color uiColor;
    [SerializeField] private GameObject spawnPortrait;

    private void Start()
    {
        index = 0;
        currentCharacterSO = characterSOList[index];
        DisplayStatsText();
        ColorTheSpawnUI();
        DisplayPortrait();
    }

    public void NextUnit()
    {
        index++;
        if (index > characterSOList.Count - 1) { index = 0;}
        currentCharacterSO = characterSOList[index];
        DisplayStatsText();
        ColorTheSpawnUI();
        DisplayPortrait();
    }

    public void PreviousUnit()
    {
        index--;
        if (index < 0) { index = characterSOList.Count - 1;}
        currentCharacterSO = characterSOList[index];
        DisplayStatsText();
        ColorTheSpawnUI();
        DisplayPortrait();
    }

    private void DisplayStatsText()
    {
        species.text = currentCharacterSO.characterSpecies;
        classType.text = currentCharacterSO.characterClass;
        maxHealth.text = currentCharacterSO.maxHealth.ToString();
        weapon.text = currentCharacterSO.baseAttack?.name;
        specialAttack.text = currentCharacterSO.specialAttack?.name;
        currentDamage.text = currentCharacterSO.baseAttack?.maxDamage.ToString();
        attackRate.text = currentCharacterSO.baseAttack?.attackRate.ToString();
        criticalChance.text = currentCharacterSO.baseAttack?.criticalChance.ToString();

        if (currentCharacterSO.specialAttack != null)
        {
            specialAttack.text = currentCharacterSO.specialAttack.name;
        }
        else
        {
            specialAttack.text = "N/A";
        }
    }

    private void ColorTheSpawnUI()
    {
        uiColor = currentCharacterSO.spawnUIColor;
        Color.RGBToHSV(uiColor, out float hue, out float saturation, out float value);
        Color buttonColorHighlighted = Color.HSVToRGB(hue, saturation - 0.2f, value + 0.2f);
        Color buttonColorPressed = Color.HSVToRGB(hue, saturation - 0.4f, value + 0.4f);
        var spawnButtonColorBlock = spawnButton.colors;

        spawnButtonColorBlock.normalColor = uiColor;
        spawnButtonColorBlock.highlightedColor = buttonColorHighlighted;
        spawnButtonColorBlock.pressedColor = buttonColorPressed;
        spawnButtonColorBlock.selectedColor = uiColor;

        spawnButton.colors = spawnButtonColorBlock;
        forwardButton.colors = spawnButtonColorBlock;
        backButton.colors = spawnButtonColorBlock;

        foreach (Image image in uiColorImages)
        {
            image.color = uiColor;
        }
    }

    private void DisplayPortrait()
    {
        Image spawnPortraitImage = spawnPortrait.GetComponent<Image>();
        spawnPortraitImage.sprite = currentCharacterSO.characterPortrait;
        spawnPortrait.GetComponent<RectTransform>().localPosition = currentCharacterSO.portraitOffset;
    }
}
