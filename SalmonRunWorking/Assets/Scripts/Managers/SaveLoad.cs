using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script controls autosaves between each turn of the game and provides the capability to save the game state at the start 
 * of each turn and to load a prior turn.
 * 
 * Author: Benjamin Person (2021)
 */
public class SaveLoad : MonoBehaviour
{
    private static List<Save> saves = new List<Save>();           //< List of the game states of each turn that has happened so far (index 0 is turn 1)
    private static int currentSaveIndex = 0;               //< The current index in the Saves list to save games to
    //private static GameManager gameManager;    //< Reference to the gameManager which speaks with the TowerManager

    /*
     * Creates a Save object and stores the object in the list of saves
     */
    public static void SaveGame()
    {
        Save currentTurn = new Save();

        // Loop through every tower and save that data in the currentTurn's save
        foreach (TowerBase tower in GameManager.Instance.GetTowerList())
        {
            // General Tower Info that all towers must save
            float[] position = new float[3];
            position[0] = tower.transform.position.x;
            position[1] = tower.transform.position.y;
            position[2] = tower.transform.position.z;

            float[] rotation = new float[3];
            rotation[0] = tower.transform.rotation.x;
            rotation[1] = tower.transform.rotation.y;
            rotation[2] = tower.transform.rotation.z;
            currentTurn.towerPositions.Add(position);
            currentTurn.towerRotations.Add(rotation);

            // Determine the type of Tower we are saving and save its data accordingly to our Save structure
            if (tower is AnglerTower)
            {
                AnglerTower towerScript = tower.GetComponent<AnglerTower>();

                currentTurn.towerTypes.Add(0);
                currentTurn.anglerPlaced.Add(tower.turnPlaced);
                currentTurn.caughtFish.Add(towerScript.fishCaught);
                
                float[] catchRates = new float[3];
                catchRates[0] = towerScript.smallCatchRate;
                catchRates[1] = towerScript.mediumCatchRate;
                catchRates[2] = towerScript.largeCatchRate;
                currentTurn.anglerCatchRates.Add(catchRates);
            }
            else if (tower is RangerTower)
            {
                RangerTower towerScript = tower.GetComponent<RangerTower>();

                currentTurn.towerTypes.Add(1);
                currentTurn.rangerPlaced.Add(tower.turnPlaced);

                float[] regulationRates = new float[3];
                regulationRates[0] = towerScript.slowdownEffectSmall;
                regulationRates[1] = towerScript.slowdownEffectMedium;
                regulationRates[2] = towerScript.slowdownEffectLarge;
                currentTurn.rangerRegulateRates.Add(regulationRates);
            }
            else if (tower is SealionTower)
            {
                SealionTower towerScript = tower.GetComponent<SealionTower>();

                currentTurn.towerTypes.Add(4);
                currentTurn.sealionAppeared.Add(tower.turnPlaced);

                float[] catchRates = new float[2];
                catchRates[0] = towerScript.maleCatchRate;
                catchRates[1] = towerScript.femaleCatchRate;
                currentTurn.sealionCatchRates.Add(catchRates);
            }
            else if (tower is Dam)
            {
                currentTurn.towerTypes.Add(2);

                currentTurn.damPlaced = 1;
            }
            else if (tower is DamLadder)
            {
                currentTurn.towerTypes.Add(3);

                currentTurn.ladderType = 0;
            }
        }

        // Save the current generation of salmon
        List<FishGenome> allFish = GameManager.Instance.school.GetFish();
        // Find all the male and female fish
        List<FishGenome> females = FishGenomeUtilities.FindFemaleGenomes(allFish);
        List<FishGenome> males = FishGenomeUtilities.FindMaleGenomes(allFish);

        // Count and save each size / gender pair
        currentTurn.smallMale = FishGenomeUtilities.FindSmallGenomes(males).Count;
        currentTurn.mediumMale = FishGenomeUtilities.FindMediumGenomes(males).Count;
        currentTurn.largeMale = FishGenomeUtilities.FindLargeGenomes(males).Count;
        currentTurn.smallFemale = FishGenomeUtilities.FindSmallGenomes(females).Count;
        currentTurn.mediumFemale = FishGenomeUtilities.FindMediumGenomes(females).Count;
        currentTurn.largeFemale = FishGenomeUtilities.FindLargeGenomes(females).Count;

        //Push the currentTurn's data to the list of saves
        saves.Insert(currentSaveIndex, currentTurn);
        currentSaveIndex++;
    }

