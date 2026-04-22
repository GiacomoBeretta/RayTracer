// This file is release under EUPL_v1.2 license. See LICENSE.md

using SixLabors.ImageSharp.Processing;

namespace TracerLib;

public class ImageTracer
{
    private HDRImage image;
    private ICamera camera;

    /// <summary>
    ///
    /// uPixel and vPixel are the coordinates inside the pixel at which the ray will be fired.
    /// If (uPixel, vPixel)=(0,0) it means the ray will be fired at the left bottom angle of the pixel
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <param name="uPixel"></param>
    /// <param name="vPixel"></param>
    /// <returns></returns>
    public Ray FireRay(int column, int row, float uPixel=0.5f, float vPixel=0.5f)
    {
        float u = (column + uPixel)/(image.Width - 1);
        float v = (row + vPixel)/(image.Height -1);
        return camera.FireRay(u, v);
    }

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