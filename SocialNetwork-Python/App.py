__author__ = 'marcel'

import datetime

from Message import Message
from Clock import Clock

class App:
    " The main class "

    def __init__(self, storage):
        self.storage = storage

    def post(self, user, text):
        self.storage.add(Message(user, text, Clock.now()))

    def get(self, user):
        messages = self.storage.get()
        return [m.text for m in messages]

    def follows(self, user, other):
        "something"

    def wall(self, user):
        "something"
