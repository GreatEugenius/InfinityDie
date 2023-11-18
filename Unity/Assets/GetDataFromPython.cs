using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Text;

public class GetDataFromPython : MonoBehaviour
{
    public int listenPort = 12345;

    public string receivedData;
    public List<float> handData = new List<float>();

    private TcpListener listener;
    private TcpClient client;
    private NetworkStream stream;

    private byte[] buffer = new byte[1024];
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            listener = new TcpListener(IPAddress.Any, listenPort);
            listener.Start();
            Debug.Log("Server is listening on port " + listenPort);
        }
        catch (Exception e)
        {
            Debug.LogError("Error starting server: " + e.Message);
        }
        receivedData = "Waiting";
    }

    // Update is called once per frame
    void Update()
    {
        if (listener == null)
            return;

        if (listener.Pending())
        {
            try
            {
                client = listener.AcceptTcpClient();
                stream = client.GetStream();
                Debug.Log("Client connected: " + client.Client.RemoteEndPoint);
            }
            catch (Exception e)
            {
                Debug.LogError("Error accepting client: " + e.Message);
            }
        }

        if (stream != null && stream.DataAvailable)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    handData = DataProcess(receivedData);
                    Debug.Log(receivedData);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error reading from client: " + e.Message);
            }
        }

    }

    private void OnDestroy()
    {
        if (listener != null)
            listener.Stop();
        if (client != null)
            client.Close();
    }

    public List<float> DataProcess(string data)
    {
        string dataStr = data;

        List<float> dataSet = new List<float>();

        string datapoint = "";

        foreach (char c in dataStr)
        {
            if (char.IsDigit(c) || c == '-' || c == '.')
            {
                datapoint += c;
            }
            else if ((c == ',' || c == ']') && CanConvertTofloat(datapoint))
            {
                dataSet.Add(float.Parse(datapoint));
                datapoint = "";
            }
        }

        return dataSet;
    }

    private bool CanConvertTofloat(string value)
    {
        float result;
        return float.TryParse(value, out result);
    }
}
