// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// ImageTracer is a class that links the Camera classes to the pixel matrix
/// </summary>
public class ImageTracer
{
    /// <summary>
    /// The image storing the result of the rendering process.
    /// </summary>
    private HDRImage _image;
    
    /// <summary>
    /// The camera defining the projection model, the observer position, and the aspect ratio.
    /// </summary>
    private ICamera _camera;
    
    /// <summary>
    /// The random generator used for Monte Carlo integration. 
    /// </summary>
    private PCG _pcg;
    
    /// <summary>
    /// Number of subdivisions per pixel axis.
    /// The total number of rays per pixel is the square of this value.
    /// </summary>
    public int PixelSideSubdivisions { get; set; }
    
    public ImageTracer(HDRImage image, ICamera camera, PCG? pcg = null, int pixelSideSubdivisions = 1)
    {
        _image = image;
        _camera = camera;
        _pcg = pcg ?? new PCG();
        if(pixelSideSubdivisions < 1) throw new ArgumentOutOfRangeException(nameof(pixelSideSubdivisions), pixelSideSubdivisions, "The number of pixel side subdivisions must be greater than 1.");
        PixelSideSubdivisions = pixelSideSubdivisions;
    }
    
    // (formule diverse da Tomasi)
    /// <summary>
    /// Returns a <see cref="Ray"/> passing through the pixel at (column, row).
    /// Since a pixel is not a dimensionless point,
    /// <paramref name="uPixel"/> and <paramref name="vPixel"/> are the coordinates inside the pixel at which the ray will be fired.
    /// A value of (uPixel, vPixel)=(0,0) corresponds to the top-left corner of the pixel.
    /// See method FireRay of <see cref="ICamera"/> for more information.
    /// </summary>
    /// <param name="column">Pixel column index in the image (0 to image.Width - 1).</param>
    /// <param name="row">Pixel row index in the image (0 to image.Height - 1).</param>
    /// <param name="uPixel">Horizontal coordinate inside the pixel in [0,1].</param>
    /// <param name="vPixel">Vertical coordinate inside the pixel in [0,1].</param>
    /// <returns></returns>
    public Ray FireRayAtPixel(int column, int row, float uPixel = 0.5f, float vPixel = 0.5f)
    {
        // the (u,v) coordinates start from the top-left corner of the unit square [0,1]x[0,1].
        float u = (column + uPixel) / _image.Width; 
        float v = (row + vPixel) / _image.Height;
        return _camera.FireRay(u, v);
    }
    
    /// <summary>
    /// Computes the color of each pixel of the <see cref="HDRImage"/> using the provided RenderFunction
    /// of the <see cref="Renderer"/> class.
    /// </summary>
    /// <remarks>
    /// If <see cref="PixelSideSubdivisions"/> &gt; 0,
    /// multiple rays are fired per pixel and the results are averaged to reduce aliasing.
    /// Otherwise, the rays are fired at the center of each pixel.
    /// </remarks>
    /// <param name="renderFunction">Function that estimates the color for a given ray.</param>
    public void FireAllRays(Func<Ray, Color> renderFunction)
    {
        for (int col = 0; col < _image.Width; col++)
        {
            for (int row = 0; row < _image.Height; row++)
            {
                Color cumcolor = new Color(0.0f,0.0f,0.0f);

                // Anti-Aliasing algorithm:
                // we subdivide the pixel in a PixelSideSubdivisions x PixelSideSubdivisions grid
                // then for each cell of this grid we fire a ray randomly.
                if (PixelSideSubdivisions > 1) //CONTROLLARE SE 1 SUDDIVISIONE AGGIUNGE UN'ULTERIORE RAGGIO A QUELLO DELL'ALGORITMO NORMALE
                {
                    for (int pixRow = 0; pixRow < PixelSideSubdivisions; pixRow++)
                    {
                        for (int pixCol = 0; pixCol < PixelSideSubdivisions; pixCol++)
                        {
                            float uPix = (pixCol + _pcg.RandomFloat()) / PixelSideSubdivisions;
                            float vPix = (pixRow + _pcg.RandomFloat()) / PixelSideSubdivisions;
                            Ray ray = FireRayAtPixel(col, row, uPix, vPix);
                            cumcolor += renderFunction(ray);
                        }
                    }
                    
                    _image[col, row] = cumcolor * (1.0f / (PixelSideSubdivisions*PixelSideSubdivisions));
                }
                else
                {
                    //otherwise the is fire at the center of each pixel
                    Ray ray = FireRayAtPixel(col, row);
                    Color color = renderFunction(ray);
                    _image[col, row] = color;
                }
            }
        }
    }
}