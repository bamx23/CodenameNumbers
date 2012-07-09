#!/usr/bin/env python
from clientHandler import client
import socket
import threading

class server(threading.Thread):

    def __init__(self, config):
        serversocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        serversocket.bind((config["host"], config["port"]))
        serversocket.listen(config["queue_len"])
        
        self.serversocket = serversocket

        threading.Thread.__init__(self)

    def run(self):
        print "Started server"
        try:
            while True:
                connection, address = self.serversocket.accept() # connection is a new socket
                print "Accepted connection [%s:%s]"%address
                cl = client(address, connection)
                cl.run()
        except KeyboardInterrupt as e:
            print "Exit"

if __name__ == '__main__':
    config =    { 
                    "host"      : socket.gethostname(),
                    "port"      : 9505,
                    "queue_len" : 50
                }
    s = server(config)
    s.run()