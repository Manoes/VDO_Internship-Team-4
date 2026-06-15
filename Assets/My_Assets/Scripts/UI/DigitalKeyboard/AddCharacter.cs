using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Required to access TMP_InputField

public class AddCharacter : MonoBehaviour
{

    [SerializeField] AudioSource asS;
    [SerializeField] AudioClip[] addCharacterSounds;
    [SerializeField] AudioClip deleteCharacterSound;

    public void SetCharacterToName()
    {
        print(" added" + gameObject.name);
        if (SaveSystem.instance == null || SaveSystem.instance.inputField == null)
        {
            Debug.LogError($"[AddCharacter] SaveSystem or InputField is missing in the scene!");
            return;
        }

        string getCurrentText = SaveSystem.instance.inputField.text;
        if (getCurrentText.Length > 9) return;
        // Safety check: Don't play sound if the array is empty
        if (asS != null && addCharacterSounds != null && addCharacterSounds.Length > 0)
        {
            asS.PlayOneShot(addCharacterSounds[UnityEngine.Random.Range(0, addCharacterSounds.Length)]);
        }

        SaveSystem.instance.inputField.text = getCurrentText + gameObject.name;
    }

    public void DeleteCharacter()
    {
        string getCurrentText = SaveSystem.instance.inputField.text;
        if (getCurrentText.Length == 0) return;
        asS.PlayOneShot(deleteCharacterSound);
        SaveSystem.instance.inputField.text = getCurrentText.Remove(getCurrentText.Length - 1);
    }
    public void AddSpace()
    {
        string getCurrentText = SaveSystem.instance.inputField.text;
        if (getCurrentText.Length > 9) return;
        SaveSystem.instance.inputField.text = getCurrentText + " ";
    }


    public void SetAudioSource(AudioSource audioSource)
    {
        asS = audioSource;
    }
}