using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    // Use [SerializeField] and standard C# naming conventions (camelCase)
    // and provide safety checks in Start() or use [RequireComponent] where appropriate.

    // --- UI REFERENCES ---
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI dialogueText; // Changed from TextMeshPro to TextMeshProUGUI (UI element)
    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private SpriteRenderer backgroundRenderer;

    // --- ART ASSETS (Using Dictionaries for scalable lookup) ---
    // Dictionaries are more robust than long if/else chains for lookups.
    [Header("Art Assets")]
    // Create custom serializable classes for Dictionaries to work in the Inspector
    public List<CharacterSpriteEntry> characterSprites;
    public List<BackgroundSpriteEntry> backgroundSprites;

    private Dictionary<string, Sprite> characterLookup = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> backgroundLookup = new Dictionary<string, Sprite>();

    // --- DIALOGUE DATA ---
    [Header("Dialogue Data")]
    public List<DialogueLine> Lines;
    private int index = 0; // Renamed Index to index for style

    void Start()
    {
        // --- Component Safety Checks ---
        // Ensure core components are present if they weren't linked in the Inspector
        if (dialogueText == null) Debug.LogError("Dialogue Text (TextMeshProUGUI) not assigned!", this);
        if (characterRenderer == null) characterRenderer = GetComponent<SpriteRenderer>(); // Example fallback

        // --- Initialize Dictionaries for fast lookup ---
        InitializeLookups();

        // Display the first line immediately
        ImprintLine();
    }

    private void InitializeLookups()
    {
        foreach (var entry in characterSprites)
        {
            if (!characterLookup.ContainsKey(entry.KeyName))
            {
                characterLookup.Add(entry.KeyName, entry.Sprite);
            }
        }

        foreach (var entry in backgroundSprites)
        {
            if (!backgroundLookup.ContainsKey(entry.KeyName))
            {
                backgroundLookup.Add(entry.KeyName, entry.Sprite);
            }
        }
    }

    private void Update()
    {
        // If I hit space. . .
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Set the current line of dialogue to the next one
            index++;
            // And redo all the text and art to match it
            ImprintLine();
        }
    }

    // Makes all the text and art match the dialogue line we're currently on
    public void ImprintLine()
    {
        // If we've hit the end of the script. . .
        if (index >= Lines.Count)
        {
            // End the game
            SceneManager.LoadScene("You Win");
            return;
        }

        // Find which line of dialogue we're currently on
        DialogueLine current = Lines[index];

        // Safely set the text
        if (dialogueText != null)
        {
            dialogueText.text = current.Text;
        }

        // Safely find and set the character art
        if (characterRenderer != null)
        {
            characterRenderer.sprite = GetCharacterSprite(current.CharacterKey);
        }

        // Safely find and set the background art
        if (backgroundRenderer != null)
        {
            backgroundRenderer.sprite = GetBackgroundSprite(current.BackgroundKey);
        }
    }

    // Convert the key string to a sprite using the dictionary
    public Sprite GetCharacterSprite(string whoKey)
    {
        // Use TryGetValue for safe access and robust default handling
        if (string.IsNullOrEmpty(whoKey))
        {
            return characterRenderer.sprite; // If blank, keep current sprite
        }

        if (characterLookup.TryGetValue(whoKey, out Sprite result))
        {
            return result;
        }

        // Log an error if a key isn't found during development
        Debug.LogWarning($"Character key '{whoKey}' not found in lookup dictionary!");
        return characterRenderer.sprite; // Fallback to current sprite
    }

    // Convert the key string to a sprite using the dictionary
    public Sprite GetBackgroundSprite(string whereKey)
    {
        if (string.IsNullOrEmpty(whereKey))
        {
            return backgroundRenderer.sprite; // If blank, keep current sprite
        }

        if (backgroundLookup.TryGetValue(whereKey, out Sprite result))
        {
            return result;
        }

        Debug.LogWarning($"Background key '{whereKey}' not found in lookup dictionary!");
        return backgroundRenderer.sprite; // Fallback to current sprite
    }
}

// --- CUSTOM SERIALIZABLE CLASSES ---

// This line makes a class appear in the Unity Inspector when used as a variable type
[System.Serializable]
public class DialogueLine
{
    // A custom class that just records dialogue, a character, and a background
    public string Text;
    // Renamed these fields to make it clear they are keys for the lookup dictionaries
    public string CharacterKey;
    public string BackgroundKey;
}

// Helper classes to display Dictionaries nicely in the Unity Inspector Lists
[System.Serializable]
public class CharacterSpriteEntry
{
    public string KeyName;
    public Sprite Sprite;
}

[System.Serializable]
public class BackgroundSpriteEntry
{
    public string KeyName;
    public Sprite Sprite;
}
