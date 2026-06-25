#!/bin/bash

# Default parameters

source config.sh

# Command

subcommand="$1"
shift

case "$subcommand" in 
	render|pfmtopng|averageimage)
		;;
	*)
		echo "Use: $0 {render|pfmtopng|averageimage} [options]"
		exit 1
		;;
esac

# Override command line

while [[ $# -gt 0 ]]; do
  case "$1" in
    --inputrender) inputrender="$2"; shift 2 ;;
    --width) width="$2"; shift 2 ;;
    --height) height="$2"; shift 2 ;;
    --algorithm) algorithm="$2"; shift 2 ;;
    --outputpfm) outputpfm="$2"; shift 2 ;;
    --outputpng) outputpng="$2"; shift 2 ;;
    --numrays) numrays="$2"; shift 2 ;;
    --maxdepth) maxdepth="$2"; shift 2 ;;
    --initstate) initstate+=( "$2" ); shift 2 ;;
    --initseq) initseq+=( "$2" ); shift 2 ;;
    --sampleside) sampleside="$2"; shift 2 ;;
    --luminosityfunction) lumfunction="$2"; shift 2 ;;
    --averageluminosity) averageluminosity="$2"; shift 2 ;; 
    --factor) factor="$2"; shift 2 ;;
    --gamma) gamma="$2"; shift 2 ;;
    --roulettestart) roulettestart="$2"; shift 2 ;;
    --rouletteprob) rouletteprob="$2"; shift 2 ;;
    --declarefloat) declarefloat+=( "$2" ); shift 2 ;;
    --inputpfm) inputpfm="$2"; shift 2 ;;
    --output) output="$2"; shift 2 ;;
    --outputaveragepfm) outputaveragepfm="$2"; shift 2 ;;
    --outputaveragepng) outputaveragepng="$2"; shift 2 ;;
    *) echo "Unknown parameter: $1"; exit 1 ;;
  esac
done

# Base command

readonly exepath="./RayTracer/bin/Debug/net10.0/RayTracer"

cmd=( "$exepath" "$subcommand" )

# Render

if [[ "$subcommand" == "render" ]]; then
	[ -n "$inputrender" ]       && cmd+=( --inputrender "$inputrender" )
	[ -n "$width" ]             && cmd+=( --width "$width" )
	[ -n "$height" ]            && cmd+=( --height "$height" )
	[ -n "$algorithm" ]         && cmd+=( --algorithm "$algorithm" )
	[ -n "$numrays" ]           && cmd+=( --numrays "$numrays" )
	[ -n "$maxdepth" ]          && cmd+=( --maxdepth "$maxdepth" )
	[ -n "$sampleside" ]        && cmd+=( --sampleside "$sampleside" )
	[ -n "$lumfunction" ]       && cmd+=( --luminosityfunction "$lumfunction" )
	[ -n "$averageluminosity" ] && cmd+=( --averageluminosity "$averageluminosity" )
	[ -n "$factor" ]            && cmd+=( --factor "$factor" )	
	[ -n "$gamma" ]             && cmd+=( --gamma "$gamma" )
	[ -n "$roulettestart" ]     && cmd+=( --roulettestart "$roulettestart" )
	[ -n "$rouletteprob" ]      && cmd+=( --rouletteprob "$rouletteprob" )
	for def in "${declarefloat[@]}"; do
        cmd+=( --declarefloat "$def" )
    	done
fi

# Pfm to Png

if [[ "$subcommand" == "pfmtopng" ]]; then
	[ -n "$inputpfm" ]          && cmd+=( --inputpfm "$inputpfm" ) 
	[ -n "$output" ]            && cmd+=( --output "$output" ) 
	[ -n "$lumfunction" ]       && cmd+=( --luminosityfunction "$lumfunction" ) 
	[ -n "$averageluminosity" ] && cmd+=( --averageluminosity "$averageluminosity" )
	[ -n "$factor" ]            && cmd+=( --factor "$factor" ) 
	[ -n "$gamma" ]             && cmd+=( --gamma "$gamma" ) 
fi

if [[ "$subcommand" == "averageimage" ]]; then
	[ -n "$outputaveragepfm" ]     && cmd+=( --outputaveragepfm "$outputaveragepfm" ) 
	[ -n "$outputaveragepng" ]     && cmd+=( --outputaveragepng "$outputaveragepng" ) 
	[ -n "$lumfunction" ]          && cmd+=( --luminosityfunction "$lumfunction" ) 
	[ -n "$averageluminosity" ]    && cmd+=( --averageluminosity "$averageluminosity" )
	[ -n "$factor" ]               && cmd+=( --factor "$factor" ) 
	[ -n "$gamma" ]                && cmd+=( --gamma "$gamma" ) 
fi

# Build

dotnet build || exit 1

# Random generator cycle

if [[ "$subcommand" == "render" ]]; then
	 if [[ "$pcgcycle" == "true" ]]; then

		for state in "${initstate[@]}"; do
			for seq in "${initseq[@]}"; do

				run_cmd=( "${cmd[@]}" )

				[ -n "$state" ] && run_cmd+=( --initstate "$state" )
				[ -n "$seq" ]   && run_cmd+=( --initseq "$seq" )
				
				pfm_name="${outputpfm%.pfm}_state${state}_seq${seq}.pfm"
				run_cmd+=( --outputpfm "$pfm_name" )
			
				png_name="${outputpng%.png}_state${state}_seq${seq}.png"
				run_cmd+=( --outputpng "$png_name" )

				time "${run_cmd[@]}"

			done

		done

	else

		run_cmd=( "${cmd[@]}" )

		[ -n "${initstate[0]}" ] && run_cmd+=( --initstate "${initstate[0]}" )
		[ -n "${initseq[0]}" ]   && run_cmd+=( --initseq "${initseq[0]}" )
		
		[ -n "$outputpfm" ]      && run_cmd+=( --outputpfm "$outputpfm" )
		[ -n "$outputpng" ]      && run_cmd+=( --outputpng "$outputpng" )

		time "${run_cmd[@]}"

	fi

else

	time "${cmd[@]}"

fi

# seq -w 0 359 | parallel -j 5 ./raytracer.sh render --declarefloat clock:{} --outputpfm frame_{}.pfm --outputpng frame_{}.png



