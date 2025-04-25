using Microsoft.AspNetCore.Mvc;

namespace LCMS.Blazor.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UploadController(IWebHostEnvironment hostingEnvironment) : ControllerBase
    {
        public IWebHostEnvironment HostingEnvironment { get; set; } = hostingEnvironment;

        [HttpPost]
        public async Task<IActionResult> Save(IFormFile files) // "files" matches the Upload SaveField value
        {
            if (files != null)
            {
                try
                {
                    var rootPath = HostingEnvironment.WebRootPath; // save to wwwroot/uploads - Blazor Server only
                    var uploadPath = string.Format("{0}\\uploads", rootPath);
                    var saveLocation = Path.Combine(uploadPath, files.FileName);

                    using (var fileStream = new FileStream(saveLocation, FileMode.Create))
                    {
                        await files.CopyToAsync(fileStream);

                        Response.StatusCode = 201;
                        await Response.WriteAsync("Upload successful.");
                    }
                }
                catch (Exception ex)
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync($"Upload failed. Exception: {ex.Message}");
                }
            }

            return new EmptyResult();
        }

        [HttpPost]
        public async Task<IActionResult> Remove([FromForm] string files) // "files" matches the Upload RemoveField value
        {
            if (files != null)
            {
                try
                {
                    var rootPath = HostingEnvironment.WebRootPath; // delete from wwwroot/uploads - Blazor Server only
                    var uploadPath = string.Format("{0}\\uploads", rootPath);
                    var fileLocation = Path.Combine(uploadPath, files);

                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);

                        Response.StatusCode = 200;
                        await Response.WriteAsync($"Delete successful.");
                    }
                }
                catch (Exception ex)
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync($"Delete failed. Exception: {ex.Message}");
                }
            }

            return new EmptyResult();
        }
    }
}
