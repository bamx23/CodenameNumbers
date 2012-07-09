import threading

class client(threading.Thread):

    def __init__(self, address, connection):
        self.u_addr = address
        self.u_conn = connection
        threading.Thread.__init__(self)

    def run(self):
        try:
            while 1:
                data = self.u_conn.recv(1024) # receive up to 1K bytes
                if data:
                    print "[%s:%s]>> %s"%(self.u_addr[0], self.u_addr[1], data)
                    self.u_conn.send("semen molodez")
                else:
                    break
        except:
            print "Some error in connection"

        try:
            self.u_conn.close()
            print "[%s:%s]>> Exited"%(self.u_addr[0], self.u_addr[1])
        except:
            pass
