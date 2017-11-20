from tkinter import *
from tkinter import filedialog
from PIL import Image,ImageTk
import numpy as np

tk=Tk()
tk.title('image to gary demo')
tk.geometry('830x375+0+0')

Label(text='resource picture:').place(x=10,y=40)
Label(text='output picture:').place(x=420,y=40)
Li=Label()
Li.place(x=10,y=65)
Lo=Label()
Lo.place(x=420,y=65)

def FileLoad():
    global Li,InImgTk,bt,array
    filename=filedialog.askopenfile()
    if filename is not None:
        im=Image.open(filename.name)  
        if(im.height>300):
            re=300/im.height
        if(im.width*re>400):
            re=400/im.width
        im=im.resize((int(im.width*re),int(im.height*re)),Image.ANTIALIAS)
        InImgTk=ImageTk.PhotoImage(im)
        Li.config(image=InImgTk)
        bt.config(state='normal')
        array=np.array(im)
Button(text='Load picture',command=FileLoad).place(x=10,y=10)

def Calculate():
    global Lo,OutImgTk,array
    height=int(array.size/array[0].size)
    width=int(array[0].size/array[0][0].size)
    for x in range(height):
        for y in range(width):
            Y=array[x][y][0]*0.114+array[x][y][1]*0.587+array[x][y][2]*0.299
            array[x][y]=[Y,Y,Y]
    OutImgTk=ImageTk.PhotoImage(Image.fromarray(array,'RGB'))
    Lo.config(image=OutImgTk)
bt=Button(text='To gray',command=Calculate)
bt.place(x=100,y=10)
bt.config(state='disable')

tk.mainloop()

