#!/bin/bash

while [[ $# -gt 0 ]]; do
  case "$1" in
    --input) input="$2"; shift 2 ;;
    --width) width="$2"; shift 2 ;;
    --height) height="$2"; shift 2 ;;
    --algorithm) algorithm="$2"; shift 2 ;;
    --output-pfm) outputpfm="$2"; shift 2 ;;
    --output-png) outputpng="$2"; shift 2 ;;
    --num-rays) numrays="$2"; shift 2 ;;
    --max-depth) maxdepth="$2"; shift 2 ;;
    --init-state) initstate="$2"; shift 2 ;;
    --init-seq) initseq="$2"; shift 2 ;;
    --sample-side) sampleside="$2"; shift 2 ;;
    --luminosityFunction) lumfunction="$2"; shift 2 ;;
    --factor) factor="$2"; shift 2 ;;
    --gamma) gamma="$2"; shift 2 ;;
    --roulette-start) roulette_start="$2"; shift 2 ;;
    --roulette-prob) roulette_prob="$2"; shift 2 ;;
    --declare-float) declare_float="$2"; shift 2 ;;
    *) echo "Unknown parameter: $1"; exit 1 ;;
  esac
done

#echo "your parameters are:"
#echo "input=$input"
#echo "width=$width"
#echo "height=$height"
#echo "algorithm=$algorithm"
#echo "outputpfm=$outputpfm"
#echo "outputpng=$outputpng"
#echo "numrays=$numrays"
#echo "maxdepth=$maxdepth"
#echo "initstate=$initstate"
#echo "initseq=$initseq"
#echo "sampleside=$sampleside"
#echo "lumfunction=$lumfunction"
#echo "factor=$factor"
#echo "gamma=$gamma"
#echo "roulette_start=$roulette_start"
#echo "roulette_prob=$roulette_prob"
#echo "declare_float=$declare_float"

readonly exePath="./RayTracer/bin/Debug/net10.0/RayTracer"

#echo "DEBUG COMMAND"

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
