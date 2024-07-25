// Ignore Spelling: env

namespace BulkyBook.Utility
{
    public static class PhotoManager
    {
        private static readonly string[] _permittedExtensions = { ".jpg", ".jpeg", ".png" };

        public static bool IsValidFile(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return file.Length > 0 && file.Length <= 1048576 && _permittedExtensions.Contains(extension);
        }

        public static async Task<string> UploadFileAsync(IFormFile file, string PathName, IWebHostEnvironment env)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string finalPath = Path.Combine(env.WebRootPath, PathName, fileName);

            if (!Directory.Exists(Path.Combine(env.WebRootPath, PathName)))
                Directory.CreateDirectory(Path.Combine(env.WebRootPath, PathName));

            using (var fileStream = new FileStream(finalPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Path.Combine("/", PathName, fileName).Replace("\\", "/");
        }

        public static void DeleteFile(string filePath, IWebHostEnvironment env)
        {
            var fullPath = Path.Combine(env.WebRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
