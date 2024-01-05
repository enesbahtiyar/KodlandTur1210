using UnityEngine;
using Photon.Pun;

public class MiniMap : MonoBehaviourPunCallbacks
{
    //kaydırma hızı
    [SerializeField] private float scrollSpeed = 1f;
    //minimum zoom seviyesi
    [SerializeField] private float minValue = 10f;
    //maximumu zoom seviyesi
    [SerializeField] private float maxValue = 60f;
    //zoomun güncel değeri
    private float currentValue;

    private void Start()
    {
        if(!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        //inputmanagerda mouse scrollwheel için önceden belirlenen ayarları kullan
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

        if(scrollDelta > 0)
        {
            currentValue += scrollSpeed;
        }
        else if (scrollDelta < 0)
        {
            currentValue -= scrollSpeed;
        }

        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
        gameObject.GetComponent<Camera>().orthographicSize = currentValue;
    }
}
