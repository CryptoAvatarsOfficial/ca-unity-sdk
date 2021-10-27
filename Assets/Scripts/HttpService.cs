﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class HttpService
{

    private readonly string apiKey;
    private readonly string baseUri;

    public HttpService(string apiKey, string baseUri)
    {
        this.apiKey = apiKey;
        this.baseUri = baseUri;
    }

    public IEnumerator Post<T>(string resource, T body, System.Action<string> callbackResult)
    {
        string json = JsonUtility.ToJson(body);
        UnityWebRequest request = new UnityWebRequest(this.baseUri + resource, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        return this.HttpMethod(request, callbackResult);
    }

    public IEnumerator Get(string resource, System.Action<string> callbackResult)
    {
        UnityWebRequest request = UnityWebRequest.Get(this.baseUri + resource);
        return this.HttpMethod(request, callbackResult);
    }

    // Pensar en refactorizar este método para que utilice HttpMethod y evitar duplicar código
    public IEnumerator GetTexture(string url, System.Action<Texture2D> callbackResult)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            throw new System.Exception("Error requesting to " + request.url + ", error: " + request.error);

        Texture2D tex = ((DownloadHandlerTexture) request.downloadHandler).texture;
        callbackResult(tex);
    }

    private IEnumerator HttpMethod(UnityWebRequest request, System.Action<string> callbackResult)
    {
        request.SetRequestHeader("API-KEY", apiKey);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.isNetworkError)
            throw new System.Exception("Error requesting to " + request.url + ", error: " + request.error);

        callbackResult(request.downloadHandler.text);
    }

}
