using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIEquipmentManagerInputManager : MonoBehaviour
{
    PlayerControls playerControls;

    PlayerUIEquipmentManager playerUIEquipmentManager;

    [Header("Inputs")]
    [SerializeField] bool unequipItemInput;

    private void Awake()
    {
        playerUIEquipmentManager = GetComponentInParent<PlayerUIEquipmentManager>();

    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.UI.Delete.performed += i => unequipItemInput = true;
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        HandlePlayerUIEquipmentManagerInputs();
    }

    private void HandlePlayerUIEquipmentManagerInputs()
    {
        if (unequipItemInput)
        {
            unequipItemInput = false;

            playerUIEquipmentManager.UnEquipSelectedItem();
        }
    }
}
