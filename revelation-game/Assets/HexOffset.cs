using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexOffset : MonoBehaviour
{

    float hexWidth;
    float hexHeight;
    public float xOffset;
    public float yOffset;
    public int numTiles;
    public GameObject hexTile;

    private int[,] terrainCoordinates = new int[10,10];

    public static int[,] HexagonCoordinates = new int[,]
    {
        { 2, 0 },
        { -2, 0 },
        { 1, 1 },
        { -1, 1 },
        { -1, -1 },
        { 1, -1 }
    };
    // Start is called before the first frame update
    void Start()
    {
        hexWidth = this.transform.localScale.x;
        hexHeight = this.transform.localScale.z;

        GameObject hex = Instantiate<GameObject>(hexTile);
        hex.transform.localScale = this.transform.localScale;
        hex.transform.localPosition = new Vector3(0, 0, 0);
        for (int y = 0; y < terrainCoordinates.GetLength(0); y++)
        {
            for (int x = 0; x < terrainCoordinates.GetLength(1); x++)
            {
                int randomValue = UnityEngine.Random.Range(0, 2);
                if (randomValue > 0)
                {
                    hex = Instantiate<GameObject>(hexTile);
                    float offsetX;
                    offsetX = y % 2 == 1 ? x * 2 * Mathf.Sqrt(3) * (hexWidth / 2) : x * 2 * Mathf.Sqrt(3) * (hexWidth / 2) + Mathf.Sqrt(3) * (hexWidth / 2);

                    float offsetY = y * .75f * (2 * hexHeight);

                    Debug.Log(x + " " + y);
                    hex.transform.position = new Vector3(offsetX, 0f, offsetY);
                }
            }
        }

        //GameObject hex = Instantiate<GameObject>(hexTile);
        //hex.transform.localScale = this.transform.localScale;
        //hex.transform.localPosition = new Vector3(0, 0, 0);
        //for (int i = 0; i < numTiles; i++)
        //{
            //int randomValue = UnityEngine.Random.Range(0, HexagonCoordinates.GetLength(0));
          //  TileOffet(HexagonCoordinates[randomValue, 0], HexagonCoordinates[randomValue, 1]);
        //}

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TileOffet(int x, int y)
    {
        GameObject hex = Instantiate<GameObject>(hexTile);
        hex.transform.localScale = this.transform.localScale;
        float offsetX = x * Mathf.Sqrt(3) * (hexWidth / 2);
        float offsetY = y * .75f *  (2 * hexHeight);

        Debug.Log(x + " " + y);
        hex.transform.position =  new Vector3(offsetX, 0f, offsetY);
    }
}
