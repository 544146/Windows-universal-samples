#!/bin/sh

youtube-dl https://www.youtube.com/watch?v=FtutLA63Cp8 -o badapple.mp4
python3 generate_bad_apple_frames.py badapple.mp4 0.071 1.38738738739 ▥▥⬜⬜
