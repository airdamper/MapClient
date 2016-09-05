using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Image
{
    public int id;
    public Texture2D tex;
    public Image(int id, Texture2D tex)
    {
        this.id = id;
        this.tex = tex;
    }
}
public struct IntRect
{
    public int x, y, w, h;
    public IntRect(int x, int y, int w, int h)
    {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
    }
    public override string ToString()
    {
        return "x = " + x + " ,y = " + y + " ,w = " + w + " ,h = " + h;
    }
}
public class Manager : OnlyOne<Manager> 
{
    public GameObject tile;
    public IntRect viewRect;

    public MoveCamera moveCamera;

    List<int> localImageId = new List<int>();

    bool isPreactLoad = false;
    List<int> viewIndex;
	void Start ()
    {
        ServerData.Instance.RepetChange(5);
        GetLocalImage();
        ShowMap();
	}
    void GetLocalImage()
    {
        string imagePath = Application.dataPath + "/images";
        if (System.IO.Directory.Exists(imagePath))
        {
            foreach(string fileName in System.IO.Directory.GetFiles(imagePath))
            {
                string name = System.IO.Path.GetFileName(fileName);
                if (name.Contains(".jpg"))
                {
                    localImageId.Add(name.Replace(".jpg", "").ToInt());
                }
            }
        }
    }

	void Update () 
    {
        if (Input.GetMouseButtonUp(0))
        {
            ShowMap();
        }
        if (!moveCamera.dragging && viewIndex != null) 
        {
            foreach (int i in viewIndex)
            {
                if(Tile.tiles.ContainsKey(i))
                    Tile.tiles[i].Stay();
            }
        }
	}
    void ShowMap()
    {
        UpdateViewRect();
        UpdateTile(viewRect);
        viewIndex = GetIndex(viewRect);
        isPreactLoad = false;
    }

    //预加载
    public void PreactLoad()
    {
        if (!isPreactLoad)
        {
            isPreactLoad = true;
            UpdateTile(GetTiles(ViewRect(moveCamera.direction)));
        }
    }

    //更新场景中的块
    void UpdateViewRect()
    {
        viewRect = GetTiles(ViewRect());
    }

    void UpdateTile(IntRect viewRect)
    {
        List<int> tiles = GetIndex(viewRect);
        foreach (KeyValuePair<int, int> kv in ServerData.Instance.Get(tiles))
        {
            Tile.Create(kv.Key, kv.Value);
        }
    }

    List<int> GetIndex(IntRect viewRect)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < viewRect.w; i++)
        {
            for (int j = 0; j < viewRect.h; j++)
            {
                result.Add((viewRect.y + j) * 100 + viewRect.x + i);
            }
        }
        return result;
    }

    public bool IsLocal(int id)
    {
        return localImageId.Contains(id);
    }

    Rect ViewRect()
    {
        return ViewRect(Vector2.zero);
    }
    Rect ViewRect(Vector2 offset)
    {
        Camera mainCamera = Camera.main;
        Transform cameraTransform = mainCamera.transform;
        float h = mainCamera.orthographicSize * 2;
        float w = mainCamera.aspect * h;
        return new Rect(cameraTransform.position.x - offset.x - w/2, cameraTransform.position.y - offset.y - h/2, w, h);
    }

    IntRect GetTiles(Rect viewRect)
    {
        int x = Mathf.FloorToInt(viewRect.x);
        if (x < 0) x = 0;
        int y = Mathf.FloorToInt(viewRect.y);
        if (y < 0) y = 0;
        int w = Mathf.FloorToInt(viewRect.width) + 2;
        if (x + w > 99) w = 100 - x;
        int h = Mathf.FloorToInt(viewRect.height) + 2;
        if (y + h > 99) h = 100 - y;
        return new IntRect(x, y, w, h);
    }

    void SaveImange(Image image)
    {
        byte[] bytes = image.tex.EncodeToPNG();
        string path = Application.dataPath + "/images";
        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);
        path = path + "/" + image.id.ToString("d3") + ".jpg";
        System.IO.File.WriteAllBytes(path, bytes);
        Debug.Log("Save image to: " + path);
    }

    void DeleteImage(int id)
    {
        string filename = Application.dataPath + "/images/" + id.ToString("d3") + ".jpg";
        if (System.IO.File.Exists(filename))
            System.IO.File.Delete(filename);
    }

    void DeleteImageDirectory()
    {
        string path = Application.dataPath + "/images";
        if (System.IO.Directory.Exists(path))
        {
            foreach (string fileName in System.IO.Directory.GetFiles(path))
                System.IO.File.Delete(fileName);
        }
    }

    void OnApplicationQuit()
    {
        DeleteImageDirectory();
        foreach(Image image in DownLoadImage.GetTexture(Tile.SaveData()))
        {
            SaveImange(image);
        }
    }
}
