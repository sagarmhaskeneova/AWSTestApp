using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AWSTestApp.Controller
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        IAmazonS3 S3Client { get; set; }
        public CatalogController(IAmazonS3 s3client)
        {
            this.S3Client = s3client;
        }
        [HttpGet]
        public async Task<List<Catalog>> Get(int? id)
        {
            Catalog obj = new Catalog();
            GetObjectRequest req = new GetObjectRequest();
            req.BucketName = "myworkbtk";
            req.Key = id + ".json";
            var response = await this.S3Client.GetObjectAsync(req);
            string responseBody = string.Empty;
            List<Catalog> blogObject;
            using (Stream responseStream = response.ResponseStream)
            using (StreamReader reader = new StreamReader(responseStream))
            {
                string contentType = response.Headers["Content-Type"];
                responseBody = reader.ReadToEnd();
                JavaScriptSerializer js = new JavaScriptSerializer();
                blogObject = js.Deserialize<List<Catalog>>(responseBody);
            }
            return blogObject;
        }
        [HttpPost]
        public async Task<bool> FileUploaad(int uploadFileName)
        {
            PutObjectResponse response = null;
            try
            {
                //var path = Path.Combine("Files", uploadFileName.ToString() + ".json");
                var path = @"C:\Users\sagar\source\repos\File\" + uploadFileName + ".json";
                using (FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    string fileExtension = Path.GetExtension(path);
                    string fileName = string.Empty;
                    fileName = $"{DateTime.Now.Ticks}{fileExtension}";

                    PutObjectRequest request = new PutObjectRequest()
                    {
                        InputStream = fsSource,
                        BucketName = "myworkbtk",
                        Key = fileName
                    };
                    response = await this.S3Client.PutObjectAsync(request);

                }
            }
            catch (Exception ex)
            {

            }
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                return true;
            else
                return false;
        }

        [HttpDelete]
        public async Task<bool> Delete(string key)
        {
            DeleteObjectResponse response = null;
            try
            {
                key = key + ".json";
                 response = await this.S3Client.DeleteObjectAsync("myworkbtk", key);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                return true;
            else
                return false;
        }
    }
}
