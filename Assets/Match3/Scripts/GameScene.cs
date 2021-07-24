using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    [Header("Background")]
    public Material blurMaterial;

    public Camera bgRenderCamera;

    private GameObject ObjPoolGamePlaying;

    [Header("Prefab Pool")]
    public GameObject PrefabGamePlayingPool;

    public JewelPackManager gemPackManager;


    public void Start()
    {
        ObjPoolGamePlaying = Instantiate(PrefabGamePlayingPool);
        var poolPlaying = ObjPoolGamePlaying.GetComponent<PathologicalGames.SpawnPool>();
        if (poolPlaying)
        {
            poolPlaying.Initialize();
        }

        GameMain.main.StartProto(MapData.main.gid);

        var bgRenderer = GameMain.main.bgRenderer;
     
        Texture bgTextu = DecorBlurImageManager.ApplyBlur(blurMaterial);    
        bgRenderer.material.mainTexture = bgTextu;
        bgRenderer.material.color = new Color(0.8f, 0.8f, 0.8f, 1f);

        Vector2 cameraSize = new Vector2(bgRenderCamera.orthographicSize * 2f * bgRenderCamera.aspect, bgRenderCamera.orthographicSize * 2f);
        bgRenderer.transform.localScale = new Vector3(cameraSize.x, cameraSize.y, 1f);
        Vector3 cameraPosition = bgRenderCamera.transform.position;
        Vector3 bgPosition = bgRenderer.transform.position;
        bgRenderer.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, bgPosition.z);
        bgRenderer.sortingLayerName = "Default";
        bgRenderer.sortingOrder = -100;
    }
}