    /*
     * Loads the game state of the turn in the list of saves to revert the game to
     */
    public static void LoadGame()
    {
        // The turn number from the Pause Menu UI slider that we want to revert the game to
        int turn = (int)GameManager.Instance.pauseMenu.turnSlider.value;

        // List counters for each kind of tower we are loading in
        int currentAngler = 0;
        int currentRanger = 0;
        int currentSealion = 0;
        int currentTower = 0;

        // Clear all the existing towers
        foreach (TowerBase tower in GameManager.Instance.GetTowerList())
        {
            Destroy(tower.transform.parent.gameObject);
        }

        // Grab the save from turn - 1 as we start at turn 0, but the pause menu slider starts at 1
        Save loadSave = saves[turn - 1];

        // Loop through each saved tower and load them back into the scene appropriately
        foreach (int towerType in loadSave.towerTypes)
        {
            // NOTE: This can also be accomplished with a Swicth statement, but this seems more readable for little loss of performance
            // Angler
            if (towerType == 0)
            {
                GameObject angler = Instantiate(GameManager.Instance.GetTowerPrefabs()[0]);
                AnglerTower towerScript = angler.GetComponent<AnglerTower>();
                angler.transform.position = new Vector3(loadSave.towerPositions[currentTower][0], loadSave.towerPositions[currentTower][1], loadSave.towerPositions[currentTower][2]);
                angler.transform.rotation = Quaternion.Euler(loadSave.towerRotations[currentTower][0], loadSave.towerRotations[currentTower][1], loadSave.towerRotations[currentTower][2]);
                towerScript.turnPlaced = loadSave.anglerPlaced[currentAngler];
                towerScript.fishCaught = loadSave.caughtFish[currentAngler];
                towerScript.smallCatchRate = loadSave.anglerCatchRates[currentAngler][0];
                towerScript.mediumCatchRate = loadSave.anglerCatchRates[currentAngler][1];
                towerScript.largeCatchRate = loadSave.anglerCatchRates[currentAngler][2];
                currentTower++;
                currentAngler++;
            }
            // Ranger
            else if (towerType == 1)
            {
                GameObject ranger = Instantiate(GameManager.Instance.GetTowerPrefabs()[1]);
                RangerTower towerScript = ranger.GetComponent<RangerTower>();
                ranger.transform.position = new Vector3(loadSave.towerPositions[currentTower][0], loadSave.towerPositions[currentTower][1], loadSave.towerPositions[currentTower][2]);
                ranger.transform.rotation = Quaternion.Euler(loadSave.towerRotations[currentTower][0], loadSave.towerRotations[currentTower][1], loadSave.towerRotations[currentTower][2]);
                towerScript.turnPlaced = loadSave.rangerPlaced[currentRanger];
                towerScript.slowdownEffectSmall = loadSave.rangerRegulateRates[currentRanger][0];
                towerScript.slowdownEffectMedium = loadSave.rangerRegulateRates[currentRanger][1];
                towerScript.slowdownEffectLarge = loadSave.rangerRegulateRates[currentRanger][2];
                currentTower++;
                currentRanger++;
            }
            // Sealion
            else if (towerType == 4)
            {
                GameObject sealion = Instantiate(GameManager.Instance.GetTowerPrefabs()[4]);
                SealionTower towerScript = sealion.GetComponent<SealionTower>();
                sealion.transform.position = new Vector3(loadSave.towerPositions[currentTower][0], loadSave.towerPositions[currentTower][1], loadSave.towerPositions[currentTower][2]);
                sealion.transform.rotation = Quaternion.Euler(loadSave.towerRotations[currentTower][0], loadSave.towerRotations[currentTower][1], loadSave.towerRotations[currentTower][2]);
                towerScript.turnPlaced = loadSave.sealionAppeared[currentSealion];
                towerScript.maleCatchRate = loadSave.sealionCatchRates[currentSealion][0];
                towerScript.femaleCatchRate = loadSave.sealionCatchRates[currentSealion][1];
                currentTower++;
                currentSealion++;
            }
            // Dam
            else if (towerType == 2)
            {
                GameObject dam = Instantiate(GameManager.Instance.GetTowerPrefabs()[2]);
                Dam towerScript = dam.GetComponent<Dam>();
                dam.transform.position = new Vector3(loadSave.towerPositions[currentTower][0], loadSave.towerPositions[currentTower][1], loadSave.towerPositions[currentTower][2]);
                dam.transform.rotation = Quaternion.Euler(loadSave.towerRotations[currentTower][0], loadSave.towerRotations[currentTower][1], loadSave.towerRotations[currentTower][2]);
                towerScript.turnPlaced = loadSave.damPlaced;
                currentTower++;
            }
            // Ladder
            else if (towerType == 3)
            {
                GameObject ladder = Instantiate(GameManager.Instance.GetTowerPrefabs()[3]);
                DamLadder towerScript = ladder.GetComponent<DamLadder>();
                ladder.transform.position = new Vector3(loadSave.towerPositions[currentTower][0], loadSave.towerPositions[currentTower][1], loadSave.towerPositions[currentTower][2]);
                ladder.transform.rotation = Quaternion.Euler(loadSave.towerRotations[currentTower][0], loadSave.towerRotations[currentTower][1], loadSave.towerRotations[currentTower][2]);
                towerScript.turnPlaced = loadSave.ladderType;
                currentTower++;
            }
        }

        // Load in the generation of fish from this turn
        List<FishGenome> revertGeneration = new List<FishGenome>();

        /*
         * This is lengthy. The gist is we COULD just grab the list via GetFish in FishSchool, but then the save
         * would not be potentially serializable if we want that in the future. So instead, we are reconstructing the 
         * appropriate generation based on the counts of small, medium, and large fish (both male and female) we saved
         * at the place stage of that turn.
         */

        // Small Male Fish
        FishGenePair[] SMgenes = new FishGenePair[FishGenome.Length];
        FishGenePair sexPair;
        sexPair.momGene = FishGenome.X;
        sexPair.dadGene = FishGenome.Y;
        FishGenePair sizePair;
        sizePair.momGene = FishGenome.b;
        sizePair.dadGene = FishGenome.b;
        SMgenes[(int)FishGenome.GeneType.Sex] = sexPair;
        SMgenes[(int)FishGenome.GeneType.Size] = sizePair;
        FishGenome smallMGenome = new FishGenome(SMgenes);
        for (int i = 0; i < loadSave.smallMale; i++)
        {
            revertGeneration.Add(smallMGenome);
        }

        // Medium Male Fish
        FishGenePair[] MMgenes = new FishGenePair[FishGenome.Length];
        sizePair.momGene = FishGenome.b;
        sizePair.dadGene = FishGenome.B;
        MMgenes[(int)FishGenome.GeneType.Sex] = sexPair;
        MMgenes[(int)FishGenome.GeneType.Size] = sizePair;
        FishGenome mediumMGenome = new FishGenome(MMgenes);
        for (int i = 0; i < loadSave.mediumMale; i++)
        {
            revertGeneration.Add(mediumMGenome);
        }

        // Large Male Fish
        FishGenePair[] LMgenes = new FishGenePair[FishGenome.Length];
        sizePair.momGene = FishGenome.B;
        sizePair.dadGene = FishGenome.B;
        LMgenes[(int)FishGenome.GeneType.Sex] = sexPair;
        LMgenes[(int)FishGenome.GeneType.Size] = sizePair;
        FishGenome largeMGenome = new FishGenome(LMgenes);
        for (int i = 0; i < loadSave.largeMale; i++)
        {
            revertGeneration.Add(largeMGenome);
        }

        // Small Female Fish
        FishGenePair[] SFgenes = new FishGenePair[FishGenome.Length];
        sexPair.dadGene = FishGenome.X;
        SFgenes[(int)FishGenome.GeneType.Sex] = sexPair;
        sizePair.momGene = FishGenome.b;
        sizePair.dadGene = FishGenome.b;
        SFgenes[(int)FishGenome.GeneType.Size] = sizePair;
        FishGenome smallFGenome = new FishGenome(SFgenes);
        for (int i = 0; i < loadSave.smallFemale; i++)
        {
            revertGeneration.Add(smallFGenome);
        }

        // Medium Female Fish
        FishGenePair[] MFgenes = new FishGenePair[FishGenome.Length];
        sizePair.momGene = FishGenome.B;
        sizePair.dadGene = FishGenome.b;
        MFgenes[(int)FishGenome.GeneType.Sex] = sexPair;
        MFgenes[(int)FishGenome.GeneType.Size] = sizePair;
        FishGenome mediumFGenome = new FishGenome(MFgenes);
        for (int i = 0; i < loadSave.mediumFemale; i++)
        {
            revertGeneration.Add(mediumFGenome);
        }

        // Large Female Fish
        FishGenePair[] LFgenes = new FishGenePair[FishGenome.Length];
        sizePair.momGene = FishGenome.B;
        sizePair.dadGene = FishGenome.B;
        LFgenes[(int)FishGenome.GeneType.Sex] = sexPair;
        LFgenes[(int)FishGenome.GeneType.Size] = sizePair;
        FishGenome largeFGenome = new FishGenome(LFgenes);
        for (int i = 0; i < loadSave.largeFemale; i++)
        {
            revertGeneration.Add(largeFGenome);
        }
        FishGenomeUtilities.Shuffle(revertGeneration);
        GameManager.Instance.school.nextGenerationGenomes = revertGeneration;

        // Remove future turns we reverted over and set the UI slider in the pause menu appropriately
        GameManager.Instance.pauseMenu.turnSlider.maxValue = turn;
        GameManager.Instance.pauseMenu.turnSlider.value = turn;
        GameManager.Instance.Turn = turn;
        GameManager.Instance.SetState(new PlaceState());
        currentSaveIndex = turn;
    }

