using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Identical to UI_Stat_Bar except that this one appears and disappears in world space (will always face camera)
public class UI_Character_Health_Bar : UI_StatBar
{
    private CharacterManager character;
    private AICharacterManager aiCharacter;
    private PlayerManager playerCharacter;

    [SerializeField] bool displayCharacterNameOnDamage = false;
    [SerializeField] float defaultTimeBeforeBarHides = 3;
    [SerializeField] float hideTimer = 0;
    [SerializeField] int currentDamageTaken = 0;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI characterDamage;
    [HideInInspector] public int oldHealthValue = 0;

    protected override void Awake()
    {
        base.Awake();

        character = GetComponentInParent<CharacterManager>();

        if (character != null)
        {
            aiCharacter = character as AICharacterManager;
            playerCharacter = character as PlayerManager;
        }
    }

    protected override void Start()
    {
        base.Start();

        gameObject.SetActive(false); 
    }

    public override void SetStat(int newValue)
    {
        if (displayCharacterNameOnDamage)
        {
            characterName.enabled = true;

            if (aiCharacter != null)
            {
                characterName.text = aiCharacter.characterName;
            }

            if (playerCharacter != null)
            {
                characterName.text = playerCharacter.playerNetworkManager.characterName.Value.ToString();
            }
        }
        else
        {
            characterName.text = "";
        }

        // Call this here in case max health changes from an effect (ex. buff)
        slider.maxValue = character.characterNetworkManager.maxHealth.Value;

        // To do: run secondary bar logic

        // Total damage taken while the bar is active
        currentDamageTaken = Mathf.RoundToInt(currentDamageTaken + (oldHealthValue - newValue));

        if (currentDamageTaken < 0) // healing
        {
            currentDamageTaken = Mathf.Abs(currentDamageTaken);
            characterDamage.text = "+ " + currentDamageTaken.ToString();
        }
        else // damaging
        {
            // currentDamageTaken = Mathf.Abs(currentDamageTaken);
            characterDamage.text = "- " + currentDamageTaken.ToString();
        }

        slider.value = newValue;

        if (character.characterNetworkManager.currentHealth.Value != character.characterNetworkManager.maxHealth.Value)
        {
            hideTimer = defaultTimeBeforeBarHides;
            gameObject.SetActive(true);
        }

    }

    private void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward); // so the object always looks at the camera

        if (hideTimer > 0)
        {
            hideTimer -= Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    private void OnDisable()
    {
        currentDamageTaken = 0;
    }
}
