from tkinter import *
from tkinter import filedialog
from tkinter import messagebox
from PIL import Image,ImageTk
import numpy as np
root=Tk()
root.title('Image project 6')
root.geometry('830x700')
np.random.seed(10)

bf=Frame(root)
bf.place(x=10,y=10)

b0=Button(bf,text='Open Picture')
b0.grid(row=0,column=0)

mask=IntVar()
mask.set(3)
e0=Entry(bf,textvariable=mask,width=5)
e0.grid(row=0,column=2)
e0.config(state='disable')

b2=Button(bf,text='Calculator:high pass')
b2.grid(row=0,column=3)
b2.config(state='disable')

T=DoubleVar()
T.set(1)
e1=Entry(bf,textvariable=T,width=5)
e1.grid(row=0,column=4)
e1.config(state='disable')

b3=Button(bf,text='Calculator:high-boost filter')
b3.grid(row=0,column=5)
b3.config(state='disable')

Label(text='Picture :').place(x=10,y=50)
iL0=Label()
iL0.place(x=10,y=70)
Label(text='').place(x=420,y=50)
iL1=Label()
iL1.place(x=420,y=70)
Label(text='High Pass :').place(x=10,y=380)
iL2=Label()
iL2.place(x=10,y=400)
Label(text='High Boost Pass :').place(x=420,y=380)
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
                G=(int(arr0[x][y][2])+int(arr0[x][y][1])+int(arr0[x][y][0]))/3
                arr0[x][y]=[G,G,G]
        imtk0=ImageTk.PhotoImage(Image.fromarray(arr0))
        iL0.config(image=imtk0)
        e0.config(state='normal')
        e1.config(state='normal')
        b2.config(state='normal')
        b3.config(state='normal')
b0.config(command=OpenFile)

def highBoostFilter(size,A,mode,arr):
    new=np.array(arr)
    blank=int(size/2)
    A=size**2-2+A
    for x in range(blank,len(arr)-blank):
        for y in range(blank,len(arr[0])-blank):
            out=A*arr[x][y][0]
            for i in range(-blank,blank+1):
                for k in range(-blank,blank+1):
                    if not(i==0 and k==0):
                        out=out-arr[x+i][y+k][0]
            if mode==1:
                out=out/9
            out=min(255,out)
            out=max(out,0)
            #out=out/size**2
            new[x][y]=[out,out,out]
    return new
def highpass():
    global imtk2
    try:
        if mask.get()%2 ==1 and mask.get()>0:
            arr=highBoostFilter(mask.get(),1,1,arr0)
            imtk2=ImageTk.PhotoImage(Image.fromarray(arr))
            iL2.config(image=imtk2)
    except:
        messagebox.showerror('Error','Input a number is odd bigger than 1.')
b2.config(command=highpass)

def highboost():
    global imtk3
    try:
        if T.get()>0:
            arr=highBoostFilter(3,T.get(),0,arr0)
            imtk3=ImageTk.PhotoImage(Image.fromarray(arr))
            iL3.config(image=imtk3)
    except:
        messagebox.showerror('Error','Input a number is odd bigger than 1.')
b3.config(command=highboost)
root.mainloop()
