using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;
    public Text HighScoreText;
    
    private bool m_Started = false;
    private static int m_Points;
    private static int m_HighestPoints;
    
    private bool m_GameOver = false;
//    private bool m_GameOver = false;

    
    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0) {
            m_Points = 0;
            const float step = 0.6f;
            int perLine = Mathf.FloorToInt(4.0f / step);
            
            int[] pointCountArray = new [] {1,1,2,2,5,5};
            for (int i = 0; i < LineCount; ++i)
            {
                for (int x = 0; x < perLine; ++x)
                {
                    Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                    var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                    brick.PointValue = pointCountArray[i];
                    brick.onDestroyed.AddListener(AddPoint);
                }
            }
        } else {
            updateHighScore();
        }
        
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0) {
            if (!m_Started)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    m_Started = true;
                    float randomDirection = Random.Range(-1.0f, 1.0f);
                    Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                    forceDir.Normalize();

                    Ball.transform.SetParent(null);
                    Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
                }
            }
            else if (m_GameOver)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";

        if(m_Points > m_HighestPoints) {
            m_HighestPoints = m_Points;
            updateHighScore();
        }
    }

    void updateHighScore()
    {
        HighScoreText.text = $"Highest Score : {m_HighestPoints}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        if(m_Points > m_HighestPoints) {
            m_HighestPoints = m_Points;
            SaveScore();
        }
    }




private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
        LoadScore();
        updateHighScore();
    }



    [System.Serializable]
    class SaveData
    {
        public int Score;
    }

    public void SaveScore()
    {
        SaveData score= new SaveData();
        score.Score = m_HighestPoints;

        string json = JsonUtility.ToJson(score);
  
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            m_HighestPoints = data.Score;
        }
    }

    public void BackToMenu() {
        SceneManager.LoadScene(1);
    }

    public void BackToGame() {
        SceneManager.LoadScene(0);
    }

    public void Exit()
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit(); // original code to quit Unity player
        #endif
    }
}
