using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace LCMS.Utilities
{
#pragma warning disable CA1416 // Validate platform compatibility

    public static class ImageUtility
    {
        /// <summary>
        /// Resize an image to fit within a bounding box.
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns>Image object.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Image? GetResizedImage(Image sourceImage, int maxWidth, int maxHeight)
        {
            if (sourceImage == null) { return null; }

            // Check for too small bounding box
            if (maxWidth < 4 || maxHeight < 4)
            {
                throw new InvalidOperationException("Bounding Box of Resize Photo must be larger than 4X4 pixels.");
            }

            // Set original width and height
            var originalWidth = (decimal)sourceImage.Width;
            var originalHeight = (decimal)sourceImage.Height;

            // Initialize desired width and height
            var desiredWidth = (decimal)maxWidth;
            var desiredHeight = (decimal)maxHeight;

            // Check for correctly size image
            if (originalWidth < desiredWidth && originalHeight < desiredHeight)
            {
                // Image fits in bounding box, keep size (center with css) If we made it biger it would stretch the image resulting in loss of quality.
                return sourceImage;
            }

            // Check for double squares
            if (originalWidth == originalHeight && desiredWidth == desiredHeight)
            {
                // Image and bounding box are square, no need to calculate aspects, just downsize it with the bounding box
                var squareImage = new Bitmap(sourceImage, (int)desiredWidth, (int)desiredHeight);
                sourceImage.Dispose();
                return squareImage;
            }

            // Check for square original image
            if (originalWidth == originalHeight)
            {
                // Image is square, bounding box isn't.  Get smallest side of bounding box and resize to a square of that center the image vertically and horizonatally with Css there will be space on one side.
                var smallSide = (int)Math.Min(desiredWidth, desiredHeight);
                var squareImage = new Bitmap(sourceImage, smallSide, smallSide);
                sourceImage.Dispose();
                return squareImage;
            }


            // Check for aspect ratio
            decimal aspectRatio;
            if (originalWidth > desiredWidth && originalHeight > desiredHeight)
            {
                // Two demensions so figure out which bounding box demension is the smallest and which original image demension is the smallest,
                // already know original image is larger than bounding box
                aspectRatio = Math.Min(desiredWidth, desiredHeight) / Math.Min(originalWidth, originalHeight);
            }
            else
            {
                // Check for image wider than bouding box
                if (originalWidth > desiredWidth)
                {
                    // Image is wider than bounding box
                    aspectRatio = desiredWidth / originalWidth; //one demension (width) so calculate the aspect ratio between the bounding box width and original image width
                }
                else
                {
                    // Image is taller than bounding box
                    aspectRatio = desiredHeight / originalHeight;
                }
            }

            // Resize image
            var resizeWidth = Convert.ToInt32(originalWidth * aspectRatio); //downscale image by r to fit in the bounding box...
            var resizeHeight = Convert.ToInt32(originalHeight * aspectRatio);
            var resizedImage = new Bitmap(sourceImage, resizeWidth, resizeHeight);
            sourceImage.Dispose();

            return resizedImage;
        }

        /// <summary>
        /// Get a thumbnail image.
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <param name="desiredWidth"></param>
        /// <param name="desiredHeight"></param>
        /// <returns>Image object.</returns>
        public static Image? GetThumbnailImage(Image sourceImage, int desiredWidth, int desiredHeight)
        {
            if (sourceImage == null) { return null; }

            // Set original width and height
            var originalWidth = sourceImage.Width;
            var originalHeight = sourceImage.Height;

            // Get width and height ratios
            var ratioWidth = (float)originalWidth / desiredWidth;
            var ratioHeight = (float)originalHeight / desiredHeight;

            // Get the smaller ratio
            var ratio = Math.Min(ratioWidth, ratioHeight);

            // Set scale width and height
            var scaleWidth = Convert.ToInt32(desiredWidth * ratio);
            var scaleHeight = Convert.ToInt32(desiredHeight * ratio);

            // Start cropping from the center
            var startX = (originalWidth - scaleWidth) / 2;
            var startY = (originalHeight - scaleHeight) / 2;

            // Resize cropped image
            var thumbnailImage = new Bitmap(desiredWidth, desiredHeight);

            // Fill-in the whole bitmap
            var destinationRectangle = new Rectangle(0, 0, thumbnailImage.Width, thumbnailImage.Height);

            // Crop the image from the specified location and size
            var sourceRectangle = new Rectangle(startX, startY, scaleWidth, scaleHeight);

            // Generate the new image
            using (var graphics = Graphics.FromImage(thumbnailImage))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(sourceImage, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);
            }

            return thumbnailImage;
        }

        /// <summary>
        /// Get the image format.
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns>ImageFormat.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static ImageFormat GetImageFormat(string inputType)
        {
            switch (inputType.ToLower())
            {
                case "gif":
                case "jif":
                    return ImageFormat.Gif;
                case "jpg":
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "png":
                case "ping":
                    return ImageFormat.Png;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Input Type '{0}' not supported.", inputType));
            }
            ;
        }

        /// <summary>
        /// Get the image content type.
        /// </summary>
        /// <param name="outputImageFormat"></param>
        /// <returns>Content type as string.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string GetImageContentType(ImageFormat outputImageFormat)
        {
            switch (outputImageFormat.ToString())
            {
                case "Jpeg":
                    return "image/jpeg";
                case "Png":
                    return "image/png";
                case "Gif":
                    return "image/gif";
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Image Format '{0}' not supported.", outputImageFormat));
            }
        }
    }

#pragma warning restore CA1416 // Validate platform compatibility

}
