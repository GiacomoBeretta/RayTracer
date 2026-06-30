# Parameters used if not specified during the execution of raytracer.sh

# Default Common options
outputpfm="demo.pfm" #not used by pfmtopng
outputpng="demo.png"
luminosityfunction="shirley"
averageluminosity=""
factor=1
gamma=1

# Default render 
inputscene="scene.txt"
width=500
height=500
sampleside=1
algorithm="pathtracing"
numrays=10 #only for pathtracing
maxdepth=2 #only for pathtracing
initstate=45 #only for pathtracing
initseq=54 #only for pathtracing
roulettestart=3 #only for pathtracing
rouletteprob=0.5 #only for pathtracing
declarefloat=()

pcgcycle=false #only for pathtracing

# Default pfmtopng
inputpfm="dome.pfm"