#!/bin/bash

if [ $# -lt 4 ]; then
    echo "Usage: $(basename $0) WIDTH HEIGHT THETA PHI [PROJECTION] [LUMFUNCTION] [FACTOR] [GAMMA]"
    exit 1
fi

readonly width="$1"
readonly height="$2"
readonly theta="$3"
readonly phi="$4"
readonly projection="${5:-"perspective"}"
readonly lumfunction="${6:-"Shirley"}"
readonly factor="${7:-1}"
readonly gamma="${8:-1}"

#echo "your parameters are:"
#echo "width=$width"
#echo "height=$height"
#echo "theta=$theta"
#echo "phi=$phi"
#echo "projection=$projection"
#echo "lumfunction=$lumfunction"
#echo "factor=$factor"
#echo "gamma=$gamma"

readonly thetaNNN=$(printf "%03d" "$theta")
readonly phiNNN=$(printf "%03d" "$phi")

readonly pngfile="image_theta${thetaNNN}_phi${phiNNN}.png"
readonly exePath="./RayTracer/bin/Debug/net10.0/RayTracer"

#echo outputFilePath="/DemoImages/image_theta${thetaNNN}_phi${phiNNN}.png"

echo "DEBUG COMMAND"
echo "$exePath demo --width "$width" --height "$height" --output "$pngfile" --theta "$theta" --phi "$phi" \
  --projection "$projection" --factor "$factor" --gamma "$gamma""

time "$exePath" demo \
  --width "$width" \
  --height "$height" \
  --output "$pngfile" \
  --theta "$theta" \
  --phi "$phi" \
  --projection "$projection" \
  --factor "$factor" \
  --gamma "$gamma"