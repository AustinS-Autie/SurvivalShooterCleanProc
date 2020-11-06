using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    // can't see 2D array in inspector
    public Transform[] roomSpawnersRow0;
    public Transform[] roomSpawnersRow1;
    public Transform[] roomSpawnersRow2;
    public Transform[] roomSpawnersRow3;

    public GameObject[] rooms;

    public int testRow = 0;
    public int testColumn = 0;
    public int testType = 0;

    //ADDED VV
    int currentRow;
    int currentCol;
    int nextRow;
    int nextCol;
    int roomToGenerate;
    int direction;
    int[,] roomLayout;
    
    bool isConnectingRoom;

    int[] possibleRooms;
    int isRight;
    int[] isRightArray;
    int RNGSeed;
    bool isFinished;
    int roomSize;


    // Use this for initialization
    //type 0: no paths
    //type 1: left/right
    //type 2: left/right/down
    //type 3: left/right/up
    //type 4: all directions
    //rooms are 30x30 size
    void Start() {

        roomLayout = new int[4, 4] {
        { 5, 5, 5, 5},
        { 5, 5, 5, 5},
        { 5, 5, 5, 5},
        { 5, 5, 5, 5},};

        direction = 0;
        currentRow = 0;
        currentCol = 0;
        roomToGenerate = Random.Range(1, 5);
        isConnectingRoom = false;
        isRightArray = new int[2] { -1, 1 };
        isRight = 1;
        
        RNGSeed = (int)Time.realtimeSinceStartup + 3; //change this number to change the room RNG
        isFinished = false;

        roomSize = 30;

        

        while (!isFinished)
        {
            Random.InitState(RNGSeed);
            
            BuildPossiblePath();

            if (currentRow >= 3)
            {
                isFinished = true;
            }
            else
                RNGSeed += 1;
        }

        Random.InitState(RNGSeed*2);
        FillInRemainder();
        //BlockSideExits(); unused, done manually

        roomLayout[0, 0] = 4;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (roomLayout[i, j] != 5)
                    AddRoom(i, j, roomLayout[i, j]);
                else
                    Debug.Log("TEST");

            }
        }

    }

    void BuildPossiblePath()
    {
        if (isFinished)
            return;

        if (isConnectingRoom)
            currentRow += 1;

        possibleRooms = new int[3] {1, 2, 3};

        if (isConnectingRoom)
        {
            possibleRooms = new int[1] { 4 };
            isConnectingRoom = false;
            isRight = isRightArray[Random.Range(0, isRightArray.Length)];

        }

        if (currentCol == 0 && roomLayout[currentRow,currentCol + 1] != 5)
        {
            if (!isConnectingRoom)
                direction = 3;
            else
                isRight = 1;
        }
        
        if(currentCol==3 && roomLayout[currentRow, currentCol - 1] != 5)
        {
            if (!isConnectingRoom)
                direction = 3;
            else
                isRight = -1;
        }

        if (direction == 3)
        {
            isConnectingRoom = true;
            possibleRooms = new int[2] { 2, 4 };
            isRight = 0;
        }

        roomToGenerate = possibleRooms[Random.Range(0, possibleRooms.Length)];
        direction = GetDirectionsFrom(roomToGenerate);

        if (isRight == 1 && direction == 2)
        {
            direction = 0;
            isRight *= -1;
        }

        if (isRight == -1 && direction == 0)
        {
            direction = 2;
            isRight *= -1;
        }

        if (WithinRange(currentRow,0,3) &&
            WithinRange(currentCol, 0, 3) && 
            roomLayout[currentRow, currentCol] == 5)
        {
            roomLayout[currentRow,currentCol] = roomToGenerate;
            //AddRoom(currentRow, currentCol, roomToGenerate);
        }

        if(WithinRange(currentCol+isRight,0,3))
            currentCol += isRight;

        //Debug.Log("Next location: " + (nextRow) + ", " + (nextCol) );
    }

    void FillInRemainder()
    {
        possibleRooms = new int[5] {2, 4, 3, 4, 4};

        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
            {
                Random.InitState(RNGSeed);

                roomToGenerate = possibleRooms[Random.Range(0, possibleRooms.Length)];

                if (roomLayout[i, j] == 5)
                {
                    roomLayout[i, j] = roomToGenerate;
                    //AddRoom(i, j, roomToGenerate);
                }
            }

        
    }


    // Update is called once per frame
    void Update() {


        RNGSeed += 1;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddRoom(testRow, testColumn, testType);
        }
    }


    public void AddRoom(int row, int column, int roomType)
    {
        Vector3 spawnPos = Vector3.zero;
        // figure out position to spawn at
        switch (row)
        {
            case 0:
                spawnPos = roomSpawnersRow0[column].position;
                break;
            case 1:
                spawnPos = roomSpawnersRow1[column].position;
                break;
            case 2:
                spawnPos = roomSpawnersRow2[column].position;
                break;
            case 3:
                spawnPos = roomSpawnersRow3[column].position;
                break;
        }

        // actually spawn it
        Instantiate(rooms[roomType], spawnPos, transform.rotation);
    }

    public int GetDirectionsFrom(int roomType)
    {
        
        int[] chanceArray;
        int arrayIndex;
        
        switch (roomType)
        {
            case 0:
                chanceArray = new int[1] { -1 };
                break;
            case 1:
                chanceArray = new int[2] { 0, 2 };
                break;
            case 2:
                chanceArray = new int[3] { 0, 2, 3 };
                break;
            case 3:
                chanceArray = new int[3] { 0, 1, 2 };
                break;
            default:
                chanceArray = new int[4] { 0, 1, 2, 3 };
                break;
        }

        Random.InitState(RNGSeed+1);

        arrayIndex = Random.Range(0, chanceArray.Length);

        return chanceArray[arrayIndex];
    }

    public bool WithinRange(int check, int minNum, int maxNum)
    {
        //Debug.Log(check + " >= " + minNum + " and " + check + " <= " + maxNum);
        return check >= minNum && check <= maxNum;
    }

    public void ResetNext() //unused
    {
        nextRow = currentRow;
        nextCol = currentCol;
        
        if (direction == 0)
            nextCol = currentCol+1;
        if (direction == 1)
            nextRow = currentRow-1;
        if (direction == 2)
            nextCol = currentCol-1;
        if (direction == 3)
            nextRow = currentRow+1;


        if (nextRow < 0) nextRow = 0;
        if (nextCol < 0) nextCol = 0;
        if (nextRow > 3) nextRow = 3;
        if (nextCol > 3) nextCol = 3;
    }
}
