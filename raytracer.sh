#!/bin/bash

subcommand="$1"
shift

case "$subcommand" in 
	render|pfmtopng)
		;;
	*)
		echo "Use: $0 {render|pfmtopng} [options]"
		exit 1
		;;
esac

while [[ $# -gt 0 ]]; do
  case "$1" in
    --input) input="$2"; shift 2 ;;
    --width) width="$2"; shift 2 ;;
    --height) height="$2"; shift 2 ;;
    --algorithm) algorithm="$2"; shift 2 ;;
    --outputpfm) outputpfm="$2"; shift 2 ;;
    --outputpng) outputpng="$2"; shift 2 ;;
    --numrays) numrays="$2"; shift 2 ;;
    --maxdepth) maxdepth="$2"; shift 2 ;;
    --initstate) initstate="$2"; shift 2 ;;
    --initseq) initseq="$2"; shift 2 ;;
    --sampleside) sampleside="$2"; shift 2 ;;
    --luminosityFunction) lumfunction="$2"; shift 2 ;;
    --factor) factor="$2"; shift 2 ;;
    --gamma) gamma="$2"; shift 2 ;;
    --roulettestart) roulette_start="$2"; shift 2 ;;
    --rouletteprob) roulette_prob="$2"; shift 2 ;;
    --declarefloat) declare_float="$2"; shift 2 ;;
    --output) output="$2"; shift 2 ;;
    *) echo "Unknown parameter: $1"; exit 1 ;;
  esac
done

readonly exepath="./RayTracer/bin/Debug/net10.0/RayTracer"

cmd=( "$exepath" "$subcommand" )

if [[ "$subcommand" == "render" ]]; then
	[ -n "$input" ]          && cmd+=( --input "$input" )
	[ -n "$width" ]          && cmd+=( --width "$width" )
	[ -n "$height" ]         && cmd+=( --height "$height" )
	[ -n "$algorithm" ]      && cmd+=( --algorithm "$algorithm" )
	[ -n "$outputpfm" ]      && cmd+=( --outputpfm "$outputpfm" )
	[ -n "$outputpng" ]      && cmd+=( --outputpng "$outputpng" )
	[ -n "$numrays" ]        && cmd+=( --numrays "$numrays" )
	[ -n "$maxdepth" ]       && cmd+=( --maxdepth "$maxdepth" )
	[ -n "$initstate" ]      && cmd+=( --initstate "$initstate" )
	[ -n "$initseq" ]        && cmd+=( --initseq "$initseq" )
	[ -n "$sampleside" ]     && cmd+=( --sampleside "$sampleside" )
	[ -n "$lumfunction" ]    && cmd+=( --luminosityFunction "$lumfunction" )
	[ -n "$factor" ]         && cmd+=( --factor "$factor" )	
	[ -n "$gamma" ]          && cmd+=( --gamma "$gamma" )
	[ -n "$roulette_start" ] && cmd+=( --roulettestart "$roulette_start" )
	[ -n "$roulette_prob" ]  && cmd+=( --rouletteprob "$roulette_prob" )
	[ -n "$declare_float" ]  && cmd+=( --declarefloat "$declare_float" )
fi

if [[ "$subcommand" == "pfmtopng" ]]; then
	[ -n "$input" ]       && cmd+=( --input "$input" ) 
	[ -n "$output" ]      && cmd+=( --output "$output" ) 
	[ -n "$lumfunction" ] && cmd+=( --luminosityFunction "$lumfunction" ) 
	[ -n "$factor" ]      && cmd+=( --factor "$factor" ) 
	[ -n "$gamma" ]       && cmd+=( --gamma "$gamma" ) 
fi

dotnet build

time "${cmd[@]}"



