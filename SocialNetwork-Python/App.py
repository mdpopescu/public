__author__ = 'marcel'

import datetime

from Message import Message
from Clock import Clock

class App:
    " The main class "

    def __init__(self, storage, users, formatter):
        self.storage = storage
        self.users = users
        self.formatter = formatter

    def post(self, user, text):
        self.storage.add(Message(user, text, Clock.now()))

    def get(self, user):
        messages = sorted(self.storage.get(), key = lambda item: item.createdOn, reverse = True)
        return [self.__format_message(m) for m in messages if m.user == user]

    def follows(self, user, other):
        self.users.add(user, other)

    def wall(self, user):
        messages = sorted(self.storage.get(), key = lambda item: item.createdOn, reverse = True)
        followed = self.users.get(user)
        relevant = followed + [user]
        return [self.__format_message_with_user(m) for m in messages if m.user in relevant]

    def __format_message(self, m):
        return m.text + " (" + self.formatter.Format(Clock.now() - m.createdOn) + " ago)"

    def __format_message_with_user(self, m):
        return m.user + ": " + self.__format_message(m)
