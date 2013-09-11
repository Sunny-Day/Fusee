using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace Fusee.Engine
{
    public class ImagehelperImp : IImagehelperImp
    {
        /// <summary>
        /// Creates a new Image with a specified size and color.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="bgColor">The color of the image. Value must be JS compatible.</param>
        /// <returns>An ImageData struct containing all necessary information for further processing.</returns>
        public ImageData CreateImage(int width, int height, String bgColor)
        {
            Bitmap bmp = new Bitmap(width, height);
            Graphics gfx = Graphics.FromImage(bmp);
            Color color = Color.FromName(bgColor);
            gfx.Clear(color);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int strideAbs = (bmpData.Stride < 0) ? -bmpData.Stride : bmpData.Stride;
            int bytes = (strideAbs) * bmp.Height;


            ImageData ret = new ImageData()
            {
                PixelData = new byte[bytes],
                Height = bmpData.Height,
                Width = bmpData.Width,
                Stride = bmpData.Stride

            };

            Marshal.Copy(bmpData.Scan0, ret.PixelData, 0, bytes);
            bmp.UnlockBits(bmpData);
            return ret;
        }

        /// <summary>
        /// Creates a new Bitmap-Object from an image file,
        /// locks the bits in the memory and makes them available
        /// for furher action (e.g. creating a texture).
        /// Method must be called before creating a texture to get the necessary
        /// ImageData struct.
        /// </summary>
        /// <param name="filename">Path to the image file you would like to use as texture.</param>
        /// <returns>An ImageData object with all necessary information for the texture-binding process.</returns>
        public ImageData LoadImage(String filename)
        {
            Bitmap bmp = new Bitmap(filename);
            //Flip y-axis, otherwise texture would be upside down
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int strideAbs = (bmpData.Stride < 0) ? -bmpData.Stride : bmpData.Stride;
            int bytes = (strideAbs) * bmp.Height;


            ImageData ret = new ImageData()
            {
                PixelData = new byte[bytes],///
                Height = bmpData.Height,
                Width = bmpData.Width,
                Stride = bmpData.Stride

            };
            Marshal.Copy(bmpData.Scan0, ret.PixelData, 0, bytes);

            bmp.UnlockBits(bmpData);
            return ret;
        }

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
        public void TextOnImage(ImageData imgData, String fontName, float fontSize, String text, String textColor, float startPosX, float startPosY)
        {

            GCHandle arrayHandle = GCHandle.Alloc(imgData.PixelData,
                                   GCHandleType.Pinned);
            Bitmap bmp = new Bitmap(imgData.Width, imgData.Height, imgData.Stride, PixelFormat.Format32bppArgb,
                                    arrayHandle.AddrOfPinnedObject());
            Color color = Color.FromName(textColor);
            Font font = new Font(fontName, fontSize, FontStyle.Regular, GraphicsUnit.World);


            Graphics gfx = Graphics.FromImage(bmp);
            gfx.TextRenderingHint = TextRenderingHint.AntiAlias;
            gfx.DrawString(text, font, new SolidBrush(color), startPosX, startPosY);

            //Flip y-axis, otherwise texture would be upside down
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int strideAbs = (bmpData.Stride < 0) ? -bmpData.Stride : bmpData.Stride;
            int bytes = (strideAbs) * bmp.Height;

            imgData.PixelData = new byte[bytes];
            imgData.Height = bmpData.Height;
            imgData.Width = bmpData.Width;
            imgData.Stride = bmpData.Stride;

            Marshal.Copy(bmpData.Scan0, imgData.PixelData, 0, bytes);
            bmp.UnlockBits(bmpData);
            // return imgData;

        }


    }


}
