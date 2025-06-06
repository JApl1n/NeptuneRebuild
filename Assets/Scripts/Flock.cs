using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// The Flock class that represents a flock of flock units.
/// </summary>
public class Flock : MonoBehaviour
{
    [Header("Flock Settings")]
    [SerializeField] private float moveSpeed = 1;
    public float MoveSpeed => useStaticValue ? MoveSpeedStatic : moveSpeed;
    private static float MoveSpeedStatic;         // whether the speed is controlled by the static slider value
    [SerializeField] private VisionCone vision;
    public HuntingSystem HuntingSystem;
    [SerializeField] private int[] netStructure = new[] { 2, 2 };        // layer structure of the neural network
    [SerializeField] private bool isBetterNet;               // whether the net is partially better than random
    [SerializeField] private bool isIdealNet;               // whether the net is fully learned
    [SerializeField] private bool usingNet;
    [SerializeField] public float iq;
    [SerializeField] private bool displayIqColor;
    [SerializeField] private bool showCenter = true;
    public bool ShowCenter => useStaticValue ? ShowCenterStatic : showCenter;
    private static bool ShowCenterStatic;
    public bool DisplayIqColor => useStaticValue ? DisplayIqColorStatic : displayIqColor;
    private static bool DisplayIqColorStatic;

    [SerializeField] private GameObject iqColorObject;
    private Material iqMaterial;
    private MeshRenderer iqMeshRenderer;
    private Material initialIqMaterial;
    private float _feedForwardTimer = -10;

    [SerializeField] private float timeBetweenFeedForward = 1f;
    [SerializeField] private int maxFlockSize;
    public int MaxFlockSize => useStaticValue ? MaxFlockSizeStatic : maxFlockSize;
    private static int MaxFlockSizeStatic;
    
    [Header("Spawn Setup")]
    [SerializeField] private FlockUnit[] flockUnitPrefabs;
    [SerializeField] private Vector3 spawnBounds;
    
    [Header("Flock Unit Speed Setup")]
    [Range(0, 10)]
    [SerializeField] private float minFlockUnitSpeed;
    public float MinFlockUnitSpeed => useStaticValue ? MinFlockUnitSpeedStatic : minFlockUnitSpeed;
    private static float MinFlockUnitSpeedStatic;
    [Range(0, 10)]
    [SerializeField] private float maxFlockUnitSpeed;
    public float MaxFlockUnitSpeed => useStaticValue ? MaxFlockUnitSpeedStatic : maxFlockUnitSpeed;
    private static float MaxFlockUnitSpeedStatic;
    
    [Header("Flock Unit Detection Distances")]
    [Range(0, 10)]
    [SerializeField] private float cohesionDistance;
    public float CohesionDistance => cohesionDistance;
    
    [Range(0, 10)]
    [SerializeField] private float avoidanceDistance;
    public float AvoidanceDistance => avoidanceDistance;
    
    [Range(0, 10)]
    [SerializeField] private float alignmentDistance;
    public float AlignmentDistance => alignmentDistance;
    
    [Range(0, 100)]
    [SerializeField] private float boundsDistance;
    
    public float BoundsDistance
    {
        get
        {
            if (vision.Target != null && vision.TargetDir.magnitude < MoveSpeed) return 1;
            return boundsDistance;
        }
    }
    
    [Range(0, 100)]
    [SerializeField] private float obstacleAvoidanceDistance;
    public float ObstacleAvoidanceDistance => useStaticValue ? ObstacleAvoidanceDistanceStatic : obstacleAvoidanceDistance;
    private static float ObstacleAvoidanceDistanceStatic;
    
    
    [Header("Weights")]
    [Range(0, 10)]
    [SerializeField] private float cohesionWeight;
    public float CohesionWeight => useStaticValue ? CohesionWeightStatic : cohesionWeight;
    private static float CohesionWeightStatic;
    
    [Range(0, 10)]
    [SerializeField] private float avoidanceWeight;
    public float AvoidanceWeight => avoidanceWeight;
    
    [Range(0, 10)]
    [SerializeField] private float alignmentWeight;
    public float AlignmentWeight => useStaticValue ? AlignmentWeightStatic : alignmentWeight;
    private static float AlignmentWeightStatic;

    [Range(0, 10)]
    [SerializeField] private float boundsWeight;
    public float BoundsWeight => boundsWeight;
    
    [Range(0, 100)]
    [SerializeField] private float obstacleAvoidanceWeight;
    public float ObstacleAvoidanceWeight => useStaticValue ? ObstacleAvoidanceWeightStatic : obstacleAvoidanceWeight;
    private static float ObstacleAvoidanceWeightStatic;

    public List<FlockUnit> AllUnits;
    
    private float emptyFlockTimer = 0;

    private FlockUnit flockUnitPrefab;
    
    [SerializeField] private bool useStaticValue;
    
    public void Awake()
    {
        // Set the static values to the initial values
        timeBetweenFeedForward = Random.Range(0.5f, 1.5f);
        flockUnitPrefab = flockUnitPrefabs[Random.Range(0, flockUnitPrefabs.Length)];
        AllUnits = new List<FlockUnit>();
        HuntingSystem = new HuntingSystem(netStructure);

        // Set the weights of the neural network to the ideal weights
        if (isIdealNet)
        {
            HuntingSystem.layers[1].weights[0, 0] = 1.0f;
            HuntingSystem.layers[1].weights[0, 1] = 0f;
            HuntingSystem.layers[1].weights[1, 0] = 0f;
            HuntingSystem.layers[1].weights[1, 1] = 1.0f;
        } else if (isBetterNet) 
        {
            HuntingSystem.layers[1].weights[0, 0] = 0.8f;
            HuntingSystem.layers[1].weights[0, 1] = 0.2f;
            HuntingSystem.layers[1].weights[1, 0] = 0.2f;
            HuntingSystem.layers[1].weights[1, 1] = 0.8f;
        }
        HuntingSystem.AssessIQ();

        // Set the vision of the flock
        vision.subject = transform;
        
        // Set the initial IQ color
        iqMeshRenderer = iqColorObject.GetComponent<MeshRenderer>();
        iqMaterial = iqMeshRenderer.material;
        initialIqMaterial = Instantiate(iqMaterial);
    }
    
