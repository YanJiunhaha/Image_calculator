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
        imtk0=ImageTk.PhotoImage(im0)
        iL0.config(image=imtk0)
        b1.config(state='normal')
b0.config(command=OpenFile)

def 
root.mainloop()
