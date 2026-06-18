# Photorealistic Ray Tracer

A C# application for generating photorealistic images using different rendering algorithms.

## Purpose

This project is a ray tracing renderer written in C# that generates photorealistic images from scene descriptions provided in text files.

## Usage

The program provides several commands:

### `render`

Reads a scene description from a text file and generates the corresponding image.

> **Note:** The scene files must be located inside the `Scene` directory.

### `averageimage`

Generates a new image by averaging multiple PFM images of the same scene rendered using different seeds and sequence identifiers for the random number generator.

The averaging is performed pixel by pixel in order to reduce image variance and noise.

This command is designed to work together with the `raytracer.sh` script. When the script is configured to generate multiple renders using different random generator states and sequence identifiers, the generated PFM files are saved using the naming convention:

```bash
${outputpfm%.pfm}_state${state}_seq${seq}.pfm
```

The command automatically filters the files contained in the input directory and processes only the files matching this pattern.

### `pfmtopng`

Converts images from the PFM (Portable Float Map) format to PNG format.

---

## Scripts

The project includes three Bash scripts that simplify image generation and post-processing operations:

* `raytracer.sh`
* `config.sh`
* `generate-animation.sh`

### `config.sh`

This file contains the default values used by the different commands supported by the renderer.

By modifying the values in this file it is possible to generate different images without specifying all parameters from the command line.

In particular, the following options can be specified as arrays:

* `initstate`
* `initseq`
* `declarefloat`

The file also defines a boolean variable named:

```bash
pcgcycle=false
```

which controls how multiple random generator states and sequence identifiers are handled.

When `pcgcycle` is set to `false` (default value), only the first value of the `initstate` and `initseq` arrays is used during rendering.

When `pcgcycle` is set to `true`, the renderer is executed for every combination of state and sequence identifier contained in the two arrays.

For example:

```bash
initstate=(45 12)
initseq=(54 2)
```

produces the following combinations:

```text
state=45, seq=54
state=45, seq=2
state=12, seq=54
state=12, seq=2
```

### `raytracer.sh`

This script automates the execution of the renderer.

It loads the default values from `config.sh`, allows them to be overridden from the command line, and invokes the selected command with the appropriate parameters.

When `pcgcycle=true`, the script automatically executes the rendering process for every combination of random generator state and sequence identifier defined in `config.sh`.

The generated files are named according to the following convention:

```bash
${outputpfm%.pfm}_state${state}_seq${seq}.pfm
```

and

```bash
${outputpng%.png}_state${state}_seq${seq}.png
```

This naming scheme is also used by the `averageimage` command to identify the images that must be averaged.

### `generate-animation.sh`

This script converts a sequence of PNG images into an MP4 video.

The script is designed to process images whose filenames follow the pattern:

```text
frame_%03d.png
```

Examples:

```text
frame_000.png
frame_001.png
frame_002.png
...
```

All matching images are combined into a single MP4 animation.

The images can be generated automatically using the GNU Parallel library. A generic command has the following form:

```bash
seq -w START_ANGLE END_ANGLE | parallel -j NUM_CORES ./raytracer.sh render --declarefloat VARIABLE_NAME:{} --outputpfm frame_{}.pfm --outputpng frame_{}.png
```

where:

* `START_ANGLE` and `END_ANGLE` define the range of values over which the animation is generated. A common use case is to let these values represent rotation angles.
* `NUM_CORES` is the number of CPU cores used in parallel by GNU Parallel.
* `VARIABLE_NAME` is the name of a floating-point variable declared inside the scene description file (`scene.txt`).

The variable can then be used inside transformations defined in the scene. For example, if a transformation depends on a variable named `clock`, the command will render one frame for each value in the specified range and substitute the current value into the variable.

This mechanism allows the generation of animation frames by varying a scene parameter such as:

* object rotation;
* object translation;
* camera movement;
* any other transformation controlled by a floating-point variable declared in the scene file.

The generated frames are saved using the naming convention:

```text
frame_000.png
frame_001.png
frame_002.png
...
```

and can subsequently be combined into an MP4 animation using `generate-animation.sh`.

---

# Scene Description Language

Scenes are described through a custom text-based language.

The language allows the definition of:

- floating-point variables;
- vectors;
- colors;
- materials;
- pigments;
- BRDFs;
- transformations;
- geometric primitives;
- cameras.

An example scene is shown below:

