#region ------ I M P O R T S --------------

from socket import *
import errno
import  threading
from OperationsHandler import *

#endregion


#region ------ C O N S T A N T S --------------

PARENT_PORT = 2000
PARENT_HOST = "192.168.2.21"  # "127.0.0.1"
PROT_START = "Hello"                                      # Initialization keyword of Protocol Establishment
LEN_UNIT_BUF = 2048                                       # Min len of buffer for receive from server socket
GOOD_RESPONSE = "OK"
BAD_RESPONSE = "ERROR"
END_LINE = "\r\n"                                         # End of line
#endregion




class ParentSessionHandler(threading.Thread):

    operationsHandler = OperationsHandler()

    def __init__(self):
        self.parent_sock = socket()
        daemon = True
        threading.Thread.__init__(self)


    #-----------------------------------------------------------------------------------------------
    # Receive data from input stream from server socket by loop
    # Each step read LEN_UNIT_BUF bytes
    # After loop we want to get only first part of split by '\r\n'
    # Return : content of input stream from server socket
    #-----------------------------------------------------------------------------------------------
    def recv_buf(self):
        #content=""
        #while True:
        #    data = self.clientSock.recv(LEN_UNIT_BUF)
        #    if not data:  break
        #    content += data
        #print content
        #return content.split(END_LINE)[0]
        return self.parent_sock.recv(LEN_UNIT_BUF).split(END_LINE)[0]

    #-----------------------------------------------------------------------------------------------
    # the function for verify Hello at beginning of communication in data
    #-----------------------------------------------------------------------------------------------
    def verify_hello(self, data):
        if len (data):
            # Verify Hello at beginning of communication
            if not (data == PROT_START ):
                self.clientSock.send("Error in protocol establishment ( 'Hello' )" + END_LINE)
                time.sleep(0.5)
                self.clientSock.close()
                return False
            return True
        return False


    def run(self):

        print "Wait connection..."
        try:
            while True:
                try:
                    self.parent_sock.connect((PARENT_HOST, PARENT_PORT))
                    break
                except:
                    continue

            print "Connected to server..."
            self.parent_sock.send(PROT_START + END_LINE)
            # Wait message beginning of communication from client
            data = self.recv_buf()
            if not self.verify_hello(data) :
                return

            self.parent_sock.send("Tal" + END_LINE)

            self.operation()

        except error, v:
            errorcode=v[0]
            if errorcode == errno.ECONNRESET:
                print 'An existing connection was forcibly closed by the remote server'
            exit(errorcode)


    def operation(self):
        while True:
            fromParent =  self.parent_sock.recv(1024)
            items = fromParent.split('#')
            oper = items[0]
            if (oper=="mouse" or oper == "closeinter" or oper == "key" ):
                sec = self.parent_sock.recv(1024)
                print sec
                for i in self.operationsHandler.operations_dict:
                    if i == oper:
                        self.operationsHandler.operations_dict[i](sec)
            elif oper == "message":
                message = items[1]
                for i in self.operationsHandler.operations_dict:
                    if i == oper:
                        self.operationsHandler.operations_dict[i](message)
            else:
                for i in self.operationsHandler.operations_dict:
                    if i == oper:
                        self.operationsHandler.operations_dict[i]()
