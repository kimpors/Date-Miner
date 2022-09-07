from pyrogram import Client
from pyrogram.errors import FloodWait
from pyrogram.types import messages_and_media
import xml.etree.ElementTree as ET
from deepface import DeepFace
from time import sleep
import keyboard
import sys
import zmq

apiId = 0
apiHash = 0
phone = 0

context = zmq.Context()
socket = context.socket(zmq.REQ)
socket.connect("tcp://127.0.0.1:5555")

socket.send_string("Hello")
apiId = socket.recv()

print(apiId)





