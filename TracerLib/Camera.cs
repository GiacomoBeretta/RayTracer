namespace TracerLib;

public interface ICamera
{
    Ray FireRay(float u, float v);
}

public struct OrthogonalCamera : ICamera
{
    public float AspectRatio { get; set; }
    public Transformation Transformation { get; set; }
    
    public OrthogonalCamera(float aspectRatio, Transformation transformation)
    {
        AspectRatio = aspectRatio;
        Transformation = transformation;
    }
    public Ray FireRay(float u, float v)
    {
        var origin = new Point(-1f, (1f - 2f * u) * this.AspectRatio, 2f * v - 1f);
        return new Ray(); //Da modificare 
    } 
    
    public struct PrespectiveCamera: ICamera
    {
        public float Distance { get; set; }
        public float AspectRatio { get; set; }
        public Transformation Transformation { get; set; }

        public PrespectiveCamera(float distance, float aspectRatio, Transformation transformation)
        {
            Distance = distance;
            AspectRatio = aspectRatio;
            Transformation = transformation;
        }
        
        public Ray FireRay(float u, float v)
        {
            return new Ray();
        }
    }
    
}