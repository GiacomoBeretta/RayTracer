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
}