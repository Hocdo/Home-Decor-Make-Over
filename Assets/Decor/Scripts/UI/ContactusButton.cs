using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactusButton : MonoBehaviour
{
    [SerializeField] GameObject add500CoinObject;
    [SerializeField] GameObject contactObject;
    [SerializeField] CollectParticles collectParticles;
    // Start is called before the first frame update
    void Start()
    {
        StatusUpdate();
    }
    public void StatusUpdate()
    {
        if (!PlayerPrefs.HasKey("LikeFanpage"))
        {
            add500CoinObject.gameObject.SetActive(true);
            contactObject.gameObject.SetActive(false);
        }
        else
        {
            add500CoinObject.gameObject.SetActive(false);
            contactObject.gameObject.SetActive(true);
        }
    }
    public void PlayCollectCoin()
    {
        if (!PlayerPrefs.HasKey("LikeFanpage"))
        {
            StartCoroutine(CoinCollect());
            PlayerPrefs.SetInt("LikeFanpage", 1);
        }
    }

    IEnumerator CoinCollect()
    {
        yield return new WaitForSeconds(0.25f);
        PlayerData.current.AddCoin(500);
        collectParticles.Play();    
        yield return new WaitForSeconds(0.5f);
        EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.CoinAnimChange, new int[] { PlayerData.current.cointCount, 1000 });
        StatusUpdate();
    }
}
