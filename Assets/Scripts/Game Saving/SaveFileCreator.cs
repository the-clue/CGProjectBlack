using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveFileCreator
{
    public string saveFileDirectoryPath = "";
    public string saveFileName = "";

    public bool CheckIfFileExists()
    {
        if (File.Exists(Path.Combine(saveFileDirectoryPath, saveFileName)))
        {
            return true;
        }
        return false;
    }

    public void DeleteSaveFile()
    {
        File.Delete(Path.Combine(saveFileDirectoryPath, saveFileName));
    }

    public void CreateNewCharacterSaveFile(CharacterSaveData characterSaveData)
    {
        string savePath = Path.Combine(saveFileDirectoryPath, saveFileName);

        try
        {
            // Create save directory if it does not already exist
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            Debug.Log("CREATING SAVE FILE AT SAVE PATH: " + savePath);

            // Serialize C# Game Data Object into JSON
            string dataToStore = JsonUtility.ToJson(characterSaveData, true);

            // Write file to system
            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                using (StreamWriter fileWriter = new StreamWriter(stream))
                {
                    fileWriter.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ERROR WHILST TRYING TO SAVE CHARACTER DATA, GAME NOT SAVED: " + savePath + "\n" + e);
        }
    }

    public CharacterSaveData LoadSaveFile()
    {
        CharacterSaveData characterSaveData = null;
        string loadPath = Path.Combine(saveFileDirectoryPath, saveFileName);

        if (File.Exists(loadPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // Deserialize data from JSON back to Unity
                characterSaveData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
        }

        return characterSaveData;
    }
}
