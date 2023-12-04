// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;

public class WorldTour : MonoBehaviour
{
    public GameObject City;
    public GameObject AQI;
    public GameObject Poll;

    [SerializeField]
    private MapRenderer _map = null;

    private void Awake()
    {
        Debug.Assert(_map != null);
    }

    public void Start()
    {
        StartCoroutine(RunTour());
    }

    private IEnumerator RunTour()
    {
        Debug.Log("Starting tour");
        GameObject Lat = GameObject.Find("Lat");
        string latcoord = Lat.GetComponent<InputField>().text;
        GameObject Long = GameObject.Find("Long");
        string longcoord = Long.GetComponent<InputField>().text;
        
        yield return _map.WaitForLoad();

        MapScene scene = new MapSceneOfLocationAndZoomLevel(new LatLon(double.Parse(latcoord), double.Parse(longcoord)), 17.0f);
        
        using (UnityWebRequest request = UnityWebRequest.Get(String.Format("https://api.waqi.info/feed/geo:" + latcoord + ";" + longcoord + "/?token=37d6f8b0ad4eab5af5f8278365d84f7bedadd080")))
        {
          yield return request.SendWebRequest();
          if (request.result == UnityWebRequest.Result.ConnectionError)
          {
            Debug.Log(request.error);
          }
          else
          {
          JSONNode itemsData = JSON.Parse(request.downloadHandler.text);
          City.GetComponent<TMP_Text>().text = itemsData["data"]["city"]["name"];
          AQI.GetComponent<TMP_Text>().text = itemsData["data"]["aqi"];
          Poll.GetComponent<TMP_Text>().text = itemsData["data"]["dominentpol"];
          // city.text  = itemsData["data"]["city"]["name"];
          }
        }
        yield return _map.SetMapScene(scene);
        yield return _map.WaitForLoad();
        yield return new WaitForSeconds(3.0f);
    }
}
