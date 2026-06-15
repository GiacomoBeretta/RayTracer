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

Number of rays spawned from each surface interaction.

Used only when the selected algorithm is `pathtracer`.

#### `--maxdepth`

Maximum recursion depth for each ray.

Used only when the selected algorithm is `pathtracer`.

#### `--initstate`

Initial seed for the random number generator.

#### `--initseq`

Sequence identifier used by the random number generator.

#### `--sampleside`

Number of subdivisions per pixel side used for anti-aliasing.

#### `--luminosityfunction`

Luminosity sampling function.

Available options:

* `shirley`
* `weighted`

#### `--factor`

Empirical factor used during image rendering.

#### `--gamma`

Gamma correction factor applied to the final image.

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
This Program works on Ubuntu 24.04 LTS. All tests are successful also MacOS 15.7.7. It's not guaranteed to work on Windows.

# Where to ask for help

# Future developments

# How to contribute

# Authors
Giacomo Beretta, Simone Selmi

# License
See the file LICENSE.md

# State of the project
