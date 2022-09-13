from pyrogram import Client
from pyrogram.errors import FloodWait
from pyrogram.types import messages_and_media
from deepface import DeepFace
from time import sleep
import sys
import zmq
import os

apiId = 0
apiHash = 0
phone = 0

def InputEvent(event, *args):
    if event == "builtins.input":
        prompt, = args
        prompt = ''.join(prompt)

        if "Enter phone number" in prompt:
            socket.send_string("Phone")
        elif "Is" in prompt:
            socket.send_string("Confirm")
        elif "Enter confirmation code" in prompt:
            socket.send_string("Code")
        else:
            return
        socket.recv()
    
context = zmq.Context()
socket = context.socket(zmq.REQ)
socket.connect("tcp://127.0.0.1:5555")
sys.addaudithook(InputEvent)


socket.send_string("Name")
name = socket.recv().decode("utf-8")

if(not os.path.isfile(name)):
    socket.send_string("ID")
    apiId = socket.recv().decode("utf-8")
    socket.send_string("Hash")
    apiHash = socket.recv().decode("utf-8")
    app = Client(name,apiId,apiHash)
else:
    app = Client(name)
    

def main():
    with app:
        try:
            app.send_message("me", "I'm work !")
        except FloodWait as ex:
            sleep(ex.value)
            
app.run(main())
    