    /*
     * Class that describes the contents of a single turn's game state at the moment of saving
     */
    [System.Serializable]
    public class Save
    {
        public int turn;        //< The turn number associated with this saved game

        /* Salmon Generation Data */
        // Currently this is being accomplished by way of getting each count of each size and gender combo, and loading in a new genome list at time of loading
        public int smallMale = 0;       //< The number of small male fish from this turn
        public int mediumMale = 0;      //< The number of medium male fish from this turn
        public int largeMale = 0;       //< The number of large male fish from this turn
        public int smallFemale = 0;     //< The number of small female fish from this turn
        public int mediumFemale = 0;    //< The number of medium female fish from this turn
        public int largeFemale = 0;     //< The number of large female fish from this turn

        /* General Tower Info */
        public List<float[]> towerPositions = new List<float[]>();      //< The serialized list of where every tower is
        public List<float[]> towerRotations = new List<float[]>();      //< The serialized list of the rotations of every tower
        public List<int> towerTypes = new List<int>();                  //< The serialized list of what type of tower each tower is

        /* Angler Data */
        public List<int> anglerPlaced = new List<int>();                                    //< The turn the angler was placed into the level
        public List<int> caughtFish = new List<int>();                                      //< The number of fish this angler has currently caught
        public List<float[]> anglerCatchRates = new List<float[]>();    //< The serialized list of each anglers' size catch rates
        //public List<float> anglerCatchReset = new List<float>();        //< The serialized list of each anglers' catch reset time

        /* Ranger Data */
        public List<int> rangerPlaced = new List<int>();                                       //< The turn the ranger was placed into the level
        public List<float[]> rangerRegulateRates = new List<float[]>();    //< The serialized list of each rangers' regulation rates for each size fish
        //public List<float> rangerRegulateReset = new List<float>();        //< The serialized list of each rangers' regulation reset time

        /* Dam Data */
        public int damPlaced = 0;                                          //< The turn the dam was placed into the level
        // The Dams position and type is already tracked in the General Tower Lists

        /* Ladder Data */
        public int ladderType = 0;        //< The type of Salmon Ladder currently placed at the dam

        /* Sealion Data */
        public List<int> sealionAppeared = new List<int>();                                  //< The turn the sealion was placed into the level
        public List<float[]> sealionCatchRates = new List<float[]>();    //< The serialized list of each sealions' size catch rates
        //public List<float> sealionCatchReset = new List<float>();        //< The serialized list of each sealions' catch reset time
    }
}
