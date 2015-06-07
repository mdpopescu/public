__author__ = 'marcel'

from Message import Message

class App:
    " The main class "

    def __init__(self, storage):
        self.storage = storage

    def post(self, user, text):
        self.storage.add(Message(user, text))

    def get(self, user):
        "something"

    def follows(self, user, other):
        "something"

    def wall(self, user):
        "something"
