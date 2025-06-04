using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;


/// <summary>
/// The DisplayAllNetworkResults class is a MonoBehaviour that displays data in a line chart.
/// </summary>
public class DisplayAllNetworkResults : MonoBehaviour
{
    private int arraySize = 0;
    private int minX = 0;
    private int maxX = 100;

    private Color colour;

    public string XName;
    public List<string> YNames;

    float time = 0;
    float updateInterval = 1;
    int numFields = 3;

    [SerializeField] public List<float> xArray = new List<float>();
    [SerializeField] public List<List<int>> yArrays = new List<List<int>>();

    [SerializeField] private LineChart chart;


    private void Start() {
        // https://www.youtube.com/watch?v=2pCkInvkwZ0
        arraySize = xArray.Count;
        
        XName = "Time";
        YNames = new List<string> {"Food", "Fish", "Shark"};

        makeChart();
        plotLines();

        chart.series[0].data.Clear();
        xArray = new List<float>();
        yArrays = new List<List<int>>();

        for (int i=0; i<numFields; i++) {
            yArrays.Add(new List<int>());
        }
    }


    private void Update() {
        time += Time.deltaTime;
        if (time > updateInterval) {
            time = 0;
            gatherData();

            displayGraphUpdate(numFields);
        }
    }

    


    /// <summary>
    /// makeChart method creates a line chart in the scene.
    /// </summary>
    public void makeChart() {
        if (transform.childCount > 0) {
            if (chart == null)  {
                chart = gameObject.transform.GetChild(0).gameObject.AddComponent<LineChart>();
                chart.Init();
            } else {
                Debug.Log("Canvas already has scatter chart child");
            }

            // chart.SetSize(600, 600);

            // Set title
            var title = chart.EnsureChartComponent<Title>();
            title.text = "Population Over Time";

            // Set whether prompt boxes and legends are displayed
            var tooltip = chart.EnsureChartComponent<Tooltip>();
            tooltip.show = true;

            var legend = chart.EnsureChartComponent<Legend>();
            legend.show = true;

            // Set axes
            var xAxis = chart.EnsureChartComponent<XAxis>();
            xAxis.splitNumber = 10;
            xAxis.boundaryGap = true;
            xAxis.type = Axis.AxisType.Value;

            var yAxis = chart.EnsureChartComponent<YAxis>();
            yAxis.type = Axis.AxisType.Value;

            // disable line symbol
            chart.series[0].symbol.show = false;

            // Clear default data
            chart.RemoveData();
        }
    }


    /// <summary>
    /// plotLine method plots the data in a line chart.
    /// </summary>
    private void plotLines() {
        for (int index=0; index<numFields; index++) {
            chart.AddSerie<Line>(YNames[index]);
            
            // chart.series[0].symbol.size = 0f;
            chart.series[index].animation.enable = true;
            chart.series[index].symbol.show = false;

            switch (index) {
                case 0:
                    colour = new Color(0, 1f, 0, 1f);  // Green for food
                    break;
                case 1:
                    colour = new Color(0, 0.5f, 1f, 1f);  // Blue for fish
                    break;
                case 2:
                    colour = new Color(1f, 0, 0, 1f);  // Red for sharks
                    break;
            }

            chart.series[index].lineStyle.color = colour;
            chart.series[index].itemStyle.color = colour;
            arraySize = xArray.Count;

            // add data to the chart
            for (int i = 0; i < arraySize; i++) {
                chart.series[index].AddData(xArray[i],  yArrays[index][i]);
            }
        }
    }


    /// <summary>
    /// gatherData method collects data from the game objects.
    /// </summary>
    private void gatherData() {
        for (int field=0; field<numFields; field++) {
            int count = GameObject.FindGameObjectsWithTag(YNames[field]).Length;
            yArrays[field].Add(count);
        }
        xArray.Add(xArray.Count * updateInterval);
    }


    /// <summary>
    /// displayGraphUpdate method updates the line chart with the latest data.
    /// </summary>
    private void displayGraphUpdate(int numFields) {
        if (chart == null)  {
            Debug.LogError("Missing Chart");
            return;
        } 

        arraySize = xArray.Count;
        if (arraySize >= 1) {
            for (int index=0; index<numFields; index++){
                chart.series[index].AddData(xArray[arraySize-1],  yArrays[index][arraySize-1]);
            }
        }
    }
    

}




