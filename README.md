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

