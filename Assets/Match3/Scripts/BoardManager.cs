using cookapps;
using DG.Tweening;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour
{
    public static readonly float MOVING_SLOT_TIME = 1f;

    public static BoardManager main;

    public static float slotoffset = 68f;

    private readonly float outlineRectObjectOffset = 15f;

    private readonly float outlineRoundObjectOffset = 20f;

    private readonly float outlineStraightHOffsetX = 34f;

    private readonly float outlineStraightHOffsetY = 34f;

    private readonly float outlineStraightVOffsetX = 34f;

    private readonly float outlineStraightVOffsetY = 34f;

    private readonly float arrowOffset = 68f;

    [HideInInspector]
    public MapBoardData boardData;

    private GameObject BoardOutlineGroup;

    public bool[,] bringDownGenerator;

    public string[] chipTypes = new string[6]
    {
        "Red",
        "Orange",
        "Yellow",
        "Green",
        "Blue",
        "Purple"
    };

    public string[] chipTypesSimple = new string[6]
    {
        "R",
        "O",
        "Y",
        "G",
        "B",
        "P"
    };

    public int CurrentMapIndex;

    private readonly float defaultMutekiTime = 0.4f;

    public Dictionary<BoardPosition, MapDataGeneratorDrop> dicGeneratorDrop;

    public Dictionary<BoardPosition, MapDataGeneratorSpecialDrop> dicGeneratorSpecialDrop;

    private Vector3 dragLineRotationVector45_1;

    private Vector3 dragLineRotationVector45_2;

    private Vector3 dragLineRotationVectorM45_1;

    private Vector3 dragLineRotationVectorM45_2;

    [HideInInspector]
    public bool enabledTutorialOutlineEffect;

    public bool IsMovingNextLine;

    public List<CandyFactory> listCandyFactory = new List<CandyFactory>();

    public List<Crow> listCrowBlock = new List<Crow>();

    public List<PastryBag> listPastryBag = new List<PastryBag>();

    public List<Slot> listRailStartSlot = new List<Slot>();

    public List<GreenSlimeParent> listSlime = new List<GreenSlimeParent>();

    public List<MilkCarton> listMilkCarton = new List<MilkCarton>();

    public List<GameObject> listCowBell = new List<GameObject>();

    public List<NumberChocolateBlock> listNumberChocolate = new List<NumberChocolateBlock>();

    public List<Wall> listWall = new List<Wall>();

    [HideInInspector]
    public GameObject mask;

    public Dictionary<string, GameObject> ObjRescueGingerMan;

    private GameObject objSweetRoadGateEnter;

    private GameObject objSweetRoadGateExit;

    private GameObject objSweetRoadGateExitArrow;

    private GameObject objSweetRoadSomething;

    private readonly List<GameObject> objTutorialEffects = new List<GameObject>();

    private GameObject objTutorialGuideCursor;

    private GameObject oldSlotGroup;

    public GameObject[] PrefabBoardOutlines = new GameObject[18];

    public int remainRescueFriendInBoard;

    [HideInInspector]
    public GameObject rescueGingerManGroup;

    private Vector3 retPosition = Vector3.zero;

    [HideInInspector]
    public GameObject slotGroup;

    public Dictionary<int, Slot> slots;

    public Sprite[] SpritesSlotBackground;

    private bool isProcessDigBoard;

    public static int width => MapData.MaxWidth;

    public static int height => MapData.MaxHeight;

    private void Awake()
    {
        main = this;
        dragLineRotationVector45_1 = new Vector3(slotoffset, slotoffset, 0f);
        dragLineRotationVector45_2 = new Vector3(0f - slotoffset, 0f - slotoffset, 0f);
        dragLineRotationVectorM45_1 = new Vector3(0f - slotoffset, slotoffset, 0f);
        dragLineRotationVectorM45_2 = new Vector3(slotoffset, 0f - slotoffset, 0f);
    }

    public void StartBoard()
    {
        CurrentMapIndex = 0;
        CreateBoard();
    }

    public void CompatibleCreateBlock(int x, int y, IBlockType type, int param1, int param2)
    {
        switch (type)
        {
            case IBlockType.FruitsBox:
            case IBlockType.Mouse:
            case IBlockType.MouseBlockEnd:
            case IBlockType.Weed:
            case IBlockType.Ribbon:
            case IBlockType.DiggingBlock:
            case IBlockType.GreenSlimeChild:
            case IBlockType.Yarn:
                break;
            case IBlockType.Crunky_HP1:
            case IBlockType.Crunky_HP2:
            case IBlockType.Crunky_HP3:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Obstacle_Crunky_lv" + (int)(type - 9 + 1));
                    item.transform.position = slot.transform.position;
                    item.name = "Block_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    Crunky component9 = item.GetComponent<Crunky>();
                    slot.SetBlock(component9);
                    component9.slot = slot;
                    component9.Initialize();
                    break;
                }
            case IBlockType.ChocolateJail:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("ChocolateJail");
                    item.transform.position = slot.transform.position;
                    item.name = "ChocolateJail_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    ChocolateJail component8 = item.GetComponent<ChocolateJail>();
                    slot.SetBlock(component8);
                    component8.slot = slot;
                    component8.Initialize();
                    break;
                }
            case IBlockType.CandyFactory_1:
            case IBlockType.CandyFactory_2:
            case IBlockType.CandyFactory_3:
            case IBlockType.CandyFactory_4:
            case IBlockType.CandyFactory_5:
            case IBlockType.CandyFactory_6:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Eff_jam_" + chipTypesSimple[(int)(type - 3)]);
                    item.transform.position = slot.transform.position;
                    item.name = "JamBottle_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    CandyFactory component13 = item.GetComponent<CandyFactory>();
                    listCandyFactory.Add(component13);
                    slot.SetBlock(component13);
                    component13.slot = slot;
                    component13.Initialize();
                    break;
                }
            case IBlockType.RescueFriend:
                {
                    remainRescueFriendInBoard++;
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Obstacle_gingerman_idle");
                    item.transform.position = slot.transform.position;
                    item.name = "RescueFriend" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    RescueFriend component12 = item.GetComponent<RescueFriend>();
                    slot.SetBlock(component12);
                    component12.slot = slot;
                    component12.Initialize();
                    break;
                }
            case IBlockType.Crow:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Eff_Crow_idle");
                    item.transform.position = slot.transform.position;
                    item.name = "Block_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    Crow component11 = item.GetComponent<Crow>();
                    listCrowBlock.Add(component11);
                    slot.SetBlock(component11);
                    component11.slot = slot;
                    component11.Initialize();
                    break;
                }
            case IBlockType.MagicalCrow:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Eff_MC_idle");
                    item.transform.position = slot.transform.position;
                    item.name = "Block_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    Crow component11 = item.GetComponent<MagicalCrow>();
                    listCrowBlock.Add(component11);
                    slot.SetBlock(component11);
                    component11.slot = slot;
                    component11.Initialize();
                    break;
                }
            case IBlockType.PastryBag:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Obstacle_pastrybag_idle");
                    item.transform.position = slot.transform.position;
                    item.name = "PastryBag_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    PastryBag component10 = item.GetComponent<PastryBag>();
                    component10.makeTurnCount = boardData.param1[x, y];
                    component10.makeChocolateCount = boardData.param2[x, y];
                    listPastryBag.Add(component10);
                    slot.SetBlock(component10);
                    component10.slot = slot;
                    component10.Initialize();
                    break;
                }
            case IBlockType.Digging_HP1:
            case IBlockType.Digging_HP2:
            case IBlockType.Digging_HP3:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Obstacle_D_normal_lv" + (int)(type - 21 + 1));
                    item.SetActive(value: false);
                    item.SetActive(value: true);
                    item.transform.position = slot.transform.position;
                    item.name = "Block_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    Dig component7 = item.GetComponent<Dig>();
                    slot.SetBlock(component7);
                    component7.slot = slot;
                    component7.Initialize();
                    break;
                }
            case IBlockType.Digging_HP1_Collect:
            case IBlockType.Digging_HP2_Collect:
            case IBlockType.Digging_HP3_Collect:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Obstacle_D_normal_Candy_lv" + (int)(type - 24 + 1));
                    item.transform.position = slot.transform.position;
                    item.name = "Block_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    Dig component7 = item.GetComponent<Dig>();
                    slot.SetBlock(component7);
                    component7.slot = slot;
                    component7.Initialize();
                    break;
                }
            case IBlockType.Digging_HP1_Bomb1:
            case IBlockType.Digging_HP2_Bomb1:
            case IBlockType.Digging_HP3_Bomb1:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Obstacle_D_Question_b1_lv" + (int)(type - 27 + 1));
                    item.transform.position = slot.transform.position;
                    item.name = "Block_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    Dig component7 = item.GetComponent<Dig>();
                    slot.SetBlock(component7);
                    component7.slot = slot;
                    component7.Initialize();
                    break;
                }
            case IBlockType.Digging_HP1_Bomb2:
            case IBlockType.Digging_HP2_Bomb2:
            case IBlockType.Digging_HP3_Bomb2:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Obstacle_D_Question_b2_lv" + (int)(type - 30 + 1));
                    item.transform.position = slot.transform.position;
                    item.name = "Block_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    Dig component7 = item.GetComponent<Dig>();
                    slot.SetBlock(component7);
                    component7.slot = slot;
                    component7.Initialize();
                    break;
                }
            case IBlockType.Digging_HP1_Bomb3:
            case IBlockType.Digging_HP2_Bomb3:
            case IBlockType.Digging_HP3_Bomb3:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Obstacle_D_Question_b3_lv" + (int)(type - 33 + 1));
                    item.transform.position = slot.transform.position;
                    item.name = "Block_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    Dig component7 = item.GetComponent<Dig>();
                    slot.SetBlock(component7);
                    component7.slot = slot;
                    component7.Initialize();
                    break;
                }
            case IBlockType.Digging_HP1_Treasure_G1:
            case IBlockType.Digging_HP2_Treasure_G1:
            case IBlockType.Digging_HP3_Treasure_G1:
            case IBlockType.Digging_HP1_Treasure_G2:
            case IBlockType.Digging_HP2_Treasure_G2:
            case IBlockType.Digging_HP3_Treasure_G2:
            case IBlockType.Digging_HP1_Treasure_G3:
            case IBlockType.Digging_HP2_Treasure_G3:
            case IBlockType.Digging_HP3_Treasure_G3:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = null;
                    int num = 3;
                    int num2 = 3;
                    switch (type)
                    {
                        case IBlockType.Digging_HP1_Treasure_G1:
                        case IBlockType.Digging_HP1_Treasure_G2:
                        case IBlockType.Digging_HP1_Treasure_G3:
                            num = 1;
                            break;
                        case IBlockType.Digging_HP2_Treasure_G1:
                        case IBlockType.Digging_HP2_Treasure_G2:
                        case IBlockType.Digging_HP2_Treasure_G3:
                            num = 2;
                            break;
                        case IBlockType.Digging_HP3_Treasure_G1:
                        case IBlockType.Digging_HP3_Treasure_G2:
                        case IBlockType.Digging_HP3_Treasure_G3:
                            num = 3;
                            break;
                    }
                    switch (type)
                    {
                        case IBlockType.Digging_HP1_Treasure_G1:
                        case IBlockType.Digging_HP2_Treasure_G1:
                        case IBlockType.Digging_HP3_Treasure_G1:
                            num2 = 1;
                            break;
                        case IBlockType.Digging_HP1_Treasure_G2:
                        case IBlockType.Digging_HP2_Treasure_G2:
                        case IBlockType.Digging_HP3_Treasure_G2:
                            num2 = 2;
                            break;
                        case IBlockType.Digging_HP1_Treasure_G3:
                        case IBlockType.Digging_HP2_Treasure_G3:
                        case IBlockType.Digging_HP3_Treasure_G3:
                            num2 = 3;
                            break;
                    }
                    item = ContentAssistant.main.GetItem($"Obstacle_D_normal_Treasure_G{num2}_lv{num}");
                    item.transform.position = slot.transform.position;
                    item.name = "Block_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    Dig component7 = item.GetComponent<Dig>();
                    component7.blockType = type;
                    slot.SetBlock(component7);
                    component7.slot = slot;
                    component7.Initialize();
                    break;
                }
            case IBlockType.DiggingBlank:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Obstacle_D_Blank");
                    item.transform.position = slot.transform.position;
                    item.name = "DigBlank_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    DigBlank component6 = item.GetComponent<DigBlank>();
                    slot.canBeCrush = false;
                    slot.canBeControl = false;
                    slot.SetWall(Side.Top);
                    slot.SetWall(Side.Bottom);
                    slot.SetWall(Side.Left);
                    slot.SetWall(Side.Right);
                    slot.SetBlock(component6);
                    component6.slot = slot;
                    component6.Initialize();
                    break;
                }
            case IBlockType.GreenSlime:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Obstacle_Slime_body_idle");
                    item.transform.position = slot.transform.position;
                    item.name = "Slime_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    GreenSlimeParent component5 = item.GetComponent<GreenSlimeParent>();
                    listSlime.Add(component5);
                    component5.slot = slot;
                    component5.Initialize();
                    slot.SetBlock(component5);
                    break;
                }
            case IBlockType.MilkCarton:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Obstacle_MilkCarton");
                    item.transform.position = slot.transform.position;
                    item.name = "MilkCorton_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    MilkCarton component4 = item.GetComponent<MilkCarton>();
                    listMilkCarton.Add(component4);
                    component4.slot = slot;
                    component4.Initialize();
                    slot.SetBlock(component4);
                    break;
                }
            case IBlockType.SpriteDrink_1_HP1:
            case IBlockType.SpriteDrink_1_HP2:
            case IBlockType.SpriteDrink_1_HP3:
            case IBlockType.SpriteDrink_2_HP1:
            case IBlockType.SpriteDrink_2_HP2:
            case IBlockType.SpriteDrink_2_HP3:
            case IBlockType.SpriteDrink_3_HP1:
            case IBlockType.SpriteDrink_3_HP2:
            case IBlockType.SpriteDrink_3_HP3:
            case IBlockType.SpriteDrink_4_HP1:
            case IBlockType.SpriteDrink_4_HP2:
            case IBlockType.SpriteDrink_4_HP3:
            case IBlockType.SpriteDrink_5_HP1:
            case IBlockType.SpriteDrink_5_HP2:
            case IBlockType.SpriteDrink_5_HP3:
            case IBlockType.SpriteDrink_6_HP1:
            case IBlockType.SpriteDrink_6_HP2:
            case IBlockType.SpriteDrink_6_HP3:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Obstacle_Sprite_Drink_" + chipTypesSimple[(int)(type - 51) / 3]);
                    item.transform.position = slot.transform.position;
                    item.name = "SpriteDrink_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    SpriteDrink component3 = item.GetComponent<SpriteDrink>();
                    component3.slot = slot;
                    component3.Initialize((int)(type - 51) / 3, (int)(type - 51) % 3);
                    slot.SetBlock(component3);
                    break;
                }
            case IBlockType.Pocket:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Obstacle_Pocket");
                    item.transform.position = slot.transform.position;
                    item.name = "Pocket_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    Pocket component2 = item.GetComponent<Pocket>();
                    component2.slot = slot;
                    component2.Initialize();
                    slot.SetBlock(component2);
                    break;
                }
            case IBlockType.NumberChocolate:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Obstacle_Number_Chocolate");
                    item.transform.position = slot.transform.position;
                    item.name = "NumberChocolate_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    NumberChocolateBlock component = item.GetComponent<NumberChocolateBlock>();
                    listNumberChocolate.Add(component);
                    component.slot = slot;
                    component.Initialize(boardData.GetRandomChip(), boardData.inOrder[x, y]);
                    slot.SetBlock(component);
                    break;
                }
            case IBlockType.NumberChocolate_1:
            case IBlockType.NumberChocolate_2:
            case IBlockType.NumberChocolate_3:
            case IBlockType.NumberChocolate_4:
            case IBlockType.NumberChocolate_5:
            case IBlockType.NumberChocolate_6:
                {
                    Slot slot = GetSlot(x, y);
                    GameObject item = ContentAssistant.main.GetItem("Obstacle_Number_Chocolate");
                    item.transform.position = slot.transform.position;
                    item.name = "NumberChocolate_" + x + "x" + y;
                    item.transform.parent = slot.transform;

                    NumberChocolateBlock component = item.GetComponent<NumberChocolateBlock>();
                    listNumberChocolate.Add(component);
                    component.slot = slot;
                    component.Initialize((int)(type - 70), boardData.inOrder[x, y]);
                    slot.SetBlock(component);
                    break;
                }
        }
    }

    public void CreateBoard()
    {
        RemoveBoard();
        boardData = MapData.main.CurrentMapBoardData;
        boardData.InitForGamePlay();
        GameMain.ResetBoard();
        if (CurrentMapIndex == 0)
        {
            GameMain.Reset();
        }
        GenerateSlots();
        GenerateBlocks();
        SetRibbonBlocks();
        SetYarnBlocks();
        RefreshTunnelWall();
        GenerateWalls();
        GenerateDropWalls();
        GenerateChips();
        GeneratePowerups();
        SetRailSlot();
        if (MapData.main.target == GoalTarget.SweetRoad)
        {
            GenerateSweetRoadGate();
        }
        else if (MapData.main.target == GoalTarget.Jelly)
        {
            GenerateJelly();
        }
        else if (MapData.main.target == GoalTarget.CollectCracker)
        {
            GenerateMilkTileFirst();
        }
        if (MapData.main.GetRescueGingerManCount() > 0)
        {
            GenerateRescueGingerMan();
        }
        if (MapData.main.target == GoalTarget.Digging)
        {
            //CPanelGameUI.Instance.InitGameCameraMasking(CPanelGameUI.GameCameraMaskingType.Digging);
        }
        DrawTutorialOutlineEffect();
        GameMain.main.enabled = true;
        GameMain.main.eventCount++;
    }

    private IEnumerator doTutorialCursorAnimation(int startX, int startY, int endX, int endY)
    {
        float zoomInTime = 0.2f;
        float moveTime = 1f;
        float zoomOutTime = 0.2f;
        float moveReverseTime = 0.74f;
        float waitTime = 0.2f;
        while ((bool)objTutorialGuideCursor)
        {
            yield return new WaitForSeconds(waitTime);
            if ((bool)objTutorialGuideCursor)
            {
                objTutorialGuideCursor.transform.DOScale(1f, zoomInTime);
            }
            yield return new WaitForSeconds(zoomInTime + 0.5f);
            if ((bool)objTutorialGuideCursor)
            {
                objTutorialGuideCursor.transform.DOMove(GetSlotPosition(endX, endY), moveTime);
            }
            yield return new WaitForSeconds(moveTime);
            if ((bool)objTutorialGuideCursor)
            {
                objTutorialGuideCursor.transform.DOScale(1.3f, zoomOutTime);
            }
            yield return new WaitForSeconds(zoomOutTime);
            if ((bool)objTutorialGuideCursor)
            {
                objTutorialGuideCursor.transform.DOMove(GetSlotPosition(startX, startY), moveReverseTime);
            }
            yield return new WaitForSeconds(moveReverseTime);
        }
    }

    private void SetRibbonBlocks()
    {
    }

    private void SetYarnBlocks()
    {
        for (int i = 0; i < boardData.width; i++)
        {
            for (int j = 0; j < boardData.height; j++)
            {
                if (!boardData.slots[i, j] || boardData.clothButton[i, j] <= 0 || !boardData.yarn[i, j])
                {
                    continue;
                }
                Side[] straightSides = Utils.straightSides;
                foreach (Side side in straightSides)
                {
                    int num = i + Utils.SideOffsetX(side);
                    int num2 = j + Utils.SideOffsetY(side);
                    if (IsInvalidCoordi(num, num2) && boardData.yarn[num, num2])
                    {
                        int num3 = FindYarnPath(side, num, num2);
                        if (num3 != -1)
                        {
                            CreateYarnAndClothButton(side, i, j, num3);
                        }
                    }
                }
            }
        }
        for (int l = 0; l < boardData.width; l++)
        {
            for (int m = 0; m < boardData.height; m++)
            {
                if (!boardData.slots[l, m] || boardData.clothButton[l, m] <= 0 || !GetBlock(l, m))
                {
                    continue;
                }
                Yarn component = GetBlock(l, m).gameObject.GetComponent<Yarn>();
                if (!component)
                {
                    continue;
                }
                Side[] straightSides2 = Utils.straightSides;
                foreach (Side side2 in straightSides2)
                {
                    int x = l + Utils.SideOffsetX(side2);
                    int y = m + Utils.SideOffsetY(side2);
                    if (!IsInvalidCoordi(x, y) || !GetBlock(x, y))
                    {
                        continue;
                    }
                    Yarn component2 = GetBlock(x, y).gameObject.GetComponent<Yarn>();
                    if ((bool)component2)
                    {
                        component.SetSideYarn(side2, component2);
                        if (component2.clothButtonLevel <= 0)
                        {
                            LinkYarns(component2, side2, x, y);
                        }
                    }
                }
            }
        }
    }

    private bool IsInvalidCoordi(int x, int y)
    {
        if (x < 0 || x >= boardData.width || y < 0 || y >= boardData.height)
        {
            return false;
        }
        return true;
    }

    private int FindYarnPath(Side side, int x, int y, int distance = 1)
    {
        if (boardData.clothButton[x, y] > 0)
        {
            return distance;
        }
        int num = x + Utils.SideOffsetX(side);
        int num2 = y + Utils.SideOffsetY(side);
        if (IsInvalidCoordi(num, num2))
        {
            if (boardData.yarn[num, num2])
            {
                return FindYarnPath(side, num, num2, distance + 1);
            }
            return -1;
        }
        return -1;
    }

    private void LinkYarns(Yarn curYarn, Side side, int x, int y)
    {
        int x2 = x + Utils.SideOffsetX(side);
        int y2 = y + Utils.SideOffsetY(side);
        if (!IsInvalidCoordi(x2, y2) || !GetBlock(x2, y2))
        {
            return;
        }
        Yarn component = GetBlock(x2, y2).gameObject.GetComponent<Yarn>();
        if ((bool)component)
        {
            curYarn.SetSideYarn(side, component);
            if (component.clothButtonLevel <= 0)
            {
                LinkYarns(component, side, x2, y2);
            }
        }
    }

    private void CreateYarnAndClothButton(Side side, int x, int y, int distance)
    {
        int num = x;
        int num2 = y;
        for (int i = 0; i < distance; i++)
        {
            Yarn component;
            if (!GetBlock(x, y))
            {
                Slot slot = GetSlot(x, y);
                GameObject item = ContentAssistant.main.GetItem("Yarn");
                item.transform.position = slot.transform.position;

                item.name = "Yarn_" + x + "x" + y;
                item.transform.parent = slot.transform;
                component = item.GetComponent<Yarn>();
                slot.SetBlock(component);
                component.slot = slot;
                component.Initialize();
                if (boardData.clothButton[x, y] > 0)
                {
                    component.CreateClothButton(boardData.clothButton[x, y]);
                }
            }
            component = GetBlock(x, y).gameObject.GetComponent<Yarn>();
            component.CreateYarn(side);
            x += Utils.SideOffsetX(side);
            y += Utils.SideOffsetY(side);
        }
    }

    private void SetRailSlot()
    {
        listRailStartSlot.Clear();
        List<Slot> list = new List<Slot>();
        BoardPosition key = default(BoardPosition);
        int num = UnityEngine.Random.Range(0, 6);
        for (int i = 0; i < boardData.width; i++)
        {
            for (int j = 0; j < boardData.height; j++)
            {
                if (!boardData.slots[i, j] || !boardData.rail[i, j])
                {
                    continue;
                }
                Slot slot = GetSlot(i, j);
                if (!slot || list.Contains(slot))
                {
                    continue;
                }
                Slot slot2 = slot;
                listRailStartSlot.Add(slot2);
                do
                {
                    key.x = slot.x;
                    key.y = slot.y;
                    if (!boardData.dicRailNextPosition.ContainsKey(key))
                    {
                        list.Remove(slot2);
                        break;
                    }
                    list.Add(slot);
                    slot.isRailRoad = true;
                    BoardPosition boardPosition = default(BoardPosition);
                    BoardPosition boardPosition2 = boardData.dicRailNextPosition[key];
                    boardPosition.x = boardPosition2.x;
                    BoardPosition boardPosition3 = boardData.dicRailNextPosition[key];
                    boardPosition.y = boardPosition3.y;
                    slot.railNextSlot = GetSlot(boardPosition.x, boardPosition.y);
                    if (Mathf.Abs(key.x - boardPosition.x) > 1 || Mathf.Abs(key.y - boardPosition.y) > 1)
                    {
                        GameObject item = ContentAssistant.main.GetItem("EffPotal" + chipTypesSimple[num % 6], slot.transform.position);
                        GameObject item2 = ContentAssistant.main.GetItem("EffPotal" + chipTypesSimple[num % 6], slot.railNextSlot.transform.position);
                        num++;
                        item.transform.SetParent(slot.transform);
                        item2.transform.SetParent(slot.railNextSlot.transform);
                        switch (boardData.railImage[key.x, key.y])
                        {
                            case 4:
                            case 8:
                            case 11:
                            case 16:
                                item.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                                break;
                            case 2:
                            case 7:
                            case 12:
                            case 14:
                                item.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
                                break;
                            case 3:
                            case 6:
                            case 9:
                            case 15:
                                item.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
                                break;
                            case 1:
                            case 5:
                            case 10:
                            case 13:
                                item.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 270f));
                                break;
                        }
                        switch (boardData.railImage[boardPosition.x, boardPosition.y])
                        {
                            case 3:
                            case 7:
                            case 10:
                            case 15:
                                item2.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                                break;
                            case 1:
                            case 6:
                            case 11:
                            case 13:
                                item2.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
                                break;
                            case 4:
                            case 5:
                            case 12:
                            case 16:
                                item2.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
                                break;
                            case 2:
                            case 8:
                            case 9:
                            case 14:
                                item2.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 270f));
                                break;
                        }
                    }
                    slot = slot.railNextSlot;
                }
                while (slot != slot2);
            }
        }
    }

    public Side GetGateDirection(int gateX, int gateY)
    {
        if (gateY == 0)
        {
            return Side.Bottom;
        }
        if (gateY == boardData.height - 1)
        {
            return Side.Top;
        }
        if (gateX == 0)
        {
            return Side.Left;
        }
        return Side.Right;
    }

    public IEnumerator GoNextMapOrClear()
    {
        CurrentMapIndex++;
        if (CurrentMapIndex != MapData.main.SubMapCount)
        {
            yield return StartCoroutine(MoveNextMap());
        }
    }

    public IEnumerator ProcessDigMoveNextLine()
    {
        if (isProcessDigBoard)
        {
            yield break;
        }
        yield return null;
        DigAddUpLineDataIndex();
        if (!DigCheckLineDataIndexEnd())
        {
            //CPanelGameUI.Instance.EnableGameCameraMasking(CPanelGameUI.GameCameraMaskingType.Digging);
            yield return StartCoroutine(ProcessDigBoard());
            if (GameMain.main.CheckDiggingCondition())
            {
                yield return StartCoroutine(ProcessDigMoveNextLine());
            }
        }
    }

    private IEnumerator MoveNextMap()
    {
        yield return StartCoroutine(Utils.WaitFor(GameMain.main.CanIWait, 0.1f));
        GameMain.main.IsMovingNextMap = true;
        yield return new WaitForSeconds(0.75f);
        //CPanelGameUI.Instance.EnableGameCameraMasking(CPanelGameUI.GameCameraMaskingType.SweetRoad);
        if ((bool)slotGroup)
        {
            SlotGravity[] componentsInChildren = slotGroup.GetComponentsInChildren<SlotGravity>(includeInactive: true);
            foreach (SlotGravity obj in componentsInChildren)
            {
                UnityEngine.Object.Destroy(obj);
            }
        }
        oldSlotGroup = slotGroup;
        slotGroup = null;
        yield return null;
        GameMain.main.SetBestMovesNull();
        Vector3 nextMapVector = Vector3.zero;
        Side moveDirection = GetGateDirection(boardData.gateExitX, boardData.gateExitY);
        MapData.main.currentBoardDataIndex++;
        MapData.main.currentLineDataIndex = 0;
        GameObject objOldOutlineGroup = BoardOutlineGroup;
        BoardOutlineGroup = null;
        CreateBoard();
        switch (moveDirection)
        {
            case Side.Top:
                nextMapVector = Vector3.up;
                break;
            case Side.Bottom:
                nextMapVector = Vector3.down;
                break;
            case Side.Left:
                nextMapVector = Vector3.left;
                break;
            case Side.Right:
                nextMapVector = Vector3.right;
                break;
        }
        float nextMapPosition = slotoffset * (float)((!(nextMapVector == Vector3.up) && !(nextMapVector == Vector3.down)) ? MapData.MaxWidth : MapData.MaxHeight) + slotoffset;
        if ((bool)oldSlotGroup)
        {
            slotGroup.transform.position = oldSlotGroup.transform.position;
        }
        slotGroup.transform.Translate(nextMapVector * nextMapPosition);
        ShortcutExtensions.DOMove(endValue: Camera.main.transform.position + nextMapVector * nextMapPosition, target: Camera.main.transform, duration: 1f);
        yield return new WaitForSeconds(1f);
        if (objOldOutlineGroup != null)
        {
            UnityEngine.Object.Destroy(objOldOutlineGroup);
        }
        if ((bool)oldSlotGroup)
        {
            Transform[] componentsInChildren2 = oldSlotGroup.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (Transform instance in componentsInChildren2)
            {
                if (PoolManager.PoolGameBlocks.IsSpawned(instance))
                {
                    PoolManager.PoolGameBlocks.Despawn(instance);
                }
                else if (PoolManager.PoolGameEffect.IsSpawned(instance))
                {
                    PoolManager.PoolGameEffect.Despawn(instance);
                }
            }
            UnityEngine.Object.Destroy(oldSlotGroup);
            oldSlotGroup = null;
        }
        if ((bool)rescueGingerManGroup)
        {
            UnityEngine.Object.Destroy(rescueGingerManGroup);
            rescueGingerManGroup = null;
        }
        //CPanelGameUI.Instance.DisableGameCameraMasking();
        GameMain.main.IsMovingNextMap = false;
    }

    public void RemoveBoard()
    {
        RemoveTutorialOutlineEffect();
        listRailStartSlot.Clear();
        if ((bool)slotGroup)
        {
            PoolManager.PoolGameEffect.DespawnAll();
            PoolManager.PoolGameBlocks.DespawnAll();
            UnityEngine.Object.Destroy(slotGroup);
        }
        if ((bool)rescueGingerManGroup)
        {
            UnityEngine.Object.Destroy(rescueGingerManGroup);
        }
    }

    public Vector3 GetSlotPosition(int x, int y)
    {
        retPosition.x = (0f - slotoffset) * (0.5f * (float)(boardData.width - 1) - (float)x);
        retPosition.y = (0f - slotoffset) * (0.5f * (float)(boardData.height - 1) - (float)y);
        retPosition.z = 0f;
        //retPosition *= xyz;
        return slotGroup.transform.position + retPosition;
    }
    IEnumerator time_1()
    {
        yield return new WaitForSeconds(1f);


        if (((float)Screen.width / (float)Screen.height) >= (1080f / 1920f))
        {
            
            slotGroup.transform.localScale = new Vector3(1, 1, 1);
            //Debug.Log("width" + Screen.width);
            //Debug.Log("height" + Screen.height);
        }
        else
        {
            slotGroup.transform.localScale = new Vector3(0.84f, 0.84f, 0.84f);
            //Debug.Log("width" + Screen.width);
            //Debug.Log("height" + Screen.height);
        }

        //Debug.Log("width" + Screen.width);
        //Debug.Log("height" + Screen.height);
    }
    private void GenerateSlots()
    {
        slotGroup = new GameObject();
        slotGroup.name = "Slots";
        slotGroup.transform.position = /*CPanelGameUI.Instance.GetBoardPosition()*/Vector2.zero;

        int num = 0;
        int num2 = 0;
        for (int i = 0; i < boardData.width; i++)
        {
            bool flag = true;
            for (int j = 0; j < boardData.height; j++)
            {
                if (boardData.slots[i, j])
                {
                    flag = false;
                    break;
                }
            }
            if (!flag)
            {
                break;
            }
            num++;
        }
        for (int num3 = boardData.width - 1; num3 >= 0; num3--)
        {
            bool flag2 = true;
            for (int k = 0; k < boardData.height; k++)
            {
                if (boardData.slots[num3, k])
                {
                    flag2 = false;
                    break;
                }
            }
            if (!flag2)
            {
                break;
            }
            num2++;
        }
        if (Mathf.Abs(num2 - num) > 0 && Mathf.Abs(num2 - num) % 2 == 1)
        {
            slotGroup.transform.SetPositionX(slotoffset / 2f);
        }

        Vector3 vector = default(Vector3);
        BoardPosition key = default(BoardPosition);
        dicGeneratorDrop = new Dictionary<BoardPosition, MapDataGeneratorDrop>(MapData.main.CurrentMapBoardData.dicGeneratorDropBlock);
        dicGeneratorSpecialDrop = new Dictionary<BoardPosition, MapDataGeneratorSpecialDrop>(MapData.main.CurrentMapBoardData.dicGeneratorSpecialDropBlock);
        bringDownGenerator = null;
        bringDownGenerator = new bool[boardData.width, boardData.height];
        slots = new Dictionary<int, Slot>();
        for (int l = 0; l < boardData.width; l++)
        {
            for (int m = 0; m < boardData.height; m++)
            {
                if (MapData.main.target == GoalTarget.Digging && !boardData.slots[l, m])
                {
                    boardData.slots[l, m] = true;
                    boardData.blocks[l, m] = IBlockType.DiggingBlank;
                }

                if (!boardData.slots[l, m])
                {
                    continue;
                }

                vector.x = (0f - slotoffset) * (0.5f * (float)(boardData.width - 1) - (float)l);
                vector.y = (0f - slotoffset) * (0.5f * (float)(boardData.height - 1) - (float)m);
                key.x = l;
                key.y = m;

                GameObject gameObject = (!boardData.rail[l, m]) ? ContentAssistant.main.GetItem("SlotEmpty", vector) : ContentAssistant.main.GetItem("SlotEmptyRail", vector);
                gameObject.name = "Slot_" + l + "x" + m;
                gameObject.transform.parent = slotGroup.transform;
                gameObject.transform.localPosition = vector;
                gameObject.transform.localScale = Vector3.one;

                Slot component = gameObject.GetComponent<Slot>();
                component.x = l;
                component.y = m;

                if (!boardData.rail[l, m])
                {
                    component.SetSlotBackground((l + m) % 2);
                    component.GenerateRockCandy(boardData.rockCandyTile[l, m]);
                }
                component.SetDropDirection(MapData.main.CurrentMapBoardData.dropDirection[l, m]);
                component.SetDropLock(MapData.main.CurrentMapBoardData.dropLock[l, m]);

                if (dicGeneratorDrop.ContainsKey(key) || dicGeneratorSpecialDrop.ContainsKey(key))
                {
                    component.gameObject.AddComponent<SlotGenerator>();
                }

                if (dicGeneratorSpecialDrop.ContainsKey(key))
                {
                    int num4 = 0;
                    for (int n = 0; n < main.dicGeneratorSpecialDrop[key].dropBlocks.Length; n++)
                    {
                        num4 += main.dicGeneratorSpecialDrop[key].dropBlocks[n].prob;
                    }
                    dicGeneratorSpecialDrop[key].totalProb = num4;
                }

                bringDownGenerator[l, m] = MapData.main.CurrentMapBoardData.bringDownGenerator[l, m];

                if (boardData.teleport[l, m] > 0)
                {
                    SlotTeleport slotTeleport = component.gameObject.AddComponent<SlotTeleport>();
                    slotTeleport.targetID = boardData.teleport[l, m];
                }

                // Spawn Bring Down End // khong nen quan tam
                if (MapData.main.target == GoalTarget.BringDown && boardData.bringDownGoal[l, m])
                {
                    component.bringDownEndSlot = true;
                    GameObject item = ContentAssistant.main.GetItem("BringDownEndInfo", vector);
                    item.name = "BringDownEndInfo";
                    item.transform.parent = gameObject.transform;
                    if (component.DropDir == DropDirection.Down)
                    {
                        item.transform.localPosition = new Vector3(0f, -48f, 0f);
                    }
                    else if (component.DropDir == DropDirection.Up)
                    {
                        item.transform.localPosition = new Vector3(0f, 48f, 0f);
                        item.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
                    }
                    else if (component.DropDir == DropDirection.Right)
                    {
                        item.transform.localPosition = new Vector3(48f, 0f, 0f);
                        item.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
                    }
                    else if (component.DropDir == DropDirection.Left)
                    {
                        item.transform.localPosition = new Vector3(-48f, 0f, 0f);
                        item.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
                    }
                }

                // Spawn Bring Down Start // khong nen quan tam
                if ((MapData.main.target == GoalTarget.BringDown || MapData.main.target == GoalTarget.CollectCracker) && bringDownGenerator[l, m])
                {
                    GameObject item = ContentAssistant.main.GetItem("BringDownStartInfo", vector);
                    item.name = "BringDownStartInfo";
                    item.transform.parent = gameObject.transform;
                    if (component.DropDir == DropDirection.Down)
                    {
                        item.transform.localPosition = new Vector3(0f, 48f, 0f);
                    }
                    else if (component.DropDir == DropDirection.Up)
                    {
                        item.transform.localPosition = new Vector3(0f, -48f, 0f);
                        item.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
                    }
                    else if (component.DropDir == DropDirection.Right)
                    {
                        item.transform.localPosition = new Vector3(-48f, 0f, 0f);
                        item.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
                    }
                    else if (component.DropDir == DropDirection.Left)
                    {
                        item.transform.localPosition = new Vector3(48f, 0f, 0f);
                        item.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
                    }
                }

                if (MapData.main.target == GoalTarget.CollectCracker && boardData.bringDownGoal[l, m])
                {
                    component.bringDownEndSlot = true;
                    GameMain.main.targetOreoMilkHeight = Mathf.Max(GameMain.main.targetOreoMilkHeight, m);
                    GameObject item2 = ContentAssistant.main.GetItem("CowBell", vector);
                    item2.name = "CowBell";
                    item2.transform.parent = gameObject.transform;
                    item2.transform.localPosition = new Vector3(0f, 23f, 0f);
                    listCowBell.Add(item2);
                }

                component.isSafeObs = boardData.safeObsSlot[l, m];
                slots.Add(l * MapData.MaxHeight + m, component);
            }
        }

        DrawBoardOutline();

        for (int num5 = 0; num5 < boardData.listMovingSlot.Count; num5++)
        {
            DrawMovingSlotOutlineEffect(boardData.listMovingSlot[num5]);
        }
        for (int num6 = 0; num6 < boardData.listRotationSlot.Count; num6++)
        {
            DrawRotationSlotOutlineEffect(boardData.listRotationSlot[num6]);
        }

        Slot[] componentsInChildren = slotGroup.GetComponentsInChildren<Slot>();
        foreach (Slot slot in componentsInChildren)
        {
            slot.Initialize();
        }

        StartCoroutine(time_1());
    }

    public void DrawMovingSlotOutlineEffect(MapDataMovingSlot movingSlot)
    {
        movingSlot.ClearEffects();
        bool[,] array = new bool[width, height];
        int num;
        int num2;
        if (movingSlot.isMoveReverse)
        {
            num = movingSlot.targetX;
            num2 = movingSlot.targetY;
        }
        else
        {
            num = movingSlot.startX;
            num2 = movingSlot.startY;
        }
        for (int i = 0; i < movingSlot.width; i++)
        {
            for (int j = 0; j < movingSlot.height; j++)
            {
                int num3 = num + i;
                int num4 = num2 - j;
                array[num3, num4] = boardData.slots[num3, num4];
            }
        }
        for (int k = 0; k < movingSlot.width; k++)
        {
            for (int l = 0; l < movingSlot.height; l++)
            {
                int num3 = num + k;
                int num4 = num2 - l;
                Slot slot = GetSlot(num3, num4);
                if ((bool)slot)
                {
                    DrawOutlineEffect(movingSlot.listEffects, slot.transform, GetOutlineSideType(array, num3, num4));
                }
            }
        }
    }

    public void DrawRotationSlotOutlineEffect(MapDataRotationSlot rotationSlot)
    {
        rotationSlot.ClearEffects();
        bool[,] array = new bool[width, height];
        int num = rotationSlot.centerX - rotationSlot.size / 2;
        int num2 = rotationSlot.centerY + rotationSlot.size / 2 - (rotationSlot.isGrid ? 1 : 0);
        for (int i = 0; i < rotationSlot.size; i++)
        {
            for (int j = 0; j < rotationSlot.size; j++)
            {
                int num3 = num + j;
                int num4 = num2 - i;
                array[num3, num4] = boardData.slots[num3, num4];
            }
        }
        for (int k = 0; k < rotationSlot.size; k++)
        {
            for (int l = 0; l < rotationSlot.size; l++)
            {
                int num3 = num + l;
                int num4 = num2 - k;
                Slot slot = GetSlot(num3, num4);
                if ((bool)slot)
                {
                    DrawOutlineEffect(rotationSlot.listEffects, slot.transform, GetOutlineSideType(array, num3, num4));
                }
            }
        }
    }

    public void DrawTutorialOutlineEffect()
    {
        objTutorialEffects.Clear();
        enabledTutorialOutlineEffect = false;
        if (boardData.tutorial1X != -1 && boardData.tutorial1Y != -1 && boardData.tutorial2X != -1 && boardData.tutorial2Y != -1 && ((Mathf.Abs(boardData.tutorial1X - boardData.tutorial2X) == 1 && boardData.tutorial1Y == boardData.tutorial2Y) || (Mathf.Abs(boardData.tutorial1Y - boardData.tutorial2Y) == 1 && boardData.tutorial1X == boardData.tutorial2X)))
        {
            enabledTutorialOutlineEffect = true;
            BoardPosition boardPosition = default(BoardPosition);
            BoardPosition boardPosition2 = default(BoardPosition);
            int num = 1;
            if (boardData.tutorial1X < boardData.tutorial2X)
            {
                num = 1;
            }
            else if (boardData.tutorial2X < boardData.tutorial1X)
            {
                num = 2;
            }
            else if (boardData.tutorial2Y < boardData.tutorial1Y)
            {
                num = 2;
            }
            if (num == 1)
            {
                boardPosition.x = boardData.tutorial1X;
                boardPosition.y = boardData.tutorial1Y;
                boardPosition2.x = boardData.tutorial2X;
                boardPosition2.y = boardData.tutorial2Y;
            }
            else
            {
                boardPosition.x = boardData.tutorial2X;
                boardPosition.y = boardData.tutorial2Y;
                boardPosition2.x = boardData.tutorial1X;
                boardPosition2.y = boardData.tutorial1Y;
            }
            objTutorialGuideCursor = UnityEngine.Object.Instantiate(GameMain.main.PrefabTutorialGuideCursor);
            objTutorialGuideCursor.transform.position = GetSlotPosition(boardPosition.x, boardPosition.y);
            objTutorialGuideCursor.transform.localPosition += new Vector3(10f, 10f, 0f);
            if (boardPosition.x == boardPosition2.x)
            {
                createTutorialOutlineEffect(boardPosition.x, boardPosition.y, Side.Bottom);
                createTutorialOutlineEffect(boardPosition.x, boardPosition.y, Side.Left);
                createTutorialOutlineEffect(boardPosition.x, boardPosition.y, Side.Right);
                createTutorialOutlineEffect(boardPosition2.x, boardPosition2.y, Side.Left);
                createTutorialOutlineEffect(boardPosition2.x, boardPosition2.y, Side.Right);
                createTutorialOutlineEffect(boardPosition2.x, boardPosition2.y, Side.Top);
                StartCoroutine(doTutorialCursorAnimation(boardPosition2.x, boardPosition2.y, boardPosition.x, boardPosition.y));
            }
            else
            {
                createTutorialOutlineEffect(boardPosition.x, boardPosition.y, Side.Bottom);
                createTutorialOutlineEffect(boardPosition.x, boardPosition.y, Side.Left);
                createTutorialOutlineEffect(boardPosition.x, boardPosition.y, Side.Top);
                createTutorialOutlineEffect(boardPosition2.x, boardPosition2.y, Side.Bottom);
                createTutorialOutlineEffect(boardPosition2.x, boardPosition2.y, Side.Right);
                createTutorialOutlineEffect(boardPosition2.x, boardPosition2.y, Side.Top);
                StartCoroutine(doTutorialCursorAnimation(boardPosition.x, boardPosition.y, boardPosition2.x, boardPosition2.y));
            }
        }
    }

    public void RemoveTutorialOutlineEffect()
    {
        enabledTutorialOutlineEffect = false;
        if (objTutorialEffects.Count > 0)
        {
            foreach (GameObject objTutorialEffect in objTutorialEffects)
            {
                if ((bool)objTutorialEffect)
                {
                    UnityEngine.Object.Destroy(objTutorialEffect);
                }
            }
            objTutorialEffects.Clear();
        }
        if ((bool)objTutorialGuideCursor)
        {
            UnityEngine.Object.Destroy(objTutorialGuideCursor);
        }
    }

    private void createTutorialOutlineEffect(int x, int y, Side side)
    {
        Vector3 zero = Vector3.zero;
        if (side != 0)
        {
            Slot slot = GetSlot(x, y);
            switch (side)
            {
                case Side.Left:
                    {
                        GameObject gameObject = UnityEngine.Object.Instantiate(GameMain.main.PrefabTutorialOutlineEffect);
                        gameObject.transform.parent = slot.transform;
                        gameObject.transform.localRotation = Quaternion.identity;
                        zero.x = (0f - slotoffset) / 2f;
                        gameObject.transform.localPosition = zero;
                        objTutorialEffects.Add(gameObject);
                        break;
                    }
                case Side.Right:
                    {
                        GameObject gameObject = UnityEngine.Object.Instantiate(GameMain.main.PrefabTutorialOutlineEffect);
                        gameObject.transform.parent = slot.transform;
                        gameObject.transform.localRotation = Quaternion.identity;
                        zero.x = slotoffset / 2f;
                        gameObject.transform.localPosition = zero;
                        objTutorialEffects.Add(gameObject);
                        break;
                    }
                case Side.Top:
                    {
                        GameObject gameObject = UnityEngine.Object.Instantiate(GameMain.main.PrefabTutorialOutlineEffect);
                        gameObject.transform.parent = slot.transform;
                        gameObject.transform.Rotate(0f, 0f, 90f);
                        zero.y = slotoffset / 2f;
                        gameObject.transform.localPosition = zero;
                        objTutorialEffects.Add(gameObject);
                        break;
                    }
                case Side.Bottom:
                    {
                        GameObject gameObject = UnityEngine.Object.Instantiate(GameMain.main.PrefabTutorialOutlineEffect);
                        gameObject.transform.parent = slot.transform;
                        gameObject.transform.Rotate(0f, 0f, 90f);
                        zero.y = (0f - slotoffset) / 2f;
                        gameObject.transform.localPosition = zero;
                        objTutorialEffects.Add(gameObject);
                        break;
                    }
            }
        }
    }

    private void DrawOutlineEffect(List<GameObject> addLists, Transform parentSlot, SideCheckFill sideData)
    {
        Vector3 zero = Vector3.zero;
        if (sideData != 0)
        {
            if ((sideData & SideCheckFill.Left) > SideCheckFill.None)
            {
                GameObject gameObject = CreateMovableBoardEffect(parentSlot);
                zero = Vector3.zero;
                zero.x = (0f - slotoffset) / 2f;
                gameObject.transform.localPosition = zero;
                addLists?.Add(gameObject);
            }
            if ((sideData & SideCheckFill.Right) > SideCheckFill.None)
            {
                GameObject gameObject = CreateMovableBoardEffect(parentSlot);
                gameObject.transform.Rotate(0f, 0f, 180f);
                zero = Vector3.zero;
                zero.x = slotoffset / 2f;
                gameObject.transform.localPosition = zero;
                addLists?.Add(gameObject);
            }
            if ((sideData & SideCheckFill.Top) > SideCheckFill.None)
            {
                GameObject gameObject = CreateMovableBoardEffect(parentSlot);
                gameObject.transform.Rotate(0f, 0f, 270f);
                zero = Vector3.zero;
                zero.y = slotoffset / 2f;
                gameObject.transform.localPosition = zero;
                addLists?.Add(gameObject);
            }
            if ((sideData & SideCheckFill.Bottom) > SideCheckFill.None)
            {
                GameObject gameObject = CreateMovableBoardEffect(parentSlot);
                gameObject.transform.Rotate(0f, 0f, 90f);
                zero = Vector3.zero;
                zero.y = (0f - slotoffset) / 2f;
                gameObject.transform.localPosition = zero;
                addLists?.Add(gameObject);
            }
        }
    }

    private GameObject CreateMovableBoardEffect(Transform parent)
    {
        GameObject gameObject = null;
        gameObject = SpawnStringGamePlaying.GetSpawnObject(SpawnStringGamePlayingType.MovableBoardEffect);
        gameObject.transform.parent = parent;
        gameObject.transform.localRotation = Quaternion.identity;
        return gameObject;
    }

    private SideCheckFill GetOutlineSideType(bool[,] baseSlots, int x, int y)
    {
        SideCheckFill sideCheckFill = SideCheckFill.None;
        if (x == 0 || (x > 0 && !baseSlots[x - 1, y]))
        {
            sideCheckFill |= SideCheckFill.Left;
        }
        if (x + 1 == width || (x + 1 < width && !baseSlots[x + 1, y]))
        {
            sideCheckFill |= SideCheckFill.Right;
        }
        if (y == 0 || (y > 0 && !baseSlots[x, y - 1]))
        {
            sideCheckFill |= SideCheckFill.Bottom;
        }
        if (y + 1 == height || (y + 1 < height && !baseSlots[x, y + 1]))
        {
            sideCheckFill |= SideCheckFill.Top;
        }
        return sideCheckFill;
    }

    private BoardOutlineType GetOutlineType(int x, int y)
    {
        int num = 0;
        int num2 = 0;
        BoardOutlineType result = BoardOutlineType.Null;
        if (x > 0 && y < height && boardData.slots[x - 1, y])
        {
            num |= 1;
        }
        if (x < width && y < height && boardData.slots[x, y])
        {
            num |= 2;
        }
        if (x > 0 && y > 0 && boardData.slots[x - 1, y - 1])
        {
            num |= 4;
        }
        if (x < width && y > 0 && boardData.slots[x, y - 1])
        {
            num |= 8;
        }
        num2 = (num ^ 0xF);
        switch (num)
        {
            case 1:
                result = BoardOutlineType.RoundRB;
                break;
            case 2:
                result = BoardOutlineType.RoundLB;
                break;
            case 4:
                result = BoardOutlineType.RoundRT;
                break;
            case 8:
                result = BoardOutlineType.RoundLT;
                break;
            default:
                switch (num2)
                {
                    case 1:
                        result = BoardOutlineType.RB;
                        break;
                    case 2:
                        result = BoardOutlineType.LB;
                        break;
                    case 4:
                        result = BoardOutlineType.RT;
                        break;
                    case 8:
                        result = BoardOutlineType.LT;
                        break;
                    default:
                        switch (num)
                        {
                            case 5:
                                result = BoardOutlineType.VR;
                                break;
                            case 10:
                                result = BoardOutlineType.VL;
                                break;
                            case 3:
                                result = BoardOutlineType.HB;
                                break;
                            case 12:
                                result = BoardOutlineType.HT;
                                break;
                            case 6:
                                result = BoardOutlineType.LT_RB;
                                break;
                            case 9:
                                result = BoardOutlineType.RT_LB;
                                break;
                        }
                        break;
                }
                break;
        }
        return result;
    }
    IEnumerator time_scalse()
    {
        yield return new WaitForSeconds(0.1f);
        BoardOutlineGroup.transform.localScale = Vector3.one;
    }
    public void DrawBoardOutline()
    {
        if ((bool)BoardOutlineGroup)
        {
            UnityEngine.Object.Destroy(BoardOutlineGroup);
        }
        BoardOutlineGroup = new GameObject("outline");
        BoardOutlineGroup.transform.parent = slotGroup.transform;
        BoardOutlineGroup.transform.localPosition = Vector3.zero;

        BoardOutlineType[,] array = new BoardOutlineType[boardData.width + 1, boardData.height + 1];

        for (int i = 0; i < boardData.width + 1; i++)
        {
            for (int j = 0; j < boardData.height + 1; j++)
            {
                array[i, j] = GetOutlineType(i, j);
            }
        }
        Vector3 position = Vector3.zero;
        for (int k = 0; k < boardData.width + 1; k++)
        {
            for (int l = 0; l < boardData.height + 1; l++)
            {
                if (array[k, l] == BoardOutlineType.Null)
                {
                    continue;
                }
                if (MapData.main.target == GoalTarget.SweetRoad)
                {
                    int num = -1;
                    int num2 = -1;
                    for (int m = 0; m < 2; m++)
                    {
                        if (m == 0)
                        {
                            num = boardData.gateEnterX;
                            num2 = boardData.gateEnterY;
                        }
                        else
                        {
                            if (CurrentMapIndex + 1 == MapData.main.SubMapCount)
                            {
                                continue;
                            }
                            if (m == 1)
                            {
                                num = boardData.gateExitX;
                                num2 = boardData.gateExitY;
                            }
                        }
                        if (num2 == boardData.height - 1)
                        {
                            if (k - 1 == num && l - 1 == num2 && array[k, l] == BoardOutlineType.RoundRT)
                            {
                                array[k, l] = BoardOutlineType.VR;
                            }
                            else if (k == num && l - 1 == num2 && array[k, l] == BoardOutlineType.RoundLT)
                            {
                                array[k, l] = BoardOutlineType.VL;
                            }
                        }
                        else if (num2 == 0)
                        {
                            if (k - 1 == num && l == num2 && array[k, l] == BoardOutlineType.RoundRB)
                            {
                                array[k, l] = BoardOutlineType.VR;
                            }
                            else if (k == num && l == num2 && array[k, l] == BoardOutlineType.RoundLB)
                            {
                                array[k, l] = BoardOutlineType.VL;
                            }
                        }
                        else if (num == 0)
                        {
                            if (k == num && l - 1 == num2 && array[k, l] == BoardOutlineType.RoundLT)
                            {
                                array[k, l] = BoardOutlineType.HT;
                            }
                            else if (k == num && l == num2 && array[k, l] == BoardOutlineType.RoundLB)
                            {
                                array[k, l] = BoardOutlineType.HB;
                            }
                        }
                        else if (num == boardData.width - 1)
                        {
                            if (k - 1 == num && l - 1 == num2 && array[k, l] == BoardOutlineType.RoundRT)
                            {
                                array[k, l] = BoardOutlineType.HT;
                            }
                            else if (k - 1 == num && l == num2 && array[k, l] == BoardOutlineType.RoundRB)
                            {
                                array[k, l] = BoardOutlineType.HB;
                            }
                        }
                    }
                }
                if (array[k, l] == BoardOutlineType.LT)
                {
                    position = GetSlotPosition(k, l - 1);
                    position.x -= outlineRectObjectOffset;
                    position.y += outlineRectObjectOffset;
                }
                else if (array[k, l] == BoardOutlineType.RT)
                {
                    position = GetSlotPosition(k - 1, l - 1);
                    position.x += outlineRectObjectOffset;
                    position.y += outlineRectObjectOffset;
                }
                else if (array[k, l] == BoardOutlineType.RB || array[k, l] == BoardOutlineType.LT_RB)
                {
                    position = GetSlotPosition(k - 1, l);
                    position.x += outlineRectObjectOffset;
                    position.y -= outlineRectObjectOffset;

                }
                else if (array[k, l] == BoardOutlineType.LB || array[k, l] == BoardOutlineType.RT_LB)
                {
                    position = GetSlotPosition(k, l);
                    position.x -= outlineRectObjectOffset;
                    position.y -= outlineRectObjectOffset;
                }
                else if (array[k, l] == BoardOutlineType.VL)
                {
                    position = GetSlotPosition(k, l);
                    position.x -= outlineStraightVOffsetX;
                    position.y -= outlineStraightVOffsetY;
                }
                else if (array[k, l] == BoardOutlineType.VR)
                {
                    position = GetSlotPosition(k - 1, l);
                    position.x += outlineStraightVOffsetX;
                    position.y -= outlineStraightVOffsetY;
                }
                else if (array[k, l] == BoardOutlineType.HT)
                {
                    position = GetSlotPosition(k, l - 1);
                    position.x -= outlineStraightHOffsetX;
                    position.y += outlineStraightHOffsetY;
                }
                else if (array[k, l] == BoardOutlineType.HB)
                {
                    position = GetSlotPosition(k, l);
                    position.x -= outlineStraightHOffsetX;
                    position.y -= outlineStraightHOffsetY;
                }
                else if (array[k, l] == BoardOutlineType.RoundLT)
                {
                    position = GetSlotPosition(k, l - 1);
                    position.x -= outlineRoundObjectOffset;
                    position.y += outlineRoundObjectOffset;
                }
                else if (array[k, l] == BoardOutlineType.RoundRT)
                {
                    position = GetSlotPosition(k - 1, l - 1);
                    position.x += outlineRoundObjectOffset;
                    position.y += outlineRoundObjectOffset;
                }
                else if (array[k, l] == BoardOutlineType.RoundLB)
                {
                    position = GetSlotPosition(k, l);
                    position.x -= outlineRoundObjectOffset;
                    position.y -= outlineRoundObjectOffset;
                }
                else
                {
                    if (array[k, l] != BoardOutlineType.RoundRB)
                    {
                        continue;
                    }
                    position = GetSlotPosition(k - 1, l);
                    position.x += outlineRoundObjectOffset;
                    position.y -= outlineRoundObjectOffset;
                }
                if (MapData.main.target == GoalTarget.Digging)
                {
                    if (array[k, l] == BoardOutlineType.RoundLT)
                    {
                        array[k, l] = BoardOutlineType.LT_Edge;
                    }
                    else if (array[k, l] == BoardOutlineType.RoundLB)
                    {
                        array[k, l] = BoardOutlineType.LB_Edge;
                    }
                    else if (array[k, l] == BoardOutlineType.RoundRT)
                    {
                        array[k, l] = BoardOutlineType.RT_Edge;
                    }
                    else if (array[k, l] == BoardOutlineType.RoundRB)
                    {
                        array[k, l] = BoardOutlineType.RB_Edge;
                    }
                }
                GameObject gameObject = UnityEngine.Object.Instantiate(PrefabBoardOutlines[(int)array[k, l]]);
                gameObject.transform.parent = BoardOutlineGroup.transform;
                gameObject.transform.position = position;
                if (MapData.main.target == GoalTarget.Digging)
                {
                    SpriteRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SpriteRenderer>();
                    foreach (SpriteRenderer spriteRenderer in componentsInChildren)
                    {
                        spriteRenderer.sortingOrder = 0;
                    }
                }
            }
        }
        StartCoroutine(time_scalse());
    }

    public void OffBoardOutline()
    {
        if (!BoardOutlineGroup)
        {
            return;
        }
        SpriteRenderer[] componentsInChildren = BoardOutlineGroup.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in componentsInChildren)
        {
            if ((bool)spriteRenderer)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            }
        }
    }

    public void FadeBoardOutline(bool isFadeIn)
    {
        StopCoroutine("ProcessFadeBoardOutline");
        StartCoroutine(ProcessFadeBoardOutline(isFadeIn));
    }

    private IEnumerator ProcessFadeBoardOutline(bool isFadeIn)
    {
        Color c = Color.white;
        float playingTime = 0.42f;
        float fadeTime = 0f;
        while (fadeTime < playingTime)
        {
            if (isFadeIn)
            {
                c.a = Mathf.InverseLerp(0f, playingTime, fadeTime);
            }
            else
            {
                c.a = Mathf.InverseLerp(0f, playingTime, playingTime - fadeTime);
            }
            if ((bool)BoardOutlineGroup)
            {
                SpriteRenderer[] componentsInChildren = BoardOutlineGroup.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer spriteRenderer in componentsInChildren)
                {
                    if ((bool)spriteRenderer)
                    {
                        spriteRenderer.color = c;
                    }
                }
            }
            fadeTime += Time.deltaTime;
            yield return null;
        }
    }

    private void GenerateBlocks()
    {
        listCrowBlock.Clear();
        listCandyFactory.Clear();
        listPastryBag.Clear();
        listSlime.Clear();
        remainRescueFriendInBoard = 0;
        for (int i = 0; i < boardData.width; i++)
        {
            for (int j = 0; j < boardData.height; j++)
            {
                if (boardData.slots[i, j])
                {
                    CompatibleCreateBlock(i, j, boardData.blocks[i, j], boardData.param1[i, j], boardData.param2[i, j]);
                }
            }
        }
        SlotGravity.Reshading();
    }

    private void GenerateWalls()
    {
        listWall.Clear();
        for (int i = 0; i < boardData.width - 1; i++)
        {
            for (int j = 0; j < boardData.height; j++)
            {
                if (boardData.wallsV[i, j])
                {
                    GameObject item = ContentAssistant.main.GetItem("Wall");
                    item.name = "WallV_" + i + "x" + j;
                    item.transform.parent = GetSlot(i, j).transform;
                    item.transform.localPosition = new Vector3(slotoffset / 2f, 0f, 0f);
                    Wall component = item.GetComponent<Wall>();
                    component.x = i;
                    component.y = j;
                    component.h = false;
                    listWall.Add(component);
                }
            }
        }
        for (int i = 0; i < boardData.width; i++)
        {
            for (int j = 0; j < boardData.height - 1; j++)
            {
                if (boardData.wallsH[i, j])
                {
                    GameObject item = ContentAssistant.main.GetItem("Wall", Vector3.zero, Quaternion.Euler(0f, 0f, -90f));
                    item.name = "WallH_" + i + "x" + j;
                    item.transform.parent = GetSlot(i, j).transform;
                    item.transform.localPosition = new Vector3(0f, slotoffset / 2f, 0f);
                    Wall component = item.GetComponent<Wall>();
                    component.x = i;
                    component.y = j;
                    component.h = true;
                    listWall.Add(component);
                }
            }
        }
        Wall[] componentsInChildren = slotGroup.GetComponentsInChildren<Wall>();
        foreach (Wall wall in componentsInChildren)
        {
            wall.Initialize();
        }
    }

    private void GenerateDropWalls()
    {
        for (int i = 0; i < boardData.width; i++)
        {
            for (int j = 0; j < boardData.height; j++)
            {
                if (boardData.dropWallsH[i, j])
                {
                    if (boardData.slots[i, j] && boardData.slots[i, j + 1])
                    {
                        main.GetNearSlot(i, j, Side.Top).SetDropWall(Side.Bottom);
                        main.GetSlot(i, j).SetDropWall(Side.Top);
                    }
                    else
                    {
                        boardData.dropWallsH[i, j] = false;
                    }
                }
                if (boardData.dropWallsV[i, j])
                {
                    if (boardData.slots[i, j] && boardData.slots[i + 1, j])
                    {
                        main.GetNearSlot(i, j, Side.Right).SetDropWall(Side.Left);
                        main.GetSlot(i, j).SetDropWall(Side.Right);
                    }
                    else
                    {
                        boardData.dropWallsV[i, j] = false;
                    }
                }
            }
        }
    }

    private void GeneratePowerups()
    {
        for (int i = 0; i < boardData.width; i++)
        {
            for (int j = 0; j < boardData.height; j++)
            {
                if (MapData.main.CurrentMapBoardData.powerUps[i, j] > 0 && boardData.slots[i, j])
                {
                    Powerup p = (Powerup)MapData.main.CurrentMapBoardData.powerUps[i, j];
                    AddPowerup(i, j, p);
                }
            }
        }
    }

    private void GenerateChips()
    {
        boardData.FirstChipGeneration();
        for (int i = 0; i < boardData.width; i++)
        {
            for (int j = 0; j < boardData.height; j++)
            {
                int chip = boardData.GetChip(i, j);
                if (chip >= 0 && chip != 9 && chip != 10 && (boardData.blocks[i, j] == IBlockType.None || MapData.IsBlockTypeIncludingChip(boardData.blocks[i, j])))
                {
                    Chip newSimpleChip = GetNewSimpleChip(i, j, new Vector3(0f, 0f, 0f), chip);
                    newSimpleChip.transform.localPosition = Vector3.zero;
                }
                if (chip == 9 && boardData.blocks[i, j] == IBlockType.None)
                {
                    GetNewStone(i, j, new Vector3(0f, 2f, 0f));
                }
            }
        }
    }

    private void GenerateJelly()
    {
        Slot.isFirstJellyComplete = false;
        for (int i = 0; i < boardData.width; i++)
        {
            for (int j = 0; j < boardData.height; j++)
            {
                if (boardData.slots[i, j])
                {
                    Slot slot = GetSlot(i, j);
                    if ((bool)slot)
                    {
                        slot.GenerateJellyLayer();
                    }
                }
            }
        }
    }

    public void GenerateJellyFirst()
    {
        //Camera.main.depth = 1f;
        for (int i = 0; i < boardData.width; i++)
        {
            for (int j = 0; j < boardData.height; j++)
            {
                if (boardData.slots[i, j])
                {
                    Slot slot = GetSlot(i, j);
                    if ((bool)slot && boardData.jellyTile[i, j])
                    {
                        slot.PaintJellyFirst();
                    }
                }
            }
        }
    }

    public void GenerateEmptyTile()
    {
        if (Slot.jellyModeTileTestLogicIndex == 0)
        {
            return;
        }
        for (int i = 0; i < boardData.width; i++)
        {
            for (int j = 0; j < boardData.height; j++)
            {
                if (!boardData.slots[i, j])
                {
                    continue;
                }
                Slot slot = GetSlot(i, j);
                if ((bool)slot)
                {
                    slot.GenerateEmptyLayer();
                    switch (boardData.blocks[i, j])
                    {
                        case IBlockType.CandyFactory_1:
                        case IBlockType.CandyFactory_2:
                        case IBlockType.CandyFactory_3:
                        case IBlockType.CandyFactory_4:
                        case IBlockType.CandyFactory_5:
                        case IBlockType.CandyFactory_6:
                        case IBlockType.PastryBag:
                            slot.EnableEmptyLayer();
                            break;
                    }
                }
            }
        }
    }

    private void GenerateMilkTileFirst()
    {
        int num = 0;
        for (int i = 0; i < boardData.width; i++)
        {
            for (int j = 0; j < boardData.height; j++)
            {
                if (boardData.slots[i, j])
                {
                    num = Mathf.Min(num, j);
                }
            }
        }
        GameMain.main.curOreoMilkHeight = num;
        for (int i = 0; i < boardData.width; i++)
        {
            for (int j = 0; j < boardData.height; j++)
            {
                if (boardData.slots[i, j])
                {
                    Slot slot = GetSlot(i, j);
                    if ((bool)slot && boardData.milkTile[i, j])
                    {
                        GameMain.main.curOreoMilkHeight = Mathf.Max(GameMain.main.curOreoMilkHeight, j);
                        slot.FillMilkTile();
                    }
                }
            }
        }
    }

    public void GenerateMilkTileRow(int height)
    {
        if (height < 0 || height >= boardData.width)
        {
            return;
        }
        for (int i = 0; i < boardData.width; i++)
        {
            if (boardData.slots[i, height])
            {
                Slot slot = GetSlot(i, height);
                if ((bool)slot)
                {
                    slot.FillMilkTile();
                }
            }
        }
    }

    private void GenerateSweetRoadGate()
    {
        if (CurrentMapIndex + 1 == MapData.main.SubMapCount)
        {
            boardData.gateExitX = (boardData.gateExitY = -1);
        }
        if (boardData.gateEnterX == boardData.gateExitX && boardData.gateEnterY == boardData.gateExitY)
        {
            return;
        }
        for (int i = 0; i < 2; i++)
        {
            GameObject gameObject;
            int num;
            int num2;
            if (i == 0)
            {
                if ((bool)objSweetRoadGateEnter)
                {
                    UnityEngine.Object.Destroy(objSweetRoadGateEnter);
                }
                objSweetRoadGateEnter = ContentAssistant.main.GetItem("SweetRoadGate");
                gameObject = objSweetRoadGateEnter;
                num = boardData.gateEnterX;
                num2 = boardData.gateEnterY;
            }
            else
            {
                if (i != 1 || boardData.gateExitX == -1 || boardData.gateExitY == -1)
                {
                    continue;
                }
                if ((bool)objSweetRoadGateExit)
                {
                    UnityEngine.Object.Destroy(objSweetRoadGateExit);
                }
                objSweetRoadGateExit = ContentAssistant.main.GetItem("SweetRoadGate");
                gameObject = objSweetRoadGateExit;
                num = boardData.gateExitX;
                num2 = boardData.gateExitY;
                if ((bool)objSweetRoadGateExitArrow)
                {
                    UnityEngine.Object.Destroy(objSweetRoadGateExitArrow);
                }
                objSweetRoadGateExitArrow = ContentAssistant.main.GetItem("Eff_straw_arrow2");
                objSweetRoadGateExitArrow.transform.Rotate(0f, 0f, 180f);
                objSweetRoadGateExitArrow.transform.parent = objSweetRoadGateExit.transform;
                objSweetRoadGateExitArrow.transform.localPosition = Vector3.zero;
            }
            if (!(GetSlot(num, num2) == null))
            {
                gameObject.transform.parent = slotGroup.transform;
                Vector3 vector = GetSlot(num, num2).transform.position;
                if (num2 == 0)
                {
                    vector += new Vector3(0f, 0f - arrowOffset, 0f);
                    gameObject.transform.Rotate(0f, 0f, 180f);
                }
                else if (num2 == boardData.height - 1)
                {
                    vector += new Vector3(0f, arrowOffset, 0f);
                }
                else if (num == 0)
                {
                    vector += new Vector3(0f - arrowOffset, 0f, 0f);
                    gameObject.transform.Rotate(0f, 0f, 90f);
                }
                else if (num == boardData.width - 1)
                {
                    vector += new Vector3(arrowOffset, 0f, 0f);
                    gameObject.transform.Rotate(0f, 0f, -90f);
                }
                gameObject.transform.position = vector;
                StartEffectSweetRoadGateFillMilk(isEnterGate: true);
            }
        }
    }

    public void StartEffectSweetRoadGateFillMilk(bool isEnterGate)
    {
        GameObject gameObject = (!isEnterGate) ? objSweetRoadGateExit : objSweetRoadGateEnter;
        if (!isEnterGate && (bool)objSweetRoadGateExitArrow)
        {
            UnityEngine.Object.Destroy(objSweetRoadGateExitArrow);
        }
        if ((bool)gameObject)
        {
            GameObject item = ContentAssistant.main.GetItem("Eff_SRmode_fillmilk");
            item.transform.parent = gameObject.transform;
            item.transform.localPosition = Vector3.zero;

            item.transform.localRotation = Quaternion.identity;
            if (!isEnterGate)
            {
                item.transform.Rotate(0f, 0f, 180f);
            }
        }
    }

    private void GenerateRescueGingerMan()
    {
        if (ObjRescueGingerMan != null)
        {
            foreach (string key2 in ObjRescueGingerMan.Keys)
            {
                if ((bool)ObjRescueGingerMan[key2])
                {
                    UnityEngine.Object.Destroy(ObjRescueGingerMan[key2]);
                }
            }
            ObjRescueGingerMan.Clear();
        }
        else
        {
            ObjRescueGingerMan = new Dictionary<string, GameObject>();
        }
        List<Vector2> list = new List<Vector2>();
        int num = 0;
        for (int i = 0; i < boardData.height; i++)
        {
            for (int j = 0; j < boardData.width; j++)
            {
                if (MapData.main.CurrentMapBoardData.rescueGinerManSize[j, i] != 0)
                {
                    num++;
                    list.Add(new Vector2(j, i));
                }
            }
        }
        for (int k = 0; k < list.Count; k++)
        {
            Vector2 vector = list[k];
            int num2 = (int)vector.x;
            Vector2 vector2 = list[k];
            int num3 = (int)vector2.y;
            RescueGingerManSize rescueGingerManSize = MapData.main.CurrentMapBoardData.rescueGinerManSize[num2, num3];
            for (int l = 0; l < Utils.GetRescueGingerManSizeWidth(rescueGingerManSize); l++)
            {
                for (int m = 0; m < Utils.GetRescueGingerManSizeHeight(rescueGingerManSize); m++)
                {
                    boardData.rescueGinerManSize[num2 + l, num3 - m] = rescueGingerManSize;
                    boardData.rescueGingerManSubPosX[num2 + l, num3 - m] = l;
                    boardData.rescueGingerManSubPosY[num2 + l, num3 - m] = m;
                }
            }
            string key = string.Empty;
            switch (rescueGingerManSize)
            {
                case RescueGingerManSize.Size1x2:
                case RescueGingerManSize.Size2x1:
                    key = "Eff_Saveginger_21";
                    break;
                case RescueGingerManSize.Size2x4:
                case RescueGingerManSize.Size4x2:
                    key = "Eff_Saveginger_42";
                    break;
                case RescueGingerManSize.Size3x6:
                case RescueGingerManSize.Size6x3:
                    key = "Eff_Saveginger_63";
                    break;
                case RescueGingerManSize.Size4x8:
                case RescueGingerManSize.Size8x4:
                    key = "Eff_Saveginger_84";
                    break;
            }
            GameObject item = ContentAssistant.main.GetItem(key);
            if (!(item == null))
            {
                item.name = num2 + "x" + num3;
                if (rescueGingerManGroup == null)
                {
                    rescueGingerManGroup = new GameObject();
                    rescueGingerManGroup.transform.parent = slotGroup.transform;
                    rescueGingerManGroup.transform.localPosition = Vector3.zero;
                }
                item.transform.parent = rescueGingerManGroup.transform;
                Vector3 zero = Vector3.zero;
                Slot slot = GetSlot(num2, num3);
                Vector3 position = slot.transform.position;
                zero.x = position.x + slotoffset / 2f * (float)(Utils.GetRescueGingerManSizeWidth(rescueGingerManSize) - 1);
                Vector3 position2 = slot.transform.position;
                zero.y = position2.y - slotoffset / 2f * (float)(Utils.GetRescueGingerManSizeHeight(rescueGingerManSize) - 1);
                item.transform.position = zero;
                if (Utils.GetRescueGingerManSizeWidth(rescueGingerManSize) > Utils.GetRescueGingerManSizeHeight(rescueGingerManSize))
                {
                    item.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                }
                ObjRescueGingerMan.Add($"{num2}x{num3}", item);
            }
        }
    }

    public GameObject GetNewFruitBoxDragLine(Vector3 from, Vector3 to)
    {
        Vector3 vector = to - from;
        vector.Normalize();
        float zAngle = 0f;
        if (vector == Vector3.left || vector == Vector3.right)
        {
            zAngle = 90f;
        }
        else if (Vector3.Distance(vector, dragLineRotationVector45_1) < 0.1f || Vector3.Distance(vector, dragLineRotationVector45_2) < 0.1f)
        {
            zAngle = -45f;
        }
        else if (Vector3.Distance(vector, dragLineRotationVectorM45_1) < 0.1f || Vector3.Distance(vector, dragLineRotationVectorM45_2) < 0.1f)
        {
            zAngle = 45f;
        }
        GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("DragLine"), from + slotoffset / 2f * vector, Quaternion.identity) as GameObject;
        gameObject.transform.Rotate(0f, 0f, zAngle);
        return gameObject;
    }

    public bool IsInvalidChipID(int chipID)
    {
        if (chipID >= 0 && chipID < 6)
        {
            return true;
        }
        return false;
    }

    public Chip GetNewSimpleChip(int x, int y, Vector3 position)
    {
        return GetNewSimpleChip(x, y, position, boardData.GetRandomChip());
    }

    public Chip GetBringDownChip(int x, int y, Vector3 position)
    {
        BringDownChip.BringDownSpriteType randomBringDown = (BringDownChip.BringDownSpriteType)boardData.GetRandomBringDown();
        GameObject spawnBlockObjectBringDown = SpawnStringBlock.GetSpawnBlockObjectBringDown((int)randomBringDown);
        spawnBlockObjectBringDown.transform.position = position;
        spawnBlockObjectBringDown.name = "BringDownObject";
        Chip component = spawnBlockObjectBringDown.GetComponent<Chip>();
        GetSlot(x, y).SetChip(component);
        BringDownChip bringDownChip = component as BringDownChip;
        bringDownChip.spriteType = randomBringDown;
        GameMain.main.createdBringDownCount++;
        return component;
    }

    public Chip GetOreoCrackerChip(int x, int y, Vector3 position)
    {
        GameObject spawnBlockObject = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.OreoCracker);
        spawnBlockObject.transform.position = position;
        spawnBlockObject.name = "Oreo";
        Chip component = spawnBlockObject.GetComponent<Chip>();
        GetSlot(x, y).SetChip(component);
        OreoCracker oreoCracker = component as OreoCracker;
        GameMain.main.createdBringDownCount++;
        return component;
    }

    public Chip GetNewStone(int x, int y, Vector3 position)
    {
        return null;
    }

    public Chip GetNewSimpleChip(int x, int y, Vector3 position, int id)
    {
        if (!IsInvalidChipID(id))
        {
            return null;
        }
        GameObject spawnBlockObjectSimpleChip = SpawnStringBlock.GetSpawnBlockObjectSimpleChip(id);
        if (spawnBlockObjectSimpleChip != null)
        {
            spawnBlockObjectSimpleChip.transform.position = position;
            spawnBlockObjectSimpleChip.name = "Chip_" + chipTypes[id];
            Chip component = spawnBlockObjectSimpleChip.GetComponent<Chip>();
            try
            {
                GetSlot(x, y).SetChip(component);
                return component;
            }
            catch (Exception)
            {
                return component;
            }
        }
        return null;
    }

    public Chip GetNewPlusMoveChip(int x, int y, Vector3 position, int id)
    {
        if (!IsInvalidChipID(id))
        {
            return null;
        }
        GameObject spawnBlockObjectPlusMoveChip = SpawnStringBlock.GetSpawnBlockObjectPlusMoveChip(id);
        spawnBlockObjectPlusMoveChip.transform.position = position;
        spawnBlockObjectPlusMoveChip.name = "PMChip_" + chipTypes[id];
        Chip component = spawnBlockObjectPlusMoveChip.GetComponent<Chip>();
        GetSlot(x, y).SetChip(component);
        component.ShowCreateEffect();
        return component;
    }

    public Chip GetNewBomb(int x, int y, Vector3 position, int id, bool isCombination = false)
    {
        if (!IsInvalidChipID(id))
        {
            return null;
        }
        GameObject spawnBlockObjectSimpleBomb = SpawnStringBlock.GetSpawnBlockObjectSimpleBomb(id);
        spawnBlockObjectSimpleBomb.transform.position = position;
        spawnBlockObjectSimpleBomb.name = "Bomb_" + chipTypes[id];
        Chip component = spawnBlockObjectSimpleBomb.GetComponent<Chip>();
        GetSlot(x, y).SetChip(component);
        component.ShowCreateEffect();
        if (isCombination)
        {
            component.SetMutekiTime(defaultMutekiTime);
        }
        return component;
    }

    public Chip GetNewRainbowBomb(int x, int y, Vector3 position, bool isCombination = false)
    {
        GameObject spawnBlockObject = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.RainbowBomb);
        spawnBlockObject.transform.position = position;
        spawnBlockObject.name = "RainbowBomb";
        Chip component = spawnBlockObject.GetComponent<Chip>();
        GetSlot(x, y).SetChip(component);
        component.ShowCreateEffect();
        if (isCombination)
        {
            component.SetMutekiTime(defaultMutekiTime);
        }
        return component;
    }

    public Chip GetNewColorHBomb(int x, int y, Vector3 position, int id, bool isCombination = false)
    {
        if (!IsInvalidChipID(id))
        {
            return null;
        }
        GameObject spawnBlockObjectHBomb = SpawnStringBlock.GetSpawnBlockObjectHBomb(id);
        spawnBlockObjectHBomb.transform.position = position;
        spawnBlockObjectHBomb.name = "ColorHBomb" + chipTypes[id];
        Chip component = spawnBlockObjectHBomb.GetComponent<Chip>();
        GetSlot(x, y).SetChip(component);
        component.ShowCreateEffect();
        if (isCombination)
        {
            component.SetMutekiTime(defaultMutekiTime);
        }
        return component;
    }

    public Chip GetNewColorVBomb(int x, int y, Vector3 position, int id, bool isCombination = false)
    {
        if (!IsInvalidChipID(id))
        {
            return null;
        }
        GameObject spawnBlockObjectVBomb = SpawnStringBlock.GetSpawnBlockObjectVBomb(id);
        spawnBlockObjectVBomb.transform.position = position;
        spawnBlockObjectVBomb.name = "ColorVBomb" + chipTypes[id];
        Chip component = spawnBlockObjectVBomb.GetComponent<Chip>();
        GetSlot(x, y).SetChip(component);
        component.ShowCreateEffect();
        if (isCombination)
        {
            component.SetMutekiTime(defaultMutekiTime);
        }
        return component;
    }

    public CandyChip GetNewCandyChip(int x, int y, Vector3 position, bool isCombination = false)
    {
        GameObject spawnBlockObject = SpawnStringBlock.GetSpawnBlockObject(SpawnStringBlockType.SmallCandy);
        spawnBlockObject.transform.position = position;
        spawnBlockObject.transform.parent = GetSlot(x, y).transform;
        CandyChip component = spawnBlockObject.GetComponent<CandyChip>();
        GetSlot(x, y).SetChip(component);
        component.ShowCreateEffect();
        if (isCombination)
        {
            component.SetMutekiTime(defaultMutekiTime);
        }
        return component;
    }

    public Chip AddPowerup(int x, int y, Powerup p, int setChipId = -1)
    {
        if (GetSlot(x, y) == null)
        {
            return null;
        }
        SlotForChip component = GetSlot(x, y).GetComponent<SlotForChip>();
        Chip chip = component.chip;
        int id = (setChipId != -1) ? setChipId : ((!chip) ? boardData.GetRandomChip() : chip.id);
        if ((bool)chip)
        {
            PoolManager.PoolGameBlocks.Despawn(chip.transform);
        }
        switch (p)
        {
            case Powerup.SimpleBomb:
                chip = main.GetNewBomb(component.slot.x, component.slot.y, component.transform.position, id);
                break;
            case Powerup.RainbowBomb:
                chip = main.GetNewRainbowBomb(component.slot.x, component.slot.y, component.transform.position);
                break;
            case Powerup.ColorHBomb:
                chip = main.GetNewColorHBomb(component.slot.x, component.slot.y, component.transform.position, id);
                break;
            case Powerup.ColorVBomb:
                chip = main.GetNewColorVBomb(component.slot.x, component.slot.y, component.transform.position, id);
                break;
            case Powerup.Plus5Move:
                chip = main.GetNewPlusMoveChip(component.slot.x, component.slot.y, component.transform.position, id);
                break;
            case Powerup.BringDownObject:
                chip = main.GetBringDownChip(component.slot.x, component.slot.y, component.transform.position);
                break;
            case Powerup.ColorRandomLineBomb:
                switch (UnityEngine.Random.Range(0, 3))
                {
                    case 0:
                        chip = main.GetNewColorHBomb(component.slot.x, component.slot.y, component.transform.position, id);
                        break;
                    case 1:
                        chip = main.GetNewColorVBomb(component.slot.x, component.slot.y, component.transform.position, id);
                        break;
                }
                break;
            case Powerup.Chameleon:
                chip = main.GetNewSimpleChip(component.slot.x, component.slot.y, component.transform.position, id);
                if (chip != null && chip is SimpleChip)
                {
                    ((SimpleChip)chip).SetPowerupChameleon();
                }
                break;
            case Powerup.OreoCracker:
                chip = main.GetOreoCrackerChip(component.slot.x, component.slot.y, component.transform.position);
                break;
        }
        return chip;
    }

    public void AddPowerup(Powerup p)
    {
        if (slotGroup == null)
        {
            return;
        }
        SimpleChip[] componentsInChildren = slotGroup.GetComponentsInChildren<SimpleChip>();
        if (componentsInChildren.Length == 0)
        {
            return;
        }
        SimpleChip simpleChip = null;
        while (simpleChip == null)
        {
            simpleChip = componentsInChildren[UnityEngine.Random.Range(0, componentsInChildren.Length - 1)];
        }
        if (!(simpleChip.parentSlot == null))
        {
            SlotForChip parentSlot = simpleChip.parentSlot;
            if ((bool)parentSlot)
            {
                AddPowerup(parentSlot.slot.x, parentSlot.slot.y, p);
            }
        }
    }

    public Slot GetSlot(int x, int y)
    {
        if (boardData.GetSlot(x, y) && slots.ContainsKey(x * MapData.MaxHeight + y))
        {
            return slots[x * MapData.MaxHeight + y];
        }
        return null;
    }

    public Slot GetSlotFromSlotPosition(int x, int y)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Slot slot = GetSlot(i, j);
                if ((bool)slot && slot.x == x && slot.y == y)
                {
                    return slot;
                }
            }
        }
        return null;
    }

    public float MoveSlot(int fromX, int fromY, int targetX, int targetY, float movingTime, Ease easeType = Ease.OutQuad)
    {
        Slot slot = GetSlot(fromX, fromY);
        if ((bool)slot)
        {
            slots[targetX * MapData.MaxHeight + targetY] = slot;
            slots[fromX * MapData.MaxHeight + fromY] = null;
            boardData.slots[targetX, targetY] = true;
            boardData.slots[fromX, fromY] = false;
            boardData.chips[targetX, targetY] = boardData.chips[fromX, fromY];
            boardData.chips[fromX, fromY] = 0;
            boardData.blocks[targetX, targetY] = boardData.blocks[fromX, fromY];
            boardData.blocks[fromX, fromY] = IBlockType.None;
            boardData.tunnel[targetX, targetY] = boardData.tunnel[fromX, fromY];
            boardData.tunnel[fromX, fromY] = 0;
            boardData.safeObsSlot[targetX, targetY] = boardData.safeObsSlot[fromX, fromY];
            slot.x = targetX;
            slot.y = targetY;
            float num = 3f;
            float num2 = 0f;
            Vector3 position = slot.transform.position;
            Vector3 slotPosition = GetSlotPosition(targetX, targetY);
            float f = (slotPosition - position).magnitude / slotoffset;
            float num3 = movingTime * Mathf.Pow(f, 1f / num) + num2;
            slot.transform.DOMove(GetSlotPosition(targetX, targetY), num3).SetEase(easeType);
            return num3;
        }
        return movingTime;
    }

    public IEnumerator TweenScaleSlot(GameObject obj, Ease easeType = Ease.OutQuad)
    {
        float tweenScaleDuration = 0.24f;
        float stopDutration = 0.5f;
        yield return obj.transform.DOScale(1.2f, tweenScaleDuration).SetEase(easeType).WaitForCompletion();
        yield return new WaitForSeconds(stopDutration);
        yield return obj.transform.DOScale(1f, tweenScaleDuration).SetEase(easeType).WaitForCompletion();
    }

    public bool CheckCanRemoveRibbonBlock(int sx, int sy, Ribbon startRibbonData)
    {
        List<Ribbon> list = new List<Ribbon>();
        if ((bool)startRibbonData)
        {
            bool flag = false;
            Ribbon ribbon = startRibbonData;
            Ribbon ribbon2 = ribbon.prevRibbon;
            list.Add(startRibbonData);
            while ((bool)ribbon2 && startRibbonData != ribbon2)
            {
                if (ribbon2.knotLevel > 0)
                {
                    flag = true;
                    break;
                }
                list.Add(ribbon2);
                Ribbon ribbon3 = ribbon2;
                ribbon2 = ribbon2.GetOtherRibbon(ribbon);
                ribbon = ribbon3;
            }
            bool flag2 = false;
            ribbon = startRibbonData;
            Ribbon ribbon4 = ribbon.nextRibbon;
            while ((bool)ribbon4 && startRibbonData != ribbon4)
            {
                if (ribbon4.knotLevel > 0)
                {
                    flag2 = true;
                    break;
                }
                list.Add(ribbon4);
                Ribbon ribbon3 = ribbon4;
                ribbon4 = ribbon4.GetOtherRibbon(ribbon);
                ribbon = ribbon3;
            }
            if (!flag && !flag2)
            {
                return true;
            }
            return false;
        }
        return false;
    }

    public void ReplaceRibbonLink(int sx, int sy, Ribbon startRibbonData)
    {
        List<Ribbon> list = new List<Ribbon>();
        if (!startRibbonData)
        {
            return;
        }
        Ribbon ribbon = startRibbonData;
        Ribbon prevRibbon = ribbon.prevRibbon;
        Ribbon ribbon2 = startRibbonData;
        Ribbon nextRibbon = ribbon2.nextRibbon;
        list.Add(startRibbonData);
        while (true)
        {
            if ((!prevRibbon && !nextRibbon) || ((bool)prevRibbon && prevRibbon.knotLevel > 0) || ((bool)nextRibbon && nextRibbon.knotLevel > 0))
            {
                return;
            }
            if (prevRibbon == nextRibbon)
            {
                break;
            }
            if ((bool)prevRibbon)
            {
                list.Add(prevRibbon);
                Ribbon ribbon3 = prevRibbon;
                prevRibbon.prevRibbon = prevRibbon.GetOtherRibbon(ribbon);
                prevRibbon.nextRibbon = ribbon;
                prevRibbon = prevRibbon.prevRibbon;
                ribbon = ribbon3;
            }
            if ((bool)nextRibbon)
            {
                list.Add(nextRibbon);
                Ribbon ribbon4 = nextRibbon;
                nextRibbon.nextRibbon = nextRibbon.GetOtherRibbon(ribbon2);
                nextRibbon.prevRibbon = ribbon2;
                nextRibbon = nextRibbon.nextRibbon;
                ribbon2 = ribbon4;
            }
        }
        prevRibbon.prevRibbon = null;
        nextRibbon.nextRibbon = null;
    }

    public void RefreshTunnelWall()
    {
        for (int i = 0; i < boardData.width; i++)
        {
            for (int j = 0; j < boardData.height; j++)
            {
                if (boardData.tunnel[i, j] == 0)
                {
                    continue;
                }
                Slot slot = GetSlot(i, j);
                if ((bool)slot)
                {
                    slot.SetWall(Side.TopLeft);
                    slot.SetWall(Side.TopRight);
                    slot.SetWall(Side.BottomLeft);
                    slot.SetWall(Side.BottomRight);
                    if (boardData.tunnel[i, j] == 1)
                    {
                        slot.SetWall(Side.Top);
                        slot.SetWall(Side.Bottom);
                    }
                    else if (boardData.tunnel[i, j] == 2)
                    {
                        slot.SetWall(Side.Left);
                        slot.SetWall(Side.Right);
                    }
                    else if (boardData.tunnel[i, j] == 3)
                    {
                        slot.SetWall(Side.Top);
                        slot.SetWall(Side.Right);
                    }
                    else if (boardData.tunnel[i, j] == 4)
                    {
                        slot.SetWall(Side.Top);
                        slot.SetWall(Side.Left);
                    }
                    else if (boardData.tunnel[i, j] == 5)
                    {
                        slot.SetWall(Side.Left);
                        slot.SetWall(Side.Bottom);
                    }
                    else if (boardData.tunnel[i, j] == 6)
                    {
                        slot.SetWall(Side.Right);
                        slot.SetWall(Side.Bottom);
                    }
                }
            }
        }
    }

    private void RefreshWall()
    {
        foreach (Wall item in listWall)
        {
            item.Initialize();
        }
    }

    public void RefreshNearSlots()
    {
        for (int i = 0; i < boardData.width; i++)
        {
            for (int j = 0; j < boardData.height; j++)
            {
                Slot slot = GetSlot(i, j);
                if ((bool)slot)
                {
                    Side side = Side.Null;
                    for (int k = 0; k < 8; k++)
                    {
                        side = Utils.allSides[k];
                        slot.nearSlot[side] = GetNearSlot(i, j, side);
                    }
                }
            }
        }
        RefreshTunnelWall();
        RefreshWall();
        SlotGravity.Reshading();
    }

    public Slot GetNearSlot(int x, int y, Side side)
    {
        int num = Utils.SideOffsetX(side);
        int num2 = Utils.SideOffsetY(side);
        int key = (x + num) * MapData.MaxHeight + y + num2;
        if (boardData.GetSlot(x + num, y + num2) && slots.ContainsKey(key))
        {
            return slots[key];
        }
        return null;
    }

    public BlockInterface GetBlock(int x, int y)
    {
        int key = x * MapData.MaxHeight + y;
        if (boardData.GetSlot(x, y) && slots.ContainsKey(key))
        {
            return slots[key].GetBlock();
        }
        return null;
    }

    public void DelaySlotCrush(float delay, int x, int y, bool radius, ScoreType scoreType = ScoreType.None, bool includeCrushChip = true, int addScoreBlockCount = 0, int fromCrushId = -1, Side crushDir = Side.Null, bool doPaintJelly = false, int subId = -1)
    {
        StartCoroutine(delayedSlotCrush(delay, x, y, radius, scoreType, includeCrushChip, addScoreBlockCount, fromCrushId, crushDir, doPaintJelly, subId));
    }

    private IEnumerator delayedSlotCrush(float delay, int x, int y, bool radius, ScoreType scoreType = ScoreType.None, bool includeCrushChip = true, int addScoreBlockCount = 0, int fromCrushId = -1, Side crushDir = Side.Null, bool doPaintJelly = false, int subId = -1)
    {
        yield return new WaitForSeconds(delay);
        SlotCrush(x, y, radius, scoreType, includeCrushChip, addScoreBlockCount, fromCrushId, crushDir, doPaintJelly, subId);
    }

    public bool SlotCrush(int x, int y, bool radius, ScoreType scoreType = ScoreType.None, bool includeCrushChip = true, int addScoreBlockCount = 0, int fromCrushId = -1, Side crushDir = Side.Null, bool doPaintJelly = false, int subId = -1)
    {
        for (int i = 0; i < boardData.listMovingSlot.Count; i++)
        {
            main.boardData.listMovingSlot[i].CheckTriggerCrushSlot(x, y);
        }
        for (int j = 0; j < boardData.listRotationSlot.Count; j++)
        {
            main.boardData.listRotationSlot[j].CheckTriggerCrushSlot(x, y);
        }
        Slot slot = main.GetSlot(x, y);
        if ((bool)slot)
        {
            if (!slot.canBeCrush)
            {
                return false;
            }
            if (!GetBlock(x, y))
            {
                if (MapData.main.target == GoalTarget.RescueVS)
                {
                    if (GameMain.main.CurrentTurn == VSTurn.Player)
                    {
                        slot.RockCandyCrush();
                    }
                    else
                    {
                        slot.RockCandyFill();
                    }
                }
                else if (MapData.main.target == GoalTarget.Jelly)
                {
                    if (doPaintJelly)
                    {
                        slot.PaintJelly();
                    }
                }
                else
                {
                    slot.RockCandyCrush();
                }
            }
            else if (GetBlock(x, y) is ChocolateJail || GetBlock(x, y) is GreenSlimeChild)
            {
                radius = false;
            }
        }
        if (includeCrushChip && (bool)slot && (bool)slot.GetChip())
        {
            Chip chip = slot.GetChip();
            if (!chip.IsMuteki)
            {
                SetScore(x, y, scoreType, chip.id, addScoreBlockCount);
                if (chip.gameObject.activeSelf)
                {
                    if (chip is ColorHBomb && (crushDir == Side.Right || crushDir == Side.Left))
                    {
                        chip = main.AddPowerup(x, y, Powerup.ColorVBomb);
                    }
                    else if (chip is ColorVBomb && (crushDir == Side.Top || crushDir == Side.Bottom))
                    {
                        chip = main.AddPowerup(x, y, Powerup.ColorHBomb);
                    }
                    chip.DestroyChip(fromCrushId, crushDir);
                }
            }
        }
        return main.BlockCrush(x, y, radius, fromCrushId, subId);
    }

    public void SetScore(int x, int y, ScoreType scoreType, int colorId = -1, int addScoreBlockCount = 0)
    {
        if (MapData.main.target == GoalTarget.RescueVS && GameMain.main.CurrentTurn == VSTurn.CPU)
        {
            return;
        }
        Slot slot = main.GetSlot(x, y);
        if ((bool)slot && scoreType != ScoreType.None)
        {
            int num = MonoSingleton<ScoreManager>.Instance.GetScoreUnit(scoreType);
            if (addScoreBlockCount > 0)
            {
                num += addScoreBlockCount * ScoreManager.baseBlockScore;
            }
            if (!GameMain.main.isBonusTime)
            {
                num *= Mathf.Max(1, GameMain.main.ComboCount);
            }
            GameMain.main.Score += num;
            ScoreEffect.SetScoreEffect(num, slot.transform.position, colorId);
        }
    }

    public bool BlockCrush(int x, int y, bool radius, int fromCrushId = -1, int subId = -1)
    {
        bool result = false;
        BlockInterface block = GetBlock(x, y);
        Slot slot = GetSlot(x, y);
        if ((bool)slot && !slot.canBeCrush)
        {
            return false;
        }
        if (!block && radius)
        {
            Side side = Side.Null;
            for (int i = 0; i < 4; i++)
            {
                side = Utils.straightSides[i];
                block = null;
                slot = null;
                slot = GetSlot(x + Utils.SideOffsetX(side), y + Utils.SideOffsetY(side));
                if (!slot || slot.canBeCrush)
                {
                    block = GetBlock(x + Utils.SideOffsetX(side), y + Utils.SideOffsetY(side));
                    if ((bool)block && block.CanBeCrushedByNearSlot())
                    {
                        block.BlockCrush(fromCrushId, subId);
                    }
                }
            }
        }
        block = GetBlock(x, y);
        if ((bool)block)
        {
            result = !block.destroying;
            block.BlockCrush(fromCrushId, subId);
        }
        return result;
    }

    public void SetChipPositionNextDownChip(int x, int y)
    {
        Slot slot = GetSlot(x, y);
        if (!slot)
        {
            return;
        }
        Chip chip = slot.GetChip();
        if (!chip)
        {
            return;
        }
        Slot nearSlotDropDown = slot.GetNearSlotDropDown();
        if (!nearSlotDropDown)
        {
            return;
        }
        Chip chip2 = nearSlotDropDown.GetChip();
        if ((bool)chip2 && chip2.move)
        {
            Vector3 position = chip2.transform.position;
            switch (slot.DropDir)
            {
                case DropDirection.Down:
                    position.y += slotoffset;
                    break;
                case DropDirection.Up:
                    position.y -= slotoffset;
                    break;
                case DropDirection.Left:
                    position.x += slotoffset;
                    break;
                case DropDirection.Right:
                    position.x -= slotoffset;
                    break;
            }
            chip.transform.position = position;
            chip.velocity = chip2.velocity;
        }
    }

    private bool IsDigBlock(int x, int y)
    {
        bool result = false;
        if (boardData.slots[x, y])
        {
            switch (boardData.blocks[x, y])
            {
                case IBlockType.Digging_HP1:
                case IBlockType.Digging_HP2:
                case IBlockType.Digging_HP3:
                case IBlockType.Digging_HP1_Collect:
                case IBlockType.Digging_HP2_Collect:
                case IBlockType.Digging_HP3_Collect:
                case IBlockType.Digging_HP1_Bomb1:
                case IBlockType.Digging_HP2_Bomb1:
                case IBlockType.Digging_HP3_Bomb1:
                case IBlockType.Digging_HP1_Bomb2:
                case IBlockType.Digging_HP2_Bomb2:
                case IBlockType.Digging_HP3_Bomb2:
                case IBlockType.Digging_HP1_Bomb3:
                case IBlockType.Digging_HP2_Bomb3:
                case IBlockType.Digging_HP3_Bomb3:
                case IBlockType.Digging_HP1_Treasure_G1:
                case IBlockType.Digging_HP2_Treasure_G1:
                case IBlockType.Digging_HP3_Treasure_G1:
                case IBlockType.Digging_HP1_Treasure_G2:
                case IBlockType.Digging_HP2_Treasure_G2:
                case IBlockType.Digging_HP3_Treasure_G2:
                case IBlockType.Digging_HP1_Treasure_G3:
                case IBlockType.Digging_HP2_Treasure_G3:
                case IBlockType.Digging_HP3_Treasure_G3:
                    result = true;
                    break;
            }
        }
        return result;
    }

    public int SearchHighestDigBlock()
    {
        int result = 0;
        switch (MapData.main.diggingScrollDirection)
        {
            case 2:
                for (int num2 = MapData.MaxHeight - 1; num2 >= 0; num2--)
                {
                    for (int n = 0; n < MapData.MaxWidth; n++)
                    {
                        if (IsDigBlock(n, num2))
                        {
                            result = MapData.MaxHeight - 1 - num2;
                        }
                    }
                }
                break;
            case 1:
                for (int k = 0; k < MapData.MaxHeight; k++)
                {
                    for (int l = 0; l < MapData.MaxWidth; l++)
                    {
                        if (IsDigBlock(l, k))
                        {
                            result = k;
                        }
                    }
                }
                break;
            case 4:
                for (int num = MapData.MaxWidth - 1; num >= 0; num--)
                {
                    for (int m = 0; m < MapData.MaxHeight; m++)
                    {
                        if (IsDigBlock(num, m))
                        {
                            result = MapData.MaxWidth - 1 - num;
                        }
                    }
                }
                break;
            case 3:
                for (int i = 0; i < MapData.MaxWidth; i++)
                {
                    for (int j = 0; j < MapData.MaxHeight; j++)
                    {
                        if (IsDigBlock(i, j))
                        {
                            result = i;
                        }
                    }
                }
                break;
        }
        return result;
    }

    private IEnumerator ProcessDigBoard()
    {
        isProcessDigBoard = true;
        SoundSFX.Play(SFXIndex.DigLine);
        switch (MapData.main.diggingScrollDirection)
        {
            case 1:
                for (int n = 0; n < MapData.MaxWidth; n++)
                {
                    for (int num2 = MapData.MaxHeight - 1; num2 >= 0; num2--)
                    {
                        DigExecuteForLoopContents(n, num2);
                    }
                }
                break;
            case 2:
                for (int k = 0; k < MapData.MaxWidth; k++)
                {
                    for (int l = 0; l < MapData.MaxHeight; l++)
                    {
                        DigExecuteForLoopContents(k, l);
                    }
                }
                break;
            case 3:
                for (int num = MapData.MaxWidth - 1; num >= 0; num--)
                {
                    for (int m = 0; m < MapData.MaxHeight; m++)
                    {
                        DigExecuteForLoopContents(num, m);
                    }
                }
                break;
            case 4:
                for (int i = 0; i < MapData.MaxWidth; i++)
                {
                    for (int j = 0; j < MapData.MaxHeight; j++)
                    {
                        DigExecuteForLoopContents(i, j);
                    }
                }
                break;
        }
        yield return new WaitForSeconds(GameMain.main.movingSlotTime);
        //CPanelGameUI.Instance.DisableGameCameraMasking();
        isProcessDigBoard = false;
    }

    private void DigExecuteForLoopContents(int x, int y)
    {
        int2 diggingDirectionXYDelta = MapData.main.GetDiggingDirectionXYDelta();
        int2 grid = default(int2);
        grid.x = x;
        grid.y = y;
        if (DigCheckRangeOut(x, y))
        {
            StartCoroutine(ProcessDigDestroySlot(grid));
            return;
        }
        MoveSlot(x, y, x + diggingDirectionXYDelta.x, y + diggingDirectionXYDelta.y, GameMain.main.movingSlotTime, GameMain.main.movingSlotEase);
        if (MapData.main.DigCheckLastLine(x, y))
        {
            Slot slot = DigCreateSlot(x, y);
            DigCreateBlock(x, y);
            DigCreateChip(x, y);
            DigCreatePowerups(x, y);
            RefreshNearSlots();
            SlotGravity.Reshading();
            if ((bool)slot)
            {
                slot.transform.DOMove(GetSlotPosition(x, y), GameMain.main.movingSlotTime).SetEase(GameMain.main.movingSlotEase);
            }
        }
    }

    private IEnumerator ProcessDigDestroySlot(int2 grid)
    {
        int2 delta = MapData.main.GetDiggingDirectionXYDelta();
        int x = grid.x;
        int y = grid.y;
        Slot fromSlot = GetSlot(x, y);
        Slot toSlot = GetSlot(x - delta.x, y - delta.y);
        DigDestroySlotData(x, y);
        fromSlot.transform.DOMove(GetSlotPosition(x + delta.x, y + delta.y), GameMain.main.movingSlotTime).SetEase(GameMain.main.movingSlotEase);
        yield return new WaitForSeconds(GameMain.main.movingSlotTime);
        DigDestroySlot(fromSlot, toSlot);
    }

    private bool DigCheckRangeOut(int x, int y)
    {
        bool result = false;
        int2 diggingDirectionXYDelta = MapData.main.GetDiggingDirectionXYDelta();
        if (y + diggingDirectionXYDelta.y >= MapData.MaxHeight || y + diggingDirectionXYDelta.y < 0 || x + diggingDirectionXYDelta.x >= MapData.MaxWidth || x + diggingDirectionXYDelta.x < 0)
        {
            result = true;
        }
        return result;
    }

    public bool DigCheckLineDataIndexEnd()
    {
        bool result = false;
        if (MapData.main.diggingScrollDirection == 1 || MapData.main.diggingScrollDirection == 2)
        {
            if (MapData.main.currentLineDataIndex > MapData.main.heightDig)
            {
                result = true;
            }
        }
        else if ((MapData.main.diggingScrollDirection == 3 || MapData.main.diggingScrollDirection == 4) && MapData.main.currentLineDataIndex > MapData.main.widthDig)
        {
            result = true;
        }
        return result;
    }

    public void DigAddUpLineDataIndex()
    {
        MapData.main.currentLineDataIndex++;
    }

    private void DigDestroySlotData(int x, int y)
    {
        slots[x * MapData.MaxHeight + y] = null;
        boardData.slots[x, y] = false;
        boardData.chips[x, y] = 0;
        boardData.blocks[x, y] = IBlockType.None;
        boardData.tunnel[x, y] = 0;
    }

    public void DigDestroySlot(Slot fromSlot, Slot toSlot)
    {
        if (fromSlot == null || toSlot == null)
        {
            return;
        }
        int x = fromSlot.x;
        int y = fromSlot.y;
        if (fromSlot.GetComponent<SlotGenerator>() != null)
        {
            BoardPosition key = default(BoardPosition);
            key.x = x;
            key.y = y;
            if (dicGeneratorDrop.ContainsKey(key) || dicGeneratorSpecialDrop.ContainsKey(key))
            {
                toSlot.gameObject.AddComponent<SlotGenerator>();
            }
        }
        Transform[] componentsInChildren = fromSlot.gameObject.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (Transform instance in componentsInChildren)
        {
            if (PoolManager.PoolGameBlocks.IsSpawned(instance))
            {
                PoolManager.PoolGameBlocks.Despawn(instance);
            }
            else if (PoolManager.PoolGameEffect.IsSpawned(instance))
            {
                PoolManager.PoolGameEffect.Despawn(instance);
            }
            else if (PoolManager.PoolGamePlaying.IsSpawned(instance))
            {
                PoolManager.PoolGamePlaying.Despawn(instance);
            }
        }
        UnityEngine.Object.DestroyImmediate(fromSlot.gameObject);
    }

    public Slot DigCreateSlot(int x, int y)
    {
        Slot slot = null;
        Vector3 vector = default(Vector3);
        BoardPosition boardPosition = default(BoardPosition);
        int2 @int = MapData.main.TransformXYConsiderCurrentLineDataIndex(x, y);
        if (!boardData.slots[x, y])
        {
            int2 diggingDirectionXYDelta = MapData.main.GetDiggingDirectionXYDelta();
            vector.x = (0f - slotoffset) * (0.5f * (float)(boardData.width - 1) - (float)(x - diggingDirectionXYDelta.x));
            vector.y = (0f - slotoffset) * (0.5f * (float)(boardData.height - 1) - (float)(y - diggingDirectionXYDelta.y));
            boardPosition.x = x;
            boardPosition.y = y;
            GameObject item = ContentAssistant.main.GetItem("SlotEmpty", vector);
            item.transform.parent = slotGroup.transform;
            item.transform.localPosition = vector;
            slot = item.GetComponent<Slot>();
            slot.x = x;
            slot.y = y;
            slot.SetSlotBackground((@int.x + @int.y) % 2);
            boardData.rockCandyTile[x, y] = MapData.main.rockCandyTileDig[@int.x, @int.y];
            slot.GenerateRockCandy(boardData.rockCandyTile[x, y]);
            boardData.dropDirection[x, y] = MapData.main.dropDirectionDig[@int.x, @int.y];
            slot.SetDropDirection(boardData.dropDirection[x, y]);
            boardData.dropLock[x, y] = MapData.main.dropLockDig[@int.x, @int.y];
            slot.SetDropLock(boardData.dropLock[x, y]);
            slot.Initialize();
            boardData.slots[x, y] = true;
            slots[x * MapData.MaxWidth + y] = slot;
        }
        return slot;
    }

    private void DigCreateChip(int x, int y)
    {
        int2 @int = MapData.main.TransformXYConsiderCurrentLineDataIndex(x, y);
        if (!boardData.slots[x, y] || (MapData.main.blocksDig[@int.x, @int.y] != 0 && !MapData.IsBlockTypeIncludingChip(MapData.main.blocksDig[@int.x, @int.y])))
        {
            MapData.main.chipsDig[@int.x, @int.y] = -1;
        }
        if (MapData.IsBlockTypeIncludingChip(MapData.main.blocksDig[@int.x, @int.y]) && MapData.main.chipsDig[@int.x, @int.y] == -1)
        {
            MapData.main.chipsDig[@int.x, @int.y] = 0;
        }
        int chipDig = boardData.GetChipDig(@int.x, @int.y);
        if (chipDig >= 0 && chipDig != 9 && chipDig != 10 && (MapData.main.blocksDig[@int.x, @int.y] == IBlockType.None || MapData.IsBlockTypeIncludingChip(MapData.main.blocksDig[@int.x, @int.y])))
        {
            Chip newSimpleChip = GetNewSimpleChip(x, y, new Vector3(0f, 0f, 0f));
            newSimpleChip.transform.localPosition = Vector3.zero;
            SimpleChip simpleChip = (SimpleChip)newSimpleChip;
            simpleChip.AttachOCCToken();
        }
        if (chipDig == 9 && MapData.main.blocksDig[@int.x, @int.y] == IBlockType.None)
        {
            GetNewStone(x, y, new Vector3(0f, 2f, 0f));
        }
    }

    private void DigCreatePowerups(int x, int y)
    {
        int2 @int = MapData.main.TransformXYConsiderCurrentLineDataIndex(x, y);
        if (MapData.main.powerUpsDig[@int.x, @int.y] > 0 && boardData.slots[x, y])
        {
            Powerup p = (Powerup)MapData.main.powerUpsDig[@int.x, @int.y];
            AddPowerup(x, y, p);
        }
    }

    public void DigCreateBlock(int x, int y)
    {
        int2 @int = MapData.main.TransformXYConsiderCurrentLineDataIndex(x, y);
        if (boardData.slots[x, y])
        {
            CompatibleCreateBlock(x, y, MapData.main.blocksDig[@int.x, @int.y], MapData.main.param1Dig[@int.x, @int.y], MapData.main.param2Dig[@int.x, @int.y]);
        }
    }
}
