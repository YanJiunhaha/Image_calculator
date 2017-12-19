from tkinter import *
from tkinter import filedialog
from tkinter import messagebox
from PIL import Image,ImageTk
import numpy as np
root=Tk()
root.title('Image project 8')
root.geometry('830x700')

bf=Frame(root)
bf.place(x=10,y=10)

b0=Button(bf,text='Open Picture')
b0.grid(row=0,column=0)

axisX=IntVar()
axisX.set(150)
e0=Entry(bf,textvariable=axisX,width=5)
e0.grid(row=0,column=1)
e0.config(state='disable')

axisY=IntVar()
axisY.set(200)
e1=Entry(bf,textvariable=axisY,width=5)
e1.grid(row=0,column=2)
e1.config(state='disable')

b1=Button(bf,text='8*8 DCT')
b1.grid(row=0,column=3)
b1.config(state='disable')

b2=Button(bf,text='General Wavelet form')
b2.grid(row=0,column=4)
b2.config(state='disable')

Label(text='Picture :').place(x=10,y=50)
iL0=Label()
iL0.place(x=10,y=70)
Label(text='Picture :').place(x=420,y=50)
iL1=Label()
iL1.place(x=420,y=70)
Label(text='').place(x=10,y=380)
iL2=Label()
iL2.place(x=10,y=400)
Label(text='General Wavelet form :').place(x=420,y=380)
iL3=Label()
iL3.place(x=420,y=400)

def test():
    arr=np.array([[[139],[148],[150],[149],[155],[164],[165],[168]],
                  [[98],[115],[130],[135],[143],[146],[142],[147]],
                  [[89],[110],[125],[128],[129],[121],[104],[106]],
                  [[96],[116],[128],[132],[134],[132],[113],[109]],
                  [[111],[125],[127],[131],[137],[137],[120],[100]],
                  [[122],[126],[126],[131],[133],[131],[126],[112]],
                  [[133],[134],[136],[138],[140],[144],[141],[139]],
                  [[138],[139],[139],[139],[140],[146],[148],[147]]])
    return arr
def OpenFile():
    global imtk0,imtk1,arr0,arr1,im0
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
                G=(int(arr0[x][y][2])+int(arr0[x][y][1])+int(arr0[x][y][0]))/3
                arr0[x][y]=[G,G,G]
        im0=Image.fromarray(arr0)
        imtk0=ImageTk.PhotoImage(im0)
        region=(50,0,350,300)
        im1=im0.crop(region)
        imtk1=ImageTk.PhotoImage(im1)
        iL1.config(image=imtk1)
        iL0.config(image=imtk0)
        b1.config(state='normal')
        b2.config(state='normal')
        e0.config(state='normal')
        e1.config(state='normal')
b0.config(command=OpenFile)

def DCT():
    global imtk0
    _N=8
    arr0=np.array(im0)
    if axisX.get()<=400 and axisX.get()>=0 and axisY.get()<=300 and axisY.get()>=0:
        arr=np.zeros((_N,_N))
        arr_out=np.zeros((_N,_N))
        for x in range(_N):
            for y in range(_N):
                arr[x][y]=arr0[axisX.get()+x][axisY.get()+y][0]
                if x==0 or y==0 or x==7 or y==7:
                    arr0[axisX.get()+x][axisY.get()+y][0]=255
        print('input image :')
        arr=test()
        print(arr)
        arr=arr-128
        print(arr)
        print(arr.sum()*((1/8)**0.5)**2)
        for x in range(_N):
            for y in range(_N):
                if x==0:
                    alphaU=(1/_N)**0.5
                else:
                    alphaU=(2/_N)**0.5
                if y==0:
                    alphaV=(1/_N)**0.5
                else:
                    alphaV=(2/_N)**0.5
                sumi=0
                for i in range(_N):
                    sumk=0
                    for k in range(_N):
                        sumk=sumk+arr[i][k]*np.cos((2*i+1)*x*np.pi/(_N*2))*np.cos((2*k+1)*y*np.pi/(_N*2))
                    sumi=sumi+sumk
                arr_out[x][y]=int(sumi*alphaU*alphaV)
        
        print('DCT :')
        print(arr_out)
    imtk0=ImageTk.PhotoImage(Image.fromarray(arr0))
    iL0.config(image=imtk0)
        
b1.config(command=DCT)


















root.mainloop()
