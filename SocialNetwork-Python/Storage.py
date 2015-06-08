__author__ = 'marcel'

class Storage:
    " The message store "

    def __init__(self):
        self.messages = []

    def add(self, message):
        self.messages.append(message)

    def get(self):
        return self.messages
