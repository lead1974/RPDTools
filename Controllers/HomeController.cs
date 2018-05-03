using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using RPDTools.Models;
using RPDTools.Services;

namespace RPDTools.Controllers
{
    [EnableCors("CorsPolicy")]
    public class HomeController : Controller
    {
        private IHostingEnvironment _env;
        private IConfiguration _config;

        public HomeController(IHostingEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }
        [HttpPost]
        public IActionResult Upload(IEnumerable<IFormFile> files)
        {
            //string url = "http://dvbi03.venturafoods.com:9502/bi-lcm/v1/si/ssi/rpd/uploadrp";

            IEnumerable<string> fileInfo = new List<string>();           
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            foreach (var file in files)
            {
                {
                    var fileName = Path.GetFileName(file.FileName); 
                    if (file.Length > 0)
                    {
                        var filePath = Path.Combine(uploads, fileName);//.Replace(@"\\", @"\");
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                    }
                    var pfileName = string.Format("-f {0}", Path.Combine(uploads, fileName)); //.Replace(@"\", "/");
                    var python_params = pfileName;
                    var res = new RunPython().Run(Path.GetFullPath("Python/obiee_rpd_upload.py"), python_params);
                    ViewBag.Result = res;


                    //var handler = new HttpClientHandler
                    //{
                    //    Credentials = new CredentialCache { { new System.Uri(url), "NTLM", new NetworkCredential("biadmin", "monkey_admin", "http://dvbi03.venturafoods.com:9502") } },
                    //    PreAuthenticate = true
                    //};
                    //using (var client = new HttpClient(handler))
                    //{                        
                    //    MultipartFormDataContent form = new MultipartFormDataContent();
                    //    form.Add(new StringContent("weblogic"), "username");
                    //    form.Add(new StringContent("biadmin12cdv"), "password");
                    //    byte[] filebytearraystring = fileToByteArray(fileName);
                    //    form.Add(new ByteArrayContent(filebytearraystring, 0, filebytearraystring.Length), "RPD", fileName);
                    //    HttpResponseMessage response = client.PostAsync(url, form).Result;

                    //    response.EnsureSuccessStatusCode();
                    //    client.Dispose();
                    //    string sd = response.Content.ReadAsStringAsync().Result;
                    //}

                }
                
            }
            return View("Index");
        }

        private byte[] fileToByteArray(string fullFilePath)
        {
            FileStream fs = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();
            return data;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetFileName(IEnumerable<IFormFile> files)
        {
            string fileName = string.Empty;

            foreach (var file in files)
            {
                var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                fileName = fileContent.FileName.ToString().Trim();
            }

            return fileName;

        }
    }
}