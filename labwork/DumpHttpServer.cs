using System.Collections.Specialized;
using System.Net;
using System.Web;
using RazorEngine;
using RazorEngine.Templating;
using Encoding = System.Text.Encoding;

namespace labwork;

public class DumpHttpServer
{
    private string _siteDirectory; 
    private HttpListener _listener; 
    private int _port; 
    private Cat _cat;

    public async Task RunAsync(string path, int port)
    {
        _siteDirectory = path;
        _port = port;
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://localhost:" + _port.ToString() + "/");
        _listener.Start();
        Console.WriteLine($"Сервер запущен на порту: {port}");
        Console.WriteLine($"Файлы сайта лежат в папке: {path}");
        await ListenAsync();
    }
    
    public void Stop()
    {
        _listener.Abort();
        _listener.Stop();
    }
    
    private async Task ListenAsync()
    {
        try
        {
            while (true)
            {
                HttpListenerContext context = await _listener.GetContextAsync();
                Process(context);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    private void Process(HttpListenerContext context)
    {
       string filename = context.Request.Url.AbsolutePath;
       Console.WriteLine(filename);
       filename = filename.Substring(1);
       filename = Path.Combine(_siteDirectory, filename);
       
       if (File.Exists(filename))
       {
           try
           {
               if (context.Request.HttpMethod == "POST")
               {
                   using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                   { 
                       string requestData = reader.ReadToEnd();
                       string catName = HttpUtility.ParseQueryString(requestData)["catName"];
                       string redirectUrl = $"http://localhost:8888/cat_stats.html?catName={catName}";
                       _cat = new Cat(catName);
                       context.Response.Redirect(redirectUrl);
                   }
               }
               if (context.Request.HttpMethod == "GET")
               {
                   NameValueCollection queryParams = context.Request.QueryString;
                   string action = queryParams["action"];

                   if (!string.IsNullOrEmpty(action))
                   {
                       HandleCatAction(action);
                       context.Response.Redirect("/cat_stats.html");
                   }
               }
               
               NameValueCollection query = context.Request.QueryString;
               string content = "";
               if (filename.Contains("html"))
               {
                   content = BuildHtml(filename, query["arg"]);
               }
               else
               {
                   content = File.ReadAllText(filename);
               }

               byte[] htmlBytes = Encoding.UTF8.GetBytes(content);
               Stream fileStream = new MemoryStream(htmlBytes);
               context.Response.ContentType = GetContentType(filename);
               context.Response.ContentLength64 = fileStream.Length;
               byte[] buffer = new byte[16 * 1024]; 
               int dataLength;
               do
               {
                   dataLength = fileStream.Read(buffer, 0, buffer.Length);
                   context.Response.OutputStream.Write(buffer, 0, dataLength);
               } while (dataLength > 0);
               fileStream.Close();
               context.Response.StatusCode = (int)HttpStatusCode.OK;
               context.Response.OutputStream.Flush();
           }
           catch (Exception e)
           {
               Console.WriteLine(e.Message);
               context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
           }
       }
       else
       {
           MakeNotImplemented(context);
       }
       context.Response.OutputStream.Close();
    }
    
    private static string GetContentType(string filename)
    {
       var dictionary = new Dictionary<string, string>
       {
           { ".css", "text/css" },
           { ".html", "text/html; charset=utf-8" }, // Для html еще укажем кодировку
           { ".ico", "image/x-icon" },
           { ".js", "application/x-javascript" },
           { ".json", "application/json" },
           { ".png", "image/png" }
       };
       string contentType = "";
       string fileExtension = Path.GetExtension(filename);
       dictionary.TryGetValue(fileExtension, out contentType);
       return contentType;
    }

    private string BuildHtml(string filename, string userstring)
    {
        string html = "";
        string layoutPath = "../../../site/layout.html";

        var razorService = Engine.Razor;
        if (!razorService.IsTemplateCached("layout", null))
        {
            razorService.AddTemplate("layout", File.ReadAllText(layoutPath));
        }

        if (!razorService.IsTemplateCached(filename, null))
        {
            razorService.AddTemplate(filename, File.ReadAllText(filename));
            razorService.Compile(filename);
        }
        
        html = razorService.Run(filename, null, new
        {
            Cat = _cat
        });
        return html;
    }
    
    private void MakeNotImplemented(HttpListenerContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        byte[] buffer = Encoding.UTF8.GetBytes("<html><head><title>404 Not Found</title></head><body><h1>404 Not Found</h1></body></html>");
        context.Response.ContentType = "text/html";
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.OutputStream.Flush();
    }
    private void HandleCatAction(string action)
    {
        switch (action)
        {
            case "feed":
                _cat.Feed();
                break;
            case "play":
                _cat.Play();
                break;
            case "sleep":
                _cat.Sleep();
                break;
            case "heal":
                _cat.Heal();
                break;
            default:
                Console.WriteLine("Error");
                break;
        }
    }

}