using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public static Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();
    static Dictionary<int, float> usingTime = new Dictionary<int, float>();

    int id { get; set; }
    int imageId { get; set; }
    float loadedTime;
	
    //图片加载结束
    void LoadComplete(Image image)
    {
        GetComponent<Renderer>().material.mainTexture = image.tex;
        loadedTime = Time.realtimeSinceStartup;
    }
    
    public bool IsTimeOut()
    {
        return Time.realtimeSinceStartup - loadedTime > 200;
    }

    public void Stay()
    {
        usingTime[imageId] += Time.deltaTime;
    }

    static Transform GetParent()
    {
        GameObject parent = GameObject.Find("Tiles");
        if (!parent)
            parent = new GameObject("Tiles");
        return parent.transform;
    }

    public static Tile Create(int id, int index)
    {
        if (tiles.ContainsKey(id))
        {
            if (tiles[id].IsTimeOut())
            {
                tiles[id].imageId = index;
                DownLoadImage.Load(index, tiles[id].gameObject, "LoadComplete", Manager.Instance.IsLocal(index));
            }
            return tiles[id];
        }
        else
        {
            Transform parent = GetParent();
            GameObject newOne = Instantiate(Manager.Instance.tile) as GameObject;
            newOne.name = id.ToString();
            newOne.transform.parent = parent;
            Tile result = newOne.GetComponent<Tile>();
            result.id = id;
            result.imageId = index;
            usingTime[index] = 0;
            newOne.transform.position = new Vector3(id % 100, id / 100, 0);
            DownLoadImage.Load(index, newOne, "LoadComplete", Manager.Instance.IsLocal(index));
            return result;
        }
    }

    public static List<int> SaveData()
    {
        List<int> result = new List<int>();
        List<KeyValuePair<int, float>> temp = new List<KeyValuePair<int, float>>();
        foreach (KeyValuePair<int, float> kv in usingTime)
        {
            temp.Add(kv);
        }
        temp.Sort(delegate(KeyValuePair<int, float> a, KeyValuePair<int, float> b) { return b.Value.CompareTo(a.Value); });
        for (int i = 0; i < temp.Count; i++)
        {
            if (i < 30)
            {
                result.Add(temp[i].Key);
            }
            else
            { 
                break;
            }
        }
        return result;
    }
}
