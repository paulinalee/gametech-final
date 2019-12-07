﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI : MonoBehaviour
{
    // Start is called before the first frame update
    private Canvas display, choiceDisplay, speechDisplay, weaponDisplay, healDisplay, roundStatusDisplay;

    private UpgradeUI upgradeDisplay;
    public GameObject choiceButtonPrefab, weaponButtonPrefab, choiceContainer, weaponContainer;
    private Text speechText;
    void Start()
    {
        display = GameObject.Find("NPCSpeech").GetComponent<Canvas>();

        weaponDisplay = GameObject.Find("WeaponSelectView").GetComponent<Canvas>();
        weaponContainer = GameObject.Find("WeaponSelectContainer/Viewport/Content");
        
        choiceDisplay = GameObject.Find("ChoiceView").GetComponent<Canvas>();
        choiceContainer = GameObject.Find("ChoiceView/ChoiceButtons");
        
        speechDisplay = GameObject.Find("SpeechView").GetComponent<Canvas>();
        speechText = GameObject.Find("SpeechView/Text").GetComponent<Text>();

        healDisplay = GameObject.Find("HealView").GetComponent<Canvas>();
        upgradeDisplay = GameObject.Find("UpgradeScreen").GetComponent<UpgradeUI>();

        roundStatusDisplay = GameObject.Find("RoundStatusView").GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void displayWeapons() {
        speechDisplay.enabled = false;
        weaponDisplay.enabled = true;
        Dictionary<string, WeaponInfo> weapons = weapons = GameObject.Find("WeaponSelect").GetComponent<WeaponSelect>().GetWeapons();
    
        // first we need to delete every child button that was previously here and redisplay
        foreach(Transform child in weaponContainer.transform) {
            GameObject.Destroy(child.gameObject);
        }

        foreach(KeyValuePair<string, WeaponInfo> entry in weapons) {
            GameObject newButton = Instantiate(weaponButtonPrefab);
            newButton.transform.SetParent(weaponContainer.transform);
            Text skillName = newButton.transform.Find("Name").GetComponent<Text>();
            skillName.text = entry.Value.name;

            Text skillDesc = newButton.transform.Find("Description").GetComponent<Text>();
            skillDesc.text = entry.Value.description;

            for(int i = 0; i < 3; i++) {
                Text skillText = newButton.transform.Find("Tooltip").GetChild(i).GetComponent<Text>();
                SkillInfo skillInfo = entry.Value.skills[i];
                LevelInfo baseLevel = skillInfo.levels[0];
                string stats = "PWR: " + baseLevel.power + "   SPD: " + baseLevel.speed + "   RNG: " + baseLevel.range;
                string skillWithStats = string.Join(Environment.NewLine, skillInfo.name, stats);
                skillText.text = skillWithStats;
            }
        }
    }

    public void resetDisplays() {
        display.enabled = true;
        speechDisplay.enabled = true;
        choiceDisplay.enabled = false;
        weaponDisplay.enabled = false;
        healDisplay.enabled = false;
    }

    public void hideDialogueBox() {
        display.enabled = false;
    }

    public void displayChoices(Dictionary<string, string> choiceToFileMap, Dictionary<string, string> metadata) {
        choiceDisplay.enabled = true;
        // make sure there are no other buttons
        foreach(Transform child in choiceContainer.transform) {
            GameObject.Destroy(child.gameObject);
        }

        foreach(KeyValuePair<string, string> entry in choiceToFileMap) {
            GameObject newButton = Instantiate(choiceButtonPrefab);
            newButton.transform.SetParent(choiceContainer.transform);
            Text choiceText = newButton.transform.Find("ChoiceText").GetComponent<Text>();
            ChoiceButton choiceButton = newButton.GetComponent<ChoiceButton>();
            choiceButton.setChoiceFile(entry.Value);
            choiceText.text = entry.Key;
            if (metadata.ContainsKey(choiceText.text)) {
                Debug.Log("metadata: " + metadata[choiceText.text]);
                // have metadata for this, so one of these choice buttons needs to toggle a screen
                switch(metadata[choiceText.text]) {
                    case "reselect":
                        choiceButton.midgameShowWeapons();
                        break;
                    case "heal":
                        choiceButton.midgameShowHeal();
                        break;
                    case "upgrade":
                        choiceButton.midgameShowUpgrade();
                        break;
                }
            }
        }
    }

    public void displayHeals() {
        healDisplay.enabled = true;
        speechDisplay.enabled = false;
        weaponDisplay.enabled = false;
    }

    public void displayUpgrades() {
        Debug.Log("showing upgrade screen");
        upgradeDisplay.showUpgrades();
    }

    public void toggleRoundStatus(bool val) {
        roundStatusDisplay.enabled = val;
    }

    public void showRoundEndMsg() {
        roundStatusDisplay.transform.Find("Status/ClearText").GetComponent<Canvas>().enabled = true;
        roundStatusDisplay.transform.Find("Status/ReadyText").GetComponent<Canvas>().enabled = false;
    }

    public void showReadyMsg() {
        roundStatusDisplay.transform.Find("Status/ReadyText").GetComponent<Canvas>().enabled = true;
        roundStatusDisplay.transform.Find("Status/ClearText").GetComponent<Canvas>().enabled = false;
    }

    public void toggleBranchingDialogue() {
        choiceDisplay.enabled = false;
        speechDisplay.enabled = true;
        healDisplay.enabled = false;
        weaponDisplay.enabled = false;
    }

    public void setSpeech(string text) {
        speechText.text = text;
    }
}