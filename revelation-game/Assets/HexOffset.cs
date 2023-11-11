using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexOffset : MonoBehaviour
{

    float hexWidth;
    float hexHeight;
    public int numTiles;
    public GameObject hexTile;
    public Material sand;
    public Material rock;
    public GameObject player;
    public float heightDiff;
    public RawImage perlin; // Reference to the RawImage component containing the height map
    Texture2D heightMap;

    private GameObject[,] terrainCoordinates = new GameObject[60,60];

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

        heightMap = perlin.texture as Texture2D;
        GenerateTerrain2();

    }
    public void UpdateMaterial(GameObject obj, Material mat)
    {
        obj.GetComponent<Renderer>().material = mat;
    }
    public void GenerateTerrain2()
    {
        for (int z = 0; z < terrainCoordinates.GetLength(0); z++)
        {
            for (int x = 0; x < terrainCoordinates.GetLength(1); x++)
            {
                GameObject hex = Instantiate(hexTile);
                hex.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.z, this.transform.localScale.y);
                terrainCoordinates[x, z] = hex;

                float offsetX = z % 2 == 1 ?
                    x * 2 * Mathf.Sqrt(3) * (hexWidth / 2) :
                    x * 2 * Mathf.Sqrt(3) * (hexWidth / 2) + Mathf.Sqrt(3) * (hexWidth / 2);

                float offsetZ = z * .75f * (2 * hexHeight);

                Color pixelColor = heightMap.GetPixel(x, z);
                float yOffset = pixelColor.r * heightDiff; // Assuming the red channel represents the height

                hex.transform.position = new Vector3(offsetX, yOffset, offsetZ);

                // Set material based on yOffset
                if (yOffset < 3f)
                {
                    Destroy(hex);
                }
                if (yOffset <= 5f)
                {
                    UpdateMaterial(hex, sand);
                }


                if (x == 0 || z == 0 || x == terrainCoordinates.GetLength(1) - 1 || z == terrainCoordinates.GetLength(0) - 1)
                {
                    UpdateMaterial(hex, sand);
                }

                if (x == terrainCoordinates.GetLength(1) / 2 && z == terrainCoordinates.GetLength(0) / 2)
                {
                    player.transform.position = hex.transform.position + new Vector3(0, (hex.transform.localScale.z / 2) + yOffset, 0);
                }
            }
        }
    }

    void GenerateTerrain()
    {
        hexWidth = this.transform.localScale.x;
        hexHeight = this.transform.localScale.z;

        int hexSpawnX = UnityEngine.Random.Range(0, terrainCoordinates.GetLength(0));
        int hexSpawnZ = UnityEngine.Random.Range(0, terrainCoordinates.GetLength(1));

        GameObject hex;
        for (int z = 0; z < terrainCoordinates.GetLength(0); z++)
        {
            for (int x = 0; x < terrainCoordinates.GetLength(1); x++)
            {
                {

                    hex = Instantiate<GameObject>(hexTile);
                    terrainCoordinates[x, z] = hex;


                    if (x == 0 || z == 0 || x == terrainCoordinates.GetLength(1) - 1 || z == terrainCoordinates.GetLength(0) - 1)
                    {
                        UpdateMaterial(hex, sand);
                    }

                    float offsetX;
                    offsetX = z % 2 == 1 ?
                        x * 2 * Mathf.Sqrt(3) * (hexWidth / 2) :
                        x * 2 * Mathf.Sqrt(3) * (hexWidth / 2) + Mathf.Sqrt(3) * (hexWidth / 2);

                    float offsetZ = z * .75f * (2 * hexHeight);

                    float offsetY = GenerateRandomWithExponentialDistribution(5f, 0, 3f);
                    if (offsetY > 7.5) { UpdateMaterial(hex, rock); }
                    else if (offsetY < .3) { offsetY = 0; }



                    Debug.Log(x + " " + z);
                    hex.transform.position = new Vector3(offsetX, offsetY, offsetZ);
                    if (hexSpawnX == x && hexSpawnZ == z)
                    {

                        player.transform.position = hex.transform.position + new Vector3(0, (hex.transform.localScale.z / 2) + offsetY, 0);
                    }
                }
            }
        }

        for (int i = 0; i < 2; i++)
        {
            int startingSide = UnityEngine.Random.Range(0, 2);
            int index = UnityEngine.Random.Range(0, terrainCoordinates.GetLength(1));
            GameObject riverStart;

            while (true)
            {
                riverStart = terrainCoordinates[startingSide, index];

                // Store the material of the destroyed tile
                Material destroyedMaterial = riverStart.GetComponent<Renderer>().material;
                Destroy(riverStart);

                // Iterate through adjacent tiles and change their materials to sand
                for (int adjacentSide = startingSide - 1; adjacentSide <= startingSide + 1; adjacentSide++)
                {
                    for (int adjacentIndex = index - 1; adjacentIndex <= index + 1; adjacentIndex++)
                    {
                        if (adjacentSide >= 0 && adjacentSide < terrainCoordinates.GetLength(0) &&
                            adjacentIndex >= 0 && adjacentIndex < terrainCoordinates.GetLength(1))
                        {
                            GameObject adjacentTile = terrainCoordinates[adjacentSide, adjacentIndex];
                            if (adjacentTile != null)
                            {
                                Renderer renderer = adjacentTile.GetComponent<Renderer>();
                                if (renderer != null)
                                {
                                    // Change the material of adjacent tiles to sand
                                    renderer.material = sand;
                                }
                            }
                        }
                    }
                }

                // Increment startingSide and index separately within their own ranges
                int randomChoice = UnityEngine.Random.Range(0, 2);
                if (randomChoice == 0)
                {
                    startingSide += 1;
                }
                else
                {
                    index += 1;
                }

                // Check for out of bounds conditions
                if (startingSide >= terrainCoordinates.GetLength(0) || startingSide < 0 ||
                    index >= terrainCoordinates.GetLength(1) || index < 0)
                {
                    break;
                }
            }
        }


    }

    // Update is called once per frame
    void Update()
    {

    }
    public float GenerateRandomWithExponentialDistribution(float lambda, float min, float max)
    {
        float u = UnityEngine.Random.value;
        float exponentialValue = -Mathf.Log(1 - u) / lambda;

        // Map the exponentialValue to the desired range (min, max) and clamp it
        return Mathf.Clamp(Mathf.Lerp(min, max, exponentialValue), min, max);
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
