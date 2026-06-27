# Parameters used if not specified during the execution of raytracer.sh

# Global default 
luminosityfunction="shirley"
averageluminosity=""
factor=1
gamma=1
outputpfm="scene_test.pfm" #not used by pfmtopng
outputpng="scene_test.png"

# Default render
inputscene="scene.txt"
width=500
height=500
sampleside=1
algorithm="flat"
numrays=10
maxdepth=2
initstate=(45)
initseq=(54)
roulettestart=3
rouletteprob=""
declarefloat=()

# Default pfmtopng
inputpfm="dome.pfm"

# Cycle parameters
pcgcycle=false