using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    // TODO: 'Settings' variables as save log

    // Save Log
    [SerializeField] bool[] obtainedRecipes = new bool[4];
    [SerializeField] int[] bestScores = new int[4];
    [SerializeField] public int stageProgress = 0; // 0: No stages cleared,    [1-4]: cleared up to [1-4] stage

    // System variables
    [SerializeField] int currentStage = 0;  // 0: None(World Map etc.), [1-4]: currently playing [1-4] stage

    public static GameManager Instance;

    private void Awake()
    {
        // Singleton pattern for persisting data between scenes
        // Destroy itself if the same object exists
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // load log automatically
        LoadLog();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateStageClear(int stageScore, bool obtainedRecipe)
    {
        // Update stage progress only if the later stage is cleared
        stageProgress = Math.Max(stageProgress, currentStage);

        // Update recipe info only if it was acquired
        obtainedRecipes[currentStage-1] = obtainedRecipes[currentStage-1] || obtainedRecipe;

        // Update best score
        bestScores[currentStage-1] = Math.Max(bestScores[currentStage-1], stageScore);

        // Save log automatically
        SaveLog();
    }

    [System.Serializable]
    class SaveData
    {
        public bool[] obtainedRecipes = new bool[4];
        public int[] bestScores = new int[4];
        public int stageProgress = 0;
    }

    private void SaveLog()
    {
        SaveData data = new SaveData();
        data.obtainedRecipes = obtainedRecipes;
        data.bestScores = bestScores;
        data.stageProgress = stageProgress;
        string json = JsonUtility.ToJson(data, true);

#if UNITY_EDITOR
        File.WriteAllText(Application.dataPath + "/Save/savefile.json", json);
#else
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
#endif
    }

    private void LoadLog()
    {
#if UNITY_EDITOR
        string path = Application.dataPath + "/Save/savefile.json";
#else
        string path = Application.persistentDataPath + "/savefile.json";
#endif
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            obtainedRecipes = data.obtainedRecipes;
            bestScores = data.bestScores;
            stageProgress = data.stageProgress;
        }
    }
}

