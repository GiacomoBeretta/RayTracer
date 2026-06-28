#!/bin/bash

# Default parameters

source config.sh

# Command

subcommand="$1"
shift

case "$subcommand" in 
	render|pfmtopng|averageimages)
		;;
	*)
		echo "Use: $0 {render|pfmtopng|averageimages} [options]"
		exit 1
		;;
esac

# Set default values for output file names
#case "$subcommand" in
#	render)
#		outputpfm="${outputrenderpfm}"
#		outputpng="${outputrenderpng}"
#		;;
#	averageimages)
#		outputpfm="${averagepfm}"
#		outputpng="${averagepng}"
#		;;
#esac

# Base command

readonly exepath="./RayTracer/bin/Debug/net10.0/RayTracer"

cmd=( "$exepath" "$subcommand" )

# Override command line

while [[ $# -gt 0 ]]; do
  case "$1" in
    --inputscene) inputscene="$2"; shift 2 ;;
	--outputpfm) outputpfm="$2"; shift 2 ;;
    --outputpng) outputpng="$2"; shift 2 ;;
    --width) width="$2"; shift 2 ;;
    --height) height="$2"; shift 2 ;;
	--sampleside) sampleside="$2"; shift 2 ;;
    --algorithm) algorithm="$2"; shift 2 ;;
    --numrays) numrays="$2"; shift 2 ;;
    --maxdepth) maxdepth="$2"; shift 2 ;;
    --initstate) initstate+=( "$2" ); shift 2 ;;
    --initseq) initseq+=( "$2" ); shift 2 ;;
    --roulettestart) roulettestart="$2"; shift 2 ;;
    --rouletteprob) rouletteprob="$2"; shift 2 ;;
    --luminosityfunction) luminosityfunction="$2"; shift 2 ;;
    --averageluminosity) averageluminosity="$2"; shift 2 ;; 
    --factor) factor="$2"; shift 2 ;;
    --gamma) gamma="$2"; shift 2 ;;
    --declarefloat) declarefloat+=( "$2" ); shift 2 ;;
    --inputpfm) inputpfm="$2"; shift 2 ;;
#    --output) output="$2"; shift 2 ;;
#    --outputaveragepfm) outputaveragepfm="$2"; shift 2 ;;
#    --outputaveragepng) outputaveragepng="$2"; shift 2 ;;
	--pcgcycle) pcgcycle="$2"; shift 2 ;;
    *) 
		echo "Unknown parameter: $1"
		echo "the available options are listed below, executing the help of the render command:"
		cmd+=( --help)
		echo "${cmd[@]}"
		"${cmd[@]}"
		exit 1 ;;
  esac
done

# Build

# dotnet build || exit 1

# Render
# (the outputpfm and outputpng, initstate and initseq options are written later)

if [[ "$subcommand" == "render" ]]; then
	[ -n "$inputscene" ]  	      && cmd+=( --inputscene "$inputscene" )
	[ -n "$width" ]          	  && cmd+=( --width "$width" )
	[ -n "$height" ]           	  && cmd+=( --height "$height" )
	[ -n "$sampleside" ]     	  && cmd+=( --sampleside "$sampleside" )
	[ -n "$algorithm" ]           && cmd+=( --algorithm "$algorithm" )
	[ -n "$numrays" ]        	  && cmd+=( --numrays "$numrays" )
	[ -n "$maxdepth" ]			  && cmd+=( --maxdepth "$maxdepth" )
	[ -n "$roulettestart" ]       && cmd+=( --roulettestart "$roulettestart" )
	[ -n "$rouletteprob" ]     	  && cmd+=( --rouletteprob "$rouletteprob" )
	[ -n "$luminosityfunction" ]  && cmd+=( --luminosityfunction "$luminosityfunction" )
	[ -n "$averageluminosity" ]   && cmd+=( --averageluminosity "$averageluminosity" )
	[ -n "$factor" ]           	  && cmd+=( --factor "$factor" )	
	[ -n "$gamma" ]            	  && cmd+=( --gamma "$gamma" )

	for def in "${declarefloat[@]}"; do
        cmd+=( --declarefloat "$def" )
    	done
fi

# Average images

if [[ "$subcommand" == "averageimages" ]]; then
	[ -n "$outputpfm" ]  		   && cmd+=( --outputpfm "$outputpfm" ) 
	[ -n "$outputpng" ]   		   && cmd+=( --outputpng "$outputpng" ) 
	[ -n "$luminosityfunction" ]   && cmd+=( --luminosityfunction "$luminosityfunction" ) 
	[ -n "$averageluminosity" ]    && cmd+=( --averageluminosity "$averageluminosity" )
	[ -n "$factor" ]               && cmd+=( --factor "$factor" ) 
	[ -n "$gamma" ]                && cmd+=( --gamma "$gamma" ) 
fi

# Pfm to Png

if [[ "$subcommand" == "pfmtopng" ]]; then
	[ -n "$inputpfm" ]        		&& cmd+=( --inputpfm "$inputpfm" ) 
	[ -n "$outputpng" ]       		&& cmd+=( --outputpng "$outputpng" ) 
	[ -n "$luminosityfunction" ]	&& cmd+=( --luminosityfunction "$luminosityfunction" ) 
	[ -n "$averageluminosity" ]		&& cmd+=( --averageluminosity "$averageluminosity" )
	[ -n "$factor" ]         		&& cmd+=( --factor "$factor" ) 
	[ -n "$gamma" ]          		&& cmd+=( --gamma "$gamma" ) 
fi

# Render: outputpfm, outputpng, initstate and initseq options

if [[ "$subcommand" == "render" ]]; then

	# Random generator cycle
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

		[ -n "${initstate[0]}" ] && cmd+=( --initstate "${initstate[0]}" )
		[ -n "${initseq[0]}" ]   && cmd+=( --initseq "${initseq[0]}" )
		
		[ -n "$outputpfm" ]      && cmd+=( --outputpfm "$outputpfm" )
		[ -n "$outputpng" ]      && cmd+=( --outputpng "$outputpng" )

		time "${cmd[@]}"

#		run_cmd=( "${cmd[@]}" )

#		[ -n "${initstate[0]}" ] && run_cmd+=( --initstate "${initstate[0]}" )
#		[ -n "${initseq[0]}" ]   && run_cmd+=( --initseq "${initseq[0]}" )
		
#		[ -n "$outputpfm" ]      && run_cmd+=( --outputpfm "$outputpfm" )
#		[ -n "$outputpng" ]      && run_cmd+=( --outputpng "$outputpng" )

#		time "${run_cmd[@]}"

	fi

else

	time "${cmd[@]}"

fi

# seq -w 0 359 | parallel -j 5 ./raytracer.sh render --declarefloat clock:{} --outputpfm frame_{}.pfm --outputpng frame_{}.png



