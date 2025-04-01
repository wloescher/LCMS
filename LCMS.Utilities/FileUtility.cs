using Microsoft.AspNetCore.Http;

namespace LCMS.Utilities
{
    public static class FileUtility
    {
        private static readonly string[] _permittedExtensions = { ".jpg", ".pdf" };

        private static readonly Dictionary<string, List<byte[]>> _fileSignature = new Dictionary<string, List<byte[]>>
                {
                    { ".jpeg", new List<byte[]>
                        {
                            new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                            new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                            new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                        }
                    },
                };

        #region Public Methods

        /// <summary>
        /// Check if the file extension is invalid.
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns>True/False</returns>
        public static bool HasInvalidFileExtension(string fileExtension)
        {
            return string.IsNullOrEmpty(fileExtension) || !_permittedExtensions.Contains(fileExtension);
        }

        /// <summary>
        /// Check if the file signature is invalid.
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <param name="fileStream"></param>
        /// <returns>True/False</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool HasInvalidFileSignature(string fileExtension, Stream fileStream)
        {
            // TODO: Add support for PDF file signature validation
            if (fileExtension == ".pdf") return false;

            // Check for supported file extension
            var supportedFileExtension = string.Empty;
            switch (fileExtension)
            {
                case ".jpg":
                case ".jpeg":
                    supportedFileExtension = ".jpeg";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("File Signature Validation not supported for '(0)' extension.", fileExtension));
            }

            // Validate file signature
            using (var reader = new BinaryReader(fileStream))
            {
                var signatures = _fileSignature[fileExtension];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
                return !signatures.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature));
            }
        }

        /// <summary>
        /// Check if the file size exceeds the limit.
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="fileSizeLimitMb"></param>
        /// <returns>True/False</returns>
        public static bool ExceedsFileSizeLimit(Stream fileStream, int fileSizeLimitMb)
        {
            return fileStream.Length > fileSizeLimitMb * 1000000;
        }

        /// <summary>
        /// Upload file to the specified directory.
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="fileDirectory"></param>
        /// <param name="fileName"></param>
        public static void UploadFile(IFormFile formFile, string fileDirectory, string fileName = "")
        {
            if (formFile == null) return;

            // Check for directory
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            // Save file
            var filePath = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}{1}", fileDirectory, string.IsNullOrEmpty(fileName) ? formFile.FileName : fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                formFile.CopyTo(stream);
            }
        }

        /// <summary>
        /// Append content to the specified file.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="path"></param>
        public static void AppendToFile(string content, string path)
        {
            using (var streamWriter = System.IO.File.AppendText(path))
            {
                streamWriter.WriteLine(content);
            }
        }

        #endregion
    }
}
