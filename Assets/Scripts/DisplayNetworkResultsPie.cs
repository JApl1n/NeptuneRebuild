using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;


/// <summary>
/// The DisplayNetworkResultsPie class is a MonoBehaviour that displays data in a pie chart.
/// </summary>
public class DisplayNetworkResultsPie : MonoBehaviour
{
    private int arraySize = 0;
    private int minX = 0;
    private int maxX = 100;

    public string XName;
    public string YName;

    float time = 0;
    public float updateInterval = 1;

    [SerializeField] public List<float> xArray = new List<float>();
    [SerializeField] public List<float> yArray = new List<float>();

    private PieChart chart;

    private Color colour;

    private GameObject[] foodObjects;
    private GameObject[] fishObjects;
    private GameObject[] sharkObjects;

    private float foodEnergy;
    private float fishEnergy;
    private float sharkEnergy;



    private void Start() {
        // https://www.youtube.com/watch?v=2pCkInvkwZ0

        chart = this.GetComponentInChildren<PieChart>();
        arraySize = xArray.Count;
        // makeChart();
        // plotScatter();
        // plotPie();
    }


    private void Update() {
        time += Time.deltaTime;
        if (time > updateInterval) {
            gatherData();

            arraySize = xArray.Count;

            // chart.series[0].ClearData();
            for (int i = 0; i < arraySize; i++) {
                chart.series[0].data[i].data[1] = (yArray[i]);
            }
            chart.series[0].AddData(0);
            chart.series[0].RemoveData(3);
            time = 0;
        }
    }


    /// <summary>
    /// displayGraphUpdate method updates the line chart with the latest data.
    /// </summary>
    private void displayGraphUpdate() {
        var chart = this.GetComponentInChildren<PieChart>();
        
        if (chart == null)  {
            Debug.LogError("Missing Chart");
        } 

        arraySize = xArray.Count;
        // chart.series[0].AddData(xArray[arraySize-1],  yArray[arraySize-1]);
    }


    /// <summary>
    /// AddData method adds data to the x and y arrays and updates the chart.
    /// </summary>
    private void AddData(float yData) {
        xArray.Add(xArray.Count * updateInterval);
        yArray.Add(yData);
    }


    /// <summary>
    /// gatherData method gathers data from the scene and stores it in the yArray.
    /// </summary>
    private void gatherData() {
        // int foodCount = getFoodCount();
        // int flockCount = getFlockCount();
        // int predatorCount = getPredatorCount();

        // yArray[0] = foodCount;
        // yArray[1] = flockCount;
        // yArray[2] = predatorCount;

        yArray[0] = getFoodBiomass();
        yArray[1] = getPreyBiomass();
        yArray[2] = getPredatorBiomass();
    }

    /// <summary>
    /// clearData method clears the x and y arrays and removes data from the chart.
    /// </summary>
    private void clearData() {
        xArray = new List<float>();
        yArray = new List<float>();

        arraySize = 0;

        chart.RemoveData();
    }

    /// <summary>
    /// getFoodCount method returns the number of food objects in the scene.
    /// </summary>
    /// <returns>The number of food objects in the scene.</returns>
    public int getFoodCount() {
        return GameObject.FindGameObjectsWithTag("Food").Length;
    }

    /// <summary>
    /// getPredatorCount method returns the number of predator objects in the scene.
    /// </summary>
    /// <returns>The number of predator objects in the scene.</returns>
    public int getPredatorCount() {
        return GameObject.FindGameObjectsWithTag("Shark").Length;
    }

    /// <summary>
    /// getFlockCount method returns the number of flock objects in the scene.
    /// </summary>
    /// <returns>The number of flock objects in the scene.</returns>
    public int getFlockCount() {
        return GameObject.FindGameObjectsWithTag("Flock").Length;
    }

    /// <summary>
    /// getPreyCount method returns the number of prey objects in the scene.
    /// </summary>
    /// <returns>The number of prey objects in the scene.</returns>
    public int getPreyCount() {
        return GameObject.FindGameObjectsWithTag("Fish").Length;
    }


    public float getFoodBiomass() {
        foodObjects = GameObject.FindGameObjectsWithTag("Food");  
        foodEnergy = 0;
        for (int i=0; i<foodObjects.Length; i++) {
            foodEnergy += foodObjects[i].GetComponent<Organism>().energy;
        }
        return foodEnergy;
    }

    public float getPreyBiomass() {
                fishObjects = GameObject.FindGameObjectsWithTag("Fish");  
        fishEnergy = 0;
        for (int i=0; i<fishObjects.Length; i++) {
            fishEnergy += fishObjects[i].GetComponent<Organism>().energy;
        }
        return fishEnergy;
    }

    public float getPredatorBiomass() {
        sharkObjects = GameObject.FindGameObjectsWithTag("Shark");  
        sharkEnergy = 0;
        for (int i=0; i<sharkObjects.Length; i++) {
            sharkEnergy += sharkObjects[i].GetComponent<Organism>().energy;
        }
        return sharkEnergy;
    }


    
    // public void SetXAxisName(int nameId) {
    //     XName = NameIdToName(nameId);
    // }
    
    // public void SetYAxisName(int nameId) {
    //     YName = NameIdToName(nameId);
    // }
    
    // public string NameIdToName(int nameId) {
    //     switch (nameId) {
    //         case 0:
    //             return "Food";
    //         case 1:
    //             return "Shark";
    //         case 2:
    //             return "Flock";
    //         case 3:
    //             return "Fish";
    //         default:
    //             return "";
    //     }
    // }

    // private int getFlockUnitAverageAge() {
    //     Flock flocks[] = GameObject.FindGameObjectsWithTag("Flock").GetComponent<Flock>();
    //     int totalAge = 0;

    //     for (int i = 0; i < flocks.Length; i++) {
    //         totalAge += flocks[i].getAverageAge();
    //     }
    // }

    // public float getFlockDiversity() {
    //     GameObject[] flocks = GameObject.FindGameObjectsWithTag("Flock");
    //     float diversity = 0;
    //     for (int layer = 1; layer < flocks[0].GetComponent<Flock>().brain.layers.Length; layer++) {
    //         for (int i = 0; i < flocks[0].GetComponent<Flock>().brain.layers[layer].size; i++) {
    //             for (int j = 0; j < flocks[0].GetComponent<Flock>().brain.layers[layer+1].size; j++) {
    //                 for (int f = 0; f < flocks.Length; f++) {
    //                     for (int g = 0; g < flocks.Length; g++ ) {
    //                         diversity += Mathf.Abs(flocks[f].GetComponent<Flock>().brain.layers[layer].weights[i, j] - flocks[g].GetComponent<Flock>().brain.layers[layer].weights[i, j]);
    //                     }
    //                 }
    //             }
    //         }
    //     }

    //     return diversity;
    // }

}


// Stat Tracker



