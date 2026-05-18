#!/bin/bash

if [ $# -lt 5 ]; then
    echo "Usage: $(basename $0) WIDTH HEIGHT ALGORITHM THETA PHI ORTHOGONAL [FACTOR] [GAMMA]"
    exit 1
fi

readonly width="$1"
readonly height="$2"
readonly algorithm="$3"
readonly theta="$4"
readonly phi="$5"
readonly orthogonal="$6"
readonly factor="${7:-1}"
readonly gamma="${8:-1}"

echo "your parameters are:"
echo "width=$width"
echo "height=$height"
echo "algorithm=$algorithm"
echo "theta=$theta"
echo "phi=$phi"
echo "orthogonal=$orthogonal"
echo "factor=$factor"
echo "gamma=$gamma"

case "$algorithm" in
    onoff)
        algo_arg="OnOff"
        ;;
    flat)
        algo_arg="Flat"
        ;;
    *)
        echo "ERROR: invalid algorithm '$algorithm' (allowed: onoff, flat)"
        exit 1
        ;;
esac


readonly thetaNNN=$(printf "%03d" "$theta")
readonly phiNNN=$(printf "%03d" "$phi")

readonly pngfile="image_theta${thetaNNN}_phi${phiNNN}.png"
readonly exePath="./RayTracer/bin/Debug/net10.0/RayTracer"

echo "DEBUG COMMAND:"
echo "$exePath Demo --width $width --height $height --output $pngfile --algorithm $algo_arg --theta $theta --phi $phi --factor $factor --gamma $gamma"

if [ -z "$orthogonal" ]; then
    time "$exePath" demo \
    	--width "$width" \
    	--height "$height" \
    	--algorithm "$algo_arg" \
        --output "$pngfile" \
        --theta "$theta" \
        --phi "$phi" \
        --factor "$factor" \
        --gamma "$gamma"
else
    time "$exePath" demo \
    	--width "$width" \
    	--height "$height" \
    	--algorithm "$algo_arg" \
        --output "$pngfile" \
        --theta "$theta" \
        --phi "$phi" \
        --orthogonal \
        --factor "$factor" \
        --gamma "$gamma"
fi
