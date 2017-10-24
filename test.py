from tkinter import *
from PIL import Image,ImageTk
import numpy as np

def calculate():
    sel=sv.get()
    if(sel=='add:k=0.1'):
        print('k=0.1')
    elif(sel=='add:k=0.2'):
        print('k=0.2')

tk=Tk()

filename1=filedialog.askopenfile()
im1=Image.open(filename1.name)
print(im1.format,im1.size,im1.mode)

filename2=filedialog.askopenfile()
im2=Image.open(filename2.name)
print(im2.format,im2.size,im2.mode)

array1=np.array(im1)
array2=np.array(im2)

string=['add:k=0.1','add:k=0.2','add:k=0.3','add:k=0.4','add:k=0.5','add:k=0.6','add:k=0.7','add:k=0.8','add:k=0.9','sub:Y-I']
select=StringVar()
select.set('select mode')
om=OptionMenu(tk,select,*string)
om.grid(row=0,column=0)

button=Button(text='calculate',command=calculate)
button.grid(row=0,column=1)

k=0.5
m=0.5
for x in range(im1.height):
    for y in range(im1.width):
        R=array1[x][y][2]*0.5+array2[x][y][2]*0.5
        G=array1[x][y][1]*0.5+array2[x][y][1]*0.5
        B=array1[x][y][0]*0.5+array2[x][y][0]*0.5
        array1[x][y]=[B,G,R]
arrayimage=Image.fromarray(array1,'RGB')
print("finish.")
#Oldphoto=ImageTk.PhotoImage(im1)
#Old2photo=ImageTk.PhotoImage(im2)
photo=ImageTk.PhotoImage(arrayimage)
#Oldlabel=Label(image=Oldphoto).grid(row=0,column=0)
#Old2label=Label(image=Old2photo).grid(row=0,column=1)
label=Label(tk,image=photo).grid(row=1,column=0)

tk.mainloop()
