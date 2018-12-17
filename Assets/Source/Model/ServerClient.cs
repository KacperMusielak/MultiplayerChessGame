using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

public class ServerClient{

    public string clientName;
    public TcpClient tcp;
    public bool isHost;

    public ServerClient(TcpClient tcp)
    {
        this.tcp = tcp;
    }
}
