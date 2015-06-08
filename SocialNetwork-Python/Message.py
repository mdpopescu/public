__author__ = 'marcel'

class Message:
    " The message "

    def __init__(self, user, text, createdOn):
        self.user = user
        self.text = text
        self.createdOn = createdOn

    def __eq__(self, other):
        return self.user == other.user and self.text == other.text and self.createdOn == other.createdOn

    def __repr__(self):
        return self.user + " said " + self.text + " at " + str(self.createdOn)
