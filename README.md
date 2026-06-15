# Photorealistic Ray Tracer

A C# application for generating photorealistic images using different rendering algorithms.

## Purpose

This project is a ray tracing renderer written in C# that generates photorealistic images from scene descriptions provided in text files.

## Usage

The program provides several commands:

### `render`

Reads a scene description from a text file and generates the corresponding image.

> **Note:** The scene file must be located inside the `Scene` directory at build time.

### `pfmtopng`

Converts images from the PFM (Portable Float Map) format to PNG format.
---

## Image Generation Script

The project includes a Bash script called `generate-image.sh` that simplifies image generation and allows customization of many rendering parameters.

### Available Options

#### `--input`

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

Maximum recursion depth for each ray, i.e. the maximum number of reflections a ray can be subjected to, before returning the backgroun color.

Used only when the selected algorithm is `pathtracer`.

#### `--initstate`

Initial seed for the random number generator.

#### `--initseq`

Sequence identifier used by the random number generator.

#### `--sampleside`

Number of subdivisions per pixel side used for anti-aliasing.

#### `--luminosityfunction`
Luminosity function used in tone mapping.

Available options:

* `shirley`
* `weighted`

#### `--factor`

<<<<<<< HEAD
Empirical factor used during image rendering.

#### `--gamma`

Gamma correction factor applied to the final image.
=======
Empirical factor used during tone mapping.

#### `--gamma`

Gamma correction factor applied during tone mapping.
>>>>>>> pathtracing

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

---

## Examples

*Examples will be added here.*


# How to install it
<<<<<<< HEAD
This Program works on Ubuntu 24.04 LTS. The unit tests were run on these OSs with the latest version of dotnet 10.0.x and they were all successful:
Windows family: 
    OS Name:                       Microsoft Windows Server 2025 Datacenter
    OS Version:                    10.0.26100 N/A Build 26100
    BIOS Version:                  Microsoft Corporation Hyper-V UEFI Release v4.1, 1/8/2026

MacOS:
    ProductName:		macOS
    ProductVersion:		15.7.7
    BuildVersion:		24G720

# Where to ask for help

# Future developments

# How to contribute

# Authors
Giacomo Beretta, Simone Selmi

# License
See the file LICENSE.md

# State of the project

