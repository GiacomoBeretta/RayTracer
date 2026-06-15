#!/bin/bash

while [[ $# -gt 0 ]] do
	case "$1" in
	--input) input="$2" ; shift 2 ;;
	--output) output="$2"; shift 2 ;;
	--luminosityFunction) lumfunction="$2" ; shift 2 ;;
	--factor) factor="$2" ; shift 2 ;;
	--gamma) gamma="$2" ; shift 2 ;;
	*) echo "Unknown parameter: $1" ; exit 1 ;;
	esac
done

readonly exepath="./RayTracer/bin/Debug/net10.0/RayTracer"

dotnet build

cmd=( "$exepath" pfmtopng )

[ -n "$input" ]       && cmd+=( --input "$input" ) 
[ -n "$output" ]      && cmd+=( --output "$output" ) 
[ -n "$lumfunction" ] && cmd+=( --luminosityFunction "$lumfunction" ) 
[ -n "$factor" ]      && cmd+=( --factor "$factor" ) 
[ -n "$gamma" ]       && cmd+=( --gamma "$gamma" ) 

time "${cmd[@]}"
