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

sel=IntVar()
sel.set(0)
fr=Radiobutton(bf,variable=sel,value=0,text='Frist-Order')
fr.grid(row=0,column=4)
fr.config(state='disable')
br=Radiobutton(bf,variable=sel,value=1,text='Bilinear interpolation')
br.grid(row=0,column=5)
br.config(state='disable')

Label(text='Image :').place(x=10,y=50)
iL0=Label()
iL0.place(x=10,y=70)

Label(text='AND Image :').place(x=420,y=50)
im1=Image.new('RGB',(400,300))
imand=Image.new('RGB',(120,50),color=(255,255,255))
im1.paste(imand,(140,70))
imtk1=ImageTk.PhotoImage(im1)
iL1=Label(image=imtk1)
iL1.place(x=420,y=70)

Label(text='Image resize :').place(x=10,y=380)
iL2=Label()
iL2.place(x=10,y=400)

Label(text='ADN Image :').place(x=420,y=380)
iL3=Label()
iL3.place(x=420,y=400)

def OpenFile():
    global imtk0,iL0,arr0
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
b0.config(command=OpenFile)
root.mainloop()



