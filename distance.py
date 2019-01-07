#!/usr/bin/python3
from PIL import Image
import numpy as np

im = Image.open('/home/kuasnclab/Downloads/Morphology Test Images/art5.gif')
arr = np.array(im)
#im.show()
for y in range(im.height):
	for x in range(im.width):
		c=0
		#(arr[y][x][0]==0)?0:1
		if(arr[y][x]==1):
			c=1
		arr[y][x]=c
for y in range(im.height):
	for x in range(im.width):
		if(y>0 and x>0 and x<(im.width-1) and y<(im.height-1) and arr[y][x]==1):
			mask=[]
			mask.append(arr[y-1][x-1])
			mask.append(arr[y-1][x])
			mask.append(arr[y-1][x+1])
			mask.append(arr[y][x-1])
			mask.sort()
			c=mask[0]+1
			arr[y][x]=c

for y in range(im.height-1,0,-1):
	for x in range(im.width-1,0,-1):
		if(y>0 and x>0 and x<(im.width-1) and y<(im.height-1)):
			mask=[]
			mask.append(arr[y][x])
			mask.append(arr[y][x+1]+1)
			mask.append(arr[y+1][x-1]+1)
			mask.append(arr[y+1][x]+1)
			mask.append(arr[y+1][x+1]+1)
			mask.sort()
			c=mask[0]
			arr[y][x]=c
im_n=Image.fromarray(arr,'L')
im_n.show()
