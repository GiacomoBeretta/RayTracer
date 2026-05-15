#!/bin/bash

# -r 25: Number of frames per second
ffmpeg -r 25 -f image2 -s 500x500 -i ./Images/image_theta000_phi%03d.png \
    -vcodec libx264 -pix_fmt yuv420p \
    ./Animations/spheres-perspective.mp4
