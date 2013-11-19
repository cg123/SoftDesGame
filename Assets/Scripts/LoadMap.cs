using UnityEngine;
using System.Collections;
using System.Linq;

public class LoadMap : MonoBehaviour
{
    public TextAsset map;
    public Sprite[] tileSprites;
    GameObject[,] tiles;

    void DoLoadMap()
    {
        string[] lines = map.text.Split('\r', '\n');
        lines = lines.Select((l) => l.Trim()).Where((l) => l != "").ToArray();

        string[] firstLine = lines[0].Split(' ', '\t');
        int width = int.Parse(firstLine[0]),
            height = int.Parse(firstLine[1]);

        tiles = new GameObject[width, height];
        int i, j;
        for (j = 0; j < height; j++)
        {
            Debug.Log(lines[j + 1]);
            string[] chunks = lines[j + 1].Trim().Split(' ', '\t');
            for (i = 0; i < width; i++)
            {
                int tileNum = int.Parse(chunks[i]);
                if (tileNum < 0) continue;

                Debug.Log(i.ToString() + ", " + j.ToString());
                Debug.Log(chunks[i]);
                tiles[i, j] = new GameObject(i.ToString() + ", " + j.ToString());
                tiles[i, j].transform.parent = transform;
                tiles[i, j].transform.localPosition = new Vector3(i, -j, 50);
                tiles[i, j].AddComponent<SpriteRenderer>();
                tiles[i, j].GetComponent<SpriteRenderer>().sprite = tileSprites[tileNum];
            }
        }
    }

    void Start()
    {
        DoLoadMap();
    }
}
