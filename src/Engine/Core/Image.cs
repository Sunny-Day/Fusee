using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class Image
    {
        public Image(ImageData imageData)
        {
            if (imageData.Width <= 0 || imageData.Height <= 0 || imageData.PixelData.Length < imageData.Width * imageData.Height)
                throw new ArgumentOutOfRangeException("imageData", "Corrupt image data");
            _imageData = imageData;
        }

        
        /// <summary>
        /// The horizontal number of pixels
        /// </summary>
        public int Width
        {
            get { return _imageData.Width; }
        }
        /// <summary>
        /// The vertical number of pixels
        /// </summary>
        public int Height
        {
            get { return _imageData.Height; }
        }
        /// <summary>
        /// The data contained within the image
        /// </summary>
        internal ImageData _imageData;
    }
}