```text
# Declare a floating-point variable named "clock"
float clock(150)

material sky_material(
    diffuse(uniform(<0.5, 0.3, 0.1>)),
    uniform(<0.7, 0.5, 1>)
)

material ground_material(
    diffuse(checkered(<0.3, 0.5, 0.1>,
                      <0.1, 0.2, 0.5>, 4)),
    uniform(<0, 0, 0>)
)

material sphere_material(
    specular(uniform(<0.5, 0.5, 0.5>)),
    uniform(<0, 0, 0>)
)

sphere(sphere_material, translation([0, 0, 1]))

plane(ground_material, identity)

plane(sky_material,
      translation([0, 0, 100]) * rotation_y(150))

camera(
    perspective,
    rotation_z(clock) * translation([-4, 0, 1]),
    1.0,
    1.0
)
```

---

## Floating-Point Variables

Floating-point variables can be declared using the following syntax:

```text
float name(value)
```

Example:

```text
float clock(150)
```

Variables can be used wherever a floating-point value is expected.

---

## Vectors

Vectors are defined using square brackets:

```text
[x, y, z]
```

Example:

```text
[1, 2, 3]
```

---

## Colors

Colors are defined using angular brackets:

```text
<r, g, b>
```

Example:

```text
<0.5, 0.3, 0.1>
```

> **TODO:** document the valid range for `r`, `g`, and `b`.

---

## Materials

Materials are defined using the following grammar:

```text
material name_material(BRDF(pigment), emitted_radiance)
```

Example:

```text
material sphere_material(
    specular(uniform(<0.5, 0.5, 0.5>)),
    uniform(<0, 0, 0>)
)
```

A material is composed of:

- a BRDF;
- an emitted radiance pigment.

Materials are stored using their declared identifier and can be referenced by geometric primitives.

The material name is used to associate a material with an object.

---

## BRDFs

A BRDF is defined as:

```text
Keyword_BRDF(pigment)
```

where `Keyword_BRDF` can be:

- `diffuse`
- `specular`

Examples:

```text
diffuse(uniform(<1, 0, 0>))
```

```text
specular(uniform(<1, 1, 1>))
```

---

## Pigments

Pigments are defined as:

```text
Keyword_Pigment(...)
```

where `Keyword_Pigment` can be:

- `uniform`
- `checkered`
- `image`

### Uniform Pigment

```text
uniform(color)
```

Example:

```text
uniform(<1, 0, 0>)
```

### Checkered Pigment

```text
checkered(color1, color2, num_step)
```

Example:

```text
checkered(<1,0,0>, <0,0,1>, 8)
```

### Image Pigment

```text
image(filename)
```

Example:

```text
image("texture.pfm")
```

---

## Geometric Primitives

Currently supported primitives are:

- `sphere`
- `plane`

Both primitives require a previously defined material identifier and a transformation.

---

## Sphere

A sphere is defined as:

```text
sphere(name_material, transformation)
```

Example:

```text
sphere(
    sphere_material,
    translation([0,0,1])
)
```

---

## Plane

A plane is defined as:

```text
plane(name_material, transformation)
```

Example:

```text
plane(
    ground_material,
    identity
)
```

---

## Material Declaration Order

Materials must be declared before the geometric primitives that use them.

The material identifier passed to a primitive must refer to an existing material.

For example, this is valid:

```text
material sphere_material(
    diffuse(uniform(<1, 0, 0>)),
    uniform(<0, 0, 0>)
)

sphere(
    sphere_material,
    identity
)
```

while this is not valid:

```text
sphere(
    sphere_material,
    identity
)

material sphere_material(
    diffuse(uniform(<1, 0, 0>)),
    uniform(<0, 0, 0>)
)
```

The parser must know the material before the primitive is defined.

A material definition cannot be directly embedded inside a primitive definition.

For example, the following syntax is not supported:

```text
sphere(
    material(
        diffuse(uniform(<1, 0, 0>)),
        uniform(<0, 0, 0>)
    ),
    identity
)
```

The material must always be defined separately and referenced by its identifier.

---

## Transformations

Transformations are defined using specific keywords.

### Identity

```text
identity
```

### Translation

```text
translation(vector)
```

Example:

```text
translation([1, 0, 0])
```

### Rotation Around X

```text
rotation_x(angle)
```

### Rotation Around Y

```text
rotation_y(angle)
```

### Rotation Around Z

