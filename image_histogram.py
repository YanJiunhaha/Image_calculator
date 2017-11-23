from tkinter import *
from tkinter import filedialog
from PIL import Image,ImageTk
import numpy as np
root=Tk()
root.title('Image project 3')
root.geometry('830x700')

bf=Frame(root)
bf.place(x=10,y=10)

b0=Button(bf,text='Open Picture')
b0.grid(row=0,column=0)

b1=Button(bf,text='Calculator')
b1.grid(row=0,column=1)
b1.config(state='disable')

Label(text='Picture :').place(x=10,y=50)
iL0=Label()
iL0.place(x=10,y=70)
Label(text='Histogram Shrink :').place(x=420,y=50)
iL1=Label()
iL1.place(x=420,y=70)
Label(text='Histogram Stretch :').place(x=10,y=380)
iL2=Label()
iL2.place(x=10,y=400)
Label(text='Histogram Equalization :').place(x=420,y=380)
iL3=Label()
iL3.place(x=420,y=400)

def OpenFile():
    global imtk0,iL0,b1,arr0
    filename=filedialog.askopenfile()
    if filename is not None:
        im0=Image.open(filename.name)
        re=1
        if(im0.height>300):
            re=300/im0.height
        if(im0.width*re>400):
            re=400/im0.width
        im0=im0.resize((int(im0.width*re),int(im0.height*re)),Image.ANTIALIAS)
        arr0=np.array(im0)
        
        for x in range(len(arr0)):
            for y in range(len(arr0[0])):
                gray=int(arr0[x][y][2]/3+arr0[x][y][1]/3+arr0[x][y][0]/3)
                arr0[x][y]=[gray,gray,gray]
        imtk0=ImageTk.PhotoImage(Image.fromarray(arr0))
        iL0.config(image=imtk0)
        b1.config(state='normal')
b0.config(command=OpenFile)

def histogram(arr,min_change=0,max_change=255):
    minimum=255
    maximum=0
    for x in range(len(arr)):
        for y in range(len(arr[0])):
            if minimum > arr[x][y][0]:
                minimum=arr[x][y][0]
            if maximum <arr[x][y][0]:
                maximum=arr[x][y][0]
    
    ratio=float(max_change-min_change)/(maximum-minimum)
    
    for x in range(len(arr)):
        for y in range(len(arr[0])):
            if ratio < 1:
                R=arr[x][y][0]*ratio+min_change
            else:
                R=(arr[x][y][0]-minimum)*ratio+min_change
            arr[x][y]=[R,R,R]
def shrink():
    global imtk1,iL1,arr0,arr1
    histogram(arr0,100,200)
    arr1=np.array(arr0)
    imtk1=ImageTk.PhotoImage(Image.fromarray(arr0))
    iL1.config(image=imtk1)
def stretch():
    global imtk2,iL2,arr0,arr1
    histogram(arr0)
    imtk2=ImageTk.PhotoImage(Image.fromarray(arr0))
    iL2.config(image=imtk2)
def equalization():
    global imtk3,iL3,arr1
    probability=np.zeros((1,256),dtype=np.int32)
    for y in range(len(arr1)):
        for x in range(len(arr1[0])):
            probability[0][int(arr1[y][x][0])]=1+probability[0][int(arr1[y][x][0])]

    
def run():
    shrink()
    stretch()
    equalization()
b1.config(command=run)      
root.mainloop()
