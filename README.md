# PixelBattle

This code generate a cool shot with HLS Unity compute shaders. It work as follows: every pixel in the image has a probability of changing color proportional to its 8 neighbors (both vertical, horizontal, and diagonal). So if a pixel has 4 neighboring pixels of the opposite color the probability it will flip color is 4/8 = 50%, if it's totally surrounded by squares of the opposite color it has a probability of flipping color of 8/8 = 100%.


To try this on your pc open a new Unity project and insert the compute shader and the c# script of this repository. Then attach the c# script to the main camera and assign the compute shader to the "Battleshader" public variable in the inspector. Optimal settings are:

Render Texture Dimensions: 1024, 1024

Grid Size: 1000

Colors: wathever you want
