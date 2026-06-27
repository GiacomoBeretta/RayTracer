using System.Net;
using TracerLib;

namespace TracerTests;

public class SceneTest
{
    private Scene scene = new Scene();
    private string filepath = Path.GetTempFileName();

    [Fact]
    public void ExpectSymbolTest()
    {
        const string symbol = "()[]<>,*#";

        File.WriteAllText(filepath, symbol);

        using (InputStream str = new InputStream(filepath))
        {
            scene.ExpectSymbol(str, "(");
            scene.ExpectSymbol(str, ")");
            scene.ExpectSymbol(str, "[");
            scene.ExpectSymbol(str, "]");
            scene.ExpectSymbol(str, "<");
            scene.ExpectSymbol(str, ">");
            scene.ExpectSymbol(str, ",");
            scene.ExpectSymbol(str, "*");
            Assert.Throws<SceneSyntaxException>(() => scene.ExpectSymbol(str, ","));
        }

        File.Delete(filepath);
    }

    [Fact]
    public void ExpectKeywordTest()
    {
        const string key = "identity  scaling  saling";
        List<Keyword> keywords = [Keyword.RotationX, Keyword.RotationY, Keyword.Scaling];

        File.WriteAllText(filepath, key);

        using (InputStream str = new InputStream(filepath))
        {
            Assert.Throws<SceneSyntaxException>(() => scene.ExpectKeyword(str, keywords));
            Assert.Equal(Keyword.Scaling, scene.ExpectKeyword(str, keywords));
            Assert.Throws<SceneSyntaxException>(() => scene.ExpectKeyword(str, keywords));
        }

        File.Delete(filepath);
    }

    [Fact]
    public void ExpectNumberTest()
    {
        const string number = "( 18+15  1e18";

        File.WriteAllText(filepath, number);

        using (InputStream str = new InputStream(filepath))
        {
            Assert.Throws<SceneSyntaxException>(() => scene.ExpectNumber(str));
            Assert.Equal(18f, scene.ExpectNumber(str));
            //Controllare perchè il segno + viene saltato
            Assert.Equal(15f, scene.ExpectNumber(str));
            Assert.True(Functions.AreClose(1e18f, scene.ExpectNumber(str)));
        }

        File.Delete(filepath);
    }

    [Fact]
    public void ExpectStringToken()
    {
        const string content = "6  identity  \"pizza\"";

        File.WriteAllText(filepath, content);

        using (InputStream str = new InputStream(filepath))
        {
            Assert.Throws<SceneSyntaxException>(() => scene.ExpectString(str));
            Assert.Throws<SceneSyntaxException>(() => scene.ExpectString(str));
            Assert.Equal("pizza", scene.ExpectString(str));
        }

        File.Delete(filepath);
    }

    [Fact]
    public void ExpectIdentifier()
    {
        const string content = "clock  identity   \"Hello world\"";

        File.WriteAllText(filepath, content);

        using (InputStream str = new InputStream(filepath))
        {
            Assert.Equal("clock", scene.ExpectIdentifier(str));
            Assert.Throws<SceneSyntaxException>(() => scene.ExpectIdentifier(str));
            Assert.Throws<SceneSyntaxException>(() => scene.ExpectIdentifier(str));
        }

        File.Delete(filepath);
    }

    [Fact]
    public void ParseVectorTest()
    {
        const string vector = "[18, 23, 0]";

        File.WriteAllText(filepath, vector);

        using (InputStream str = new InputStream(filepath))
        {
            Assert.True(Vector._AreVectorsClose(new Vector(18f, 23f, 0f), scene.ParseVector(str)));
        }

        File.Delete(filepath);
    }

    [Fact]
    public void ParseColorTest()
    {
        const string color = "<0.5, 0.21, 0.75>";

        File.WriteAllText(filepath, color);

        using (InputStream str = new InputStream(filepath))
        {
            Assert.True(Color._AreColorsClose(new Color(0.5f, 0.21f, 0.75f), scene.ParseColor(str)));
        }

        File.Delete(filepath);
    }

    [Fact]
    public void ParsePigmentTest()
    {
        const string pigment = "uniform(<0.3, 0.5, 0.1>)\n" +
                               "checkered(<0.4, 0.6, 0.5>, <0.1, 0, 0.9>, 4)";

        File.WriteAllText(filepath, pigment);

        using (InputStream str = new InputStream(filepath))
        {
            Pigment uniform = scene.ParsePigment(str);

            CheckeredPigment checkered = (CheckeredPigment)scene.ParsePigment(str);

            Color uColor = uniform.GetColor(new Vector2D(0.5f, 0.5f));
            Color cColor1 = checkered.GetColor(new Vector2D(0.1f, 0.1f));
            Color cColor2 = checkered.GetColor(new Vector2D(0.1f, 0.9f));

            Assert.True(Color._AreColorsClose(new Color(0.3f, 0.5f, 0.1f), uColor));
            Assert.True(Color._AreColorsClose(new Color(0.4f, 0.6f, 0.5f), cColor1));
            Assert.True(Color._AreColorsClose(new Color(0.1f, 0f, 0.9f), cColor2));
            Assert.Equal(4, checkered.NumSteps);
        }

        File.Delete(filepath);
    }

