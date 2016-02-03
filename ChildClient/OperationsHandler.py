#region ------ I M P O R T S --------------
import win32gui
import win32con
import win32api
from PIL import ImageGrab
import os
import time
import pyHook
import win32process

#endregion

#region ------ C O N S T A N T S --------------
COMMAND_TO_SHUTDOWM = "shutdown -s"
COMMAND_TO_INTERCLOSE = "ipconfig/release"
COMMAND_TO_INTEROPEN ="ipconfig /renew"
data = ""
proc=[]
#endregion

class OperationsHandler:

    def __init__(self):
        self.operations_dict = {"save" : self.screens_shot, "closeinter": self.closeinter, "key" : self.Key_Lock,
                                "shutdown": self.shutdown, "message": self.show_message, "mouse": self.Mouse_Lock }


    def screens_shot(self):
        #Take a screenshot
        try:
            im = ImageGrab.grab()
            im.save('screenshot.png')
        except Exception as e:
            print "EROOR saving screenshut"
            raise e

    def shutdown(self):
        #Shutdown the computer
        try:
            command_result = os.popen(COMMAND_TO_SHUTDOWM).read()
        except Exception as e:
            print "EROOR to shutdown"
            raise e

    def closeinter(self,sec):
        #Close the internet
        try:
            command_result = os.popen(COMMAND_TO_INTERCLOSE).read()
            sec = int(sec)
            time.sleep(sec)
            command_result = os.popen(COMMAND_TO_INTEROPEN).read()
        except Exception as e:
            print "EROOR to close the internet"
            raise e

    def show_message(self,message):
        #Show the message
        try:
            print win32gui.MessageBox(None,message,"Message",1)
        except Exception as e:
            print "EROOR Show the message"
            raise e

    def Mouse_Lock(self,sec):
        #Lock and Unlock the MOUSE
        hm = pyHook.HookManager()
        hm.MouseAll = False
        hm.HookMouse()
        sec = int(sec)
        time.sleep(sec)
        hm = pyHook.HookManager()
        hm.MouseAll = True
        hm.HookMouse()

    def Key_Lock(self,sec):
        #Lock and Unlock the KEYBOARD
        hm = pyHook.HookManager()
        hm.KeyAll = False
        hm.HookKeyboard()
        sec = int(sec)
        time.sleep(sec)
        hm = pyHook.HookManager()
        hm.KeyAll = True
        hm.HookKeyboard()

    def Send_Process(self):
        #Open process at computer
        processes = win32process.EnumProcesses()
        for pid in processes:
            try:
                handle = win32api.OpenProcess(win32con.PROCESS_ALL_ACCESS, False, pid)
                exe = win32process.GetModuleFileNameEx(handle, 0)
                exe=exe.rsplit("\\")
                exe=exe[-1]
                proc.append(exe)
            except: pass
        x = ",".join(proc)
        #send x
        print x

