using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Object2DApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadObjects(string environmentId)
    {
        string route = "/environments/" + environmentId + "/objects";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequest(route);
        return ParseObjectListResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateObject(Object obj)
    {
        string route = "/environments/" + obj.environmentId + "/objects";
        string data = JsonUtility.ToJson(obj);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequest(route, data);
        return ParseObjectResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> UpdateObject(Object obj)
    {
        string route = "/environments/" + obj.environmentId + "/objects/" + obj.id;
        string data = JsonUtility.ToJson(obj);

        return await webClient.SendPutRequest(route, data);
    }

    private IWebRequestReponse ParseObjectResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                Object obj = JsonUtility.FromJson<Object>(data.Data);
                WebRequestData<Object> parsedWebRequestData = new WebRequestData<Object>(obj);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    private IWebRequestReponse ParseObjectListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                List<Object> objects = JsonHelper.ParseJsonArray<Object>(data.Data);
                WebRequestData<List<Object>> parsedData = new WebRequestData<List<Object>>(objects);
                return parsedData;
            default:
                return webRequestResponse;
        }
    }
}
