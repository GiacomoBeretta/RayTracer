#!/bin/bash

if [ $# -lt 4 ]; then
    echo "Usage: $(basename $0) WIDTH HEIGHT THETA PHI [ORTHOGONAL] [FACTOR] [GAMMA]"
    exit 1
fi

readonly width="$1"
readonly height="$2"
readonly theta="$3"
readonly phi="$4"
readonly orthogonal="$5"
readonly factor="$6"
readonly gamma="$7"

echo your parameters are:
echo width=$width
echo height=$height
echo theta=$theta
echo phi=$phi
echo orthogonal=$orthogonal
echo factor=$factor
echo gamma=$gamma

readonly thetaNNN=$(printf "%03d" $theta)
readonly phiNNN=$(printf "%03d" $phi)
#readonly pfmfile=image$angleNNN.pfm
readonly pngfile=image_theta${thetaNNN}_phi${phiNNN}.png
readonly exePath=./RayTracer/bin/Debug/net10.0/RayTracer

if [ -z "$orthogonal" ]; then
    echo "$exePath demo $width $height" --output=$pngfile --theta=$theta --phi=$phi --factor=$factor --gamma=$gamma
    time $exePath demo $width $height --output=$pngfile --theta=$theta --phi=$phi --factor=$factor --gamma=$gamma
else
    echo "$exePath demo $width $height" --output=$pngfile --theta=$theta --phi=$phi --orthogonal --factor=$factor --gamma=$gamma 
    time $exePath demo $width $height --output=$pngfile --theta=$theta --phi=$phi --orthogonal --factor=$factor --gamma=$gamma
fi


