from tkinter import *
from tkinter import filedialog
from PIL import Image,ImageTk
import numpy as np
root=Tk()
root.title('image project 1')
root.geometry('830x700')

bt=Button(text='Combine:kI1+mI2')
bt.config(state='disable')
bt.place(x=230,y=10)
string=['k=0.1','k=0.2','k=0.3','k=0.4','k=0.5','k=0.6','k=0.7','k=0.8','k=0.9']
sel=StringVar()
sel.set(string[0])
om=OptionMenu(root,sel,*string)
om.place(x=350,y=7)
om.config(state='disable')
Label(text='Image1:').place(x=10,y=40)
L0=Label()
L0.place(x=10,y=65)
Label(text='Image2:').place(x=420,y=40)
L1=Label()
L1.place(x=420,y=65)
Label(text='Y1_image-I1_image:').place(x=10,y=370)
L2=Label()
L2.place(x=10,y=400)
Label(text='kI1+mI2:').place(x=420,y=370)
L3=Label()
L3.place(x=420,y=400)

Fbool0=bool(0)
Fbool1=bool(0)
def OpenFile1():
    global L0,imtk0,arr0,Fbool0,Fbool1,L2,imtk2
    file=filedialog.askopenfile()
    if file is not None:
        Fbool0=bool(1)
        im=Image.open(file.name)  
        if(im.height>300):
            re=300/im.height
        if(im.width*re>400):
            re=400/im.width
        im=im.resize((int(im.width*re),int(im.height*re)),Image.ANTIALIAS)
        arr0=np.array(im)
        imtk0=ImageTk.PhotoImage(im)
        L0.config(image=imtk0)
        if Fbool0 and Fbool1:
            bt.config(state='normal')
            om.config(state='normal')

        arr2=np.array(im)
        height=int(arr2.size/arr2[0].size)
        width=int(arr2[0].size/arr2[0][0].size)
        for x in range(height):
            for y in range(width):
                Yimage=arr2[x][y][2]*0.299+arr2[x][y][1]*0.587+arr2[x][y][0]*0.114
                Iimage=arr2[x][y][2]*0.333+arr2[x][y][1]*0.333+arr2[x][y][0]*0.333
                D=Yimage-Iimage
                arr2[x][y]=[D,D,D]
        imtk2=ImageTk.PhotoImage(Image.fromarray(arr2))
        L2.config(image=imtk2)
Button(text='Open picture: I1',command=OpenFile1).place(x=10,y=10)

def OpenFile2():
    global L1,imtk1,arr1,Fbool0,Fbool1,arr3
    file=filedialog.askopenfile()
    if file is not None:
        Fbool1=bool(1)
        im=Image.open(file.name)  
        if(im.height>300):
            re=300/im.height
        if(im.width*re>400):
            re=400/im.width
        im=im.resize((int(im.width*re),int(im.height*re)),Image.ANTIALIAS)
        arr1=np.array(im)
        arr3=np.array(im)
        imtk1=ImageTk.PhotoImage(im)
        L1.config(image=imtk1)
        if Fbool0 and Fbool1:
            bt.config(state='normal')
            om.config(state='normal')
Button(text='Open picture: I2',command=OpenFile2).place(x=120,y=10)

def Combine():
    global L3,imtk3,arr0,arr1,sel,arr3,k,m
    height=int(arr3.size/arr3[0].size)
    width=int(arr3[0].size/arr3[0][0].size)
    k=0
    if sel.get() == 'k=0.1':
        k=0.1
    elif sel.get() == 'k=0.2':
        k=0.2
    elif sel.get() == 'k=0.3':
        k=0.3
    elif sel.get() == 'k=0.4':
        k=0.4
    elif sel.get() =='k=0.5':
        k=0.5
        print(0.5)
    elif sel.get() == 'k=0.6':
        k=0.6
    elif sel.get() == 'k=0.7':
        k=0.7
    elif sel.get() == 'k=0.8':
        k=0.8
    elif sel.get() == 'k=0.9':
        k=0.9
    m=1-k
    for x in range(height):
        for y in range(width):
            arr3[x][y][2]=arr0[x][y][2]*k+arr1[x][y][2]*m
            arr3[x][y][1]=arr0[x][y][1]*k+arr1[x][y][1]*m
            arr3[x][y][0]=arr0[x][y][0]*k+arr1[x][y][0]*m
    imtk3=ImageTk.PhotoImage(Image.fromarray(arr3))
    L3.config(image=imtk3)
bt.config(command=Combine)
    
root.mainloop()

