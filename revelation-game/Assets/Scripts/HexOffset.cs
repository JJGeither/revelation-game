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
    public GameObject tree;
    public GameObject rockObject;
    public GameObject pineTree;
    public GameObject chest;
    public float heightDiff;
    public int dimensions;

    public RawImage perlin;
    public RawImage treePerlin;
    private Texture2D heightMap;
    private Texture2D treeMap;
    public PerlinNoiseGenerator perlinNoiseGenerator;
    public PerlinNoiseGenerator treeNoiseGenerator;

    private GameObject[,] terrainCoordinates;

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
        terrainCoordinates = new GameObject[dimensions, dimensions];
        int seed = UnityEngine.Random.Range(0, 10000); // Generate a random seed between 0 and 9999
        UnityEngine.Random.InitState(seed);
        perlinNoiseGenerator.DisplayPerlinTerrain(UnityEngine.Random.Range(0, 10000));
        treeNoiseGenerator.DisplayPerlinTree(UnityEngine.Random.Range(0, 10000));
        heightMap = perlin.texture as Texture2D;
        treeMap = treePerlin.texture as Texture2D;
        hexWidth = hexTile.transform.localScale.x;
        hexHeight = hexTile.transform.localScale.z;
        GenerateTerrain();

    }
    public void UpdateMaterial(GameObject obj, Material mat)
    {
        obj.GetComponent<Renderer>().material = mat;
    }

    public void GenerateRocks(int x, int z)
    {

        int rocksCount = UnityEngine.Random.Range(-1, 3);

        for (int i = 0; i < rocksCount; i++)
        {
            GameObject rockObj = Instantiate(rockObject);
            GameObject hex = terrainCoordinates[x, z];
            float objectHeight = hex.GetComponent<MeshCollider>().bounds.size.y / 2;
            rockObj.transform.SetParent(hex.transform, true);

            // Adjust position as needed
            float randomOffsetX = UnityEngine.Random.Range(0, 15) * (UnityEngine.Random.value > 0.5f ? 1 : -1);
            float randomOffsetZ = UnityEngine.Random.Range(0, 15) * (UnityEngine.Random.value > 0.5f ? 1 : -1);

            rockObj.transform.position = new Vector3(
                hex.transform.position.x + randomOffsetX,
                hex.transform.position.y + objectHeight,
                hex.transform.position.z + randomOffsetZ
            );

            //float randYRotation = UnityEngine.Random.Range(0, 360);
            //rockObj.transform.rotation = Quaternion.Euler(0, randYRotation, 0);

        }
    }



    public void GenerateChests(int x, int z)
    {
        Debug.Log("Chest attempt");
        if (UnityEngine.Random.Range(1, 101) > 95)
        {
            GameObject chestObj = Instantiate(chest);
            GameObject hex = terrainCoordinates[x, z];
            float objectHeight = hex.GetComponent<MeshCollider>().bounds.size.y / 2;
            chestObj.transform.parent = hex.transform;

            // Adjust position as needed
            float randomOffsetX = UnityEngine.Random.Range(0, 15) * (UnityEngine.Random.value > 0.5f ? 1 : -1);
            float randomOffsetZ = UnityEngine.Random.Range(0, 15) * (UnityEngine.Random.value > 0.5f ? 1 : -1);

            chestObj.transform.position = new Vector3(
                hex.transform.position.x + randomOffsetX,
                hex.transform.position.y + objectHeight,
                hex.transform.position.z + randomOffsetZ
            );

            float randYRotation = UnityEngine.Random.Range(0, 360);
            chestObj.transform.rotation = Quaternion.Euler(0, randYRotation, 0);
            randYRotation = UnityEngine.Random.Range(1f, 2f); // Adjust the range for scaling
            chestObj.transform.localScale *= randYRotation;
        }
    }

    public void GenerateTrees(int x, int z)
    {
        Color pixelColor = treeMap.GetPixel(x, z);
        if (pixelColor.g == 1)
        {
            for (int i = 0; i < UnityEngine.Random.Range(1, 4); i++)
            {
                GameObject treeObj;
                if (UnityEngine.Random.value > 0.5f)
                {
                    treeObj = Instantiate(tree);
                }
                else
                {
                    treeObj = Instantiate(pineTree);
                }
                GameObject hex = terrainCoordinates[x, z];
                float objectHeight = hex.GetComponent<MeshCollider>().bounds.size.y / 2;
                treeObj.transform.parent = hex.transform;
                float randomOffsetX = UnityEngine.Random.Range(0, 15) * (UnityEngine.Random.value > 0.5f ? 1 : -1);
                float randomOffsetZ = UnityEngine.Random.Range(0, 15) * (UnityEngine.Random.value > 0.5f ? 1 : -1);

                treeObj.transform.position = new Vector3(
                    hex.transform.position.x + randomOffsetX,
                    hex.transform.position.y + objectHeight,
                    hex.transform.position.z + randomOffsetZ
                );

                float randYRotation = UnityEngine.Random.Range(0, 360);
                treeObj.transform.rotation = Quaternion.Euler(0, randYRotation, 0);
                randYRotation = UnityEngine.Random.Range(1f, 2f); // Adjust the range for scaling
                treeObj.transform.localScale *= randYRotation;

            }

        }

    }
    public void GenerateTerrain()
    {
        for (int z = 0; z < terrainCoordinates.GetLength(0); z++)
        {
            for (int x = 0; x < terrainCoordinates.GetLength(1); x++)
            {
                Color pixelColor = heightMap.GetPixel(x, z);
                float yOffset = pixelColor.g * heightDiff; // Assuming the red channel represents the height
                if (pixelColor.b != 1)
                {
                    GameObject hex = Instantiate(hexTile);

                    hex.transform.parent = this.transform;
                    terrainCoordinates[x, z] = hex;

                    float offsetX = z % 2 == 1 ?
                        x * 2 * Mathf.Sqrt(3) * (hexWidth / 2) :
                        x * 2 * Mathf.Sqrt(3) * (hexWidth / 2) + Mathf.Sqrt(3) * (hexWidth / 2);

                    float offsetZ = z * .75f * (2 * hexHeight);

                    hex.transform.position = new Vector3(offsetX, yOffset, offsetZ);

                    // Set material based on yOffset
                    if (pixelColor.r == 1)
                    {
                        UpdateMaterial(hex, sand);
                    } else
                    {
                        GenerateTrees(x, z);
                    }

                    GenerateRocks(x, z);
                    GenerateChests(x, z);

                    if (x == 0 || z == 0 || x == terrainCoordinates.GetLength(1) - 1 || z == terrainCoordinates.GetLength(0) - 1)
                    {
                        UpdateMaterial(hex, sand); 
                    }




                    float objectHeight = hex.GetComponent<MeshCollider>().bounds.size.y / 2;
                    if (x == terrainCoordinates.GetLength(1) / 2 && z == terrainCoordinates.GetLength(0) / 2)
                    {

                        player.transform.position = new Vector3(hex.transform.position.x, hex.transform.position.y + objectHeight + 5 , hex.transform.position.z);

                    }
                }
                
            }
        }
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
