// This file is release under EUPL_v1.2 license. See LICENSE.md

//da riscrivere meglio la docstring della classe una volta capito meglio cosa fa e magari rivedere le altre docstring

using SixLabors.ImageSharp.Processing;

namespace TracerLib;

/// <summary>
/// ImageTracer is a class
/// </summary>
public class ImageTracer
{
    private HDRImage image;
    private ICamera camera;

    public ImageTracer(HDRImage image, ICamera camera)
    {
        this.image = image;
        this.camera = camera;
    }
    
    //attenzione che Tomasi ha una formula diversa: da chiedere!!
    /// <summary>
    /// Returns the <c>Ray</c> that passes through the pixel at (column, row).
    /// Since a pixel is not a point uPixel and vPixel are the coordinates inside the pixel at which the ray will be fired.
    /// If (uPixel, vPixel)=(0,0) it means the ray will be fired at the left bottom angle of the pixel
    /// See <c>Camera</c> for more information
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <param name="uPixel"></param>
    /// <param name="vPixel"></param>
    /// <returns></returns>
    public Ray FireRay(int column, int row, float uPixel = 0.5f, float vPixel = 0.5f)
    {
        float u = (column + uPixel) / image.Width; 
        float v = 1 - (1 + row - vPixel) / image.Height; //equivale a (image.Height - 1 - row + vPixel) / image.Height;
        return camera.FireRay(u, v);
    }

    /// <summary>
    /// Fires a ray for each pixel and use the <c>Renderer</c> function to solve the rendering equation and compute the color.
    /// </summary>
    /// <param name="Renderer"></param>
    public void FireAllRays(Func<Ray, Color> Renderer)
    {
        for (int col = 0; col < image.Width; col++)
        {
            for (int row = 0; row < image.Height; row++)
            {
                Ray ray = FireRay(col, row);
                Color color = Renderer(ray);
                image[col, row] = color;
            }
        }
    }
}