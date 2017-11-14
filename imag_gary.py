from tkinter import *
from tkinter import filedialog
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
    global imtk0,iL0,arr0,b1
    filename=filedialog.askopenfile()
    if filename is not None:
        im=Image.open(filename.name)
        re=1
        if(im.height>300):
            re=300/im.height
        if(im.width*re>400):
            re=400/im.width
        im=im.resize((int(im.width*re),int(im.height*re)),Image.ANTIALIAS)
        arr0=np.array(im)
        imtk0=ImageTk.PhotoImage(im)
        iL0.config(image=imtk0)
        b1.config(state='normal')
b0.config(command=OpenFile)

def ANDimage():
    global imtk1,iL1,arr1,arr0
    for x in range(int(arr0.size/arr0[0].size)):
        for y in range(int(arr0[0].size/arr0[0][0].size)):
            arr1[x][y][2]=arr0[x][y][2]&arr1[x][y][2]
            arr1[x][y][1]=arr0[x][y][1]&arr1[x][y][1]
            arr1[x][y][0]=arr0[x][y][0]&arr1[x][y][0]
    imtk1=ImageTk.PhotoImage(Image.fromarray(arr1))
    iL1.config(image=imtk1)
    b2.config(state='normal')
    b3.config(state='normal')
    fr.config(state='normal')
    br.config(state='normal')
b1.config(command=ANDimage)

def RoomIn():
    global imtk2,imtk3,iL2,iL3,arr0,arr1,arr2,arr3,sel
    scale=1
    '''
    try:'''
    scale=inNum.get()
    print('ha')
    imageX=int(arr0.size/arr0[0].size)
    imageY=int(arr0[0].size/arr0[0][0].size)
    resizeX=int(imageX*scale)
    resizeY=int(imageY*scale)
    arr2=np.array(Image.new('RGB',(resizeX,resizeY)))
    
    ANDx=int(arr1.size/arr1[0].size)
    ANDy=int(arr1[0].size/arr1[0][0].size)
    arr3=np.array(Image.new('RGB',(int(ANDx*scale),int(ANDy*scale))))
    if sel.get() == 0 :#frist order
        for x in range(resizeX):
            for y in range(resizeY):
                oldy=int(y/scale)
                oldx=int(x/scale)
                r=y%scale
                if oldy>=0 and oldx>=0 and oldy<(imageY-1) and oldx<imageX :
                    R=(arr0[oldx][oldy][2]*(scale-r)+arr0[oldx][oldy+1][2]*r)/scale
                    arr2[x][y][2]=R
                    arr2[x][y][1]=(arr0[oldx][oldy][1]*(scale-r)+arr0[oldx][oldy+1][1]*r)/scale
                    arr2[x][y][0]=(arr0[oldx][oldy][0]*(scale-r)+arr0[oldx][oldy+1][0]*r)/scale
        for x in range(resizeX):
            for y in range(resizeY):
                oldy=int(y/scale)
                oldx=int(x/scale)
                r=y%scale
                if oldy>=0 and oldx>=0 and oldy<(imageY-1) and oldx<imageX :
                    R=(arr0[oldx][oldy][2]*(scale-r)+arr0[oldx+1][oldy][2]*r)/scale
                    arr2[x][y][2]=R
                    arr2[x][y][1]=(arr0[oldx][oldy][1]*(scale-r)+arr0[oldx+1][oldy][1]*r)/scale
                    arr2[x][y][0]=(arr0[oldx][oldy][0]*(scale-r)+arr0[oldx+1][oldy][0]*r)/scale
    imtk2=ImageTk.PhotoImage(Image.fromarray(arr2))
    iL2.config(image=imtk2)
    '''                          
    except:
        messagebox.showerror('Error','Input a number bigger than 1.')
        '''
b2.config(command=RoomIn)

root.mainloop()


