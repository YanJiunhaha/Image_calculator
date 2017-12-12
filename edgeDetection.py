from tkinter import *
from tkinter import filedialog
from tkinter import messagebox
from PIL import Image,ImageTk
import numpy as np

root=Tk()
root.title('Image project 7')
root.geometry('830x700')

bf=Frame(root)
bf.place(x=10,y=10)

b0=Button(bf,text='Open Picture')
b0.grid(row=0,column=0)

b1=Button(bf,text='Prewitt Operator')
b1.grid(row=0,column=1)
b1.config(state='disable')

b2=Button(bf,text='Sobel Operator')
b2.grid(row=0,column=2)
b2.config(state='disable')

Label(text='Picture :').place(x=10,y=50)
iL0=Label()
iL0.place(x=10,y=70)
Label(text='').place(x=420,y=50)
iL1=Label()
iL1.place(x=420,y=70)
Label(text='Horizontal&Vertical:').place(x=10,y=380)
iL2=Label()
iL2.place(x=10,y=400)
Label(text='Diagonal:').place(x=420,y=380)
iL3=Label()
iL3.place(x=420,y=400)

class PrewittOperator:
    def __init__():
        pass
    def horizontal():
        return np.array([[-1,-1,-1],[0,0,0],[1,1,1]])
    def vertical():
        return np.array([[-1,0,1],[-1,0,1],[-1,0,1]])
    def diagonalR():
        return np .array([[-1,-1,0],[-1,0,1],[0,1,1]])
    def diagonalL():
        return np .array([[0,1,1],[-1,0,1],[-1,-1,0]])
class SobelOperator:
    def __init__():
        pass
    def horizontal():
        return np.array([[-1,-2,-1],[0,0,0],[1,2,1]])
    def vertical():
        return np.array([[-1,0,-1],[-2,0,2],[-1,0,1]])
    def diagonalR():
        return np .array([[-2,-1,0],[-1,0,1],[0,1,2]])
    def diagonalL():
        return np .array([[0,1,2],[-1,0,1],[-2,-1,0]])
    
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
        b1.config(state='normal')
        b2.config(state='normal')
b0.config(command=OpenFile)

def maskFilter(mask,arr):
    new=np.array(arr)
    for x in range(1,len(arr)-1):
        for y in range(1,len(arr[0])-1):
            G=0
            for i in range(-1,2):
                for k in range(-1,2):
                    G=mask[i+1][k+1]*arr[i+x][k+y][0]+G
            G=min(255,G)
            G=max(G,0)
            new[x][y]=[G,G,G]
    return new
def Prewitt():
    global imtk2,imtk3
    mask=PrewittOperator.horizontal()+PrewittOperator.vertical()
    arr=maskFilter(mask,arr0)
    imtk2=ImageTk.PhotoImage(Image.fromarray(arr))
    iL2.config(image=imtk2)
    mask=PrewittOperator.diagonalR()+PrewittOperator.diagonalL()
    arr=maskFilter(mask,arr0)
    imtk3=ImageTk.PhotoImage(Image.fromarray(arr))
    iL3.config(image=imtk3)
b1.config(command=Prewitt)
def Sobel():
    global imtk2,imtk3
    mask=SobelOperator.horizontal()+SobelOperator.vertical()
    arr=maskFilter(mask,arr0)
    imtk2=ImageTk.PhotoImage(Image.fromarray(arr))
    iL2.config(image=imtk2)
    mask=SobelOperator.diagonalR()+SobelOperator.diagonalL()
    arr=maskFilter(mask,arr0)
    imtk3=ImageTk.PhotoImage(Image.fromarray(arr))
    iL3.config(image=imtk3)
    
b2.config(command=Sobel)
root.mainloop()
