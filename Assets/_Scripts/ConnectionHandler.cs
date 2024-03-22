using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

public class ConnectionHandler : MonoBehaviour
{
    static string hostUrl = "www.google.com";

    private static async Task<bool> PingAsync()
    {
        System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();

        PingReply result = await ping.SendPingAsync(hostUrl);
        return result.Status == IPStatus.Success;
    }


    private async void TestPing()
    {
        bool result = await PingAsync();
        print(result);
    }

    private void Start()
    {
        TestPing();
    }
}
