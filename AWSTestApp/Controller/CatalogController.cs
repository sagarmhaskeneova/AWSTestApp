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
    public class CatalogController: ControllerBase
    {
        IAmazonS3 S3Client { get; set; }
        public CatalogController(IAmazonS3 s3client)
        {
            this.S3Client = s3client;
        }
       // [Route("[action]/{category}", Name = "GetProductByCategory")]
        [HttpGet]
        //[ProducesResponseType(typeof(IEnumerable<Catalog>), (int)HttpStatusCode.OK)]
        public async Task<List<Catalog>> Get(int? id)
        {
           Catalog obj = new Catalog();
           GetObjectRequest req = new GetObjectRequest();
            req.BucketName = "myworkbtk";
            req.Key = id+".json";
            var response = await this.S3Client.GetObjectAsync(req);
            string responseBody = string.Empty;
            List<Catalog> blogObject;
            using (Stream responseStream = response.ResponseStream)
               
            using (StreamReader reader = new StreamReader(responseStream))
            {
                string contentType = response.Headers["Content-Type"];
                responseBody = reader.ReadToEnd();
                //blogObject = JsonConvert.DeserializeObject<dynamic>(responseBody);

                JavaScriptSerializer js = new JavaScriptSerializer();
                 blogObject = js.Deserialize<List<Catalog>>(responseBody);

                // var o = JsonConvert.DeserializeObject<JObject>(responseBody);
                //obj.id = blogObject.id;
                //obj.name = blogObject.name;
            }
           
            return blogObject;
        }
        
    }
}