    [Fact]
    public void ParseBRDFTest()
    {
        const string brdf =
            "diffuse(uniform(<0.3, 0.5, 0.1>)) specular(checkered(<0.4, 0.6, 0.5>, <0.1, 0, 0.9>, 4))";

        File.WriteAllText(filepath, brdf);

        using (InputStream str = new InputStream(filepath))
        {
            BRDF diffuse = scene.ParseBRDF(str);
            BRDF specular = scene.ParseBRDF(str);

            Color uColor = diffuse.Pigment.GetColor(new Vector2D(0.5f, 0.5f));
            Color cColor1 = specular.Pigment.GetColor(new Vector2D(0.1f, 0.1f));
            Color cColor2 = specular.Pigment.GetColor(new Vector2D(0.1f, 0.9f));

            Assert.True(Color._AreColorsClose(new Color(0.3f, 0.5f, 0.1f), uColor));
            Assert.True(Color._AreColorsClose(new Color(0.4f, 0.6f, 0.5f), cColor1));
            Assert.True(Color._AreColorsClose(new Color(0.1f, 0f, 0.9f), cColor2));
        }

        File.Delete(filepath);
    }

    [Fact]
    public void ParseMaterialTest()
    {
        const string mat =
            "ground_material(diffuse(checkered(<0.3, 0.5, 0.1>, <0.1, 0.2, 0.5>, 4)), uniform(<0, 0, 0>))";
        File.WriteAllText(filepath, mat);

        using (InputStream str = new InputStream(filepath))
        {
            scene.ParseMaterial(str, out string name, out Material material);

            Color cColor1 = material.Pigment.GetColor(new Vector2D(0.1f, 0.1f));
            Color cColor2 = material.Pigment.GetColor(new Vector2D(0.1f, 0.9f));
            Color uColor = material.EmittedRadiance.GetColor(new Vector2D(0.5f, 0.5f));

            Assert.True(Color._AreColorsClose(new Color(0.3f, 0.5f, 0.1f), cColor1));
            Assert.True(Color._AreColorsClose(new Color(0.1f, 0.2f, 0.5f), cColor2));
            Assert.True(Color._AreColorsClose(new Color(0f, 0f, 0f), uColor));
            Assert.Equal("ground_material", name);
        }

        File.Delete(filepath);
    }

    [Fact]
    public void ParseTransformationTest()
    {
        const string transform = "rotation_z(30) * translation([-4, 0, 1])";

        File.WriteAllText(filepath, transform);

        using (InputStream str = new InputStream(filepath))
        {
            Transformation transformation = scene.ParseTransformation(str);
            Transformation t = new Transformation(Axis.Z, Functions.DegToRad(30)) *
                               new Transformation(new Vector(-4f, 0f, 1f));

            Assert.True(Transformation.AreTransformationsClose(t, transformation));
        }

        File.Delete(filepath);
    }

    [Fact]
    public void ParseSphereTest()
    {
        const string sph = "sphere_material(specular(uniform(<0.5, 0.5, 0.5>)),uniform(<0, 0, 0>))" +
                           "(sphere_material, translation([0, 0, 1]))";

        File.WriteAllText(filepath, sph);

        using (InputStream str = new InputStream(filepath))
        {
            scene.ParseMaterial(str, out string name, out Material material);
            scene.Materials[name] = material;

            Sphere sphere = scene.ParseSphere(str);
            Color colorSphere = sphere.Material.Pigment.GetColor(new Vector2D(0.5f, 0.5f));
            Color radianceSphere = sphere.Material.EmittedRadiance.GetColor(new Vector2D(0.5f, 0.5f));
            Transformation transformSphere = sphere.Transform;

            Assert.True(Color._AreColorsClose(new Color(0.5f, 0.5f, 0.5f), colorSphere));
            Assert.True(Color._AreColorsClose(new Color(0f, 0f, 0f), radianceSphere));
            Assert.True(Transformation.AreTransformationsClose(new Transformation(new Vector(0f, 0f, 1f)),
                transformSphere));
        }

        File.Delete(filepath);
    }

    [Fact]
    public void ParsePlaneTest()
    {
        const string pln =
            "ground_material(diffuse(checkered(<0.3, 0.5, 0.1>, <0.1, 0.2, 0.5>, 4)), uniform(<0, 0, 0>))" +
            "(ground_material, identity)";

        File.WriteAllText(filepath, pln);

        using (InputStream str = new InputStream(filepath))
        {
            scene.ParseMaterial(str, out string name, out Material material);
            scene.Materials[name] = material;

            Plane plane = scene.ParsePlane(str);
            Color colorPlane1 = plane.Material.Pigment.GetColor(new Vector2D(0.1f, 0.1f));
            Color colorPlane2 = plane.Material.Pigment.GetColor(new Vector2D(0.1f, 0.9f));
            Color radiancePlane = plane.Material.EmittedRadiance.GetColor(new Vector2D(0.5f, 0.5f));
            Transformation transformPlane = plane.Transform;

            Assert.True(Color._AreColorsClose(new Color(0.3f, 0.5f, 0.1f), colorPlane1));
            Assert.True(Color._AreColorsClose(new Color(0.1f, 0.2f, 0.5f), colorPlane2));
            Assert.True(Color._AreColorsClose(new Color(0f, 0f, 0f), radiancePlane));
            Assert.True(Transformation.AreTransformationsClose(new Transformation(), transformPlane));
            Assert.Single(scene.Materials);
        }

        File.Delete(filepath);
    }

    [Fact]
    public void ParseCameraTest()
    {
        const string cam = "(perspective, rotation_z(30) * translation([-4, 0, 1]), 1.0, 1.0)";

        File.WriteAllText(filepath, cam);

        using (InputStream str = new InputStream(filepath))
        {
            var camera = (PerspectiveCamera)scene.ParseCamera(str);

            Assert.True(Transformation.AreTransformationsClose(
                new Transformation(Axis.Z, Functions.DegToRad(30)) * new Transformation(new Vector(-4f, 0f, 1f)),
                camera.Transformation));
            Assert.True(Functions.AreClose(1.0f, camera.AspectRatio));
            Assert.True(Functions.AreClose(1.0f, camera.Distance));
        }

        File.Delete(filepath);
    }
}