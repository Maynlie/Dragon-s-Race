using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GroundGenerator : MonoBehaviour
{
    public Camera mainCamera;
    public Transform startPoint; //Point from where ground tiles will start
    public PlatformTile[] tiles;
    public PlatformTile startTile;
    public PlatformTile zigzag2;
    public PlatformTile zigzag4;
    public PlatformTile zigzag6;
    public PlatformTile cross;
    public float movingSpeed = 12;
    public int tilesToPreSpawn = 15; //How many tiles should be pre-spawned
    public int nbTutoTiles = 3;
    int nbTemplateNoStar = 0;
    public Text scoreTxt;
    public Text menuTxt;
    public GameObject title;
    string menuString;

    List<PlatformTile> spawnedTiles = new List<PlatformTile>();
    [HideInInspector]
    public bool gameOver = false;
    [HideInInspector]
    public bool invincibleMode = false;
    static bool gameStarted = false;
    [HideInInspector]
    public int score = 0;
    int scoreMax = 0;
    Vector3 currentSpawnPos;
    System.Random random = new System.Random();

    public static GroundGenerator instance;
    // Start is called before the first frame update
    void Start()
    {
        currentSpawnPos = startPoint.position;
        for (int j = 0; j < nbTutoTiles; j++)
        {

            PlatformTile spawnedStartTile = Instantiate(startTile, currentSpawnPos, Quaternion.identity) as PlatformTile;
            currentSpawnPos = spawnedStartTile.endPoint.position - startTile.startPoint.localPosition;
            spawnedStartTile.transform.SetParent(transform);
            spawnedTiles.Add(spawnedStartTile);
        }

        for (int i = 0; i < tilesToPreSpawn; i++)
        {
            CoordonatedGeneration();
        }

            menuString = menuTxt.text;
        scoreMax = PlayerPrefs.GetInt("ScoreMax");
        scoreTxt.text = "x" + scoreMax.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        // Move the object upward in world space x unit/second.
        if (!gameOver && gameStarted)
        {
            title.SetActive(false);
            menuTxt.text = "";
            scoreTxt.text = "x" + score.ToString();
            transform.Translate(-spawnedTiles[0].transform.forward * Time.deltaTime * (movingSpeed * (invincibleMode ? 1.5f : 1) + (score / 10)), Space.World);
        }

        if (mainCamera.WorldToViewportPoint(spawnedTiles[0].endPoint.position).z < 0)
        {
            //Move the tile to the front if it's behind the Camera
            
            PlatformTile tileTmp = spawnedTiles[0];
            if(tileTmp.CompareTag("Separator"))
            {
                CoordonatedGeneration();
            }
            Destroy(tileTmp.gameObject);
            spawnedTiles.RemoveAt(0);
        }

        if (gameOver || !gameStarted)
        {
            if ((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
            {
                if (gameOver)
                {
                    menuTxt.text = "";
                    title.SetActive(false);
                    //Restart current scene
                    Scene scene = SceneManager.GetActiveScene();
                    SceneManager.LoadScene(scene.name);
                    
                }
                else
                {
                    //Start the game
                    gameStarted = true;
                    menuTxt.text = "";
                    title.SetActive(false);
                }
            }
        }

        
    }

    void  CoordonatedGeneration()
    {
        bool spawnCross = false;
        if(nbTemplateNoStar > 0 && !invincibleMode)
        {
            int p = random.Next(1, 10);
            if (p <= nbTemplateNoStar)
            {
                spawnCross = true;
                nbTemplateNoStar = 0;
            }
        }
        

        int randomNumber = random.Next(0, tiles.Length);
        PlatformTile original = tiles[randomNumber];
        PlatformTile brother;

        if(spawnCross)
        {
            currentSpawnPos = spawnedTiles[spawnedTiles.Count - 1].endPoint.position - cross.startPoint.localPosition;
            PlatformTile spawnedCross = Instantiate(cross, currentSpawnPos, Quaternion.identity) as PlatformTile;

            spawnedCross.transform.SetParent(transform);
            spawnedTiles.Add(spawnedCross);
        }
        else if(original.CompareTag("Corridor") || original.CompareTag("Narrow"))
        {
            int iteration = random.Next(2, 4);
            for(int i = 0; i < iteration; i++)
            {
                //currentSpawnPos -= original.startPoint.localPosition;
                currentSpawnPos = spawnedTiles[spawnedTiles.Count - 1].endPoint.position - original.startPoint.localPosition;
                PlatformTile spawnedCorridor= Instantiate(original, currentSpawnPos, Quaternion.identity) as PlatformTile;

                spawnedCorridor.transform.SetParent(transform);
                spawnedTiles.Add(spawnedCorridor);
            }
            if(!invincibleMode)
                nbTemplateNoStar++;
        } 
        else if(original.CompareTag("ZigZag1"))
        {
            brother = zigzag2;
            brotherGeneration(original, brother);
            if (!invincibleMode)
                nbTemplateNoStar++;
        }
        else if (original.CompareTag("ZigZag3"))
        {
            brother = zigzag4;
            brotherGeneration(original, brother);
            if (!invincibleMode)
                nbTemplateNoStar++;
        }
        else if (original.CompareTag("ZigZag5"))
        {
            brother = zigzag6;
            brotherGeneration(original, brother);
            if (!invincibleMode)
                nbTemplateNoStar++;
        }

        currentSpawnPos = spawnedTiles[spawnedTiles.Count - 1].endPoint.position - startTile.startPoint.localPosition;
        PlatformTile spawnedStartTile = Instantiate(startTile, currentSpawnPos, Quaternion.identity) as PlatformTile;
        spawnedStartTile.transform.SetParent(transform);
        spawnedTiles.Add(spawnedStartTile);
    }

    void brotherGeneration(PlatformTile t1, PlatformTile t2)
    {
        int iteration = random.Next(1, 3);
        for (int i = 0; i < iteration; i++)
        {
            currentSpawnPos = spawnedTiles[spawnedTiles.Count - 1].endPoint.position - t1.startPoint.localPosition;
            PlatformTile spawnedT1 = Instantiate(t1, currentSpawnPos, Quaternion.identity) as PlatformTile;

            spawnedT1.transform.SetParent(transform);
            spawnedTiles.Add(spawnedT1);

            currentSpawnPos = spawnedTiles[spawnedTiles.Count - 1].endPoint.position - t2.startPoint.localPosition;
            PlatformTile spawnedT2 = Instantiate(t2, currentSpawnPos, Quaternion.identity) as PlatformTile;

            spawnedT2.transform.SetParent(transform);
            spawnedTiles.Add(spawnedT2);
        }

        currentSpawnPos = spawnedTiles[spawnedTiles.Count - 1].endPoint.position - startTile.startPoint.localPosition;
        PlatformTile spawnedStartTile = Instantiate(startTile, currentSpawnPos, Quaternion.identity) as PlatformTile;
        spawnedStartTile.transform.SetParent(transform);
        spawnedTiles.Add(spawnedStartTile);
    }

    public void lostTheGame()
    {
        gameOver = true;
        menuTxt.text = menuString;
        title.SetActive(true);
        if (score > scoreMax)
        { 
            scoreMax = score;
            PlayerPrefs.SetInt("ScoreMax", scoreMax);
            PlayerPrefs.Save();
        } else
        {
            scoreMax = PlayerPrefs.GetInt("ScoreMax");
        }
        scoreTxt.text = "x" + scoreMax.ToString();
        score = 0;
    }
}
