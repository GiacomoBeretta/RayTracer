// This file is release under EUPL_v1.2 license. See LICENSE.md

//da riscrivere meglio la docstring della classe una volta capito meglio cosa fa e magari rivedere le altre docstring

using SixLabors.ImageSharp.Processing;

namespace TracerLib;

/// <summary>
/// ImageTracer is a class that links the Camera classes to the pixel matrix
/// </summary>
public class ImageTracer
{
    private HDRImage _image;
    private ICamera _camera;
    private PCG _pcg;
    public int SamplePerSide { get; set; }

    public ImageTracer(HDRImage image, ICamera camera, PCG? pcg = null, int samplePerSide=0)
    {
        this._image = image;
        this._camera = camera;
        _pcg = pcg ?? new PCG();
        SamplePerSide = samplePerSide;
    }
    
    //attenzione che Tomasi ha una formula diversa: da chiedere!!
    /// <summary>
    /// Returns the <c>Ray</c> that passes through the pixel at (column, row).
    /// Since a pixel is not a dimensionless point,
    /// uPixel and vPixel are the coordinates inside the pixel at which the ray will be fired.
    /// If (uPixel, vPixel)=(0,0) it means the ray will be fired at the left bottom angle of the pixel
    /// See method FireRay of <c>Camera</c> for more information
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <param name="uPixel"></param>
    /// <param name="vPixel"></param>
    /// <returns></returns>
    public Ray FireRay(int column, int row, float uPixel = 0.5f, float vPixel = 0.5f)
    {
        //the formulas for u and v are different because columns start from left like the u coordinate
        //while the rows start from the top, contrary to v that starts from the bottom
        float u = (column + uPixel) / _image.Width; 
        float v = 1 - (1 + row - vPixel) / _image.Height; //equivale a (image.Height - 1 - row + vPixel) / image.Height;
        return _camera.FireRay(u, v);
    }

    /// <summary>
    /// Fires a ray for each pixel and use the <c>Renderer</c> function to solve the rendering equation and compute the color.
    /// </summary>
    /// <param name="renderer"></param>
    public void FireAllRays(Func<Ray, Color> renderer)
    {
        for (int col = 0; col < _image.Width; col++)
        {
            for (int row = 0; row < _image.Height; row++)
            {
                var cumcolor = new Color(0.0f,0.0f,0.0f);

                if (SamplePerSide > 0)
                {
                    for (var pixRow = 0; pixRow < SamplePerSide; pixRow++)
                    {
                        for (var pixCol = 0; pixCol < SamplePerSide; pixCol++)
                        {
                            var uPix = (pixCol + _pcg.RandomFloat()) / SamplePerSide;
                            var vPix = (pixRow + _pcg.RandomFloat()) / SamplePerSide;
                            var ray = FireRay(col, row, uPix, vPix);
                            cumcolor += renderer(ray);
                        }
                    }

                    _image[col, row] = cumcolor;
                }
                else
                {
                    var ray = FireRay(col, row);
                    var color = renderer(ray);
                    _image[col, row] = color;
                }
            }
        }
    }
}

delegate Color Renderer(Ray ray);
    
    //= ray =>world.RayIntersection(ray) != null ? new Color(1.0f, 1.0f, 1.0f) : new Color(0.0f, 0.0f, 0.0f);