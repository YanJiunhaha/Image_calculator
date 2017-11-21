from tkinter import *
from tkinter import filedialog
from tkinter import messagebox
from PIL import Image,ImageTk
import numpy as np
root=Tk()
root.title('Image project 2')
root.geometry('830x700')

bf=Frame(root)
bf.place(x=10,y=10)

b0=Button(bf,text='Open Picture')
b0.grid(row=0,column=0)

b1=Button(bf,text='And YanJiun\'s Eye')
b1.grid(row=0,column=1)
b1.config(state='disable')

b2=Button(bf,text='Room-in')
b2.grid(row=0,column=2)
b2.config(state='disable')

b3=Button(bf,text='Room-Out')
b3.grid(row=0,column=3)
b3.config(state='disable')

inNum=DoubleVar()
inNum.set(1.0)
entry=Entry(bf,textvariable=inNum,width=10)
entry.grid(row=0,column=4)

sel=IntVar()
sel.set(0)
fr=Radiobutton(bf,variable=sel,value=0,text='Frist-Order')
fr.grid(row=0,column=5)
fr.config(state='disable')
br=Radiobutton(bf,variable=sel,value=1,text='Bilinear interpolation')
br.grid(row=0,column=6)
br.config(state='disable')

Label(text='Image :').place(x=10,y=50)
iL0=Label()
iL0.place(x=10,y=70)

Label(text='AND Image :').place(x=420,y=50)
im1=Image.new('RGB',(400,300))
imand=Image.new('RGB',(120,50),color=(255,255,255))
im1.paste(imand,(140,70))
arr1=np.array(im1)
imtk1=ImageTk.PhotoImage(im1)
iL1=Label(image=imtk1)
iL1.place(x=420,y=70)

Label(text='Image resize :').place(x=10,y=380)
iL2=Label()
iL2.place(x=10,y=400)

Label(text='ADN Image resize :').place(x=420,y=380)
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

def ANDimage():
    global imtk1,iL1,arr0,arr1
    for x in range(int(arr0.size/arr0[0].size)):
        for y in range(int(arr0[0].size/arr0[0][0].size)):
            arr1[x][y][2]=arr0[x][y][2]&arr1[x][y][2]
            arr1[x][y][1]=arr0[x][y][1]&arr1[x][y][1]
            arr1[x][y][0]=arr0[x][y][0]&arr1[x][y][0]
    im1=Image.fromarray(arr1)
    imtk1=ImageTk.PhotoImage(im1)
    iL1.config(image=imtk1)
    b2.config(state='normal')
    b3.config(state='normal')
    fr.config(state='normal')
    br.config(state='normal')
b1.config(command=ANDimage)

def RoomIn():
    global imtk2,imtk3,iL2,iL3,arr0,arr1,arr2,arr3,sel
    
    try:
        scale=inNum.get()
        imageY=int(arr0.size/arr0[0].size)
        imageX=int(arr0[0].size/arr0[0][0].size)
        arr2=np.array(Image.new('RGB',(400,300)))
        ANDy=int(arr1.size/arr1[0].size)
        ANDx=int(arr1[0].size/arr1[0][0].size)
        arr3=np.array(Image.new('RGB',(400,300)))
        if sel.get() == 0 :#frist order
            for x in range(len(arr2)):
               for y in range(len(arr2[0])):
                    h=int(y/scale)
                    w=int(x/scale)
                    r=y%scale
                    if h<len(arr0[0])-1 and w<len(arr0):
                        arr2[x][y][2]=(arr0[w][h][2]*(scale-r)+arr0[w][h+1][2]*r)/scale
                        arr2[x][y][1]=(arr0[w][h][1]*(scale-r)+arr0[w][h+1][1]*r)/scale
                        arr2[x][y][0]=(arr0[w][h][0]*(scale-r)+arr0[w][h+1][0]*r)/scale
            for x in range(len(arr3)):
                for y in range(len(arr3[0])):
                    h=int(y/scale)+140
                    w=int(x/scale)+70
                    r=y%scale
                    if h<len(arr1[0])-1 and w<len(arr1) :
                        arr3[x][y][2]=(arr1[w][h][2]*(scale-r)+arr1[w][h+1][2]*r)/scale
                        arr3[x][y][1]=(arr1[w][h][1]*(scale-r)+arr1[w][h+1][1]*r)/scale
                        arr3[x][y][0]=(arr1[w][h][0]*(scale-r)+arr1[w][h+1][0]*r)/scale
        else :#bilinear interpolation
            for x in range(len(arr2)):
                for y in range(len(arr2[0])):
                    h=int(y/scale)
                    w=int(x/scale)
                    h_m=y%scale
                    w_m=x%scale
                    arr=[]
                    if h<len(arr0[0])-1 and w<len(arr0)-1:
                        arr.append(arr0[w][h])
                        arr.append(arr0[w+1][h])
                        arr.append(arr0[w][h+1])
                        arr.append(arr0[w+1][h+1])
                        m0=np.array([h-y,y-h])
                        m1=np.array([[arr[0],arr[1]],[arr[2],arr[3]]])
                        m2=np.array([[w-x],[x-w]])
                        arr3[x][y][2]=int()
                    
        imtk2=ImageTk.PhotoImage(Image.fromarray(arr2))
        iL2.config(image=imtk2)
        imtk3=ImageTk.PhotoImage(Image.fromarray(arr3))
        iL3.config(image=imtk3)           
    except:
        messagebox.showerror('Error','Input a number bigger than 1.')
b2.config(command=RoomIn)

root.mainloop()
