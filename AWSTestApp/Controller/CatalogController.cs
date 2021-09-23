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
using System.Runtime.Serialization.Json;
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
        //[HttpGet]
        //public async Task<Catalog> Get(int? id)
        //{
        //    Catalog obj = new Catalog();
        //    GetObjectRequest req = new GetObjectRequest();
        //    req.BucketName = "myworkbtk";
        //    req.Key = id + ".json";
        //    var response = await this.S3Client.GetObjectAsync(req);
        //    string responseBody = string.Empty;
        //    Catalog blogObject;
        //    using (Stream responseStream = response.ResponseStream)
        //    using (StreamReader reader = new StreamReader(responseStream))
        //    {
        //        string contentType = response.Headers["Content-Type"];
        //        responseBody = reader.ReadToEnd();
        //        JavaScriptSerializer js = new JavaScriptSerializer();
        //        blogObject = js.Deserialize<Catalog>(responseBody);
        //    }
        //    return blogObject;
        //}


        [HttpGet]
        public async Task<List<Catalog>> Get()
        {
            Catalog obj = new Catalog();
            GetObjectRequest req = new GetObjectRequest();
            req.BucketName = "myworkbtk";
         //   var response = await this.S3Client.GetObjectAsync(req);
            string responseBody = string.Empty;
            List<Catalog>  lst =new List<Catalog>();
            //using (Stream responseStream = response.ResponseStream)
            //using (StreamReader reader = new StreamReader(responseStream))
            //{
            //    string contentType = response.Headers["Content-Type"];
            //    responseBody = reader.ReadToEnd();
            //    JavaScriptSerializer js = new JavaScriptSerializer();
            //    blogObject = js.Deserialize<List<Catalog>>(responseBody);
            //}

            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = "myworkbtk",
                MaxKeys = 10
            };
            ListObjectsV2Response response = await this.S3Client.ListObjectsV2Async(request);

            foreach (S3Object entry in response.S3Objects)
            {
                Console.WriteLine("key = {0} size = {1}",
                    entry.Key, entry.Size);
                //blogObject.Add(new Catalog {id = entry.Key,name=entry. })
            }

            return lst;
        }

        [HttpPost]
        public async Task<bool> FileUploaad([FromBody] Catalog obj)
        {
            PutObjectResponse response = null;
            try
            {
                var path = @"C:\Users\sagar\source\repos\File\" + obj.id + ".json";
                string json = new JavaScriptSerializer().Serialize(obj);
                //write string to file
                System.IO.File.WriteAllText(path, json);
                using (FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
                {
                    string fileExtension = Path.GetExtension(path);
                    string fileName = string.Empty;
                    fileName = $"{obj.id}{fileExtension}";
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
