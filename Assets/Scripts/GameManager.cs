using System.Collections.Generic;
using System.IO;
using UnityEditor.Overlays;
using UnityEngine;

[System.Serializable]
class UserData
{
    public UserData(string name, int score)
    {
        this.name = name;
        this.score = score;
    }

    public string name;
    public int score;
}

[System.Serializable]
class SaveData
{
    public List<UserData> users = new List<UserData>();
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private SaveData _saveData;
    private UserData _bestUser;
    private UserData _currentUser;

    public static GameManager Instance {
        get {
            if ( _instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
                if(_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(GameManager).Name);
                    _instance = singletonObject.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        Init();
        DontDestroyOnLoad(gameObject);
    }

    private void Init()
    {
        LoadSaveData();
    }

    public string GetBestScore()
    {
        if(_bestUser == null)
        {
            return "Best Score: None";
        }
        return "Best Score: " + _bestUser.name + " <" + _bestUser.score + ">";
    }
    public void NewCurrentUser(string name)
    {
        _currentUser = new UserData(name, 0);
    }
    public void UpdateCurrentScore(int score)
    {
        _currentUser.score = score;
    }

    private void AsignBestUser()
    {
        foreach (UserData data in _saveData.users)
        {
            if (_bestUser == null || data.score > _bestUser.score)
            {
                _bestUser = data;
            }
        }
    }

    public void WriteSaveData()
    {
        bool userFound = false;
        for(int i = 0; i < _saveData.users.Count; i++)
        {
            Debug.Log($"Verificando usuario: {_saveData.users[i].name}, score actual: {_saveData.users[i].score}");
            if (_saveData.users[i].name == _currentUser.name)
            {
                userFound = true;
                Debug.Log($"Usuario encontrado: {_currentUser.name}, comparando scores: {_saveData.users[i].score} < {_currentUser.score}");
                if (_saveData.users[i].score < _currentUser.score)
                {
                    Debug.Log($"Nuevo highscore para {_currentUser.name}: {_currentUser.score}");
                    _saveData.users[i].score = _currentUser.score;
                }
            }
        }
        if (!userFound)
        {
            Debug.Log($"Nuevo usuario agregado: {_currentUser.name}, score: {_currentUser.score}");
            _saveData.users.Add(_currentUser);
        }
        AsignBestUser();
        string json = JsonUtility.ToJson(_saveData);
        File.WriteAllText(Application.persistentDataPath + "/bestScores.json", json);
    }

    public void LoadSaveData()
    {
        string path = Application.persistentDataPath + "/bestScores.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            _saveData = JsonUtility.FromJson<SaveData>(json);
            AsignBestUser();
        }
    }
}