    void Update()
    {
        // Remove any null units from the flock
        for (int i = AllUnits.Count - 1; i >= 0; i--)
        {
            if (AllUnits[i] == null) AllUnits.RemoveAt(i);
        }
        
        // Move each unit in the flock
        for (var i = 0; i < AllUnits.Count; i++)
        {
            AllUnits[i].MoveUnit();
        } 
        
        // Rotate the flock towards the target
        if (usingNet) HuntingSystem.Rotate(transform);
        Move();
        
        // Manage the flock, including removing the flock if it is empty
        ManageFlock();
    }
    
    protected void FixedUpdate()
    {
        // Feed forward the neural network
        if (usingNet)
        {
            if (Time.time - _feedForwardTimer > timeBetweenFeedForward)
            {
                HuntingSystem.FeedForward(transform, vision);
                timeBetweenFeedForward = Random.Range(0.5f, 1.5f);
                iqMaterial.color = DisplayIqColor ? HuntingSystem.iqColor : initialIqMaterial.color;
                _feedForwardTimer = Time.time;
            }
        }

        iqMeshRenderer.enabled = ShowCenter;
    }
    
    /// <summary>
    /// CreateUnit method creates a new flock unit and adds to the current flock.
    /// </summary>
    public void CreateUnit() {
        Vector3 pos = transform.position + new Vector3(Random.Range(-spawnBounds.x, spawnBounds.x), 
            Random.Range(-spawnBounds.y, spawnBounds.y),
            Random.Range(-spawnBounds.z, spawnBounds.z));
        Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360), 0);
        FlockUnit unit = Instantiate(flockUnitPrefab, pos, rot);
        AddUnit(unit);
    }
    
    /// <summary>
    /// AddUnit method adds a flock unit to the current flock.
    /// </summary>
    public void AddUnit(FlockUnit unit) {
        if (AllUnits == null) AllUnits = new List<FlockUnit>();
        // If the flock is not full, add the unit to the flock
        if(AllUnits.Count < MaxFlockSize) {
            AllUnits.Add(unit);
            AllUnits.Last().AssignFlock(this);
            AllUnits.Last().InitializeUnit(Random.Range(minFlockUnitSpeed, maxFlockUnitSpeed));
            if (AllUnits.Count == 1) vision.targetTags = unit.MyPreys;
        }
        else
        {
            // If the flock is full, create a new flock and add the unit to the new flock
            Flock newFlock = Instantiate(this, transform.position + new Vector3(1, 0, 0), Quaternion.identity);
            newFlock.HuntingSystem = new HuntingSystem(HuntingSystem.CopyLayers());
            newFlock.HuntingSystem.Mutate();
            newFlock.AddUnit(unit);
            unit.generation++;
        }
    }

    /// <summary>
    /// RemoveUnit method removes a flock unit from the current flock.
    /// </summary>
    private void ManageFlock() {
        // If the flock is empty, destroy the flock
        if (AllUnits.Count == 0)
        {
            emptyFlockTimer += Time.deltaTime;
            if (emptyFlockTimer > 5)
            {
                Destroy(gameObject);
            }
        }
        else emptyFlockTimer = 0;
    }
    
    /// <summary>
    /// Move method moves the flock in the forward direction.
    /// </summary>
    void Move()
    {
        transform.Translate(Vector3.forward * (MoveSpeed * Time.deltaTime));
    }
    
    /// <summary>
    /// VisualiseNet method visualises the neural network of the flock.
    /// </summary>
    public void VisualiseNet()
    {
        HuntingSystem.VisualiseNet();
    }
    
    public static void SetMoveSpeed(float speed)
    {
        MoveSpeedStatic = speed;
    }
    
    public static void SetMaxFlockSize(float size)
    {
        MaxFlockSizeStatic = (int)size;
    }
    
    public static void SetCohesionWeightStatic(float weight)
    {
        CohesionWeightStatic = weight;
    }
    
    public static void SetObstacleAvoidanceDistanceStatic(float distance)
    {
        ObstacleAvoidanceDistanceStatic = distance;
    }
    
    public static void SetObstacleAvoidanceWeightStatic(float weight)
    {
        ObstacleAvoidanceWeightStatic = weight;
    }

    public static void SetFlockUnitAlignmentWeightStatic(float alignment)
    {
        AlignmentWeightStatic = alignment;
    }
    
    public static void SetMaxFlockUnitSpeedStatic(float speed)
    {
        MaxFlockUnitSpeedStatic = speed;
    }
    
    public static void SetMinFlockUnitSpeedStatic(float speed)
    {
        MinFlockUnitSpeedStatic = speed;
    }
    
    public static void SetDisplayIqColorStatic(bool displayIqColor)
    {
        DisplayIqColorStatic = displayIqColor;
    }
    
    public static void SetShowCenterStatic(bool showCenter)
    {
        ShowCenterStatic = showCenter;
    }
}
