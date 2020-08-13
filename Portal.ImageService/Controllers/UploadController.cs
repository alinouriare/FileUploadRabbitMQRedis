using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Portal.ImageService.Data;
using RabbitMQ.Client;

namespace Portal.ImageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly ILogger<UploadController> _logger;
        private readonly ImageDbContext _db;

        public UploadController(ILogger<UploadController> logger, ImageDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpPost("Uo")]
       
        public async Task<IActionResult> PostFiles([FromForm]IFormFile file)
        {
            
            var img = new Image
            {
                Id = new Guid(),
                Full =await file.GetBytes(),
                TimeCreated = DateTime.Now
            };

            _db.Images.Add(img);
            await _db.SaveChangesAsync();

            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();


            channel.QueueDeclare(queue: "image_crop",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string msg = img.Id.ToString();
            var body = Encoding.UTF8.GetBytes(msg);

            channel.BasicPublish(exchange: "",
                 routingKey: "image_crop",
                 basicProperties: null,
                 body: body);

            return Ok(new { Success = true });

        }
       

    }
    public static class FormFileExtensions
    {
        public static async Task<byte[]> GetBytes(this IFormFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
