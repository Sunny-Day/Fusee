using System;

namespace Fusee.Engine
{
    public interface IImagehelperImp
    {
        /// <summary>
        /// Creates a new Image with a specified size and color.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="bgColor">The color of the image. Value must be JS compatible.</param>
        /// <returns>An ImageData struct containing all necessary information for further processing.</returns>
        ImageData CreateImage(int width, int height, String bgColor);

        /// <summary>
        /// Creates a new Bitmap-Object from an image file,
        /// locks the bits in the memory and makes them available
        /// for furher action (e.g. creating a texture).
        /// Method must be called before creating a texture to get the necessary
        /// ImageData struct.
        /// </summary>
        /// <param name="filename">Path to the image file you would like to use as texture.</param>
        /// <returns>An ImageData object with all necessary information for the texture-binding process.</returns>
        ImageData LoadImage(String filename);

        /// <summary>
        /// Maps a specified text onto an existing image.
        /// </summary>
        /// <param name="imgData">The ImageData struct with the PixelData from the image.</param>
        /// <param name="fontName">The name of the text-font.</param>
        /// <param name="fontSize">The size of the text-font.</param>
        /// <param name="text">The text that sould be mapped on the iamge.</param>
        /// <param name="textColor">The color of the text-font.</param>
        /// <param name="startPosX">The horizontal start-position of the text on the image.</param>
        /// <param name="startPosY">The vertical start-position of the text on the image.</param>
        void TextOnImage(ImageData imgData, String fontName, float fontSize, String text, String textColor, float startPosX, float startPosY);
    }
}
