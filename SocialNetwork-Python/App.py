__author__ = 'marcel'

import datetime

from Message import Message
from Clock import Clock

class App:
    " The main class "

    def __init__(self, storage, formatter):
        self.storage = storage
        self.formatter = formatter

    def post(self, user, text):
        self.storage.add(Message(user, text, Clock.now()))

    def get(self, user):
        messages = sorted(self.storage.get(), key = lambda item: item.createdOn, reverse = True)
        return [self.__format_message(m) for m in messages]

    def follows(self, user, other):
        "something"

    def wall(self, user):
        "something"

    def __format_message(self, m):
        return m.text + " (" + self.formatter.Format(Clock.now() - m.createdOn) + " ago)"
