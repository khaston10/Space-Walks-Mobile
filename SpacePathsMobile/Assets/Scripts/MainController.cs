using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{

    #region Global Variables
    public int currentPuzzleDifficulty;

    public int amountOfEasySolved;
    public int amountOfMediumSolved;
    public int amountOfHardSolved;

    public float averageTimeForEasy;
    public float averageTimeForMed;
    public float averageTimeForHard;

    public float musicVolume;
    public float sfxVolume;

    #endregion

    #region Variables - Text and Images

    public Text timeText;
    private float timer;
    private float mins;
    private float secs;
    public GameObject[] BGImages;

    #endregion

    #region Variables Sounds

    public AudioMixer masterMixer;
    public GameObject MainPanel;
    public GameObject SettingsPanel;
    public GameObject PuzzleSolvedPanel;
    public Button skipButton;
    public Slider musicSlider;
    public Slider SFXSlider;

    #endregion

    #region Variables Vertices

    public bool pathStarted = false;
    private bool puzzleIsSolvable;
    public Button[] vertices;
    public Image[] verticesBackgroundImages;
    public Text[] degreeText;
    int amountOfVertices = 0;
    int amountOfColors = 0;
    List<int> verticiesAvailable = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
    List<string> colorsAvailable = new List<string> { "cyan", "red", "blue", "magenta" };
    List<int> verticiesInUse = new List<int>();
    List<string> colorsInUse = new List<string>();
    List<string> colorsToBeUsed = new List<string>();
    List<int> verticesStartingSize = new List<int>();
    private float pathContinuetimer = .1f;

    // This will hold the current path.
    private List<int> currentPath = new List<int>();

    // THe line drawer is used to draw lines between two points.
    private List<Transform> pathPoints = new List<Transform>();
    public GameObject LineDrawer;

    float tempAvergae = 0;

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        LoadData();
        LoadBGImage();
        SetVolumeSlidersAndLevelsOnStart();
        SetNewPuzzle();

    }

    // Update is called once per frame
    void Update()
    {
        GetInputs(); 
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        mins = Mathf.Floor(timer / 60);
        secs = timer % 60;
        timeText.text = mins.ToString() + " : " + Mathf.RoundToInt(secs).ToString();

        // When the path has been started we need to limit how quickly the user can add to the path.
        if (pathStarted && pathContinuetimer > 0) pathContinuetimer -= Time.deltaTime;
    }

    #region Functions

    #region Save/Load

    public void SaveData()
    {
        GlobalController.Instance.amountOfEasySolved = amountOfEasySolved;
        GlobalController.Instance.amountOfMediumSolved = amountOfMediumSolved;
        GlobalController.Instance.amountOfHardSolved = amountOfHardSolved;
        GlobalController.Instance.averageTimeForEasy = averageTimeForEasy;
        GlobalController.Instance.averageTimeForMed = averageTimeForMed;
        GlobalController.Instance.averageTimeForHard = averageTimeForHard;
        GlobalController.Instance.musicVolume = musicVolume;
        GlobalController.Instance.sfxVolume = sfxVolume;
        GlobalController.Instance.currentPuzzleDifficulty = currentPuzzleDifficulty;

        // Save to player prefs.
        PlayerPrefs.SetInt("amountOfEasySolved", amountOfEasySolved);
        PlayerPrefs.SetInt("amountOfMediumSolved", amountOfMediumSolved);
        PlayerPrefs.SetInt("amountOfHardSolved", amountOfHardSolved);
        PlayerPrefs.SetFloat("averageTimeForEasy", averageTimeForEasy);
        PlayerPrefs.SetFloat("averageTimeForMed", averageTimeForMed);
        PlayerPrefs.SetFloat("averageTimeForHard", averageTimeForHard);
    }

    public void LoadData()
    {
        amountOfEasySolved = GlobalController.Instance.amountOfEasySolved;
        amountOfMediumSolved = GlobalController.Instance.amountOfMediumSolved;
        amountOfHardSolved = GlobalController.Instance.amountOfHardSolved;
        averageTimeForEasy = GlobalController.Instance.averageTimeForEasy;
        averageTimeForMed = GlobalController.Instance.averageTimeForMed;
        averageTimeForHard = GlobalController.Instance.averageTimeForHard;
        musicVolume = GlobalController.Instance.musicVolume;
        sfxVolume = GlobalController.Instance.sfxVolume;
        currentPuzzleDifficulty = GlobalController.Instance.currentPuzzleDifficulty;
    }

    public void LoadBGImage()
    {
        Instantiate(BGImages[Random.Range(0, BGImages.Length)]);
    }

    #endregion

    #region Functions Sound

    public void ToggleSettingsPanel()
    {
        PlayAudioClip(1);

        if (SettingsPanel.activeInHierarchy)
        {
            MainPanel.SetActive(true);
            SettingsPanel.SetActive(false);
        }

        else
        {
            MainPanel.SetActive(false);
            SettingsPanel.SetActive(true);
        }
    }

    public void SetVolumeSlidersAndLevelsOnStart()
    {
        masterMixer.SetFloat("MusicVol", musicVolume);
        masterMixer.SetFloat("SFXVol", sfxVolume);
        musicSlider.value = musicVolume;
        SFXSlider.value = sfxVolume;

    }

    public void AdjustMusicVol(float vol)
    {
        masterMixer.SetFloat("MusicVol", vol);
        musicVolume = vol;
    }

    public void AdjustSFXVol(float vol)
    {
        masterMixer.SetFloat("SFXVol", vol);
        sfxVolume = vol;
    }


    public void PlayAudioClip(int clip)
    {
        /*
        0 - Mouse click Good 1
        1 - Mouse click Good 2
        2 - Mouse click Bad 1
        3 - Mouse click Electric 1
        4 - Mouse click Electric 2
        5 - Celebrate
        */

        GameObject.Find("Sfx Source").GetComponent<SoundEffectsController>().PlayClip(clip);

    }

    #endregion

    #region Functions Vertices

    public void SetNewPuzzle()
    {
        // For easy puzzles we will have a min of 3 vertices, and max of 6.
        // All colors will be used.For the time being colors have not been worked in and the number of 4 is used for all levels.

        // For med puzzles we will have a min of 5 vertices, and max of 8.
        // 3 colors will be used.For the time being colors have not been worked in and the number of 4 is used for all levels.

        // For hard puzzles we will have a min of 7 vertices, and max of 12.
        // 2 colors will be used. For the time being colors have not been worked in and the number of 4 is used for all levels.

        if (currentPuzzleDifficulty == 0)
        {
            amountOfVertices = Random.Range(3, 5);
            amountOfColors = 4;
            

        }
            
        else if (currentPuzzleDifficulty == 1)
        {
            amountOfVertices = Random.Range(5, 8);
            amountOfColors = 4;

        }
            
        else if (currentPuzzleDifficulty == 2)
        {
            amountOfVertices = Random.Range(7, 12);
            amountOfColors = 4;
        } 
        
        for (int i= 0; i < amountOfVertices; i++)
        {
            int indexOfNextVertex = Random.Range(0, verticiesAvailable.Count);
            verticiesInUse.Add(verticiesAvailable[indexOfNextVertex]);
            verticiesAvailable.RemoveAt(indexOfNextVertex);
        }

        for (int i = 0; i < amountOfColors; i++)
        {
            int indexOfNextColor = Random.Range(0, colorsAvailable.Count);
            colorsToBeUsed.Add(colorsAvailable[indexOfNextColor]);
            colorsAvailable.RemoveAt(indexOfNextColor);
        }

        // Hide all the vertices that will not be used.
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < verticiesInUse.Count; i++)
        {
            vertices[verticiesInUse[i]].gameObject.SetActive(true);
        }

        // To ensure the puzzle is solvable we need to decide on the vertices sizes but creating a path first.
        #region Create a temp path for solvabilty.

        bool tempPathIsCompleted = false;
        List<int> tempPath = new List<int>();

        //1. Pick a start point.
        tempPath.Add(verticiesInUse[Random.Range(0, verticiesInUse.Count)]);

        //2. While the path is not completed, continue to build the path.
        int loopCount = 0;
        while (!tempPathIsCompleted)
        {
            loopCount += 1;
            int nextVertexInPath = verticiesInUse[Random.Range(0, verticiesInUse.Count)];

            // Check to ensure the path does not point back to the same vertex.
            if (nextVertexInPath != tempPath[tempPath.Count - 1])
            {
                // Check to ensure that the proposed edge is not already listed in the path.
                bool edgeAlreadyExists = false;

                for (int i = 0; i < tempPath.Count - 1; i++)
                {
                     if (tempPath[i] == tempPath[tempPath.Count - 1] && tempPath[i + 1] == nextVertexInPath)
                    {
                        edgeAlreadyExists = true;
                    }

                    if (tempPath[i + 1] == tempPath[tempPath.Count - 1] && tempPath[i] == nextVertexInPath)
                    {
                        edgeAlreadyExists = true;
                    }
                }

                if (!edgeAlreadyExists)
                {
                    tempPath.Add(nextVertexInPath);
                }
            }

            // Check to see if the path has the minimium requirements to end. This means every vertex in use is listed on the temp path at least once.
            tempPathIsCompleted = true;

            for (int i = 0; i < verticiesInUse.Count; i++)
            {
                if (tempPath.Contains(verticiesInUse[i]))
                {
                    
                }

                else
                {
                    tempPathIsCompleted = false;
                }
            }

            // To make paths more complicated there is a 50% chance that the path will be continued even if the path meets the minium reqs.
            if (tempPathIsCompleted)
            {
                if (Random.Range(0, 100) > 50 && loopCount < 1000)
                {
                    tempPathIsCompleted = false;
                }
            }
        }

        // Update the starting size based upon the temp path.
        for (int i = 0; i < verticiesInUse.Count; i++)
        {
            int degreeOfVertex = 0;

            if (tempPath[0] == verticiesInUse[i]) degreeOfVertex += 1; // Only count the start point once.

            for (int j = 1; j < tempPath.Count - 1; j++)
            {
                if (tempPath[j] == verticiesInUse[i]) degreeOfVertex += 2;
            }

            if (tempPath[tempPath.Count - 1] == verticiesInUse[i]) degreeOfVertex += 1; // Only count the end point once.

            verticesStartingSize.Add(degreeOfVertex);
        }

        

        string pathAsString = "";

        for (int i = 0; i < tempPath.Count; i++)
        {
            pathAsString += tempPath[i].ToString();
            pathAsString += " ,";
        }

        print("Path: " + pathAsString);

        #endregion

        // Set Size and Color.
        for (int i = 0; i < verticiesInUse.Count; i++)
        {
            var indexOfColor = Random.Range(0, colorsToBeUsed.Count);
            colorsInUse.Add(colorsToBeUsed[indexOfColor]); // Save this information so that the vertex color can be restored if the puzzle needs to be reset.
            ChangeVertexColor(verticiesInUse[i], colorsToBeUsed[indexOfColor]);
            ChangeVertexSize(verticiesInUse[i], verticesStartingSize[i]);
        }

    }

    public void StartPath(int vertexNum)
    {
        // Disable the skip button if it is still enabled.
        skipButton.gameObject.SetActive(false);

        PlayAudioClip(4);

        if (!pathStarted && vertices[vertexNum].GetComponent<RectTransform>().rect.height > 30)
        {
            pathStarted = true;

            // Add point the the current path.
            currentPath.Add(vertexNum);
            pathPoints.Add(vertices[vertexNum].transform);
        }
    }

    public void EndPath()
    {
        PlayAudioClip(2);

        // Check to see if the path has been competed correctly.
        if (IsPuzzleComplete())
        {
            // Save data.
            if (currentPuzzleDifficulty == 0)
            {
                amountOfEasySolved += 1;

                // Calculate new average.
                tempAvergae = (((amountOfEasySolved - 1) * averageTimeForEasy) + timer) / amountOfEasySolved;
                averageTimeForEasy = tempAvergae;
            }

            else if (currentPuzzleDifficulty == 1)
            {
                amountOfMediumSolved += 1;

                // Calculate new average.
                tempAvergae = (((amountOfMediumSolved - 1) * averageTimeForMed) + timer) / amountOfMediumSolved;
                averageTimeForMed = tempAvergae;
            }

            else if (currentPuzzleDifficulty == 2)
            {
                amountOfHardSolved += 1;

                // Calculate new average.
                tempAvergae = (((amountOfHardSolved - 1) * averageTimeForHard) + timer) / amountOfHardSolved;
                averageTimeForHard = tempAvergae;
            }

            SaveData();

            PlayAudioClip(5);

            // Give Player Feedback.
            TogglePuzzleSolvedPanel();
        }

        else
        {
            // If it has not we need to reset the puzzle.
            // Play the shale animation.
            for (int i = 0; i < verticiesInUse.Count; i++)
            {
                PlayAnimation(verticesBackgroundImages[verticiesInUse[i]].gameObject, "Shake");
            }

            for (int i = 0; i < verticiesInUse.Count; i++)
            {
                ChangeVertexSize(verticiesInUse[i], verticesStartingSize[i]);
                ChangeVertexColor(verticiesInUse[i], colorsInUse[i]);
            }

        }

        pathStarted = false;

        // Clear the current Path and Path Points.
        currentPath.Clear();
        pathPoints.Clear();

        //Draw a line between these points.
        UpdateThePathRenderer();

    }

    public bool IsPuzzleComplete()
    {
        // Returns true if the puzzle is complete.
        // Check to see if all vertices are now size zero.
        for (int i = 0; i <  verticiesInUse.Count; i++)
        {
            if (GetVertexSize(verticiesInUse[i]) > 0)
            {
                return false;
            }
        }

        return true;
    }

    public void ContinuePath(int vertexNum)
    {
        if (pathStarted && vertices[vertexNum].GetComponent<RectTransform>().rect.height > 30 && pathContinuetimer <= 0)
        {
            // For the time being we will not allow a path from a vertex to the same vertex.
            if (currentPath[currentPath.Count - 1] != vertexNum)
            {

                // We also do not want to add an edge if it already exists.
                if (!CheckIfEdgeExists(currentPath[currentPath.Count-1], vertexNum))
                {
                    // We need to check the size of the previously selected vertex to ensure it still have a size of 1 or greater.
                    if (GetVertexSize(currentPath[currentPath.Count - 1]) > 0)
                    {
                        // Change the size of the next vertex and the one the edge is leaving..
                        ChangeVertexSize(vertexNum, Mathf.RoundToInt(vertices[vertexNum].GetComponent<RectTransform>().rect.height - 40) / 10);
                        ChangeVertexSize(currentPath[currentPath.Count - 1], Mathf.RoundToInt(vertices[currentPath[currentPath.Count - 1]].GetComponent<RectTransform>().rect.height - 40) / 10);

                        // Change the color to grey if the vertex is size 0.
                        if (vertices[vertexNum].GetComponent<RectTransform>().rect.height == 30)
                        {
                            ChangeVertexColor(vertexNum, "grey");
                        }

                        // Reset path continue timer so the path can not grow too fast.
                        pathContinuetimer = .1f;

                        // Add point the the current path.
                        currentPath.Add(vertexNum);
                        pathPoints.Add(vertices[vertexNum].transform);

                        //Draw a line between these points.
                        UpdateThePathRenderer();

                        PlayAudioClip(0);
                    }

                    
                }
                
            }
        }

    }

    public void ChangeVertexColor(int vertexNum, string color)
    {
        // Valid colors - 'green', 'red', 'blue', 'white', 'grey', 'magenta', 'cyan' 

        Color col = vertices[vertexNum].GetComponent<Image>().color;

        if (color == "green") vertices[vertexNum].GetComponent<Image>().color = new Color(0, 225, 0, .3f);
        else if (color == "red") vertices[vertexNum].GetComponent<Image>().color = new Color(225, 0, 0, .3f);
        else if (color == "blue") vertices[vertexNum].GetComponent<Image>().color = new Color(0, 0, 225, .3f);
        else if (color == "cyan") vertices[vertexNum].GetComponent<Image>().color = new Color(0, 225, 255, .3f);
        else if (color == "magenta") vertices[vertexNum].GetComponent<Image>().color = new Color(255, 0, 255, .3f);
        else if (color == "white") vertices[vertexNum].GetComponent<Image>().color = new Color(225, 225, 225, .3f);
        else if (color == "grey") vertices[vertexNum].GetComponent<Image>().color = new Color(100, 100, 100, .3f);
        else print("Invalid Color Selected");        

    }

    public void ChangeVertexSize(int vertexNum, int size)
    {
        // A vertex with size 0 should set to (30, 30).
        // A vertex with size 5, which is considered the max, should be set at (80, 80).

        vertices[vertexNum].GetComponent<RectTransform>().sizeDelta = new Vector2((10*size) + 30, (10 * size) + 30);

        // Update the degree text.
        UpdateDegreeText(vertexNum, size);
    }

    public void UpdateDegreeText(int vertexNum, int size)
    {
        degreeText[vertexNum].text = size.ToString();
    }

    public void UpdateThePathRenderer()
    {
        LineDrawer.GetComponent<LineDrawer>().SetUpLine(pathPoints);
    }

    public bool CheckIfEdgeExists(int vertexStart, int VertextEnd)
    {
        // Returns true if the edge already exists.
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            if (currentPath[i] == vertexStart && currentPath[i + 1] == VertextEnd) return true;
            if (currentPath[i] == VertextEnd && currentPath[i + 1] == vertexStart) return true;
        }

        return false;
    }

    public void TogglePuzzleSolvedPanel()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        PuzzleSolvedPanel.SetActive(true);
    }

    public void ClickNextPuzzle()
    {
        PlayAudioClip(1);

        SceneManager.LoadScene(1);
    }

    public void ClickMainMenu()
    {
        PlayAudioClip(2);

        SceneManager.LoadScene(0);
    }

    public void ClickSkip()
    {
        PlayAudioClip(2);

        SceneManager.LoadScene(1);
    }

    public int GetVertexSize(int vertexNum)
    {
        //print((float)(vertices[vertexNum].GetComponent<RectTransform>().rect.height - 30f) / (float)10);
        return Mathf.RoundToInt((float)(vertices[vertexNum].GetComponent<RectTransform>().rect.height - 30f) / (float)10);
        
    }


    #endregion

    #region Function Animations

    public void PlayAnimation(GameObject objectToChangeAnimation, string nameOfAnimation)
    {
        var anim = objectToChangeAnimation.GetComponent<Animator>();
        anim.Play(nameOfAnimation);
    }

    #endregion


    #region Functions Control Detection

    public void GetInputs()
    {
        if (Input.GetMouseButtonUp(0) && pathStarted)
        {
            EndPath();
        }
    }

    #endregion

    #endregion
}
