using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Relics : MonoBehaviour
{
    public GameObject[] relics;
    public Button[] relicButtons;
    public Text description;
    public GameObject secret;

    private GameObject selectedRelic;
    private Button selectedRelicButton;
    private string[] relicDescriptions = new string[] 
    {
        "- Relic of Certainty -\n\nMarks a progression of 30m !\n\nYou are now certain of your path\n\nActivate to grant a 20m bonus at start",
        "- Relic of Serenity -\n\nMarks a progression of 75m !\n\nMay a peacful mind guide you to greatness\n\nActivate to allow more control of the ball mid-air",
        "- Relic of Vanity -\n\nMarks a progression of 150m !\n\nHow far is the road ahead?\n\nActivate to grant a 100m bonus at start",
        "- Relic of Transcendence -\n\nMarks a progression of 300m !\n\nAbove and beyond, what an impressive feat\n\nActivate to jump higher",
        "- Relic of Aeon -\n\nMarks a progression of 600m !\n\nGlory to the one who has withstood the test of time\n\nActivate to keep the world in stasis",
        "- Relic of Infinity -\n\nMarks a progression of 1000m !\n\nYou have made the impossible possible...\nYou have achieved infinity !\n\nActivate to switch color theme"
    };

    void OnEnable()
    {
        for (int i = 0; i < relics.Length; i++)
        {
            if ((PlayerData.relicsCollected & (1 << i)) != 0)
            {
                relics[i].GetComponent<Renderer>().enabled = true;
                relicButtons[i].interactable = true;
            } else
            {
                relics[i].GetComponent<Renderer>().enabled = false;
                relicButtons[i].interactable = false;
            }
        }
        
        if (PlayerData.relicActivated == -1)
        {
            if (PlayerData.relicsCollected == 0)
            {
                description.text = "You have yet to find any relic.\nKeep Going!";
            }
            else
            {
                description.text = "Select a relic to activate";
            }
        } else
        {
            selectedRelic = relics[PlayerData.relicActivated];
            selectedRelicButton = relicButtons[PlayerData.relicActivated];
            HighlightRelic(true);

            description.text = relicDescriptions[PlayerData.relicActivated];
        }
        

        if (PlayerData.relicsCollected == 63)
        {
            secret.SetActive(true);
        } else
        {
            secret.SetActive(false);
        }
    }

    void OnDisable()
    {
        HighlightRelic(false);
    }

    public void OnRelicSelected(int i)
    {
        HighlightRelic(false);
        selectedRelic = relics[i];
        selectedRelicButton = relicButtons[i];
        HighlightRelic(true);

        PlayerData.relicActivated = i;
        GameEvents.RelicActivated(i);
        description.text = relicDescriptions[i];
    }

    public void CancelSelection()
    {
        HighlightRelic(false);
        PlayerData.relicActivated = -1;
        if (PlayerData.relicsCollected == 0)
        {
            description.text = "You have yet to find any relic.\nKeep Going!";
        }
        else
        {
            description.text = "Select a relic to activate";
        }
    }

    private void HighlightRelic(bool state)
    {
        if (selectedRelic != null && selectedRelicButton != null)
        {
            selectedRelic.transform.localScale *= state ? 1.5f : 1 / 1.5f;
            selectedRelicButton.transform.GetChild(0).gameObject.SetActive(state);
        }
    }
}
