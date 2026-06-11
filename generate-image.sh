#!/bin/bash

readonly input="$1"
readonly width="$2"
readonly height="$3"
readonly algorithm="$4"
readonly outputpfm="$5"
readonly outputpng="$6"
readonly numrays="$7"
readonly maxdepth="$8"
readonly initstate="$9"
readonly initseq="${10}"
readonly sampleside="${11}"
readonly lumfunction="${12}"
readonly factor="${13}"
readonly gamma="${14}"
readonly roulette_start="${15}"
readonly roulette_prob="${16}"
readonly declare_float="${17}"

echo "your parameters are:"
echo "input=$input"
echo "width=$width"
echo "height=$height"
echo "algorithm=$algorithm"
echo "outputpfm=$outputpfm"
echo "outputpng=$outputpng"
echo "numrays=$numrays"
echo "maxdepth=$maxdepth"
echo "initstate=$initstate"
echo "initseq=$initseq"
echo "sampleside=$sampleside"
echo "lumfunction=$lumfunction"
echo "factor=$factor"
echo "gamma=$gamma"
echo "roulette_start=$roulette_start"
echo "roulette_prob=$roulette_prob"
echo "declare_float=$declare_float"

readonly exePath="./RayTracer/bin/Debug/net10.0/RayTracer"

echo "DEBUG COMMAND"

cmd=( "$exePath" render )

[ -n "$input" ]          && cmd+=( --input "$input" )
[ -n "$width" ]          && cmd+=( --width "$width" )
[ -n "$height" ]         && cmd+=( --height "$height" )
[ -n "$algorithm" ]      && cmd+=( --algorithm "$algorithm" )
[ -n "$outputpfm" ]      && cmd+=( --output-pfm "$outputpfm" )
[ -n "$outputpng" ]      && cmd+=( --output-png "$outputpng" )
[ -n "$numrays" ]        && cmd+=( --num-rays "$numrays" )
[ -n "$maxdepth" ]       && cmd+=( --max-depth "$maxdepth" )
[ -n "$initstate" ]      && cmd+=( --init-state "$initstate" )
[ -n "$initseq" ]        && cmd+=( --init-seq "$initseq" )
[ -n "$sampleside" ]     && cmd+=( --sample-side "$sampleside" )
[ -n "$lumfunction" ]    && cmd+=( --luminosityFunction "$lumfunction" )
[ -n "$factor" ]         && cmd+=( --factor "$factor" )
[ -n "$gamma" ]          && cmd+=( --gamma "$gamma" )
[ -n "$roulette_start" ] && cmd+=( --roulette-start "$roulette_start" )
[ -n "$roulette_prob" ]  && cmd+=( --roulette-prob "$roulette_prob" )
[ -n "$declare_float" ]  && cmd+=( --declare-float "$declare_float" )

dotnet build

time "${cmd[@]}"
