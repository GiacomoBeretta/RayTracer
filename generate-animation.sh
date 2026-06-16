#!/bin/bash

# -r 25: Number of frames per second
ffmpeg -r 25 -f image2 -s 500x500 -i ./PngImages/frame_%03d.png \
    -vcodec libx264 -pix_fmt yuv420p \
    ./Animations/Animation_test.mp4
