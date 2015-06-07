__author__ = 'marcel'

class Message:
    " The message "

    def __init__(self, user, text):
        self.user = user
        self.text = text

    def __eq__(self, other):
        return self.user == other.user and self.text == other.text