```text
rotation_z(angle)
```

where `angle` is expressed in degrees.

### Scaling

```text
scaling(x, y, z)
```

Example:

```text
scaling(2, 1, 1)
```

### Transformation Composition

An arbitrary number of transformations can be concatenated using the `*` operator.

Example:

```text
translation([0,0,1]) *
rotation_y(45) *
scaling(2,2,2)
```

---

## Cameras

Cameras are defined as:

```text
camera(
    Keyword_camera,
    transformation,
    aspect_ratio,
    distance
)
```

where `Keyword_camera` can be:

- `orthogonal`
- `perspective`

Example:

```text
camera(
    perspective,
    identity,
    1.0,
    1.0
)
```

### Note on Orthogonal Cameras

Although an orthogonal camera does not use the `distance` parameter internally, the parameter must still be specified in order for the scene to be parsed correctly.

Example:

```text
camera(
    orthogonal,
    identity,
    1.0,
    1.0
)
```

---

## Comments

Comments can be inserted using the `#` character.

Example:

```text
# This is a comment
float clock(45)
```

## Common Options

The following options are available in multiple commands.

### `--luminosityfunction`

Luminosity function used in tone mapping.

Available options:

* `shirley`
* `weighted`

### `--factor`

Empirical factor used during tone mapping.

### `--gamma`

Gamma correction factor applied during tone mapping.

---

## Render Command

### Available Options

#### `--inputrender`

Name of the scene file to be rendered.

The file must be located inside the `Scene` directory.

#### `--width`

Output image width in pixels.

#### `--height`

Output image height in pixels.

#### `--algorithm`

Rendering algorithm to use.

Available options:

* `onoff`
* `flat`
* `pathtracer`

#### `--outputpfm`

Name of the generated PFM image.

The file will be saved in the `PfmImages` directory.

#### `--outputpng`

Name of the generated PNG image.

The file will be saved in the `PngImages` directory.

#### `--numrays`

Number of rays scattered at each reflection.

Used only when the selected algorithm is `pathtracer`.

#### `--maxdepth`

Maximum recursion depth for each ray, i.e. the maximum number of reflections a ray can be subjected to, before returning the background color.

Used only when the selected algorithm is `pathtracer`.

#### `--initstate`

Initial seed for the random number generator.

#### `--initseq`

Sequence identifier used by the random number generator.

#### `--sampleside`

Number of subdivisions per pixel side used for anti-aliasing.

#### `--declarefloat` or `-d`

Declares a floating-point variable using the following syntax:

```bash
--declarefloat=NAME:VALUE
```

#### `--roulettestart`

Number of reflections after which the Russian Roulette termination algorithm starts.

#### `--rouletteprob`

Optional fixed probability used by the Russian Roulette algorithm.

If omitted, the probability is computed dynamically at each recursion step.

The following options are also available:

* `--luminosityfunction`
* `--factor`
* `--gamma`

---

## Average Image Command

### Available Options

#### `--inputaverage`

Name of the directory containing the PFM files to be averaged.

#### `--outputaverage`

Name of the generated averaged image.

The command generates both a PFM image and its corresponding PNG representation.

The following options are also available:

* `--luminosityfunction`
* `--factor`
* `--gamma`

---

## PFM to PNG Command

### Available Options

#### `--inputpfm`

Name of the PFM file to convert.

#### `--output`

Name of the generated PNG image.

The following options are also available:

* `--luminosityfunction`
* `--factor`
* `--gamma`

---

## Examples

*Examples will be added here.*

## Dependencies

- .NET 10 SDK
- Bash
- GNU Parallel (optional, required for animation generation)
- FFmpeg (required by generate-animation.sh)

# How to install it

This Program works on Ubuntu 24.04 LTS. The unit tests were run on these OSs with the latest version of dotnet 10.0.x and they were all successful:

Windows family:

```
OS Name:                       Microsoft Windows Server 2025 Datacenter
OS Version:                    10.0.26100 N/A Build 26100
BIOS Version:                  Microsoft Corporation Hyper-V UEFI Release v4.1, 1/8/2026
```

MacOS:

```
ProductName:                   macOS
ProductVersion:                15.7.7
BuildVersion:                  24G720
```

# Where to ask for help

# Future developments

# How to contribute

# Authors

Giacomo Beretta, Simone Selmi

# License

See the file LICENSE.md

# State of the project

