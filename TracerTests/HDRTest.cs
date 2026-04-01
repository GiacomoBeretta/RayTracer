using Colors;
using Hdr;

namespace Hdr.Test;

public class HdrTest
{
    [Fact]
    public void ValidCoord()
    {
        var img = new HdrImage(100, 100);
        Assert.True(img._Valid_coord(53,68));
        Assert.False(img._Valid_coord(200,39));
    }
    
    [Fact]
    public void PixelOffset()
    {
        var img = new HdrImage(100, 100);
        Assert.Equal(203, img.pixel_offset(3, 2));
    }

    [Fact]
    public void TestClampImage()
    {
        var img = new HdrImage(2, 1);
        
        img.Set_pixel(0, 0, new Color(0.5f, 1.0f, 1.5f));
        img.Set_pixel(1, 0, new Color(50.0f, 100.0f, 150.0f));
        
        img.Clamp_Image();

        foreach (var pixel in img.Pixels)
        {
            Assert.True(pixel.R is >= 0 and <= 1);
            Assert.True(pixel.G is >= 0 and <= 1);
            Assert.True(pixel.B is >= 0 and <= 1);
        }
    }
    
}