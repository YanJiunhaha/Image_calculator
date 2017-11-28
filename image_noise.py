from tkinter import *
from tkinter import filedialog
from PIL import Image,ImageTk
import numpy as np
root=Tk()
root.title('Image project 4')
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
Label(text='normal noise :').place(x=420,y=50)
iL1=Label()
iL1.place(x=420,y=70)
Label(text='uniform noise :').place(x=10,y=380)
iL2=Label()
iL2.place(x=10,y=400)
Label(text='impulse noise :').place(x=420,y=380)
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
        imtk0=ImageTk.PhotoImage(im0)
        iL0.config(image=imtk0)
        b1.config(state='normal')
b0.config(command=OpenFile)

def normal_noise(mean=0,variance=1,probability=0.2):
    global imtk1,iL1,arr0
    arr1=np.array(arr0)
    for x in range(len(arr0)):
        for y in range(len(arr0[0])):
            if np.random.rand()<probability:#課本設定發生機率20%
                r=np.random.normal(mean,variance)
                #=np.random.rand()
                #heta=np.random.rand()
                #=variance*np.cos(theta*2*np.pi)*(-np.log(u))**0.5+mean
                r=min(127,r)
                r=max(-128,r)
                for i in range(3):
                    noise=arr0[x][y][i]+r
                    noise=min(noise,255)
                    noise=max(noise,0)
                    arr1[x][y][i]=noise
    imtk1=ImageTk.PhotoImage(Image.fromarray(arr1))
    iL1.config(image=imtk1)
    
def uniform_noise(low=0,high=255,probability=0.2):
    global imtk2,iL2,arr0
    arr2=np.array(arr0)
    for x in range(len(arr0)):
        for y in range(len(arr0[0])):
            if np.random.rand()<probability:#課本設定發生機率20%
                r=np.random.uniform(low,high)
                for i in range(3):
                    noise=arr0[x][y][i]+r
                    noise=min(noise,255)
                    noise=max(noise,0)
                    arr2[x][y][i]=noise
    imtk2=ImageTk.PhotoImage(Image.fromarray(arr2))
    iL2.config(image=imtk2)
    
def impulse_noise(probability=0.2):
    global imtk3,iL3,arr0
    arr3=np.array(arr0)
    for x in range(len(arr0)):
        for y in range(len(arr0[0])):
            if np.random.rand()<probability/2: #課本設定發生機率10%
                arr3[x][y]=[0,0,0]
    for x in range(len(arr0)):
        for y in range(len(arr0[0])):
            if np.random.rand()<probability/2: #課本設定發生機率10%
                arr3[x][y]=[255,255,255]
    imtk3=ImageTk.PhotoImage(Image.fromarray(arr3))
    iL3.config(image=imtk3)
    
def calculator():
    normal_noise(0,60,0.2)
    uniform_noise(50,120,0.2)
    impulse_noise(0.2)
b1.config(command=calculator)

root.mainloop()
