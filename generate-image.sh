#!/bin/bash

if [ $# -lt 5 ]; then
    echo "Usage: $(basename $0) WIDTH HEIGHT ALGORITHM THETA PHI [PROJECTION] [LUMINOSITY FUNCTION] [FACTOR] [GAMMA]"
    exit 1
fi

readonly width="$1"
readonly height="$2"
readonly algorithm="${3:-"onoff"}"
readonly theta="$4"
readonly phi="$5"
readonly projection="${6:-"perspective"}"
readonly lumfunction="${7:-"Shirley"}"
readonly factor="${8:-1}"
readonly gamma="${9:-1}"

echo "your parameters are:"
echo "width=$width"
echo "height=$height"
echo "algorithm=$algorithm"
echo "theta=$theta"
echo "phi=$phi"
echo "projection=$projection"
echo "lumfunction=$lumfunction"
echo "factor=$factor"
echo "gamma=$gamma"

readonly thetaNNN=$(printf "%03d" "$theta")
readonly phiNNN=$(printf "%03d" "$phi")

readonly pngfile="image_theta${thetaNNN}_phi${phiNNN}.png"
readonly exePath="./RayTracer/bin/Debug/net10.0/RayTracer"
#echo outputFilePath="/DemoImages/image_theta${thetaNNN}_phi${phiNNN}.png"

echo "DEBUG COMMAND"
echo "$exePath demo --width "$width" --height "$height" --algorithm "$algorithm" --output "$pngfile" --theta "$theta" --phi "$phi" --projection "$projection" --factor "$factor" --gamma "$gamma""

dotnet build 
time "$exePath" demo \
  --width "$width" \
  --height "$height" \
  --algorithm "$algorithm" \
  --output "$pngfile" \
  --theta "$theta" \
  --phi "$phi" \
  --projection "$projection" \
  --factor "$factor" \
  --gamma "$gamma"
