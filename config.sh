# Parameters used if not specified during the execution of raytracer.sh

# Default Common options
outputpfm="scene_test.pfm" #not used by pfmtopng
outputpng="prova.png"
luminosityfunction="shirley"
averageluminosity=""
factor=1
gamma=1

# Default render 
inputscene="scene.txt"
width=500
height=500
sampleside=1
algorithm="flat"
numrays=10 #only for pathtracing
maxdepth=2 #only for pathtracing
initstate=(45) #only for pathtracing
initseq=(54) #only for pathtracing
roulettestart=3 #only for pathtracing
rouletteprob="" #only for pathtracing
declarefloat=()

pcgcycle=false #only for pathtracing

# Default pfmtopng
inputpfm="dome.pfm"

