using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The GameController class is a MonoBehaviour that controls the game.
/// </summary>
public class GameController : MonoBehaviour
{
    private float manualScale = 1.0f;
    
    /// <summary>
    /// SetTimeScale method sets the time scale of the game.
    /// </summary>
    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale*manualScale;
    }
    
    /// <summary>
    /// ResetGame method resets the game, removes all creatures.
    /// </summary>
    public void ResetGame()
    {
        Time.timeScale = manualScale;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
