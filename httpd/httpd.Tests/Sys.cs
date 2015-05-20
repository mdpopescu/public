using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace httpd.Tests
{
  public static class Sys
  {
    public static int FindAvailablePort()
    {
      var listener = new TcpListener(IPAddress.Loopback, 0);
      listener.Start();
      var port = ((IPEndPoint) listener.LocalEndpoint).Port;
      listener.Stop();

      return port;
    }

    public static Process Run(string fileName, string args)
    {
      var startInfo = new ProcessStartInfo(fileName, args);
      var process = new Process { StartInfo = startInfo };
      process.Start();

      return process;
    }
  }
}