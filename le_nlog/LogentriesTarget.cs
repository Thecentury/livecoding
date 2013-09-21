// Type: NLog.Targets.LogentriesTarget
// Assembly: le_nlog, Version=2.1.8.0, Culture=neutral, PublicKeyToken=null
// MVID: A0C8B904-D7BC-41A4-8681-F066372CD65F
// Assembly location: C:\Dev\LiveCoding\packages\le_nlog.2.2.0\lib\Net40\le_nlog.dll

using NLog;
using NLog.Common;
using NLog.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace NLog.Targets
{
  [Target("Logentries")]
  public class LogentriesTarget : TargetWithLayout
  {
    private static readonly UTF8Encoding UTF8 = new UTF8Encoding();
    private static readonly ASCIIEncoding ASCII = new ASCIIEncoding();
    private static readonly X509Certificate2 LE_API_CERT = new X509Certificate2(Encoding.UTF8.GetBytes("-----BEGIN CERTIFICATE-----\r\nMIIFSjCCBDKgAwIBAgIDBQMSMA0GCSqGSIb3DQEBBQUAMGExCzAJBgNVBAYTAlVT\r\nMRYwFAYDVQQKEw1HZW9UcnVzdCBJbmMuMR0wGwYDVQQLExREb21haW4gVmFsaWRh\r\ndGVkIFNTTDEbMBkGA1UEAxMSR2VvVHJ1c3QgRFYgU1NMIENBMB4XDTEyMDkxMDE5\r\nNTI1N1oXDTE2MDkxMTIxMjgyOFowgcExKTAnBgNVBAUTIEpxd2ViV3RxdzZNblVM\r\nek1pSzNiL21hdktiWjd4bEdjMRMwEQYDVQQLEwpHVDAzOTM4NjcwMTEwLwYDVQQL\r\nEyhTZWUgd3d3Lmdlb3RydXN0LmNvbS9yZXNvdXJjZXMvY3BzIChjKTEyMS8wLQYD\r\nVQQLEyZEb21haW4gQ29udHJvbCBWYWxpZGF0ZWQgLSBRdWlja1NTTChSKTEbMBkG\r\nA1UEAxMSYXBpLmxvZ2VudHJpZXMuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8A\r\nMIIBCgKCAQEAxcmFqgE2p6+N9lM2GJhe8bNUO0qmcw8oHUVrsneeVA66hj+qKPoJ\r\nAhGKxC0K9JFMyIzgPu6FvuVLahFZwv2wkbjXKZLIOAC4o6tuVb4oOOUBrmpvzGtL\r\nkKVN+sip1U7tlInGjtCfTMWNiwC4G9+GvJ7xORgDpaAZJUmK+4pAfG8j6raWgPGl\r\nJXo2hRtOUwmBBkCPqCZQ1mRETDT6tBuSAoLE1UMlxWvMtXCUzeV78H+2YrIDxn/W\r\nxd+eEvGTSXRb/Q2YQBMqv8QpAlarcda3WMWj8pkS38awyBM47GddwVYBn5ZLEu/P\r\nDiRQGSmLQyFuk5GUdApSyFETPL6p9MfV4wIDAQABo4IBqDCCAaQwHwYDVR0jBBgw\r\nFoAUjPTZkwpHvACgSs5LdW6gtrCyfvwwDgYDVR0PAQH/BAQDAgWgMB0GA1UdJQQW\r\nMBQGCCsGAQUFBwMBBggrBgEFBQcDAjAdBgNVHREEFjAUghJhcGkubG9nZW50cmll\r\ncy5jb20wQQYDVR0fBDowODA2oDSgMoYwaHR0cDovL2d0c3NsZHYtY3JsLmdlb3Ry\r\ndXN0LmNvbS9jcmxzL2d0c3NsZHYuY3JsMB0GA1UdDgQWBBRaMeKDGSFaz8Kvj+To\r\nj7eMOtT/zTAMBgNVHRMBAf8EAjAAMHUGCCsGAQUFBwEBBGkwZzAsBggrBgEFBQcw\r\nAYYgaHR0cDovL2d0c3NsZHYtb2NzcC5nZW90cnVzdC5jb20wNwYIKwYBBQUHMAKG\r\nK2h0dHA6Ly9ndHNzbGR2LWFpYS5nZW90cnVzdC5jb20vZ3Rzc2xkdi5jcnQwTAYD\r\nVR0gBEUwQzBBBgpghkgBhvhFAQc2MDMwMQYIKwYBBQUHAgEWJWh0dHA6Ly93d3cu\r\nZ2VvdHJ1c3QuY29tL3Jlc291cmNlcy9jcHMwDQYJKoZIhvcNAQEFBQADggEBAAo0\r\nrOkIeIDrhDYN8o95+6Y0QhVCbcP2GcoeTWu+ejC6I9gVzPFcwdY6Dj+T8q9I1WeS\r\nVeVMNtwJt26XXGAk1UY9QOklTH3koA99oNY3ARcpqG/QwYcwaLbFrB1/JkCGcK1+\r\nAg3GE3dIzAGfRXq8fC9SrKia+PCdDgNIAFqe+kpa685voTTJ9xXvNh7oDoVM2aip\r\nv1xy+6OfZyGudXhXag82LOfiUgU7hp+RfyUG2KXhIRzhMtDOHpyBjGnVLB0bGYcC\r\n566Nbe7Alh38TT7upl/O5lA29EoSkngtUWhUnzyqYmEMpay8yZIV4R9AuUk2Y4HB\r\nkAuBvDPPm+C0/M4RLYs=\r\n-----END CERTIFICATE-----"));
    private static char[] trimChars = new char[2]
    {
      '\r',
      '\n'
    };
    private static string[] posix_newline = new string[2]
    {
      "\r\n",
      "\n"
    };
    private static string line_separator = "\x2028";
    private static Regex isGuid = new Regex("^(\\{){0,1}[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}(\\}){0,1}$", RegexOptions.Compiled);
    public const string VERSION = "2.1.6";
    private const int QUEUE_SIZE = 32768;
    private const string LE_API = "api.logentries.com";
    private const int LE_TOKEN_PORT = 10000;
    private const int LE_TOKEN_TLS_PORT = 20000;
    private const int LE_HTTP_PORT = 80;
    private const int LE_HTTP_SSL_PORT = 443;
    private const int MIN_DELAY = 100;
    private const int MAX_DELAY = 10000;
    private const string LE = "LE: ";
    private const string CONFIG_TOKEN = "LOGENTRIES_TOKEN";
    private const string CONFIG_ACCOUNT_KEY = "LOGENTRIES_ACCOUNT_KEY";
    private const string CONFIG_LOCATION = "LOGENTRIES_LOCATION";
    private const string INVALID_TOKEN = "\n\nIt appears your LOGENTRIES_TOKEN parameter in web/app.config is invalid!\n\n";
    private const string INVALID_HTTP_PUT = "\n\nIt appears your LOGENTRIES_ACCOUNT_KEY or LOGENTRIES_LOCATION parameters in web/app.config are invalid!\n\n";
    private const string QUEUE_OVERFLOW = "\n\nLogentries Buffer Queue Overflow. Message Dropped!\n\n";
    private readonly Random random = new Random();
    private LogentriesTarget.LogentriesTcpClient tcp_client;
    public Thread thread;
    public bool started;
    private string m_Token;
    private string m_Key;
    private string m_Location;
    private bool m_HttpPut;
    private bool m_Ssl;
    public BlockingCollection<string> queue;

    [RequiredParameter]
    public bool Debug { get; set; }

    public string Token
    {
      get
      {
        return this.m_Token;
      }
      set
      {
        this.m_Token = value;
      }
    }

    public bool HttpPut
    {
      get
      {
        return this.m_HttpPut;
      }
      set
      {
        this.m_HttpPut = value;
      }
    }

    public bool Ssl
    {
      get
      {
        return this.m_Ssl;
      }
      set
      {
        this.m_Ssl = value;
      }
    }

    public string Key
    {
      get
      {
        return this.m_Key;
      }
      set
      {
        this.m_Key = value;
      }
    }

    public string Location
    {
      get
      {
        return this.m_Location;
      }
      set
      {
        this.m_Location = value;
      }
    }

    public bool KeepConnection { get; set; }

    static LogentriesTarget()
    {
    }

    public LogentriesTarget()
    {
      this.queue = new BlockingCollection<string>(32768);
      this.thread = new Thread(new ThreadStart(this.run_loop));
      this.thread.Name = "Logentries NLog Target";
      this.thread.IsBackground = true;
    }

	  protected override void InitializeTarget()
	  {
		  base.InitializeTarget();
	  }

	  private void openConnection()
    {
      try
      {
        if (this.tcp_client == null)
          this.tcp_client = new LogentriesTarget.LogentriesTcpClient(this.HttpPut, this.Ssl);
        this.tcp_client.Connect();
        if (!this.HttpPut)
          return;
        string s = string.Format("PUT /{0}/hosts/{1}/?realtime=1 HTTP/1.1\r\n\r\n", (object) this.m_Key, (object) this.m_Location);
        this.tcp_client.Write(LogentriesTarget.ASCII.GetBytes(s), 0, s.Length);
      }
      catch
      {
        throw new IOException();
      }
    }

    private void reopenConnection()
    {
      this.closeConnection();
      int maxValue = 100;
      while (true)
      {
        try
        {
          this.openConnection();
          break;
        }
        catch (Exception ex)
        {
          if (this.Debug)
            this.WriteDebugMessages("Unable to connect to Logentries", ex);
        }
        maxValue *= 2;
        if (maxValue > 10000)
          maxValue = 10000;
        int millisecondsTimeout = maxValue + this.random.Next(maxValue);
        try
        {
          Thread.Sleep(millisecondsTimeout);
        }
        catch
        {
          throw new ThreadInterruptedException();
        }
      }
    }

    private void closeConnection()
    {
      if (this.tcp_client == null)
        return;
      this.tcp_client.Close();
    }

    public void run_loop()
    {
      try
      {
        this.reopenConnection();
label_8:
        while (true)
        {
          bool flag = true;
          string str = this.queue.Take();
          foreach (string oldValue in LogentriesTarget.posix_newline)
            str = str.Replace(oldValue, LogentriesTarget.line_separator);
          string s = (string) (!this.HttpPut ? (object) (this.Token + str) : (object) str) + (object) '\n';
          byte[] bytes = LogentriesTarget.UTF8.GetBytes(s);
          while (true)
          {
            flag = true;
            try
            {
              this.tcp_client.Write(bytes, 0, bytes.Length);
              goto label_8;
            }
            catch (IOException ex)
            {
              this.WriteDebugMessages("Logentries encountered an error when writing to the TCP client stream.", (Exception) ex);
              this.reopenConnection();
            }
          }
        }
      }
      catch (ThreadInterruptedException ex)
      {
        this.WriteDebugMessages("Logentries asynchronous socket interrupted", (Exception) ex);
      }
      this.closeConnection();
    }

    private void addLine(string line)
    {
      this.WriteDebugMessages("Queueing " + line);
      if (this.queue.TryAdd(line))
        return;
      this.queue.Take();
      if (!this.queue.TryAdd(line))
        this.WriteDebugMessages("\n\nLogentries Buffer Queue Overflow. Message Dropped!\n\n");
    }

    private bool checkCredentials()
    {
      NameValueCollection appSettings = ConfigurationManager.AppSettings;
      if (!this.HttpPut)
      {
        if (this.checkValidUUID(this.m_Token))
          return true;
        if (Enumerable.Contains<string>((IEnumerable<string>) appSettings.AllKeys, "LOGENTRIES_TOKEN") && this.checkValidUUID(appSettings["LOGENTRIES_TOKEN"]))
        {
          this.m_Token = appSettings["LOGENTRIES_TOKEN"];
          return true;
        }
        else
        {
          this.WriteDebugMessages("\n\nIt appears your LOGENTRIES_TOKEN parameter in web/app.config is invalid!\n\n");
          return false;
        }
      }
      else
      {
        if (this.m_Key != "" && this.checkValidUUID(this.m_Key) && this.m_Location != "")
          return true;
        if (Enumerable.Contains<string>((IEnumerable<string>) appSettings.AllKeys, "LOGENTRIES_ACCOUNT_KEY") && this.checkValidUUID(appSettings["LOGENTRIES_ACCOUNT_KEY"]))
        {
          this.m_Key = appSettings["LOGENTRIES_ACCOUNT_KEY"];
          if (Enumerable.Contains<string>((IEnumerable<string>) appSettings.AllKeys, "LOGENTRIES_LOCATION") && appSettings["LOGENTRIES_LOCATION"] != "")
          {
            this.m_Location = appSettings["LOGENTRIES_LOCATION"];
            return true;
          }
        }
        this.WriteDebugMessages("\n\nIt appears your LOGENTRIES_ACCOUNT_KEY or LOGENTRIES_LOCATION parameters in web/app.config are invalid!\n\n");
        return false;
      }
    }

    protected override void Write(LogEventInfo logEvent)
    {
      if (!this.started && this.checkCredentials())
      {
        this.WriteDebugMessages("Starting Logentries asynchronous socket client");
        this.thread.Start();
        this.started = true;
      }
      string line = this.Layout.Render(logEvent).TrimEnd(LogentriesTarget.trimChars);
      try
      {
        if (logEvent.Exception != null)
        {
          string str = ((object) logEvent.Exception).ToString();
          if (str.Length > 0)
          {
            line = line + ", ";
            line = line + str;
          }
        }
      }
      catch
      {
      }
      this.addLine(line);
    }

    protected override void CloseTarget()
    {
      base.CloseTarget();
      this.thread.Interrupt();
    }

    public void TestClose()
    {
      base.CloseTarget();
    }

    private void WriteDebugMessages(string message, Exception e)
    {
      message = "LE: " + message;
      if (!this.Debug)
        return;
      string[] strArray = new string[2]
      {
        message,
        ((object) e).ToString()
      };
      foreach (string str in strArray)
      {
        Console.Error.WriteLine(str);
        InternalLogger.Debug(str);
      }
    }

    private void WriteDebugMessages(string message)
    {
      message = "LE: " + message;
      if (!this.Debug)
        return;
      Console.Error.WriteLine(message);
      InternalLogger.Debug(message);
    }

    private static bool IsGuid(string candidate, out Guid output)
    {
      bool flag = false;
      output = Guid.Empty;
      if (LogentriesTarget.isGuid.IsMatch(candidate))
      {
        output = new Guid(candidate);
        flag = true;
      }
      return flag;
    }

    public bool checkValidUUID(string uuid_input)
    {
      if (uuid_input == "" || uuid_input == null)
        return false;
      Guid output = Guid.NewGuid();
      return LogentriesTarget.IsGuid(uuid_input, out output);
    }

    private class LogentriesTcpClient
    {
      private TcpClient client = (TcpClient) null;
      private Stream stream = (Stream) null;
      private SslStream ssl_stream = (SslStream) null;
      private bool ssl_choice = false;
      private int port;

      public LogentriesTcpClient(bool httpPut, bool ssl)
      {
        this.ssl_choice = ssl;
        if (!ssl)
          this.port = httpPut ? 80 : 10000;
        else
          this.port = httpPut ? 443 : 20000;
      }

      private Stream getTheStream()
      {
        return this.ssl_choice ? (Stream) this.ssl_stream : this.stream;
      }

      public void Connect()
      {
        this.client = new TcpClient("api.logentries.com", this.port);
        this.client.NoDelay = true;
        this.stream = (Stream) this.client.GetStream();
        if (!this.ssl_choice)
          return;
        this.ssl_stream = new SslStream(this.stream, false, (RemoteCertificateValidationCallback) ((sender, cert, chain, errors) => cert.GetCertHashString() == LogentriesTarget.LE_API_CERT.GetCertHashString()));
        this.ssl_stream.AuthenticateAsClient("api.logentries.com");
      }

      public void Write(byte[] buffer, int offset, int count)
      {
        this.getTheStream().Write(buffer, offset, count);
      }

      public void Flush()
      {
        this.getTheStream().Flush();
      }

      public void Close()
      {
        if (this.client == null)
          return;
        try
        {
          this.client.Close();
        }
        catch
        {
        }
      }
    }
  }
}
