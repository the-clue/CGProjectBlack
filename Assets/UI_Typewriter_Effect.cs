using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Typewriter_Effect : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;  // Reference to the TextMeshPro component
    public float typingSpeed = 0.05f;    // Speed of text appearance
    public float punctuationPause = 0.3f; // Extra delay after punctuation marks

    private void Start()
    {
        // Start the typewriter effect
        StartCoroutine(ShowText());
    }

    private IEnumerator ShowText()
    {
        // Ensure the text is not visible at the start
        textMeshPro.maxVisibleCharacters = 0;

        // Get the total number of characters in the text
        int totalCharacters = textMeshPro.text.Length;

        // Reveal the text one character at a time
        for (int i = 0; i <= totalCharacters; i++)
        {
            textMeshPro.maxVisibleCharacters = i;

            // Check if the current character is punctuation and pause if necessary
            if (i > 0 && IsPunctuation(textMeshPro.text[i - 1]))
            {
                yield return new WaitForSeconds(punctuationPause);
            }
            else
            {
                yield return new WaitForSeconds(typingSpeed);
            }
        }
    }

    // Function to check if a character is a punctuation mark
    private bool IsPunctuation(char character)
    {
        return character == '.' || character == ',' || character == '!' || character == '?';
    }
}
