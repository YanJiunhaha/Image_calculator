from tkinter import *
from tkinter import filedialog
from tkinter import messagebox
from PIL import Image,ImageTk
import numpy as np
root=Tk()
root.title('Image project 5')
root.geometry('830x700')
np.random.seed(10)

bf=Frame(root)
bf.place(x=10,y=10)

b0=Button(bf,text='Open Picture')
b0.grid(row=0,column=0)

b1=Button(bf,text='noise')
b1.grid(row=0,column=1)
b1.config(state='disable')

mask=IntVar()
mask.set(3)
e0=Entry(bf,textvariable=mask,width=5)
e0.grid(row=0,column=2)
e0.config(state='disable')

b2=Button(bf,text='Calculator:lowpass')
b2.grid(row=0,column=3)
b2.config(state='disable')

T=IntVar()
T.set(1)
e1=Entry(bf,textvariable=T,width=5)
e1.grid(row=0,column=4)
e1.config(state='disable')

b3=Button(bf,text='Calculator:alphaTrimmed')
b3.grid(row=0,column=5)
b3.config(state='disable')

Label(text='Picture :').place(x=10,y=50)
iL0=Label()
iL0.place(x=10,y=70)
Label(text='(1):').place(x=420,y=50)
iL1=Label()
iL1.place(x=420,y=70)
Label(text='(2) :').place(x=10,y=380)
iL2=Label()
iL2.place(x=10,y=400)
Label(text='(3) :').place(x=420,y=380)
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
        e0.config(state='normal')
        b2.config(state='normal')
b0.config(command=OpenFile)

def normal_noise(mean,variance,probability,arr0):
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
    return arr1

def impulse_noise(probability,arr0):
    arr3=np.array(arr0)
    for x in range(len(arr0)):
        for y in range(len(arr0[0])):
            if np.random.rand()<probability/2: #課本設定發生機率10%
                arr3[x][y]=[0,0,0]
    for x in range(len(arr0)):
        for y in range(len(arr0[0])):
            if np.random.rand()<probability/2: #課本設定發生機率10%
                arr3[x][y]=[255,255,255]
    return arr3

def alphaTrimmed(size,alpha,arr):
    new=np.array(arr)
    blank=int(size/2)
    for x in range(blank,len(arr)-blank):
        for y in range(blank,len(arr[0])-blank):
            Filter_R=[]
            Filter_G=[]
            Filter_B=[]
            for i in range(-blank,blank+1):
                for k in range(-blank,blank+1):
                    Filter_R.append(arr[x+i][y+k][2])
                    Filter_G.append(arr[x+i][y+k][1])
                    Filter_B.append(arr[x+i][y+k][0])
            Filter_R.sort()
            Filter_G.sort()
            Filter_B.sort()
            
            R=0;G=0;B=0
            for i in range(alpha,len(Filter_R)-alpha):
                R=R+Filter_R[i]
                G=G+Filter_G[i]
                B=B+Filter_B[i]
            R=R/(size**2-2*alpha)
            G=G/(size**2-2*alpha)
            B=B/(size**2-2*alpha)
            new[x][y]=[B,G,R]
    return new

def noise():
    global imtk2,imtk3,arr2,arr3
    arr2=impulse_noise(0.3,arr0)
    imtk2=ImageTk.PhotoImage(Image.fromarray(arr2))
    iL2.config(image=imtk2)
    arr3=normal_noise(0,400,0.2,arr0)
    arr3=impulse_noise(0.1,arr3)
    imtk3=ImageTk.PhotoImage(Image.fromarray(arr3))
    iL3.config(image=imtk3)
    e1.config(state='normal')
    b3.config(state='normal')
b1.config(command=noise)

def lowpass():
    global imtk1
    try:
        if mask.get()%2 ==1 and mask.get()>0:
            arr=alphaTrimmed(mask.get(),0,arr0)
            imtk1=ImageTk.PhotoImage(Image.fromarray(arr))
            iL1.config(image=imtk1)
    except:
        messagebox.showerror('Error','Input a number is odd bigger than 1.')
b2.config(command=lowpass)

def Trimmed():
    global imtk2,imtk3
    try:
        if mask.get()%2==1 and mask.get()>0 and T.get()>=0 and mask.get()/2>T.get():
            arr=alphaTrimmed(mask.get(),int(mask.get()/2),arr2)
            imtk2=ImageTk.PhotoImage(Image.fromarray(arr))
            iL2.config(image=imtk2)
            arr=alphaTrimmed(3,T.get(),arr3)
            imtk3=ImageTk.PhotoImage(Image.fromarray(arr))
            iL3.config(image=imtk3)
    except:
        messagebox.showerror('Error','Error.')
b3.config(command=Trimmed)
root.mainloop()
