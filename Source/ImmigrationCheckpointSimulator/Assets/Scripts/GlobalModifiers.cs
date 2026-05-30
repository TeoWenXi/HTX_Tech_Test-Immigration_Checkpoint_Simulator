using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class GlobalValues : MonoBehaviour
{
    [Header("============ Level Creation Section ============")]

    [Header("Random Seed - If 0, a random seed will be generated on start")]
    public string seed = "";
    [HideInInspector]
    public string newSeed = "";
    [HideInInspector]
    public string currScenarioMode = "Default";
    public string newScenarioMode = "Default";

    //Security Checkpoint
    [Header("Security Checkpoint")]
    public QueueLayout SecurityQueueLayout = QueueLayout.QL_SINGLE_LINES;
    public StationsConfig SecurityStations;

    [Space(10)]

    //Security Secondary Check
    [Header("Security Secondary Check")]
    public StationsConfig SecuritySecondaryStations;

    [Space(10)]

    //Immigration
    [Header("Immigration Checkpoint")]
    public QueueLayout ImmigrationQueueLayout = QueueLayout.QL_SINGLE_LINES;
    public StationsConfig ImmigrationStations;

    [Space(10)]

    //Immigration Secondary Check
    [Header("Immigration Secondary Check")]
    public StationsConfig ImmigrationSecondaryStations;

    private void OnValidate()
    {
        SecurityStations.UpdateTotalStations();
        SecuritySecondaryStations.UpdateTotalStations();
        ImmigrationStations.UpdateTotalStations();
        ImmigrationSecondaryStations.UpdateTotalStations();
    }

    [Space(20)]

    [Header("============ Value Modifiers Section ============")]

    [Header("Pre Gate Waiting Area - (0.1 = 10% chance)")]
    [Range(0f, 1f)] public float baseChance     = 0f;
    [Range(0f, 5f)] public float waitingTimeMin = 0f;
    [Range(0f, 5f)] public float waitingTimeMax = 5f;

    [Space(10)]

    [Header("Security Processing Time - (Seconds)")]
    [Range(0f, 10f)] public float SCP_BaseTime                   = 5f;
    [Range(0f, 10f)] public float SCP_TravellerTypeModifier      = 2f;
    [Range(0f, 5f)]  public float SCP_BaggegeModifier            = 2f;
    [Range(0f, 5f)]  public float SCP_DocumentReadinessModifier  = 3f;
    [Range(0f, 10f)] public float SCP_RiskProfileModifier        = 4f;

    [Space(10)]
    
    [Header("Security Secondary Check Chance - (0.1 = 10% chance)")]
    [Range(0f, 1f)]  public float SCPS_SecondaryCheckBaseChance       = 0.02f;
    [Range(0f, 1f)]  public float SCPS_RiskProfileMedium              = 0.08f;
    [Range(0f, 1f)]  public float SCPS_RiskProfileHigh                = 0.2f;
    [Range(0f, 1f)]  public float SCPS_BaggageCountMultiplier         = 0.01f;
    [Range(0f, 1f)]  public float SCPS_DocumentReadinessMultiplier    = 0.1f;
    [Range(-1f, 0f)] public float SCPS_TravellerTypeBusiness          = -0.03f;
    [Range(-1f, 0f)] public float SCPS_TravellerTypeVIP               = -0.05f;
    [Range(0f, 1f)]  public float SCPS_AddtionalModifier              = 0f;

    [Space(10)]

    [Header("Security Secondary Processing Time - (Seconds)")]
    [Range(0f, 50f)]  public float SS_SecondaryCheckBaseTime       = 15f;
    [Range(0f, 50f)]  public float SS_RiskProfileMedium            = 8f;
    [Range(0f, 50f)]  public float SS_RiskProfileHigh              = 20f;
    [Range(0f, 10f)]  public float SS_BaggageCountMultiplier       = 3f;
    [Range(0f, 10f)]  public float SS_DocumentReadinessMultiplier  = 5f;
    [Range(-30f, 0f)] public float SS_TravellerTypeBusiness        = -3f;
    [Range(-30f, 0f)] public float SS_TravellerTypeVIP             = -5f;
    [Range(0f, 50f)]  public float SS_RandomVariance               = 5f;

    [Space(10)]

    [Header("Immigration Processing Time - (Seconds)")]
    [Range(0f, 10f)] public float ICP_BaseTime                  = 8f;
    [Range(0f, 10f)] public float ICP_TravellerTypeModifier     = 2f;
    [Range(0f, 5f)]  public float ICP_DocumentReadinessModifier = 6f;
    [Range(0f, 10f)] public float ICP_RiskProfileModifier       = 5f;
    [Range(0f, 50f)] public float ICP_RandomVariance            = 3f;

    [Space(10)]

    [Header("Immigration Secondary Check Chance - (0.1 = 10% chance)")]
    [Range(0f, 1f)]  public float ICPS_SecondaryCheckBaseChance     = 0.015f;
    [Range(0f, 1f)]  public float ICPS_RiskProfileMedium            = 0.1f;
    [Range(0f, 1f)]  public float ICPS_RiskProfileHigh              = 0.25f;
    [Range(0f, 1f)]  public float ICPS_DocumentReadinessMultiplier  = 0.06f;
    [Range(-1f, 0f)] public float ICPS_TravellerTypeBusiness        = -0.04f;
    [Range(-1f, 0f)] public float ICPS_TravellerTypeVIP             = -0.07f;
    [Range(0f, 1f)]  public float ICPS_AddtionalModifier            = 0f;

    [Space(10)]

    [Header("Immigration Secondary Processing Time - (Seconds)")]
    [Range(0f, 50f)]  public float IS_SecondaryCheckBaseTime        = 18f;
    [Range(0f, 50f)]  public float IS_RiskProfileMedium             = 12f;
    [Range(0f, 50f)]  public float IS_RiskProfileHigh               = 28f;
    [Range(0f, 10f)]  public float IS_DocumentReadinessMultiplier   = 10f;
    [Range(-30f, 0f)] public float IS_TravellerTypeBusiness         = -4f;
    [Range(-30f, 0f)] public float IS_TravellerTypeVIP              = -7f;
    [Range(0f, 50f)]  public float IS_RandomVariance                = 6f;
}

[Serializable]
public class StationsConfig
{
    [Range(0, 10)] public int Rows = 5;
    [Range(0, 10)] public int StationsPerRow = 5;
    [SerializeField] private int totalStations;
    public int totalDisabledStations = 0;

    public void UpdateTotalStations()
    {
        totalStations = Rows * StationsPerRow;
    }
}