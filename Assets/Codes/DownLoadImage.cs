using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DownLoadImage : MonoBehaviour
{
    //static string urlFormatHttp = "http://image6.tuku.cn/pic/sucai/concept_person_199_220/s{0}.jpg";
    static string urlFormatHttp = "http://imgm.photophoto.cn/069/071/043/0710430{0}.jpg";
    static string urlFormatFile = "file://{0}/images/{1}.jpg";
    //已经下载的图片
    static Dictionary<int, Texture2D> loaded = new Dictionary<int, Texture2D>();
    //正在下载队列
    static Dictionary<int, List<DownLoadImage>> loading = new Dictionary<int, List<DownLoadImage>>();
    //static List<int> loadingList = new List<int>();
    static Queue loadingList = new Queue();
    //最大下载任务数量
    static int loadingMax = 3;
    //当前下载数量
    static int loadingCurrent = 0;

    GameObject target { get; set; }
    string furnitureName { get; set; }
    int id { get; set; }
    string url { get; set; }
    
    public static void Load(int id, GameObject target, string functionName, bool local)
    {
        GameObject go = new GameObject();
        DownLoadImage dl = go.AddComponent<DownLoadImage>();
        dl.id = id;
        dl.target = target;
        dl.furnitureName = functionName;
        if (local)
            dl.url = string.Format(urlFormatFile, Application.dataPath, id.ToString("d3"));
        else
            dl.url = string.Format(urlFormatHttp, id.ToString("d3"));
        if (loaded.ContainsKey(id))
        {
            dl.Complete(loaded[id]);
        }
        else
        {
            if (loading.ContainsKey(id))
            {
                loading[id].Add(dl);
            }
            else
            {
                loadingList.Enqueue(id);
                loading[id] = new List<DownLoadImage>();
                loading[id].Add(dl);
                if (loadingCurrent < loadingMax)
                    Next();
            }
        }
        
    }
    void Run()
    {
        StartCoroutine(Process(url));
    }
    void Complete(Texture2D tex)
    {
        if (!loaded.ContainsKey(id))
        {
            loaded[id] = tex;
        }
        try
        {
            if (loading.ContainsKey(id))
            {
                foreach (DownLoadImage dl in loading[id])
                {
                    dl.Send(tex);
                }
            }
            else
            {
                Send(tex);
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError(id + " : " + e.ToString());
        }
        loading.Remove(id);
        loadingCurrent--;
        Next();
    }
    void Send(Texture2D tex)
    {
        if (target)
            target.SendMessage(furnitureName, new Image(id, tex), SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
    IEnumerator Process(string url)
    {
        WWW stream;
        yield return stream = new WWW(url);
        if (stream.error == null)
        {
            Texture2D tex = new Texture2D(4, 4);
            stream.LoadImageIntoTexture(tex);
            Complete(tex);
        }
        else
        {
            Debug.Log("LoadImageError[" + id + "]: " + stream.error);
            Destroy(gameObject);
            Next();
        }
    }
    static void Next()
    {
        if (loadingList.Count > 0)
        {
            loading[(int)loadingList.Dequeue()][0].Run();
            loadingCurrent++;
        }
        else
        {
            Manager.Instance.PreactLoad();
        }
    }

    public static List<Image> GetTexture(List<int> index)
    {
        List<Image> result = new List<Image>();
        foreach (int i in index)
        {
            result.Add(new Image(i, loaded[i]));
        }
        return result;
    }
}
